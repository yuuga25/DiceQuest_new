using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class ShopController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ImageData_Character data_Character;
    [SerializeField] private ImageData_AnotherCostume data_AnotherCostume;
    [SerializeField] private Data_Attribute data_Attribute;

    [Header("UI")]
    [SerializeField] private GameObject loadingObj;
    [SerializeField] private GameObject errorObj;
    [SerializeField] private List<CanvasGroup> displays = new List<CanvasGroup>();
    [Space(20)]
    [SerializeField] private Text text_ItemName;
    [SerializeField] private GameObject confScreen;

    [Header("キャラクター")]
    private const string PASSWORD_CHARS = "0123456789ABCDEFGHIJKLMNPQRSTUVWXYZ";
    [SerializeField] private List<int> product_CharacterList = new List<int>();
    [SerializeField] private GameObject ch_Parent;
    [SerializeField] private GameObject ch_Content;

    public async void ChangeUI_Shop(CanvasGroup trueObject)
    {
        foreach (var i in displays)
        {
            if (i.gameObject.activeSelf && trueObject.gameObject != i.gameObject)
            {
                await i.DOFade(0, 0.25f).AsyncWaitForCompletion();
                i.gameObject.SetActive(false);
            }
            else if (!trueObject.gameObject.activeSelf)
            {
                trueObject.gameObject.SetActive(true);
                trueObject.alpha = 0;
            }
        }

        confScreen.SetActive(false);
        await trueObject.DOFade(1, 0.3f).AsyncWaitForCompletion();
    }

    public void DisplayShop_Character()
    {
        text_ItemName.text = "キャラクター";

        foreach (Transform obj in ch_Parent.transform) Destroy(obj.gameObject);
        foreach(var i in product_CharacterList)
        {
            var content = Instantiate(ch_Content, ch_Parent.transform);

            var charaData = DataList.mcDatas.Find(x => x.mc_Id == i);

            var att = data_Attribute.dataBases[charaData.mc_Attribute];

            var gradient = content.GetComponent<UICornersGradient>();
            gradient.m_topLeftColor = att.at_Color1;
            gradient.m_topRightColor = att.at_Color2;
            gradient.m_bottomLeftColor = gradient.m_topRightColor;
            gradient.m_bottomRightColor = gradient.m_topLeftColor;

            content.transform.Find("background").Find("Character").GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == charaData.mc_Id).img_Stand;
            content.transform.Find("Icon").GetComponent<Image>().sprite = att.at_Icon;
            var star = content.transform.Find("Text_Star").GetComponent<Text>();
            star.text = "";
            for (var j = 0; j < charaData.mc_Rarity; j++) star.text += "★";
            content.transform.Find("Text_Name").GetComponent<Text>().text = $"{charaData.mc_Name}";
            content.transform.Find("Face").Find("Image").GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == charaData.mc_Id).img_Face;

            var price = content.transform.Find("Text_Price").GetComponent<Text>();
            int value = 0;
            switch (charaData.mc_Rarity)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    value = 25;
                    break;
                case 4:
                    value = 100;
                    break;
                case 5:
                default:
                    value = 450;
                    break;
            }
            price.text = $"魔法石:{value}";

            if (DataList.userData.user_Character.Count >= 200)
            {
                content.GetComponent<Button>().enabled = false;
                var errorObj = content.transform.Find("Impossible");
                errorObj.gameObject.SetActive(true);
                errorObj.Find("Text").GetComponent<Text>().text = $"キャラクターの\n所持数が最大のため\n購入できません";
            }

            content.GetComponent<Button>().onClick.RemoveAllListeners();
            content.GetComponent<Button>().onClick.AddListener(() => Confirmation_Chara(value, charaData.mc_Id));
        }
        
        ChangeUI_Shop(displays[0]);
    }
    public void DisplayShop_Costume()
    {
        text_ItemName.text = "コスチューム";

        foreach (Transform obj in ch_Parent.transform) Destroy(obj.gameObject);
        foreach(var h in data_AnotherCostume.dataLists)
        {
            for (int i = 1; i < h.img_Stand.Count; i++)
            {
                var content = Instantiate(ch_Content, ch_Parent.transform);

                var charaData = DataList.mcDatas.Find(x => x.mc_Id == h.img_Id);

                var att = data_Attribute.dataBases[charaData.mc_Attribute];

                var gradient = content.GetComponent<UICornersGradient>();
                gradient.m_topLeftColor = att.at_Color1;
                gradient.m_topRightColor = att.at_Color2;
                gradient.m_bottomLeftColor = gradient.m_topRightColor;
                gradient.m_bottomRightColor = gradient.m_topLeftColor;

                content.transform.Find("background").Find("Character").GetComponent<Image>().sprite = h.img_Stand[i];
                content.transform.Find("Icon").GetComponent<Image>().sprite = att.at_Icon;
                var star = content.transform.Find("Text_Star").GetComponent<Text>();
                star.text = "";
                for (var j = 0; j < charaData.mc_Rarity; j++) star.text += "★";
                content.transform.Find("Text_Name").GetComponent<Text>().text = $"{charaData.mc_Name}";
                content.transform.Find("Face").Find("Image").GetComponent<Image>().sprite = h.img_Face[i];

                var price = content.transform.Find("Text_Price").GetComponent<Text>();
                price.text = "魔法石:50";

                bool flag = false;
                var data = DataList.userData.user_DataCostume;
                switch (h.img_Id)
                {
                    case 1392:
                        if (data.cos_Gerbera[i] == true) flag = true;
                        break;
                    case 1135:
                        if (data.cos_Percival[i] == true) flag = true;
                        break;
                    case 1173:
                        if (data.cos_Odin[i] == true) flag = true;
                        break;
                    case 1090:
                        if (data.cos_Jormungand[i] == true) flag = true;
                        break;
                    case 1221:
                        if (data.cos_Frist[i] == true) flag = true;
                        break;
                    case 1345:
                        if (data.cos_Valkyrie[i] == true) flag = true;
                        break;
                }
                if (flag)
                {
                    content.GetComponent<Button>().enabled = false;
                    var errorObj = content.transform.Find("Impossible");
                    errorObj.gameObject.SetActive(true);
                }

                content.GetComponent<Button>().onClick.RemoveAllListeners();
                var num = i;
                content.GetComponent<Button>().onClick.AddListener(() => Confirmation_Cos(50, charaData.mc_Id, num));
            }
        }

        ChangeUI_Shop(displays[0]);
    }
    public void DisplayShop_MagicStone()
    {
        text_ItemName.text = "魔法石";
        ChangeUI_Shop(displays[1]);
    }
    #region キャラクター
    private void Confirmation_Chara(int value, int id)
    {
        confScreen.SetActive(true);

        var charaData = DataList.mcDatas.Find(x => x.mc_Id == id);

        var obj = confScreen.transform.Find("Success");

        if (value > DataList.userData.user_MagicStone)
        {
            obj.gameObject.SetActive(false);
            confScreen.transform.Find("Failure").gameObject.SetActive(true);
            return;
        }
        obj.gameObject.SetActive(true);
        confScreen.transform.Find("Failure").gameObject.SetActive(false);

        obj.Find("Text_Conf").GetComponent<Text>().text = "魔法石を消費してこのキャラクターを入手します";
        obj.Find("Text_SaleValue").GetComponent<Text>().text = $"消費:{value}";

        var content = obj.Find("Content_CharacterIcon");
        content.gameObject.SetActive(true);
        obj.Find("Content_ShopMagicStone").gameObject.SetActive(false);
        content.GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == charaData.mc_Id).img_Face;

        var att = data_Attribute.dataBases[charaData.mc_Attribute];

        var gradient = content.Find("Flame").GetComponent<UICornersGradient>();
        gradient.m_topLeftColor = att.at_Color1;
        gradient.m_topRightColor = att.at_Color2;
        gradient.m_bottomLeftColor = gradient.m_topRightColor;
        gradient.m_bottomRightColor = gradient.m_topLeftColor;

        content.Find("Icon").GetComponent<Image>().sprite = att.at_Icon;

        obj.Find("Button_Yes").GetComponent<Button>().onClick.RemoveAllListeners();
        obj.Find("Button_Yes").GetComponent<Button>().onClick.AddListener(() => Decision_Chara(value, id));
    }
    private void Decision_Chara(int value, int id)
    {
        loadingObj.SetActive(true);
        errorObj.SetActive(false);

        Data_Character chara = new Data_Character();

        chara.c_Id = id;

        chara.c_Level = 1;

        var sb = new System.Text.StringBuilder(12);

        for (int j = 0; j < 16; j++)
        {
            int pos = Random.Range(0, PASSWORD_CHARS.Length);
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

        DataList.userData.user_MagicStone -= value;

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
                confScreen.SetActive(false);
                loadingObj.SetActive(false);
                GetComponent<HomeUIDisplay>().SetHomeMenuUI();
                DisplayShop_Character();
            },
            error =>
            {
                errorObj.SetActive(true);
                loadingObj.SetActive(false);
                var button = errorObj.transform.Find("Button").GetComponent<ButtonExtention>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => Decision_Chara(value, id));
            });
    }
    #endregion
    #region コスチューム
    private void Confirmation_Cos(int value, int id, int num)
    {
        confScreen.SetActive(true);

        var charaData = DataList.mcDatas.Find(x => x.mc_Id == id);
        var cosData = data_AnotherCostume.dataLists.Find(x => x.img_Id == id);

        var obj = confScreen.transform.Find("Success");

        if (value > DataList.userData.user_MagicStone)
        {
            obj.gameObject.SetActive(false);
            confScreen.transform.Find("Failure").gameObject.SetActive(true);
            return;
        }
        obj.gameObject.SetActive(true);
        confScreen.transform.Find("Failure").gameObject.SetActive(false);

        obj.Find("Text_Conf").GetComponent<Text>().text = "魔法石を消費してこのコスチュームを入手します";
        obj.Find("Text_SaleValue").GetComponent<Text>().text = $"消費:{value}";

        var content = obj.Find("Content_CharacterIcon");
        obj.Find("Content_ShopMagicStone").gameObject.SetActive(false);
        content.gameObject.SetActive(true);
        content.GetComponent<Image>().sprite = cosData.img_Face[num];

        var att = data_Attribute.dataBases[charaData.mc_Attribute];

        var gradient = content.Find("Flame").GetComponent<UICornersGradient>();
        gradient.m_topLeftColor = att.at_Color1;
        gradient.m_topRightColor = att.at_Color2;
        gradient.m_bottomLeftColor = gradient.m_topRightColor;
        gradient.m_bottomRightColor = gradient.m_topLeftColor;

        content.Find("Icon").GetComponent<Image>().sprite = att.at_Icon;

        obj.Find("Button_Yes").GetComponent<Button>().onClick.RemoveAllListeners();
        obj.Find("Button_Yes").GetComponent<Button>().onClick.AddListener(() => Decision_Cos(value, id, num));
    }
    private void Decision_Cos(int value, int id, int num)
    {
        loadingObj.SetActive(true);
        errorObj.SetActive(false);

        var cosData = DataList.userData.user_DataCostume;
        switch (id)
        {
            case 1392:
                cosData.cos_Gerbera[num] = true;
                break;
            case 1135:
                cosData.cos_Percival[num] = true;
                break;
            case 1173:
                cosData.cos_Odin[num] = true;
                break;
            case 1090:
                cosData.cos_Jormungand[num] = true;
                break;
            case 1221:
                cosData.cos_Frist[num] = true;
                break;
            case 1345:
                cosData.cos_Valkyrie[num] = true;
                break;
        }

        DataList.userData.user_MagicStone -= value;

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
                confScreen.SetActive(false);
                loadingObj.SetActive(false);
                GetComponent<HomeUIDisplay>().SetHomeMenuUI();
                DisplayShop_Costume();
            },
            error =>
            {
                errorObj.SetActive(true);
                loadingObj.SetActive(false);
                var button = errorObj.transform.Find("Button").GetComponent<ButtonExtention>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => Decision_Cos(value, id,num));
            });
    }
    #endregion
}

