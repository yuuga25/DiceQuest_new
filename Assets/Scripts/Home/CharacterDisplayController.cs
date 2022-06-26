using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using Cysharp.Threading.Tasks;

public class CharacterDisplayController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ImageData_Character data_Character;
    [SerializeField] private ImageData_AnotherCostume data_AnotherCostume;
    [SerializeField] private Data_Attribute data_Attribute;

    [Header("UI")]
    [SerializeField] private GameObject loadingObj;
    [SerializeField] private GameObject errorObj;

    #region 編成画面
    [Header("編成画面UI")]
    [SerializeField] private List<Image> fo_StandImages = new List<Image>();
    [SerializeField] private List<GameObject> fo_IconImages = new List<GameObject>();
    [SerializeField] private List<CanvasGroup> displays = new List<CanvasGroup>();
    [Space(20)]
    [SerializeField] private GameObject fo_Parent_CharaList;
    [SerializeField] private GameObject fo_Content_CharaList;
    [SerializeField] private List<GameObject> covers = new List<GameObject>();
    [SerializeField] private Text fo_Text_CenterMessage;
    [Space(20)]
    [SerializeField] private List<GameObject> fo_Obj_FormationChara = new List<GameObject>();
    [SerializeField] private Text fo_Text_HpSum;
    [SerializeField] private Text fo_Text_DPSum;
    [Space(20)]
    [SerializeField] private Dropdown fo_Dropdown;
    [SerializeField] private InputField fo_InputField;
    [SerializeField] private Text fo_Text_Count;
    #endregion
    #region 強化
    [Header("強化")]
    [SerializeField] private GameObject re_Obj_Parent;
    [SerializeField] private Dropdown re_Dropdown;
    [SerializeField] private InputField re_InputField;
    [SerializeField] private Text re_Text_Count;
    [Space(20)]
    [SerializeField] private GameObject en_Obj_Parent;
    [SerializeField] private GameObject en_Obj_Content_N;
    [SerializeField] private Text en_Text_Cost;
    [SerializeField] private Text en_Text_Exp;
    [SerializeField] private GameObject en_Obj_Chara;
    [SerializeField] private Text en_Text_Lv;
    [SerializeField] private Text en_Text_Attack;
    [SerializeField] private Text en_Text_Defense;
    [SerializeField] private Text en_Text_HP;
    [SerializeField] private Text en_Text_ExpValue;
    [SerializeField] private Slider en_Slider_Exp;
    [SerializeField] private Button en_Button_Decision;
    [Space(20)]
    [SerializeField] private GameObject ms_Obj_Parent;
    [SerializeField] private Dropdown ms_Dropdown;
    [SerializeField] private InputField ms_Inputfield;
    [SerializeField] private Slider ms_Slider;
    [SerializeField] private Text ms_Text_Cost;
    [SerializeField] private Text ms_Text_Exp;
    [SerializeField] private Text ms_Text_ExpValue;
    [SerializeField] private Text ms_Text_Level;
    [SerializeField] private Text ms_Text_Count;
    [Space(20)]
    [SerializeField] private GameObject re_Conf;
    [SerializeField] private GameObject re_Conf_Error;
    [SerializeField] private GameObject ec_Obj_Parent;
    [SerializeField] private Text ec_Text_Cost;
    [SerializeField] private Text ec_Text_Conf_Error;
    [SerializeField] private Text ec_Text_Cost_Error;
    #endregion
    #region キャラ一覧
    [Header("キャラ一覧")]
    [SerializeField] private GameObject cl_Obj_Parent;
    [SerializeField] private GameObject cl_Obj_Content;
    [SerializeField] private Dropdown cl_Dropdown;
    [SerializeField] private InputField cl_InputField;
    [SerializeField] private Text cl_Text_Count;
    #endregion
    #region キャラクターステータス
    [Header("キャラクターステータスUI")]
    [SerializeField] private List<Image> cs_Images_Stand = new List<Image>();
    [SerializeField] private Image cs_Image_Face;
    [SerializeField] private Image cs_Image_Icon;
    [SerializeField] private UICornersGradient cs_UICorners;
    [Space(20)]
    [SerializeField] private Text cs_Text_Name;
    [SerializeField] private Text cs_Text_Star;
    [SerializeField] private Text cs_Text_BattleType;
    [SerializeField] private Text cs_Text_DicePower;
    [SerializeField] private Text cs_Text_Level;
    [SerializeField] private Text cs_Text_UntilLevelUp;
    [SerializeField] private Text cs_Text_Uses;
    [SerializeField] private Text cs_Text_SkillName;
    [SerializeField] private Text cs_Text_SkillExplanation;
    [Space(20)]
    [SerializeField] private Slider cs_Slider_Attack;
    [SerializeField] private Slider cs_Slider_Defense;
    [SerializeField] private Slider cs_Slider_HP;
    [SerializeField] private Text cs_Text_Attack;
    [SerializeField] private Text cs_Text_Defense;
    [SerializeField] private Text cs_Text_HP;
    [Space(20)]
    [SerializeField] private GameObject cs_Obj_Ability1;
    [SerializeField] private GameObject cs_Obj_Ability2;
    [SerializeField] private GameObject cs_Obj_Ability3;
    [Space(20)]
    [SerializeField] private Button cs_Button_CostumeChange;
    [SerializeField] private Button cs_Button_Back;
    private CanvasGroup oneBeforeScreen;
    #endregion
    #region コスチュームチェンジ
    [Header("コスチュームチェンジUI")]
    [SerializeField] private GameObject cc_Obj_Parent;
    [SerializeField] private GameObject cc_Obj_Content;
    [SerializeField] private Image cc_Img_Stand;
    [SerializeField] private Button cc_Button_Decision;
    #endregion
    #region 売却
    [Header("売却")]
    [SerializeField] private GameObject sale_Obj_Parent;
    [SerializeField] private GameObject sale_Obj_Content;
    [SerializeField] private GameObject sale_Obj_Conf;
    [SerializeField] private Dropdown sale_Dropdown;
    [SerializeField] private InputField sale_InputField;
    [SerializeField] private GameObject saleConf_Obj_Parent;
    [SerializeField] private GameObject saleConf_Obj_Content;
    [SerializeField] private Text sale_Text_Count;
    #endregion
    #region 図鑑
    [Header("図鑑")]
    [SerializeField] private GameObject pb_Obj_Parent;
    [SerializeField] private GameObject pb_Obj_Content;
    [SerializeField] private Dropdown pb_DropDown;
    [SerializeField] private InputField pb_Inputfield;
    #endregion

    #region 全体
    /// <summary>
    /// キャラクター画面のImageたちをセットする
    /// </summary>
    public void SetCharacterMenu()
    {
        for (var i = 0; i < 4; i++)
        {
            var unit = DataList.userData.user_Character.Find(x => x.c_UniqueId == DataList.userData.user_Formations[i]);
            var mcData = DataList.mcDatas.Find(x => x.mc_Id == unit.c_Id);

            var gradient = fo_IconImages[i].transform.Find("Flame").GetComponent<UICornersGradient>();
            var icon = fo_IconImages[i].transform.Find("Image").GetComponent<Image>();

            gradient.m_topLeftColor = data_Attribute.dataBases[mcData.mc_Attribute].at_Color1;
            gradient.m_topRightColor = data_Attribute.dataBases[mcData.mc_Attribute].at_Color2;
            gradient.m_bottomRightColor = gradient.m_topLeftColor;
            gradient.m_bottomLeftColor = gradient.m_topRightColor;

            icon.sprite = data_Attribute.dataBases[mcData.mc_Attribute].at_Icon;

            if (mcData.mc_Costume)
            {
                fo_StandImages[i].sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mcData.mc_Id).img_Stand[unit.c_CostumeId];
                fo_IconImages[i].GetComponent<Image>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mcData.mc_Id).img_Face[unit.c_CostumeId];
            }
            else
            {
                fo_StandImages[i].sprite = data_Character.dataLists.Find(x => x.img_Id == mcData.mc_Id).img_Stand;
                fo_IconImages[i].GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == mcData.mc_Id).img_Face;
            }

            fo_IconImages[i].GetComponent<ButtonExtention>().onLongPress.RemoveAllListeners();
            fo_IconImages[i].GetComponent<ButtonExtention>().onLongPress.AddListener(() => TopToCharacterStatus(unit.c_UniqueId));
        }

        ChangeUI_Character(displays[0]);
    }
    public void TopToCharacterStatus(string uniqueId)
    {
        selectCharaUniqueId = uniqueId;
        oneBeforeScreen = displays[0];
        DisplayCharaStatus(uniqueId);
    }
    /// <summary>
    /// キャラクター画面のUIを切り替える
    /// </summary>
    /// <param name="trueObject"></param>
    public async void ChangeUI_Character(CanvasGroup trueObject)
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

        if(trueObject.gameObject == displays[1].gameObject) DisplayFormation();
        if (trueObject.gameObject == displays[4].gameObject) DisplayCharacterList();
        if (trueObject.gameObject == displays[2].gameObject) DisplayReinforcement();
        if (trueObject.gameObject == displays[8].gameObject) DisplayEnhanced(ms_SelectCharaUniqueId);
        if (trueObject.gameObject == displays[9].gameObject) DisplayMaterialSelection(ms_SelectCharaUniqueId);

        await trueObject.DOFade(1, 0.3f).AsyncWaitForCompletion();
    }
    #endregion
    #region 編成
    public void DisplayFormation()
    {
        fo_Text_Count.text = $"{DataList.userData.user_Character.Count}/200";

        fo_Dropdown.onValueChanged.RemoveAllListeners();
        fo_Dropdown.onValueChanged.AddListener((x) => DisplayFormation());
        fo_InputField.onValueChanged.RemoveAllListeners();
        fo_InputField.onValueChanged.AddListener((x) => DisplayFormation());

        List<Data_Character> aa = new List<Data_Character>();
        var a = DataList.userData.user_Character.OrderBy(x => x.c_Attribute);
        foreach (var i in a) aa.Add(i);
        DataList.userData.user_Character.Clear();
        foreach (var i in aa) DataList.userData.user_Character.Add(i);

        List<Data_Character> chData = new List<Data_Character>();
        List<MasterData_Character> mcData = new List<MasterData_Character>();
        List<MasterData_Character> provisional = new List<MasterData_Character>();
        for (var e = 0; e < DataList.userData.user_Character.Count; e++)
        {
            for (var h = 0; h < DataList.mcDatas.Count; h++)
            {
                if (DataList.userData.user_Character[e].c_Id == DataList.mcDatas[h].mc_Id)
                {
                    provisional.Add(DataList.mcDatas[h]);
                }
            }
        }
        var hoge = provisional.OrderBy(x => x.mc_Attribute);
        foreach (var hh in hoge) mcData.Add(hh);

        switch (fo_Dropdown.value)
        {
            case 0:
                var list = DataList.userData.user_Character.OrderBy(x => x.c_Attribute);
                foreach (var i in list) chData.Add(i);
                break;
            case 1:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Level);
                foreach (var i in list) chData.Add(i);
                break;
            case 2:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Attack);
                foreach (var i in list) chData.Add(i);
                break;
            case 3:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Defense);
                foreach (var i in list) chData.Add(i);
                break;
            case 4:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_HP);
                foreach (var i in list) chData.Add(i);
                break;
            case 5:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_DicePower);
                foreach (var i in list) chData.Add(i);
                break;
            case 6:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Rarity);
                foreach (var i in list) chData.Add(i);
                break;
            case 7:
                list = DataList.userData.user_Character.OrderBy(x => x.c_BattleType);
                foreach (var i in list) chData.Add(i);
                break;
        }

        foreach (Transform obj in fo_Parent_CharaList.transform)
        {
            Destroy(obj.gameObject);
        }

        for(var i = 0;i < DataList.userData.user_Character.Count; i++)
        {
            var unitData = chData[i];
            var mc_UnitData = DataList.mcDatas.Find(x => x.mc_Id == unitData.c_Id);

            if (!mc_UnitData.mc_Name.Contains(fo_InputField.text))
            {
                continue;
            }

            var content = Instantiate(fo_Content_CharaList, fo_Parent_CharaList.transform);

            var Flame = content.transform.Find("Flame").GetComponent<UICornersGradient>();
            var Icon = content.transform.Find("Icon").GetComponent<Image>();
            var text_Level = content.transform.Find("Text").GetComponent<Text>();
            var Number = content.transform.Find("Number");

            Flame.m_topLeftColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color1;
            Flame.m_topRightColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color2;
            Flame.m_bottomLeftColor = Flame.m_topRightColor;
            Flame.m_bottomRightColor = Flame.m_topLeftColor;

            Icon.sprite = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Icon;

            switch (fo_Dropdown.value)
            {
                case 0:
                case 1:
                    text_Level.text = $"Lv.{unitData.c_Level}";
                    break;
                case 2: text_Level.text = $"Atk.{unitData.c_Attack}"; break;
                case 3: text_Level.text = $"Def.{unitData.c_Defense}"; break;
                case 4: text_Level.text = $"Hp.{unitData.c_HP}"; break;
                case 5: text_Level.text = $"DP.{mc_UnitData.mc_DicePower}"; break;
                case 6:
                    var star = "";
                    for (var j = 0; j < mc_UnitData.mc_Rarity; j++)
                    {
                        star += "★";
                    }
                    text_Level.text = $"{star}";
                    break;
                case 7:
                    var battleType = "";
                    switch (mc_UnitData.mc_BattleType)
                    {
                        case 0: battleType = "バランス"; break;
                        case 1: battleType = "アタッカー"; break;
                        case 2: battleType = "ディフェンス"; break;
                    }
                    text_Level.text = $"{battleType}";
                    break;
            }

            for (int j = 0; j < DataList.userData.user_Formations.Length; j++)
            {
                if(unitData.c_UniqueId == DataList.userData.user_Formations[j])
                {
                    Number.gameObject.SetActive(true);
                    Number.Find("Text_Number").GetComponent<Text>().text = $"{j + 1}";
                    break;
                }
                else
                {
                    Number.gameObject.SetActive(false);
                }
            }

            if (mc_UnitData.mc_Costume)
            {
                content.GetComponent<Image>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face[unitData.c_CostumeId];
            }
            else
            {
                content.GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face;
            }

            var button = content.GetComponent<ButtonExtention>();
            button.onClick.AddListener(() => { ShortPress(unitData.c_UniqueId); });
            button.onLongPress.AddListener(() => { LongPress(unitData.c_UniqueId); });
        }

        covers[0].SetActive(true);
        covers[1].SetActive(false);
        fo_Dropdown.gameObject.SetActive(true);
        fo_InputField.gameObject.SetActive(true);

        fo_Text_CenterMessage.text = $"入れ替えるキャラクターを一覧から選択してください";
        oneBeforeScreen = displays[1];

        int hpSum = 0;
        int dpSum = 0;

        for(var i = 0; i < fo_Obj_FormationChara.Count; i++)
        {
            var unitData = DataList.userData.user_Character.Find(x => x.c_UniqueId == DataList.userData.user_Formations[i]);
            var mc_UnitData = DataList.mcDatas.Find(x => x.mc_Id == unitData.c_Id);

            var corner = fo_Obj_FormationChara[i].transform.Find("Flame").GetComponent<UICornersGradient>();
            corner.m_topLeftColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color1;
            corner.m_topRightColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color2;
            corner.m_bottomLeftColor = corner.m_topRightColor;
            corner.m_bottomRightColor = corner.m_topLeftColor;
            corner.enabled = false;
            corner.enabled = true;

            fo_Obj_FormationChara[i].transform.Find("Image").GetComponent<Image>().sprite = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Icon;

            if (mc_UnitData.mc_Costume)
            {
                fo_Obj_FormationChara[i].GetComponent<Image>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face[unitData.c_CostumeId];
            }
            else
            {
                fo_Obj_FormationChara[i].GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face;
            }

            hpSum += unitData.c_HP;
            dpSum += unitData.c_DicePower;

            var button = fo_Obj_FormationChara[i].GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => ReplaceCharaSelect(unitData.c_UniqueId));
        }
        fo_Text_HpSum.text = $"{hpSum}";
        if (dpSum > 6)
        {
            dpSum = 6;
            fo_Text_DPSum.color = new Color(1, 0.6682344f, 0.3915094f);
        }
        else fo_Text_DPSum.color = new Color(0.1960784f, 0.1960784f, 0.1960784f);
        fo_Text_DPSum.text = $"{dpSum}";
    }
    public void ClickListChara()
    {
        covers[1].SetActive(true);
        covers[0].SetActive(false);
        fo_Dropdown.gameObject.SetActive(false);
        fo_InputField.gameObject.SetActive(false);
        fo_Text_CenterMessage.text = $"入れ替えるキャラクターを編成から選択してください";
    }
    public void ReplaceCharaSelect(string unitUniqueId)
    {
        int isTeamNum = -1;
        int inTeam = -1;

        for (var i = 0; i < DataList.userData.user_Formations.Length; i++)
        {
            if (unitUniqueId == DataList.userData.user_Formations[i])
            {
                isTeamNum = i;
            }
            if (selectCharaUniqueId == DataList.userData.user_Formations[i])
            {
                inTeam = i;
            }
        }
        if (inTeam > -1)
        {
            (DataList.userData.user_Formations[isTeamNum], DataList.userData.user_Formations[inTeam]) = (DataList.userData.user_Formations[inTeam], DataList.userData.user_Formations[isTeamNum]);
        }
        else if (isTeamNum > -1)
        {
            (DataList.userData.user_Formations[isTeamNum], selectCharaUniqueId) = (selectCharaUniqueId, DataList.userData.user_Formations[isTeamNum]);
        }

        loadingObj.SetActive(true);
        errorObj.SetActive(false);

        DataList.userData.user_FavoriteChara = DataList.userData.user_Character.Find(x => x.c_UniqueId == DataList.userData.user_Formations[0]);

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
                this.gameObject.GetComponent<HomeUIDisplay>().SetHomeMenuUI();
                DisplayFormation();
            },
            error =>
            {
                errorObj.SetActive(true);
                loadingObj.SetActive(false);
                var button = errorObj.transform.Find("Button").GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => ReplaceCharaSelect(unitUniqueId));
            });
    }

    private List<String> batchList = new List<string>();
    public void BatchSelection()
    {
        fo_Dropdown.onValueChanged.RemoveAllListeners();
        fo_Dropdown.onValueChanged.AddListener((x) => BatchSelection());
        fo_InputField.onValueChanged.RemoveAllListeners();
        fo_InputField.onValueChanged.AddListener((x) => BatchSelection());

        List<Data_Character> chData = new List<Data_Character>();
        List<MasterData_Character> mcData = new List<MasterData_Character>();
        List<MasterData_Character> provisional = new List<MasterData_Character>();
        for (var e = 0; e < DataList.userData.user_Character.Count; e++)
        {
            for (var h = 0; h < DataList.mcDatas.Count; h++)
            {
                if (DataList.userData.user_Character[e].c_Id == DataList.mcDatas[h].mc_Id)
                {
                    provisional.Add(DataList.mcDatas[h]);
                }
            }
        }
        var hoge = provisional.OrderBy(x => x.mc_Attribute);
        foreach (var hh in hoge) mcData.Add(hh);

        switch (fo_Dropdown.value)
        {
            case 0:
                var list = DataList.userData.user_Character.OrderBy(x => x.c_Attribute);
                foreach (var i in list) chData.Add(i);
                break;
            case 1:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Level);
                foreach (var i in list) chData.Add(i);
                break;
            case 2:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Attack);
                foreach (var i in list) chData.Add(i);
                break;
            case 3:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Defense);
                foreach (var i in list) chData.Add(i);
                break;
            case 4:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_HP);
                foreach (var i in list) chData.Add(i);
                break;
            case 5:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_DicePower);
                foreach (var i in list) chData.Add(i);
                break;
            case 6:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Rarity);
                foreach (var i in list) chData.Add(i);
                break;
            case 7:
                list = DataList.userData.user_Character.OrderBy(x => x.c_BattleType);
                foreach (var i in list) chData.Add(i);
                break;
        }

        foreach (Transform obj in fo_Parent_CharaList.transform) Destroy(obj.gameObject);
        for (var i = 0; i < DataList.userData.user_Character.Count; i++)
        {
            var unitData = chData[i];
            var mc_UnitData = DataList.mcDatas.Find(x => x.mc_Id == unitData.c_Id);

            if (!mc_UnitData.mc_Name.Contains(fo_InputField.text) && fo_InputField.text != null)
            {
                continue;
            }

            var content = Instantiate(fo_Content_CharaList, fo_Parent_CharaList.transform);

            var Flame = content.transform.Find("Flame").GetComponent<UICornersGradient>();
            var Icon = content.transform.Find("Icon").GetComponent<Image>();
            var text_Level = content.transform.Find("Text").GetComponent<Text>();
            content.transform.Find("Number").gameObject.SetActive(false);

            Flame.m_topLeftColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color1;
            Flame.m_topRightColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color2;
            Flame.m_bottomLeftColor = Flame.m_topRightColor;
            Flame.m_bottomRightColor = Flame.m_topLeftColor;

            Icon.sprite = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Icon;

            switch (fo_Dropdown.value)
            {
                case 0:
                case 1:
                    text_Level.text = $"Lv.{unitData.c_Level}";
                    break;
                case 2: text_Level.text = $"Atk.{unitData.c_Attack}"; break;
                case 3: text_Level.text = $"Def.{unitData.c_Defense}"; break;
                case 4: text_Level.text = $"Hp.{unitData.c_HP}"; break;
                case 5: text_Level.text = $"DP.{mc_UnitData.mc_DicePower}"; break;
                case 6:
                    var star = "";
                    for (var j = 0; j < mc_UnitData.mc_Rarity; j++)
                    {
                        star += "★";
                    }
                    text_Level.text = $"{star}";
                    break;
                case 7:
                    var battleType = "";
                    switch (mc_UnitData.mc_BattleType)
                    {
                        case 0: battleType = "Balance"; break;
                        case 1: battleType = "Attacker"; break;
                        case 2: battleType = "Defense"; break;
                    }
                    text_Level.text = $"{battleType}";
                    break;
            }

            if (mc_UnitData.mc_Costume)
            {
                content.GetComponent<Image>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face[unitData.c_CostumeId];
            }
            else
            {
                content.GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face;
            }

            var button = content.GetComponent<ButtonExtention>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectFormation(unitData.c_UniqueId));

            for (var k = 0; k < batchList.Count; k++)
            {
                if (unitData.c_UniqueId == batchList[k])
                {
                    content.transform.Find("Number").gameObject.SetActive(true);
                    content.transform.Find("Number").Find("Text_Number").GetComponent<Text>().text = $"{k + 1}";
                }
            }
        }

        fo_Text_CenterMessage.text = $"一括選択中\nキャラクターを４体選択してください";
    }

    private string selectCharaUniqueId;
    void LongPress(string uniqueId)
    {
        selectCharaUniqueId = uniqueId;
        DisplayCharaStatus(selectCharaUniqueId);
    }
    void ShortPress(string uniqueId)
    {
        selectCharaUniqueId = uniqueId;
        ClickListChara();
    }
    private void SelectFormation(string uniqueId)
    {
        if (batchList.Contains(uniqueId))
        {
            batchList.Remove(uniqueId);
            BatchSelection();
        }
        else
        {
            if(batchList.Count >= 3)
            {
                batchList.Add(uniqueId);
                DataList.userData.user_Formations = new string[4];
                for(var i = 0; i < batchList.Count; i++)
                {
                    DataList.userData.user_Formations[i] = batchList[i];
                }

                loadingObj.SetActive(true);
                errorObj.SetActive(false);

                DataList.userData.user_FavoriteChara = DataList.userData.user_Character.Find(x => x.c_UniqueId == DataList.userData.user_Formations[0]);

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
                        batchList = new List<string>();
                        fo_InputField.text = "";
                        DisplayFormation();
                        this.gameObject.GetComponent<HomeUIDisplay>().SetHomeMenuUI();
                    },
                    error =>
                    {
                        errorObj.SetActive(true);
                        loadingObj.SetActive(false);
                        var button = errorObj.transform.Find("Button").GetComponent<Button>();
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(() => SelectFormation(uniqueId));
                    });
            }
            else
            {
                batchList.Add(uniqueId);
                BatchSelection();
            }
        }
    }
    public void ClearBatchSelectionList()
    {
        batchList.Clear();
        fo_Dropdown.value = 0;
    }
    #endregion
    #region 強化
    public void DisplayReinforcement()
    {
        re_Text_Count.text = $"{DataList.userData.user_Character.Count}/200";

        List<Data_Character> aa = new List<Data_Character>();
        var a = DataList.userData.user_Character.OrderBy(x => x.c_Attribute);
        foreach (var i in a) aa.Add(i);
        DataList.userData.user_Character.Clear();
        foreach (var i in aa) DataList.userData.user_Character.Add(i);

        List<Data_Character> chData = new List<Data_Character>();
        List<MasterData_Character> mcData = new List<MasterData_Character>();
        List<MasterData_Character> provisional = new List<MasterData_Character>();
        for (var e = 0; e < DataList.userData.user_Character.Count; e++)
        {
            for (var h = 0; h < DataList.mcDatas.Count; h++)
            {
                if (DataList.userData.user_Character[e].c_Id == DataList.mcDatas[h].mc_Id)
                {
                    provisional.Add(DataList.mcDatas[h]);
                }
            }
        }
        var hoge = provisional.OrderBy(x => x.mc_Attribute);
        foreach (var hh in hoge) mcData.Add(hh);

        switch (re_Dropdown.value)
        {
            case 0:
                var list = DataList.userData.user_Character.OrderBy(x => x.c_Attribute);
                foreach (var i in list) chData.Add(i);
                break;
            case 1:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Level);
                foreach (var i in list) chData.Add(i);
                break;
            case 2:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Attack);
                foreach (var i in list) chData.Add(i);
                break;
            case 3:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Defense);
                foreach (var i in list) chData.Add(i);
                break;
            case 4:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_HP);
                foreach (var i in list) chData.Add(i);
                break;
            case 5:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_DicePower);
                foreach (var i in list) chData.Add(i);
                break;
            case 6:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Rarity);
                foreach (var i in list) chData.Add(i);
                break;
            case 7:
                list = DataList.userData.user_Character.OrderBy(x => x.c_BattleType);
                foreach (var i in list) chData.Add(i);
                break;
        }

        foreach (Transform obj in re_Obj_Parent.transform) Destroy(obj.gameObject);

        for (var i = 0; i < DataList.userData.user_Character.Count; i++)
        {
            var unitData = chData[i];
            var mc_UnitData = DataList.mcDatas.Find(x => x.mc_Id == unitData.c_Id);

            if (!mc_UnitData.mc_Name.Contains(re_InputField.text))
            {
                continue;
            }

            var content = Instantiate(cl_Obj_Content, re_Obj_Parent.transform);

            var Flame = content.transform.Find("Flame").GetComponent<UICornersGradient>();
            var Icon = content.transform.Find("Icon").GetComponent<Image>();
            var text_Level = content.transform.Find("Text").GetComponent<Text>();
            var Number = content.transform.Find("Number");

            Flame.m_topLeftColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color1;
            Flame.m_topRightColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color2;
            Flame.m_bottomLeftColor = Flame.m_topRightColor;
            Flame.m_bottomRightColor = Flame.m_topLeftColor;

            Icon.sprite = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Icon;

            switch (re_Dropdown.value)
            {
                case 0:
                case 1:
                    text_Level.text = $"Lv.{unitData.c_Level}";
                    break;
                case 2: text_Level.text = $"Atk.{unitData.c_Attack}"; break;
                case 3: text_Level.text = $"Def.{unitData.c_Defense}"; break;
                case 4: text_Level.text = $"Hp.{unitData.c_HP}"; break;
                case 5: text_Level.text = $"DP.{mc_UnitData.mc_DicePower}"; break;
                case 6:
                    var star = "";
                    for (var j = 0; j < mc_UnitData.mc_Rarity; j++)
                    {
                        star += "★";
                    }
                    text_Level.text = $"{star}";
                    break;
                case 7:
                    var battleType = "";
                    switch (mc_UnitData.mc_BattleType)
                    {
                        case 0: battleType = "バランス"; break;
                        case 1: battleType = "アタッカー"; break;
                        case 2: battleType = "ディフェンス"; break;
                    }
                    text_Level.text = $"{battleType}";
                    break;
            }

            for (int j = 0; j < DataList.userData.user_Formations.Length; j++)
            {
                if (unitData.c_UniqueId == DataList.userData.user_Formations[j])
                {
                    Number.gameObject.SetActive(true);
                    Number.Find("Text_Number").GetComponent<Text>().text = $"{j + 1}";
                    break;
                }
                else
                {
                    Number.gameObject.SetActive(false);
                }
            }

            if (mc_UnitData.mc_Costume)
            {
                content.GetComponent<Image>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face[unitData.c_CostumeId];
            }
            else
            {
                content.GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face;
            }

            var button = content.GetComponent<ButtonExtention>();
            button.onLongPress.AddListener(() => { LongPress(unitData.c_UniqueId); });

            if (unitData.c_Level >= 100)
            {
                content.transform.Find("Image_Max").gameObject.SetActive(true);
            }
            else
            {
                button.onClick.AddListener(() => { DisplayEnhanced(unitData.c_UniqueId); ChangeUI_Character(displays[8]); });
            }

            oneBeforeScreen = displays[2];
        }

        materialList.Clear();
    }
    private List<string> materialList = new List<string>();
    private void DisplayEnhanced(string uniqueId)
    {
        var unitData = DataList.userData.user_Character.Find(x => x.c_UniqueId == uniqueId);
        var mc_UnitData = DataList.mcDatas.Find(x => x.mc_Id == unitData.c_Id);

        en_Obj_Chara.transform.Find("Icon").GetComponent<Image>().sprite = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Icon;
        var Flame = en_Obj_Chara.transform.Find("Flame").GetComponent<UICornersGradient>();

        if (mc_UnitData.mc_Costume)
        {
            en_Obj_Chara.GetComponent<Image>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face[unitData.c_CostumeId];
        }
        else
        {
            en_Obj_Chara.GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face;
        }

        Flame.m_topLeftColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color1;
        Flame.m_topRightColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color2;
        Flame.m_bottomLeftColor = Flame.m_topRightColor;
        Flame.m_bottomRightColor = Flame.m_topLeftColor;

        int expPoint = 0;
        int requiredAmount = 0;

        for (var i = 0; i < materialList.Count; i++)
        {
            var chara = DataList.userData.user_Character.Find(x => x.c_UniqueId == materialList[i]);

            expPoint += Mathf.FloorToInt((chara.c_Level + chara.c_Rarity) * (1500 * chara.c_Rarity));
            requiredAmount += Mathf.FloorToInt(expPoint / (chara.c_Rarity * 10));
        }
        en_Text_Exp.text = $"{expPoint}";
        en_Text_Cost.text = $"{requiredAmount}";

        expPoint += unitData.c_ExpPoint;

        int levelUpCount = 0;   //レベルの上昇値
        int untilLevelUp = 0;   //expPointとの比較用

        untilLevelUp = Mathf.FloorToInt((100 + (50 - mc_UnitData.mc_RiseValue)) * 0.01f * (unitData.c_Level) * (100 + (50 - mc_UnitData.mc_RiseValue)));
        print($"untilLevelUp:Sum:{untilLevelUp}");

        while(true)
        {
            expPoint -= untilLevelUp;
            if(expPoint < 0)
            {
                expPoint += untilLevelUp;
                break;
            }
            else
            {
                levelUpCount++;
                untilLevelUp = Mathf.FloorToInt((100 + (50 - mc_UnitData.mc_RiseValue)) * 0.01f * (unitData.c_Level + levelUpCount) * (100 + (50 - mc_UnitData.mc_RiseValue)));
            }
        }
        print($"levelUpCount{levelUpCount}:untilLevelUp:Sum{untilLevelUp}");
        en_Text_Lv.text = $"Lv.{unitData.c_Level} ➡ {unitData.c_Level + levelUpCount}　+{levelUpCount}";

        en_Slider_Exp.maxValue = untilLevelUp;
        en_Slider_Exp.minValue = untilLevelUp * -0.075f;
        en_Slider_Exp.value = expPoint;
        en_Text_ExpValue.text = $"{untilLevelUp - expPoint}";
        
        int min = 0;
        min = Mathf.FloorToInt((100 + (50 - mc_UnitData.mc_RiseValue)) * 0.01f * (unitData.c_Level - 1) * (100 + (50 - mc_UnitData.mc_RiseValue)));

        if (unitData.c_Level + levelUpCount >= 100)
        {
            en_Text_Lv.text = $"Lv.{unitData.c_Level} ➡ MAX　+{levelUpCount}";
            en_Text_ExpValue.text = $"---";
            en_Slider_Exp.value = en_Slider_Exp.maxValue;
        }
        if (min == unitData.c_ExpPoint)
        {
            en_Slider_Exp.value = en_Slider_Exp.minValue;
        }

        int attack = 0;
        int defense = 0;
        int hp = 0;
        int level = unitData.c_Level + levelUpCount;

        hp = Mathf.FloorToInt(mc_UnitData.mc_HP * 2.4f / mc_UnitData.mc_RiseValue) * level + mc_UnitData.mc_HP;

        float A = 1.0f;
        float D = 1.0f;

        switch (mc_UnitData.mc_BattleType)
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
        attack = Mathf.FloorToInt((mc_UnitData.mc_Attack * 2.4f / mc_UnitData.mc_RiseValue) * A) * level + mc_UnitData.mc_Attack;
        defense = Mathf.FloorToInt((mc_UnitData.mc_Defense * 2.4f / mc_UnitData.mc_RiseValue) * D) * level + mc_UnitData.mc_Defense;

        en_Text_Attack.text = $"{unitData.c_Attack} ➡ {attack}　+{attack - unitData.c_Attack}";
        en_Text_Defense.text = $"{unitData.c_Defense} ➡ {defense}　+{defense - unitData.c_Defense}";
        en_Text_HP.text = $"{unitData.c_HP} ➡ {hp}　+{hp - unitData.c_HP}";

        foreach (Transform obj in en_Obj_Parent.transform) Destroy(obj.gameObject);
        for(var i = 0; i < 15; i++)
        {
            if(materialList.Count > i)
            {
                var m_Content = Instantiate(fo_Content_CharaList, en_Obj_Parent.transform);

                unitData = DataList.userData.user_Character.Find(x => x.c_UniqueId == materialList[i]);
                mc_UnitData = DataList.mcDatas.Find(x => x.mc_Id == unitData.c_Id);

                if (!mc_UnitData.mc_Name.Contains(re_InputField.text))
                {
                    continue;
                }

                Flame = m_Content.transform.Find("Flame").GetComponent<UICornersGradient>();
                var Icon = m_Content.transform.Find("Icon").GetComponent<Image>();
                var text_Level = m_Content.transform.Find("Text").GetComponent<Text>();
                m_Content.transform.Find("Number").gameObject.SetActive(false);

                Flame.m_topLeftColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color1;
                Flame.m_topRightColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color2;
                Flame.m_bottomLeftColor = Flame.m_topRightColor;
                Flame.m_bottomRightColor = Flame.m_topLeftColor;

                Icon.sprite = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Icon;

                text_Level.text = $"Lv.{unitData.c_Level}";

                if (mc_UnitData.mc_Costume)
                {
                    m_Content.GetComponent<Image>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face[unitData.c_CostumeId];
                }
                else
                {
                    m_Content.GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face;
                }

                var button = m_Content.GetComponent<ButtonExtention>();
                button.onClick.AddListener(() => { DisplayMaterialSelection(uniqueId); ChangeUI_Character(displays[9]); });
            }
            else
            {
                var m_Content = Instantiate(en_Obj_Content_N, en_Obj_Parent.transform);

                var button = m_Content.GetComponent<ButtonExtention>();
                button.onClick.AddListener(() => { DisplayMaterialSelection(uniqueId); ChangeUI_Character(displays[9]); });
            }

            en_Button_Decision.onClick.RemoveAllListeners();
            en_Button_Decision.onClick.AddListener(() => EnhancedConfirmation(requiredAmount));
        }

        ms_SelectCharaUniqueId = uniqueId;
    }
    private string ms_SelectCharaUniqueId;
    private void DisplayMaterialSelection(string _selectCharaUniqueId)
    {
        ms_Text_Count.text = $"{DataList.userData.user_Character.Count}/200";

        var _unitData = DataList.userData.user_Character.Find(x => x.c_UniqueId == _selectCharaUniqueId);
        var _mc_UnitData = DataList.mcDatas.Find(x => x.mc_Id == _unitData.c_Id);

        int expPoint = 0;
        int requiredAmount = 0;

        for (var h = 0; h < materialList.Count; h++)
        {
            var chara = DataList.userData.user_Character.Find(x => x.c_UniqueId == materialList[h]);

            expPoint += Mathf.FloorToInt((chara.c_Level + chara.c_Rarity) * (1500 * chara.c_Rarity));
            requiredAmount += Mathf.FloorToInt(expPoint / (chara.c_Rarity * 10));
        }
        ms_Text_Exp.text = $"{expPoint}";
        ms_Text_Cost.text = $"{requiredAmount}";

        expPoint += _unitData.c_ExpPoint;

        int levelUpCount = 0;   //レベルの上昇値
        int untilLevelUp = 0;   //expPointとの比較用

        untilLevelUp = Mathf.FloorToInt((100 + (50 - _mc_UnitData.mc_RiseValue)) * 0.01f * (_unitData.c_Level) * (100 + (50 - _mc_UnitData.mc_RiseValue)));

        while (true)
        {
            expPoint -= untilLevelUp;
            if (expPoint < 0)
            {
                expPoint += untilLevelUp;
                break;
            }
            else
            {
                levelUpCount++;
                untilLevelUp = Mathf.FloorToInt((100 + (50 - _mc_UnitData.mc_RiseValue)) * 0.01f * (_unitData.c_Level + levelUpCount) * (100 + (50 - _mc_UnitData.mc_RiseValue)));
            }
        }
        ms_Text_Level.text = $"Lv.{_unitData.c_Level} ➡ {_unitData.c_Level + levelUpCount}　+{levelUpCount}";

        ms_Slider.maxValue = untilLevelUp;
        ms_Slider.minValue = untilLevelUp * -0.075f;
        ms_Slider.value = expPoint;
        ms_Text_ExpValue.text = $"{untilLevelUp - expPoint}";

        int min = 0;
        min = Mathf.FloorToInt((100 + (50 - _mc_UnitData.mc_RiseValue)) * 0.01f * (_unitData.c_Level - 1) * (100 + (50 - _mc_UnitData.mc_RiseValue)));

        if (_unitData.c_Level + levelUpCount >= 100)
        {
            ms_Text_Level.text = $"Lv.{_unitData.c_Level} ➡ MAX　+{100 - _unitData.c_Level}";
            ms_Text_ExpValue.text = $"---";
            ms_Slider.value = ms_Slider.maxValue;
        }
        if (min == _unitData.c_ExpPoint)
        {
            ms_Slider.value = ms_Slider.minValue;
        }

        ms_Dropdown.onValueChanged.RemoveAllListeners();
        ms_Dropdown.onValueChanged.AddListener((x) => DisplayMaterialSelection(_selectCharaUniqueId));
        ms_Inputfield.onValueChanged.RemoveAllListeners();
        ms_Inputfield.onValueChanged.AddListener((x) => DisplayMaterialSelection(_selectCharaUniqueId));

        List<Data_Character> aa = new List<Data_Character>();
        var a = DataList.userData.user_Character.OrderBy(x => x.c_Attribute);
        foreach (var i in a) aa.Add(i);
        DataList.userData.user_Character.Clear();
        foreach (var i in aa) DataList.userData.user_Character.Add(i);

        List<Data_Character> chData = new List<Data_Character>();
        List<MasterData_Character> mcData = new List<MasterData_Character>();
        List<MasterData_Character> provisional = new List<MasterData_Character>();
        for (var e = 0; e < DataList.userData.user_Character.Count; e++)
        {
            for (var h = 0; h < DataList.mcDatas.Count; h++)
            {
                if (DataList.userData.user_Character[e].c_Id == DataList.mcDatas[h].mc_Id)
                {
                    provisional.Add(DataList.mcDatas[h]);
                }
            }
        }
        var hoge = provisional.OrderBy(x => x.mc_Attribute);
        foreach (var hh in hoge) mcData.Add(hh);

        switch (ms_Dropdown.value)
        {
            case 0:
                var list = DataList.userData.user_Character.OrderBy(x => x.c_Attribute);
                foreach (var i in list) chData.Add(i);
                break;
            case 1:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Level);
                foreach (var i in list) chData.Add(i);
                break;
            case 2:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Attack);
                foreach (var i in list) chData.Add(i);
                break;
            case 3:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Defense);
                foreach (var i in list) chData.Add(i);
                break;
            case 4:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_HP);
                foreach (var i in list) chData.Add(i);
                break;
            case 5:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_DicePower);
                foreach (var i in list) chData.Add(i);
                break;
            case 6:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Rarity);
                foreach (var i in list) chData.Add(i);
                break;
            case 7:
                list = DataList.userData.user_Character.OrderBy(x => x.c_BattleType);
                foreach (var i in list) chData.Add(i);
                break;
        }

        foreach (Transform obj in ms_Obj_Parent.transform) Destroy(obj.gameObject);

        for (var i = 0; i < DataList.userData.user_Character.Count; i++)
        {
            var unitData = chData[i];
            var mc_UnitData = DataList.mcDatas.Find(x => x.mc_Id == unitData.c_Id);

            if (!mc_UnitData.mc_Name.Contains(ms_Inputfield.text))
            {
                continue;
            }

            var content = Instantiate(cl_Obj_Content, ms_Obj_Parent.transform);

            var Flame = content.transform.Find("Flame").GetComponent<UICornersGradient>();
            var Icon = content.transform.Find("Icon").GetComponent<Image>();
            var text_Level = content.transform.Find("Text").GetComponent<Text>();
            var number = content.transform.Find("Number");
            number.gameObject.SetActive(false);

            Flame.m_topLeftColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color1;
            Flame.m_topRightColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color2;
            Flame.m_bottomLeftColor = Flame.m_topRightColor;
            Flame.m_bottomRightColor = Flame.m_topLeftColor;

            Icon.sprite = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Icon;

            switch (ms_Dropdown.value)
            {
                case 0:
                case 1:
                    text_Level.text = $"Lv.{unitData.c_Level}";
                    break;
                case 2: text_Level.text = $"Atk.{unitData.c_Attack}"; break;
                case 3: text_Level.text = $"Def.{unitData.c_Defense}"; break;
                case 4: text_Level.text = $"Hp.{unitData.c_HP}"; break;
                case 5: text_Level.text = $"DP.{mc_UnitData.mc_DicePower}"; break;
                case 6:
                    var star = "";
                    for (var j = 0; j < mc_UnitData.mc_Rarity; j++)
                    {
                        star += "★";
                    }
                    text_Level.text = $"{star}";
                    break;
                case 7:
                    var battleType = "";
                    switch (mc_UnitData.mc_BattleType)
                    {
                        case 0: battleType = "Balance"; break;
                        case 1: battleType = "Attacker"; break;
                        case 2: battleType = "Defense"; break;
                    }
                    text_Level.text = $"{battleType}";
                    break;
            }

            if(unitData.c_UniqueId == _selectCharaUniqueId)
            {
                content.transform.Find("Image_Target").gameObject.SetActive(true);
            }
            for (var j = 0; j < materialList.Count; j++)
            {
                if(materialList[j] == unitData.c_UniqueId)
                {
                    number.gameObject.SetActive(true);
                    number.Find("Text_Number").GetComponent<Text>().text = $"{j + 1}";

                    break;
                }
            }

            if (mc_UnitData.mc_Costume)
            {
                content.GetComponent<Image>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face[unitData.c_CostumeId];
            }
            else
            {
                content.GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face;
            }

            bool isTeam = false;

            for (var j = 0; j < DataList.userData.user_Formations.Length; j++)
            {
                if (unitData.c_UniqueId == DataList.userData.user_Formations[j])
                {
                    isTeam = true;
                }
            }

            if(materialList.Count >= 15 && !materialList.Contains(unitData.c_UniqueId))
            {
                content.transform.Find("Image_SelectMax").gameObject.SetActive(true);
            }
            else if(unitData.c_Level + levelUpCount >= 100 && !materialList.Contains(unitData.c_UniqueId))
            {
                content.transform.Find("Image_Max").gameObject.SetActive(true);
            }
            else if (unitData.c_UniqueId == _selectCharaUniqueId)
            {
                content.transform.Find("Image_Target").gameObject.SetActive(true);
            }
            else if (isTeam)
            {
                content.transform.Find("Image_Team").gameObject.SetActive(true);
            }
            else if (unitData.c_UniqueId == DataList.userData.user_FavoriteChara.c_UniqueId)
            {
                content.transform.Find("Image_Favorite").gameObject.SetActive(true);
            }
            else if(unitData.c_UniqueId != _selectCharaUniqueId)
            {
                var button = content.GetComponent<ButtonExtention>();
                button.onClick.AddListener(() => { SelectMaterial(unitData.c_UniqueId); });
                button.onLongPress.AddListener(() => { LongPress(unitData.c_UniqueId); });
            }

            oneBeforeScreen = displays[9];
            ms_SelectCharaUniqueId = _selectCharaUniqueId;
        }
    }
    public void SelectMaterial(string uniqueId)
    {
        if (materialList.Contains(uniqueId))
        {
            materialList.Remove(uniqueId);
        }
        else
        {
            materialList.Add(uniqueId);
        }

        if(materialList.Count >= 15)
        {
            ChangeUI_Character(displays[8]);
        }
        else DisplayMaterialSelection(ms_SelectCharaUniqueId);
    }
    public void MaterialReset(CanvasGroup nowDisplay)
    {
        materialList.Clear();
        ChangeUI_Character(nowDisplay);
    }
    private void EnhancedConfirmation(int costValue)
    {
        if(costValue > DataList.userData.user_Money)
        {
            ec_Text_Cost_Error.text = $"必要金額：{costValue}";
            ec_Text_Conf_Error.text = $"所持金が必要金額に達していないため\nキャラクターを強化することができません。";
            re_Conf_Error.SetActive(true);
        }
        else if(materialList.Count <= 0)
        {
            ec_Text_Cost_Error.text = $"";
            ec_Text_Conf_Error.text = $"強化に素材として使用する\nキャラクターが選択されていません";
            re_Conf_Error.SetActive(true);
        }
        else
        {
            ec_Text_Cost.text = $"必要金額：{costValue}";

            foreach (Transform obj in ec_Obj_Parent.transform) Destroy(obj.gameObject);
            for(var i = 0; i < materialList.Count; i++)
            {
                var unitData = DataList.userData.user_Character.Find(x => x.c_UniqueId == materialList[i]);
                var mc_UnitData = DataList.mcDatas.Find(x => x.mc_Id == unitData.c_Id);

                var content = Instantiate(saleConf_Obj_Content, ec_Obj_Parent.transform);

                var Flame = content.transform.Find("Flame").GetComponent<UICornersGradient>();
                var Icon = content.transform.Find("Icon").GetComponent<Image>();
                var text_Level = content.transform.Find("Text").GetComponent<Text>();
                content.transform.Find("Number").gameObject.SetActive(false);

                Flame.m_topLeftColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color1;
                Flame.m_topRightColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color2;
                Flame.m_bottomLeftColor = Flame.m_topRightColor;
                Flame.m_bottomRightColor = Flame.m_topLeftColor;

                Icon.sprite = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Icon;

                text_Level.text = $"Lv.{unitData.c_Level}";

                if (mc_UnitData.mc_Costume)
                {
                    content.GetComponent<Image>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face[unitData.c_CostumeId];
                }
                else
                {
                    content.GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face;
                }

                var button = content.GetComponent<ButtonExtention>().enabled = false;
            }

            re_Conf.SetActive(true);
        }
    }
    public void EnhancedDecision()
    {
        errorObj.SetActive(false);
        loadingObj.SetActive(true);

        var unitData = DataList.userData.user_Character.Find(x => x.c_UniqueId == ms_SelectCharaUniqueId);
        var mc_UnitData = DataList.mcDatas.Find(x => x.mc_Id == unitData.c_Id);

        int expPoint = 0;
        int requiredAmount = 0;

        for (var i = 0; i < materialList.Count; i++)
        {
            var chara = DataList.userData.user_Character.Find(x => x.c_UniqueId == materialList[i]);

            expPoint += Mathf.FloorToInt((chara.c_Level + chara.c_Rarity) * (1500 * chara.c_Rarity));
            requiredAmount += Mathf.FloorToInt(expPoint / (chara.c_Rarity * 10));
        }
        DataList.userData.user_Money -= requiredAmount;

        expPoint += unitData.c_ExpPoint;

        int levelUpCount = 0;   //レベルの上昇値
        int untilLevelUp = 0;   //expPointとの比較用

        untilLevelUp = Mathf.FloorToInt((100 + (50 - mc_UnitData.mc_RiseValue)) * 0.01f * (unitData.c_Level) * (100 + (50 - mc_UnitData.mc_RiseValue)));

        while (true)
        {
            expPoint -= untilLevelUp;
            if (expPoint < 0)
            {
                expPoint += untilLevelUp;
                break;
            }
            else
            {
                levelUpCount++;
                untilLevelUp = Mathf.FloorToInt((100 + (50 - mc_UnitData.mc_RiseValue)) * 0.01f * (unitData.c_Level + levelUpCount) * (100 + (50 - mc_UnitData.mc_RiseValue)));
            }
        }
        unitData.c_ExpPoint = expPoint;
        unitData.c_UntilLevelUp = untilLevelUp;
        unitData.c_Level += levelUpCount;

        int level = unitData.c_Level;

        unitData.c_HP = Mathf.FloorToInt(mc_UnitData.mc_HP * 2.4f / mc_UnitData.mc_RiseValue) * level + mc_UnitData.mc_HP;

        float A = 1.0f;
        float D = 1.0f;

        switch (mc_UnitData.mc_BattleType)
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
        unitData.c_Attack = Mathf.FloorToInt((mc_UnitData.mc_Attack * 2.4f / mc_UnitData.mc_RiseValue) * A) * level + mc_UnitData.mc_Attack;
        unitData.c_Defense = Mathf.FloorToInt((mc_UnitData.mc_Defense * 2.4f / mc_UnitData.mc_RiseValue) * D) * level + mc_UnitData.mc_Defense;

        for (var i = 0; i < materialList.Count; i++)
        {
            var chara = DataList.userData.user_Character.Find(x => x.c_UniqueId == materialList[i]);
            DataList.userData.user_Character.Remove(chara);
        }

        en_Save();
    }
    private void en_Save()
    {
        errorObj.SetActive(false);
        loadingObj.SetActive(true);

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
                materialList.Clear();
                sale_Obj_Conf.SetActive(false);
                loadingObj.SetActive(false);
                re_Conf.SetActive(false);
                GetComponent<HomeUIDisplay>().SetHomeMenuUI();
                DisplayEnhanced(ms_SelectCharaUniqueId);
            },
            error =>
            {
                errorObj.SetActive(true);
                loadingObj.SetActive(false);
                var button = errorObj.transform.Find("Button").GetComponent<ButtonExtention>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => en_Save());
            });
    }
    #endregion
    #region キャラ一覧
    public void DisplayCharacterList()
    {
        List<Data_Character> chData = new List<Data_Character>();
        List<MasterData_Character> mcData = new List<MasterData_Character>();
        List<MasterData_Character> provisional = new List<MasterData_Character>();
        for (var e = 0; e < DataList.userData.user_Character.Count; e++)
        {
            for (var h = 0; h < DataList.mcDatas.Count; h++)
            {
                if (DataList.userData.user_Character[e].c_Id == DataList.mcDatas[h].mc_Id)
                {
                    provisional.Add(DataList.mcDatas[h]);
                }
            }
        }
        var hoge = provisional.OrderBy(x => x.mc_Attribute);
        foreach (var hh in hoge) mcData.Add(hh);

        switch (cl_Dropdown.value)
        {
            case 0:
                var list = DataList.userData.user_Character.OrderBy(x => x.c_Attribute);
                foreach (var i in list) chData.Add(i);
                break;
            case 1:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Level);
                foreach (var i in list) chData.Add(i);
                break;
            case 2:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Attack);
                foreach (var i in list) chData.Add(i);
                break;
            case 3:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Defense);
                foreach (var i in list) chData.Add(i);
                break;
            case 4:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_HP);
                foreach (var i in list) chData.Add(i);
                break;
            case 5:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_DicePower);
                foreach (var i in list) chData.Add(i);
                break;
            case 6:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Rarity);
                foreach (var i in list) chData.Add(i);
                break;
            case 7:
                list = DataList.userData.user_Character.OrderBy(x => x.c_BattleType);
                foreach (var i in list) chData.Add(i);
                break;
        }

        foreach (Transform obj in cl_Obj_Parent.transform) Destroy(obj.gameObject);
        for(var i = 0; i < DataList.userData.user_Character.Count; i++)
        {
            var unitData = chData[i];
            var mc_UnitData = DataList.mcDatas.Find(x => x.mc_Id == unitData.c_Id);

            if (!mc_UnitData.mc_Name.Contains(cl_InputField.text) && cl_InputField.text != null)
            {
                continue;
            }

            var content = Instantiate(cl_Obj_Content, cl_Obj_Parent.transform);

            var Flame = content.transform.Find("Flame").GetComponent<UICornersGradient>();
            var Icon = content.transform.Find("Icon").GetComponent<Image>();
            var text_Level = content.transform.Find("Text").GetComponent<Text>();
            content.transform.Find("Number").gameObject.SetActive(false);

            Flame.m_topLeftColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color1;
            Flame.m_topRightColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color2;
            Flame.m_bottomLeftColor = Flame.m_topRightColor;
            Flame.m_bottomRightColor = Flame.m_topLeftColor;

            Icon.sprite = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Icon;

            switch (cl_Dropdown.value)
            {
                case 0:
                case 1:
                    text_Level.text = $"Lv.{unitData.c_Level}";
                    break;
                case 2: text_Level.text = $"Atk.{unitData.c_Attack}"; break;
                case 3: text_Level.text = $"Def.{unitData.c_Defense}"; break;
                case 4: text_Level.text = $"Hp.{unitData.c_HP}"; break;
                case 5: text_Level.text = $"DP.{mc_UnitData.mc_DicePower}"; break;
                case 6:
                    var star = "";
                    for (var j = 0; j < mc_UnitData.mc_Rarity; j++)
                    {
                        star += "★";
                    }
                    text_Level.text = $"{star}";
                    break;
                case 7:
                    var battleType = "";
                    switch (mc_UnitData.mc_BattleType)
                    {
                        case 0: battleType = "バランス"; break;
                        case 1: battleType = "アタッカー"; break;
                        case 2: battleType = "ディフェンス"; break;
                    }
                    text_Level.text = $"{battleType}";
                    break;
            }

            if (mc_UnitData.mc_Costume)
            {
                content.GetComponent<Image>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face[unitData.c_CostumeId];
            }
            else
            {
                content.GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face;
            }

            var button = content.GetComponent<ButtonExtention>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => Connection(unitData.c_UniqueId));
        }

        cl_Text_Count.text = $"{DataList.userData.user_Character.Count}/200";
    }

    private void Connection(string uniqueId)
    {
        oneBeforeScreen = displays[4];

        selectCharaUniqueId = uniqueId;
        DisplayCharaStatus(selectCharaUniqueId);
    }
    #endregion
    #region キャラクターステータス
    public async void DisplayCharaStatus(string uniqueId)
    {
        var unitData = DataList.userData.user_Character.Find(x => x.c_UniqueId == uniqueId);
        var mc_UnitData = DataList.mcDatas.Find(x => x.mc_Id == unitData.c_Id);

        Image stand = cs_Images_Stand[0];
        Image face = cs_Image_Face;

        if (mc_UnitData.mc_Costume)
        {
            stand.sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Stand[unitData.c_CostumeId];
            face.sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face[unitData.c_CostumeId];
            cs_Button_CostumeChange.gameObject.SetActive(true);
            cs_Button_CostumeChange.onClick.RemoveAllListeners();
            cs_Button_CostumeChange.onClick.AddListener(() => DisplayCostumeIcons(mc_UnitData));
        }
        else
        {
            stand.sprite = data_Character.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Stand;
            face.sprite = data_Character.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face;
            cs_Button_CostumeChange.gameObject.SetActive(false);
        }

        foreach (var i in cs_Images_Stand) i.sprite = stand.sprite;
        cs_Image_Face.sprite = face.sprite;
        cs_Image_Icon.sprite = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Icon;

        cs_UICorners.m_topLeftColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color1;
        cs_UICorners.m_topRightColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color2;
        cs_UICorners.m_bottomLeftColor = cs_UICorners.m_topRightColor;
        cs_UICorners.m_bottomRightColor = cs_UICorners.m_topLeftColor;

        cs_Text_Name.text = mc_UnitData.mc_Name;
        cs_Text_Star.text = "";
        for (var i = 0; i < mc_UnitData.mc_Rarity; i++) cs_Text_Star.text += "★";
        string battleType = "";
        switch (mc_UnitData.mc_BattleType)
        {
            case 0: battleType = "バランス型"; break;
            case 1: battleType = "アタッカー型"; break;
            case 2: battleType = "ディフェンス型"; break;
        }
        cs_Text_BattleType.text = $"戦型：{battleType}";
        cs_Text_DicePower.text = $"ダイスパワー：{mc_UnitData.mc_DicePower}";
        cs_Text_Level.text = $"Lv.{unitData.c_Level}";
        cs_Text_UntilLevelUp.text = $"次のレベルまで：{unitData.c_UntilLevelUp}";
        cs_Text_Uses.text = $"出撃回数：{unitData.c_Uses}";

        for(var j = 0; j < 3; j++)
        {
            switch (j)
            {
                case 0:
                    if (mc_UnitData.mc_Ability1 == 0)
                    {
                        cs_Obj_Ability1.SetActive(false);
                    }
                    else
                    {
                        //ここにアビリティの詳細を入れる　ID
                    }
                    break;
                case 1:
                    if (mc_UnitData.mc_Ability2 == 0)
                    {
                        cs_Obj_Ability2.SetActive(false);
                    }
                    else
                    {
                        //ここにアビリティの詳細を入れる　ID
                    }
                    break;
                case 2:
                    if (mc_UnitData.mc_Ability3 == 0)
                    {
                        cs_Obj_Ability3.SetActive(false);
                    }
                    else
                    {
                        //ここにアビリティの詳細を入れる　ID
                    }
                    break;
            }
        }

        cs_Text_SkillName.text = $"スキル：　※ここにスキルの名前を入れる※　";
        cs_Text_SkillExplanation.text = $"※ここにスキルの詳細を入れる※";

        float statusSumTotal = (unitData.c_Attack + unitData.c_Defense + unitData.c_HP) / 2;
        cs_Slider_Attack.maxValue = statusSumTotal;
        cs_Slider_Defense.maxValue = statusSumTotal;
        cs_Slider_HP.maxValue = statusSumTotal;

        cs_Button_Back.onClick.RemoveAllListeners();
        cs_Button_Back.onClick.AddListener(() => ChangeUI_Character(oneBeforeScreen));
        if (oneBeforeScreen == displays[0]) cs_Button_Back.onClick.AddListener(() => SetCharacterMenu());

        float a = unitData.c_Attack / 100f;
        float d = unitData.c_Defense / 100f;
        float h = unitData.c_HP / 100f;

        ChangeUI_Character(displays[6]);

        cs_Slider_Attack.value = 0;
        cs_Slider_Defense.value = 0;
        cs_Slider_HP.value = 0;

        for (var k = 0;k < 100; k++)
        {
            await UniTask.Delay(10);
            cs_Slider_Attack.value += a;
            cs_Text_Attack.text = $"攻撃力：{Mathf.FloorToInt(cs_Slider_Attack.value)}";
            cs_Slider_Defense.value += d;
            cs_Text_Defense.text = $"防御力：{Mathf.FloorToInt(cs_Slider_Defense.value)}";
            cs_Slider_HP.value += h;
            cs_Text_HP.text = $"HP：{Mathf.FloorToInt(cs_Slider_HP.value)}";
        }

        await UniTask.Delay(100);

        cs_Slider_Attack.value = unitData.c_Attack;
        cs_Slider_Defense.value = unitData.c_Defense;
        cs_Slider_HP.value = unitData.c_HP;
        cs_Text_Attack.text = $"攻撃力：{cs_Slider_Attack.value}";
        cs_Text_Defense.text = $"防御力：{cs_Slider_Defense.value}";
        cs_Text_HP.text = $"HP：{cs_Slider_HP.value}";
    } 
    public void DisplayCostumeIcons(MasterData_Character mcData)
    {
        bool[] charaFlags = new bool[0];
        switch (mcData.mc_Id)
        {
            case 1090: charaFlags = DataList.userData.user_DataCostume.cos_Jormungand; break;
            case 1135: charaFlags = DataList.userData.user_DataCostume.cos_Percival; break;
            case 1173: charaFlags = DataList.userData.user_DataCostume.cos_Odin; break;
            case 1221: charaFlags = DataList.userData.user_DataCostume.cos_Frist; break;
            case 1345: charaFlags = DataList.userData.user_DataCostume.cos_Valkyrie; break;
            case 1392: charaFlags = DataList.userData.user_DataCostume.cos_Gerbera; break;
        }

        foreach (Transform obj in cc_Obj_Parent.transform) Destroy(obj.gameObject);

        for(var i = 0; i < charaFlags.Length; i++)
        {
            var content = Instantiate(cc_Obj_Content, cc_Obj_Parent.transform);

            var icon = content.transform.Find("Image_CharaIcon");
            icon.GetComponent<Image>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mcData.mc_Id).img_Face[i];
            icon.GetComponent<Button>().onClick.RemoveAllListeners();
            var num = i;
            icon.GetComponent<Button>().onClick.AddListener(() => CC_ChangeStandImage(mcData, num));

            if (charaFlags[i]) 
            {
                content.transform.Find("Image_Rock").gameObject.SetActive(false);
            }
        }

        cc_Img_Stand.sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mcData.mc_Id).img_Stand[DataList.userData.user_Character.Find(x => x.c_UniqueId == selectCharaUniqueId).c_CostumeId];

        ChangeUI_Character(displays[7]);

        cc_Button_Decision.gameObject.SetActive(false);
    }
    public void CC_ChangeStandImage(MasterData_Character mcData ,int cosId)
    {
        cc_Img_Stand.sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mcData.mc_Id).img_Stand[cosId];

        cc_Button_Decision.onClick.RemoveAllListeners();
        cc_Button_Decision.onClick.AddListener(() => CC_Decision(cosId));
        cc_Button_Decision.gameObject.SetActive(true);
    }
    public void CC_Decision(int cosId)
    {
        loadingObj.SetActive(true);
        errorObj.SetActive(false);
        var charaData = DataList.userData.user_Character.Find(x => x.c_UniqueId == selectCharaUniqueId);
        charaData.c_CostumeId = cosId;

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
                DisplayCharaStatus(selectCharaUniqueId);
            },
            error =>
            {
                errorObj.SetActive(true);
                loadingObj.SetActive(false);
                var button = errorObj.transform.Find("Button").GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => CC_Decision(cosId));
            });
    }
    #endregion
    #region 売却
    private List<String> saleCharaUniqueId = new List<string>();
    private int _saleValue;
    public void DislplaySaleCharacter()
    {
        sale_Text_Count.text = $"{DataList.userData.user_Character.Count}/200";

        List<Data_Character> chData = new List<Data_Character>();
        List<MasterData_Character> mcData = new List<MasterData_Character>();
        List<MasterData_Character> provisional = new List<MasterData_Character>();
        for (var e = 0; e < DataList.userData.user_Character.Count; e++)
        {
            for (var h = 0; h < DataList.mcDatas.Count; h++)
            {
                if (DataList.userData.user_Character[e].c_Id == DataList.mcDatas[h].mc_Id)
                {
                    provisional.Add(DataList.mcDatas[h]);
                }
            }
        }
        var hoge = provisional.OrderBy(x => x.mc_Attribute);
        foreach (var hh in hoge) mcData.Add(hh);

        switch (sale_Dropdown.value)
        {
            case 0:
                var list = DataList.userData.user_Character.OrderBy(x => x.c_Attribute);
                foreach (var i in list) chData.Add(i);
                break;
            case 1:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Level);
                foreach (var i in list) chData.Add(i);
                break;
            case 2:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Attack);
                foreach (var i in list) chData.Add(i);
                break;
            case 3:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Defense);
                foreach (var i in list) chData.Add(i);
                break;
            case 4:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_HP);
                foreach (var i in list) chData.Add(i);
                break;
            case 5:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_DicePower);
                foreach (var i in list) chData.Add(i);
                break;
            case 6:
                list = DataList.userData.user_Character.OrderByDescending(x => x.c_Rarity);
                foreach (var i in list) chData.Add(i);
                break;
            case 7:
                list = DataList.userData.user_Character.OrderBy(x => x.c_BattleType);
                foreach (var i in list) chData.Add(i);
                break;
        }

        foreach (Transform obj in sale_Obj_Parent.transform) Destroy(obj.gameObject);
        for (var i = 0; i < DataList.userData.user_Character.Count; i++)
        {
            var unitData = chData[i];
            var mc_UnitData = DataList.mcDatas.Find(x => x.mc_Id == unitData.c_Id);

            if (!mc_UnitData.mc_Name.Contains(sale_InputField.text) && sale_InputField.text != null)
            {
                continue;
            }

            var content = Instantiate(sale_Obj_Content, sale_Obj_Parent.transform);

            var Flame = content.transform.Find("Flame").GetComponent<UICornersGradient>();
            var Icon = content.transform.Find("Icon").GetComponent<Image>();
            var text_Level = content.transform.Find("Text").GetComponent<Text>();
            content.transform.Find("Number").gameObject.SetActive(false);

            Flame.m_topLeftColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color1;
            Flame.m_topRightColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color2;
            Flame.m_bottomLeftColor = Flame.m_topRightColor;
            Flame.m_bottomRightColor = Flame.m_topLeftColor;

            Icon.sprite = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Icon;

            switch (sale_Dropdown.value)
            {
                case 0:
                case 1:
                    text_Level.text = $"Lv.{unitData.c_Level}";
                    break;
                case 2: text_Level.text = $"Atk.{unitData.c_Attack}"; break;
                case 3: text_Level.text = $"Def.{unitData.c_Defense}"; break;
                case 4: text_Level.text = $"Hp.{unitData.c_HP}"; break;
                case 5: text_Level.text = $"DP.{mc_UnitData.mc_DicePower}"; break;
                case 6:
                    var star = "";
                    for (var j = 0; j < mc_UnitData.mc_Rarity; j++)
                    {
                        star += "★";
                    }
                    text_Level.text = $"{star}";
                    break;
                case 7:
                    var battleType = "";
                    switch (mc_UnitData.mc_BattleType)
                    {
                        case 0: battleType = "バランス"; break;
                        case 1: battleType = "アタッカー"; break;
                        case 2: battleType = "ディフェンス"; break;
                    }
                    text_Level.text = $"{battleType}";
                    break;
            }

            if (mc_UnitData.mc_Costume)
            {
                content.GetComponent<Image>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face[unitData.c_CostumeId];
            }
            else
            {
                content.GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face;
            }

            bool isTeam = false;

            for(var j = 0; j < DataList.userData.user_Formations.Length; j++)
            {
                if(unitData.c_UniqueId == DataList.userData.user_Formations[j])
                {
                    isTeam = true;
                }
            }

            if (isTeam)
            {
                content.transform.Find("Image_Team").gameObject.SetActive(true);
            }
            else if (unitData.c_UniqueId == DataList.userData.user_FavoriteChara.c_UniqueId)
            {
                content.transform.Find("Image_Favorite").gameObject.SetActive(true);
            }
            else
            {
                var button = content.GetComponent<ButtonExtention>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SaleCharacterSelect(unitData.c_UniqueId));
            }

            for(var k = 0; k < saleCharaUniqueId.Count; k++)
            {
                if (unitData.c_UniqueId == saleCharaUniqueId[k])
                {
                    content.transform.Find("Number").gameObject.SetActive(true);
                    content.transform.Find("Number").Find("Text_Number").GetComponent<Text>().text = $"{k + 1}";
                }
            }
        }
    }
    public void SaleCharacterSelect(string uniqueId)
    {
        if (saleCharaUniqueId.Contains(uniqueId))
        {
            saleCharaUniqueId.Remove(uniqueId);
        }
        else
        {
            saleCharaUniqueId.Add(uniqueId);
        }
        DislplaySaleCharacter();
    }
    public void ClearSalaCharaList()
    {
        saleCharaUniqueId = new List<string>();
    }
    public void SaleConfDisplay(bool isOn)
    {
        if (isOn)
        {
            int saleValue = 0;
            foreach (Transform obj in saleConf_Obj_Parent.transform) Destroy(obj.gameObject);
            for (var i = 0; i < saleCharaUniqueId.Count; i++)
            {
                var unitData = DataList.userData.user_Character.Find(x => x.c_UniqueId == saleCharaUniqueId[i]);
                var mc_UnitData = DataList.mcDatas.Find(x => x.mc_Id == unitData.c_Id);

                var content = Instantiate(saleConf_Obj_Content, saleConf_Obj_Parent.transform);

                var Flame = content.transform.Find("Flame").GetComponent<UICornersGradient>();
                var Icon = content.transform.Find("Icon").GetComponent<Image>();
                var text_Level = content.transform.Find("Text").GetComponent<Text>();
                content.transform.Find("Number").gameObject.SetActive(false);

                Flame.m_topLeftColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color1;
                Flame.m_topRightColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color2;
                Flame.m_bottomLeftColor = Flame.m_topRightColor;
                Flame.m_bottomRightColor = Flame.m_topLeftColor;

                Icon.sprite = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Icon;

                text_Level.text = $"Lv.{unitData.c_Level}";

                if (mc_UnitData.mc_Costume)
                {
                    content.GetComponent<Image>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face[unitData.c_CostumeId];
                }
                else
                {
                    content.GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face;
                }

                var button = content.GetComponent<ButtonExtention>().enabled = false;

                if (3 >= unitData.c_Rarity) saleValue += 3000;
                else if (4 == unitData.c_Rarity) saleValue += 7500;
                else if (5 <= unitData.c_Rarity) saleValue += 12500;
            }

            sale_Obj_Conf.SetActive(true);
            sale_Obj_Conf.transform.Find("Text_SaleValue").GetComponent<Text>().text = $"売却額：{saleValue}";
            _saleValue = saleValue;
        }
        else
        {
            sale_Obj_Conf.SetActive(false);
        }
    }
    public void SaleDecision()
    {
        loadingObj.SetActive(true);
        errorObj.SetActive(false);

        for (var i = 0; i < saleCharaUniqueId.Count; i++)
        {
            var chara = DataList.userData.user_Character.Find(x => x.c_UniqueId == saleCharaUniqueId[i]);
            DataList.userData.user_Character.Remove(chara);
        }
        DataList.userData.user_Money += _saleValue;

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
                saleCharaUniqueId = new List<string>();
                sale_Obj_Conf.SetActive(false);
                loadingObj.SetActive(false);
                GetComponent<HomeUIDisplay>().SetHomeMenuUI();
                DislplaySaleCharacter();
            },
            error =>
            {
                errorObj.SetActive(true);
                loadingObj.SetActive(false);
                var button = errorObj.transform.Find("Button").GetComponent<ButtonExtention>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SaleDecision());
            });
    }
    #endregion
    #region 図鑑
    public void DisplayPictureBook()
    {
        List<MasterData_Character> mcData = new List<MasterData_Character>();

        switch (pb_DropDown.value)
        {
            case 0:
                var list = DataList.mcDatas.OrderBy(x => x.mc_Id);
                foreach (var i in list) mcData.Add(i);
                break;
            case 1:
                list = DataList.mcDatas.OrderBy(x => x.mc_Attribute);
                foreach (var i in list) mcData.Add(i);
                break;
            case 2:
                list = DataList.mcDatas.OrderBy(x => x.mc_Rarity);
                foreach (var i in list) mcData.Add(i);
                break;
            case 3:
                list = DataList.mcDatas.OrderBy(x => x.mc_BattleType);
                foreach (var i in list) mcData.Add(i);
                break;
        }

        foreach (Transform obj in pb_Obj_Parent.transform) Destroy(obj.gameObject);
        for (var i = 0; i < DataList.mcDatas.Count; i++)
        {
            var mc_UnitData = mcData[i];

            if (!mc_UnitData.mc_Name.Contains(pb_Inputfield.text) && pb_Inputfield.text != null)
            {
                continue;
            }

            var content = Instantiate(pb_Obj_Content, pb_Obj_Parent.transform);

            var Flame = content.transform.Find("Flame").GetComponent<UICornersGradient>();
            var Icon = content.transform.Find("Icon").GetComponent<Image>();
            var text_Level = content.transform.Find("Text").GetComponent<Text>();
            content.transform.Find("Number").gameObject.SetActive(false);

            Flame.m_topLeftColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color1;
            Flame.m_topRightColor = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Color2;
            Flame.m_bottomLeftColor = Flame.m_topRightColor;
            Flame.m_bottomRightColor = Flame.m_topLeftColor;

            Icon.sprite = data_Attribute.dataBases[mc_UnitData.mc_Attribute].at_Icon;

            switch (pb_DropDown.value)
            {
                case 0:
                case 1:
                    text_Level.text = $"No.{mc_UnitData.mc_Id}";
                    break;
                case 2:
                    var star = "";
                    for (var j = 0; j < mc_UnitData.mc_Rarity; j++)
                    {
                        star += "★";
                    }
                    text_Level.text = $"{star}";
                    break;
                case 3:
                    var battleType = "";
                    switch (mc_UnitData.mc_BattleType)
                    {
                        case 0: battleType = "バランス"; break;
                        case 1: battleType = "アタッカー"; break;
                        case 2: battleType = "ディフェンス"; break;
                    }
                    text_Level.text = $"{battleType}";
                    break;
            }
            if (!DataList.userData.user_PictureBook.Contains(mc_UnitData.mc_Id))
            {
                content.transform.Find("Image_Lock").gameObject.SetActive(true);
            }
            content.GetComponent<ButtonExtention>().enabled = false;

            if (mc_UnitData.mc_Costume)
            {
                content.GetComponent<Image>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face[0];
                for (int j = 1; j < data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face.Count; j++)
                {
                    var copy = Instantiate(content, pb_Obj_Parent.transform);
                    copy.GetComponent<Image>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face[j];

                    bool[] cosFlag = new bool[0];
                    switch (mc_UnitData.mc_Id)
                    {
                        case 1090: cosFlag = DataList.userData.user_DataCostume.cos_Jormungand; break;
                        case 1392: cosFlag = DataList.userData.user_DataCostume.cos_Gerbera; break;
                        case 1173: cosFlag = DataList.userData.user_DataCostume.cos_Odin; break;
                        case 1345: cosFlag = DataList.userData.user_DataCostume.cos_Valkyrie; break;
                        case 1221: cosFlag = DataList.userData.user_DataCostume.cos_Frist; break;
                        case 1135: cosFlag = DataList.userData.user_DataCostume.cos_Percival; break;
                    }

                    if(cosFlag[j] == false)
                    {
                        copy.transform.Find("Image_Lock").gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                content.GetComponent<Image>().sprite = data_Character.dataLists.Find(x => x.img_Id == mc_UnitData.mc_Id).img_Face;
            }
        }
    }
    #endregion
}
