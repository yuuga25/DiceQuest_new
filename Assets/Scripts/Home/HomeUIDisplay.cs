using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PlayFab;

public class HomeUIDisplay : MonoBehaviour
{
    [Header("Data")]
    public ImageData_Character data_Character;
    public ImageData_AnotherCostume data_AnotherCostume;

    [Header("Text")]
    [SerializeField] private Text text_Rank;
    [SerializeField] private Text text_Name;
    [SerializeField] private Text text_NickName;
    [SerializeField] private Text text_UntilRankUp;
    [SerializeField] private Text text_Money;
    [SerializeField] private Text text_MagicStone;

    [Header("Image")]
    [SerializeField] private Image image_FavoriteChara;
    [SerializeField] private Image image_MainChara1;
    [SerializeField] private Image image_MainChara2;
    [SerializeField] private Image image_MainChara3;
    [SerializeField] private Image image_MainChara4;

    [Header("Display")]
    [SerializeField] private List<CanvasGroup> displays = new List<CanvasGroup>();

    private void Awake()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn()) UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
    }

    private void Start()
    {
        SetHomeMenuUI();
        foreach (var i in displays) { i.alpha = 0; i.gameObject.SetActive(false); }
        displays[0].gameObject.SetActive(true); displays[0].alpha = 1; 
    }

    /// <summary>
    /// ホーム画面のUI及びImage系統をセットする関数
    /// </summary>
    public void SetHomeMenuUI()
    {
        text_Rank.text = DataList.userData.user_Rank.ToString();
        text_Name.text = DataList.userData.user_Name;
        text_NickName.text = DataList.userData.user_NickName0 + DataList.userData.user_NickName1;
        text_UntilRankUp.text = DataList.userData.user_UntilRankUp.ToString();
        text_Money.text = DataList.userData.user_Money.ToString();
        text_MagicStone.text = DataList.userData.user_MagicStone.ToString();

        var mcData = DataList.mcDatas.Find(x => x.mc_Id == DataList.userData.user_FavoriteChara.c_Id);
        var chara = DataList.userData.user_FavoriteChara;
        if (mcData.mc_Costume)
        {
            image_FavoriteChara.sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == chara.c_Id).img_Face[chara.c_CostumeId];
        }
        else
        {
            image_FavoriteChara.sprite = data_Character.dataLists.Find(x => x.img_Id == chara.c_Id).img_Face;
        }

        for (int unit = 0; unit < DataList.userData.user_Formations.Length; unit++)
        {
            Sprite sprite = null;

            var unitData = DataList.userData.user_Character.Find(x => x.c_UniqueId == DataList.userData.user_Formations[unit]);
            var mc_UnitData = DataList.mcDatas.Find(x => x.mc_Id == unitData.c_Id);

            if (mc_UnitData.mc_Costume)
            {
                sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == unitData.c_Id).img_Stand[unitData.c_CostumeId];
            }
            else
            {
                sprite = data_Character.dataLists.Find(x => x.img_Id == unitData.c_Id).img_Stand;
            }

            switch (unit)
            {
                case 0: image_MainChara1.sprite = sprite; break;
                case 1: image_MainChara2.sprite = sprite; break;
                case 2: image_MainChara3.sprite = sprite; break;
                case 3: image_MainChara4.sprite = sprite; break;
            }
        }
    }

    public async void ChangeUI(CanvasGroup trueObject)
    {
        foreach(var i in displays)
        {
            if (i.gameObject.activeSelf && trueObject.gameObject != i.gameObject)
            {
                await i.DOFade(0, 0.25f).AsyncWaitForCompletion();
                i.gameObject.SetActive(false);
            }
            else if(!trueObject.gameObject.activeSelf)
            {
                trueObject.gameObject.SetActive(true);
                trueObject.alpha = 0;
            }
        }
        await trueObject.DOFade(1, 0.3f).AsyncWaitForCompletion();
    }
}
