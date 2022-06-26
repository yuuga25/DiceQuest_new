using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using Cysharp.Threading.Tasks;

public class GachaController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ImageData_Character data_Character;
    [SerializeField] private Data_Attribute data_Attribute;
    [SerializeField] private Data_Gacha data_Gacha;
    [SerializeField] private List<int> list_Constant;

    [Header("UI")]
    [SerializeField] private Text text_Count;
    [SerializeField] private UICornersGradient gradient;
    [SerializeField] private List<GameObject> obj_Character;
    [SerializeField] private GameObject obj_ConfirmationScreen;
    [Space(20)]
    [SerializeField] private GameObject gacha_Top;
    [SerializeField] private GameObject gacha_Result;
    [SerializeField] private GameObject loadingObj;
    [SerializeField] private GameObject errorObj;
    [SerializeField] private GameObject obj_Parent;
    [SerializeField] private GameObject obj_Content;
    private const string PASSWORD_CHARS = "0123456789ABCDEFGHIJKLMNPQRSTUVWXYZ";
    private List<Data_Character> list_GachaResult = new List<Data_Character>();

    public static string dayOfWeekName;
    private bool isResult;
    private void Start()
    {
        dayOfWeekName = DateTime.Now.ToString("ddd");
        isResult = false;
    }
    private void Update()
    {
        if(dayOfWeekName != DateTime.Now.ToString("ddd") && isResult)
        {
            dayOfWeekName = DateTime.Now.ToString("ddd");
            DisplayGachaTop();
        }
    }

    public void DisplayGachaTop()
    {
        gacha_Top.SetActive(true);
        gacha_Result.SetActive(false);
        obj_ConfirmationScreen.SetActive(false);

        text_Count.text = $"{DataList.userData.user_Character.Count}/200";

        gradient.m_topLeftColor = data_Gacha.dataBases.Find(x => x.dayOfWeek == dayOfWeekName).color1;
        gradient.m_topRightColor = data_Gacha.dataBases.Find(x => x.dayOfWeek == dayOfWeekName).color2;
        gradient.m_bottomLeftColor = gradient.m_topRightColor;
        gradient.m_bottomRightColor = gradient.m_topLeftColor;

        for(var i = 0; i < obj_Character.Count; i++)
        {
            var charaData = DataList.mcDatas[0];

            switch (i)
            {
                case 0:
                    charaData = DataList.mcDatas.Find(x => x.mc_Id == data_Gacha.dataBases.Find(xx => xx.dayOfWeek == dayOfWeekName).picId_1);
                    break;
                case 1:
                    charaData = DataList.mcDatas.Find(x => x.mc_Id == data_Gacha.dataBases.Find(xx => xx.dayOfWeek == dayOfWeekName).picId_2);
                    break;
                case 2:
                    charaData = DataList.mcDatas.Find(x => x.mc_Id == data_Gacha.dataBases.Find(xx => xx.dayOfWeek == dayOfWeekName).picId_3);
                    break;
            }

            obj_Character[i].GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == charaData.mc_Id).img_Stand;
            obj_Character[i].transform.Find("Icon").GetComponent<Image>().sprite = data_Attribute.dataBases[charaData.mc_Attribute].at_Icon;

            var texts = obj_Character[i].transform.Find("Image");
            texts.Find("Text").GetComponent<Text>().text = $"{charaData.mc_Name}";
            var star = texts.Find("Text (1)").GetComponent<Text>();

            star.text = "";

            for(var j = 0;j < charaData.mc_Rarity; j++)
            {
                star.text += "★";
            }
        }
    }

    public void DrawGacha(int value)
    {
        var success = obj_ConfirmationScreen.transform.Find("Success");
        var failure = obj_ConfirmationScreen.transform.Find("Failure");

        success.gameObject.SetActive(false);
        failure.gameObject.SetActive(false);

        if(DataList.userData.user_MagicStone < 5 * value || DataList.userData.user_Character.Count + value >= 200)
        {
            failure.gameObject.SetActive(true);

            var text = failure.Find("Text_Conf").GetComponent<Text>();

            if(DataList.userData.user_MagicStone < 5 * value)
            {
                text.text = "消費する魔法石が足りないため\nガチャを引くことが出来ません";
            }
            else if (DataList.userData.user_Character.Count >= 200)
            {
                text.text = "所持しているキャラクターが\n最大数に達しているため\nガチャを引くことが出来ません";
            }
            else if (DataList.userData.user_Character.Count + value >= 200)
            {
                text.text = "所持しているキャラクターが\n最大数に達してしまう\nガチャを引くことが出来ません";
            }

            obj_ConfirmationScreen.SetActive(true);
        }
        else
        {
            success.gameObject.SetActive(true);

            var text = success.Find("Text_Conf").GetComponent<Text>();
            text.text = $"魔法石を消費してガチャを\n{value}回引いてもよろしいですか？";

            success.Find("Text_SaleValue").GetComponent<Text>().text = $"消費：{value * 5}";

            obj_ConfirmationScreen.SetActive(true);

            success.Find("Button_Yes").GetComponent<Button>().onClick.RemoveAllListeners();
            success.Find("Button_Yes").GetComponent<Button>().onClick.AddListener(() => GachaExecution(value));
        }
    }

    private void GachaExecution(int value)
    {
        loadingObj.SetActive(true);
        errorObj.SetActive(false);
        obj_ConfirmationScreen.SetActive(false);

        List<int> star3 = new List<int>();
        List<int> star4 = new List<int>();
        list_GachaResult = new List<Data_Character>();

        foreach(var i in list_Constant)
        {
            var chara = DataList.mcDatas.Find(x => x.mc_Id == i);
            if(chara.mc_Rarity == 3)
            {
                star3.Add(i);
            }
            else if (chara.mc_Rarity == 4)
            {
                star4.Add(i);
            }
        }

        for (var i = 0; i < value; i++)
        {
            var probability = UnityEngine.Random.Range(1, 10001);

            int charaId = 0;

            if(probability > 70)
            {
                if(probability > 3000)
                {
                    charaId = star3[UnityEngine.Random.Range(0, star3.Count)];
                }
                else
                {
                    charaId = star4[UnityEngine.Random.Range(0, star4.Count)];
                }
            }
            else
            {
                var r = UnityEngine.Random.Range(0, 3);
                switch (r)
                {
                    case 0:
                        charaId = data_Gacha.dataBases.Find(x => x.dayOfWeek == dayOfWeekName).picId_1;
                        break;
                    case 1:
                        charaId = data_Gacha.dataBases.Find(x => x.dayOfWeek == dayOfWeekName).picId_2;
                        break;
                    case 2:
                        charaId = data_Gacha.dataBases.Find(x => x.dayOfWeek == dayOfWeekName).picId_3;
                        break;
                }
            }

            Data_Character chara = new Data_Character();
            chara.c_Id = charaId;
            chara.c_Level = 1;

            var sb = new System.Text.StringBuilder(12);

            for (int j = 0; j < 16; j++)
            {
                int pos = UnityEngine.Random.Range(0, PASSWORD_CHARS.Length);
                char c = PASSWORD_CHARS[pos];
                sb.Append(c);
            }

            chara.c_UniqueId = sb.ToString();
            sb = null;

            chara.c_CostumeId = 0;
            chara.c_Uses = 0;

            var charaData = DataList.mcDatas.Find(x => x.mc_Id == chara.c_Id);

            chara.c_UntilLevelUp = Mathf.FloorToInt(((100 + (50 - charaData.mc_RiseValue)) * 0.01f) * chara.c_Level * (100 + (50 - charaData.mc_RiseValue)));
            chara.c_ExpPoint = 10;

            chara.c_HP = Mathf.FloorToInt(charaData.mc_HP * 2.4f / charaData.mc_RiseValue) * chara.c_Level + charaData.mc_HP;

            float A = 1.0f;
            float D = 1.0f;

            switch (charaData.mc_BattleType)
            {
                case 0:
                    break;
                case 1:
                    A = 1.1f;
                    D = 0.9f;
                    break;
                case 2:
                    D = 1.1f;
                    A = 0.9f;
                    break;
            }
            chara.c_Attack = Mathf.FloorToInt((charaData.mc_Attack * 2.4f / charaData.mc_RiseValue) * A) * chara.c_Level + charaData.mc_Attack;
            chara.c_Defense = Mathf.FloorToInt((charaData.mc_Defense * 2.4f / charaData.mc_RiseValue) * D) * chara.c_Level + charaData.mc_Defense;

            chara.c_Rarity = charaData.mc_Rarity;
            chara.c_Attribute = charaData.mc_Attribute;
            chara.c_BattleType = charaData.mc_BattleType;
            chara.c_DicePower = charaData.mc_DicePower;

            if (!DataList.userData.user_PictureBook.Contains(chara.c_Id))
            {
                DataList.userData.user_PictureBook.Add(chara.c_Id);
            }

            DataList.userData.user_Character.Add(chara);
            list_GachaResult.Add(chara);
        }

        DataList.userData.user_MagicStone -= value * 5;

        DataSave(value);
    }

    private void DataSave(int value)
    {
        PlayFabClientAPI.UpdateUserData(
               new UpdateUserDataRequest
               {
                   Data = new Dictionary<string, string>()
                   {
                    {"PlayerData", PlayFabSimpleJson.SerializeObject(DataList.userData)}
                   }
               },
               result =>
               {
                   loadingObj.SetActive(false);
                   GetComponent<HomeUIDisplay>().SetHomeMenuUI();
                   GachaAnim(value);
               },
               error =>
               {
                   errorObj.SetActive(true);
                   loadingObj.SetActive(false);
                   var button = errorObj.transform.Find("Button").GetComponent<ButtonExtention>();
                   button.onClick.RemoveAllListeners();
                   button.onClick.AddListener(() => DataSave(value));
               });
    }

    private async void GachaAnim(int value)
    {
        gacha_Top.SetActive(false);
        gacha_Result.SetActive(true);

        List<GameObject> gachaObj = new List<GameObject>();

        foreach (Transform obj in obj_Parent.transform) Destroy(obj.gameObject);

        for (var i = 0; i < value; i++)
        {
            var content = Instantiate(obj_Content, obj_Parent.transform);

            var charaData = list_GachaResult[i];

            switch (charaData.c_Rarity)
            {
                case 3:
                    content.transform.Find("Cover").Find("Dice3").gameObject.SetActive(true);
                    break;
                case 4:
                    content.transform.Find("Cover").Find("Dice4").gameObject.SetActive(true);
                    break;
                case 5:
                    content.transform.Find("Cover").Find("Dice5").gameObject.SetActive(true);
                    break;
            }

            var gradient = content.transform.Find("Cover").GetComponent<UICornersGradient>();
            gradient.m_topLeftColor = data_Attribute.dataBases[charaData.c_Attribute].at_Color1;
            gradient.m_topRightColor = data_Attribute.dataBases[charaData.c_Attribute].at_Color2;
            gradient.m_bottomLeftColor = gradient.m_topRightColor;
            gradient.m_bottomRightColor = gradient.m_topLeftColor;

            var characterImage = content.transform.Find("Character");
            characterImage.GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == charaData.c_Id).img_Face;

            var flame = characterImage.Find("Flame").GetComponent<UICornersGradient>();
            flame.m_topLeftColor = data_Attribute.dataBases[charaData.c_Attribute].at_Color1;
            flame.m_topRightColor = data_Attribute.dataBases[charaData.c_Attribute].at_Color2;
            flame.m_bottomLeftColor = flame.m_topRightColor;
            flame.m_bottomRightColor = flame.m_topLeftColor;

            characterImage.Find("Icon").GetComponent<Image>().sprite = data_Attribute.dataBases[charaData.c_Attribute].at_Icon;

            var rarityText = characterImage.Find("Text").GetComponent<Text>();
            rarityText.text = "";

            for(var j = 0; j < charaData.c_Rarity; j++)
            {
                rarityText.text += "★";
            }

            gachaObj.Add(content);
            await UniTask.Delay(200);
        }

        await UniTask.Delay(1000);

        for(var i = 0; i < value; i++)
        {
            gachaObj[i].transform.Find("Cover").gameObject.SetActive(false);
            gachaObj[i].transform.Find("Character").gameObject.SetActive(true);
            await UniTask.Delay(600);
        }
    }
}
