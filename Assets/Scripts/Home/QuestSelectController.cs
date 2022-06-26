using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using Cysharp.Threading.Tasks;
using System.Linq;

public class QuestSelectController : MonoBehaviour
{
    [SerializeField] private ImageData_Character data_Character;
    [SerializeField] private ImageData_AnotherCostume data_AnotherCostume;
    [SerializeField] private Data_Attribute data_Attribute;

    [SerializeField] private List<CanvasGroup> displays = new List<CanvasGroup>();

    [SerializeField] private GameObject parent_Obj;
    [SerializeField] private GameObject content_Obj;

    [SerializeField] private GameObject obj_Loading;
    [SerializeField] private GameObject obj_Error;
    [SerializeField] private CanvasGroup obj_WhiteScreen;

    [Space(20)]
    [SerializeField] private string gameScene;
    [Header("確認画面")]
    [SerializeField] private List<GameObject> characterUnit = new List<GameObject>();
    [SerializeField] private Text text_QuestTitle;
    [SerializeField] private Text text_QuestChara;
    [SerializeField] private Button button_Confirmation;

    public async void ChangeUI_Quest(CanvasGroup trueObject)
    {
        foreach(var i in displays)
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

        await trueObject.DOFade(1, 0.3f).AsyncWaitForCompletion();
    }
    public void SetQuest()
    {
        #region クエストコンテンツ生成
        foreach (Transform obj in parent_Obj.transform) Destroy(obj.gameObject);

        foreach(var i in DataList.msDatas)
        {
            var content = Instantiate(content_Obj, parent_Obj.transform);

            var charaData = DataList.mcDatas.Find(x => x.mc_Id == i.stage_CharaId);

            var flame = content.transform.Find("Flame").GetComponent<UICornersGradient>();
            flame.m_topLeftColor = data_Attribute.dataBases[charaData.mc_Attribute].at_Color1;
            flame.m_topRightColor = data_Attribute.dataBases[charaData.mc_Attribute].at_Color2;
            flame.m_bottomLeftColor = flame.m_topRightColor;
            flame.m_bottomRightColor = flame.m_topLeftColor;

            content.transform.Find("Text_QuestName").GetComponent<Text>().text = $"{i.stage_Name}";
            var text_CharaName = content.transform.Find("Text_QuestChara").GetComponent<Text>();
            text_CharaName.text = $"{charaData.mc_Name}";
            var textGradient = text_CharaName.gameObject.GetComponent<UITextCornersGradient>();
            textGradient.m_topLeftColor = flame.m_topLeftColor;
            textGradient.m_topRightColor = flame.m_topRightColor;
            textGradient.m_bottomLeftColor = flame.m_topRightColor;
            textGradient.m_bottomRightColor = flame.m_topLeftColor;

            content.transform.Find("Text_QuestLevel").GetComponent<Text>().text = $"推奨レベル　Lv.{i.stage_Recommendation}";

            content.transform.Find("Icon").GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == charaData.mc_Id).img_Face;

            content.GetComponent<Button>().onClick.AddListener(() => SetConfirmation(i.stage_Id));
        }
        #endregion

        ChangeUI_Quest(displays[0]);
    }
    private void SetConfirmation(int stageNum)
    {
        for(var i = 0;i < 4; i++)
        {
            var iconImage = characterUnit[i].transform.Find("Icon/Image").GetComponent<Image>();
            var iconFlame = characterUnit[i].transform.Find("Icon/Flame").GetComponent<UICornersGradient>();
            var text_CharaName = characterUnit[i].transform.Find("Text_CharaName").GetComponent<Text>();
            var text_Count = characterUnit[i].transform.Find("Text_Count").GetComponent<Text>();
            var text_Status = characterUnit[i].transform.Find("Text_Status").GetComponent<Text>();

            var unit = DataList.userData.user_Character.Find(x=>x.c_UniqueId == DataList.userData.user_Formations[i]);
            var md_Unit = DataList.mcDatas.Find(x => x.mc_Id == unit.c_Id);

            if (md_Unit.mc_Costume)
            {
                iconImage.sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == unit.c_Id).img_Face[unit.c_CostumeId];
            }
            else
            {
                iconImage.sprite = data_Character.dataLists.Find(x => x.img_Id == unit.c_Id).img_Face;
            }

            iconFlame.m_topLeftColor = data_Attribute.dataBases[md_Unit.mc_Attribute].at_Color1;
            iconFlame.m_topRightColor = data_Attribute.dataBases[md_Unit.mc_Attribute].at_Color2;
            iconFlame.m_bottomLeftColor = iconFlame.m_topRightColor;
            iconFlame.m_bottomRightColor = iconFlame.m_topLeftColor;

            text_CharaName.text = md_Unit.mc_Name;
            text_Count.text = $"{i + 1}";
            text_Status.text = $"HP：{unit.c_HP}　攻撃力：{unit.c_Attack}　防御力：{unit.c_Defense}";
        }

        text_QuestTitle.text = DataList.msDatas.Find(x => x.stage_Id == stageNum).stage_Name;
        text_QuestChara.text = "VS" + DataList.mcDatas.Find(x => x.mc_Id == DataList.msDatas.Find(X => X.stage_Id == stageNum).stage_CharaId).mc_Name;

        button_Confirmation.onClick.RemoveAllListeners();
        button_Confirmation.onClick.AddListener(() => StartQuest(stageNum));

        ChangeUI_Quest(displays[1]);
    }

    private async void StartQuest(int stageNum)
    {
        obj_Loading.SetActive(true);
        for (var i = 0; i < DataList.userData.user_Formations.Length; i++)
        {
            await UniTask.Delay(100);
            var unit = DataList.userData.user_Character.Find(x => x.c_UniqueId == DataList.userData.user_Formations[i]);
            unit.c_Uses++;
            DataList_Battle.unit.Add(unit);
        }

        DataList_Battle.id_Quest = stageNum;

        LoadScene();
    }

    private void LoadScene()
    {
        var asyncLoadScene = SceneManager.LoadSceneAsync(gameScene);
        asyncLoadScene.allowSceneActivation = false;

        PlayFabClientAPI.UpdateUserData(
            new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>()
                {
                    {"PlayerData", PlayFabSimpleJson.SerializeObject(DataList.userData)}
                }
            },
            async result =>
            {
                obj_Loading.SetActive(false);
                obj_WhiteScreen.gameObject.SetActive(true);
                obj_WhiteScreen.alpha = 0;
                obj_WhiteScreen.DOFade(1, 1);
                await UniTask.Delay(1000);
                asyncLoadScene.allowSceneActivation = true;
            },
            error =>
            {
                obj_Error.SetActive(true);
                obj_Loading.SetActive(false);
                var button = obj_Error.transform.Find("Button").GetComponent<ButtonExtention>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => LoadScene());
            });
    }

}
