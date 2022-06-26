using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using PlayFab;
using Cysharp.Threading.Tasks;

public class InGameManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ImageData_Character data_Character;
    [SerializeField] private ImageData_AnotherCostume data_AnotherCostume;
    [SerializeField] private Data_Attribute data_Attribute;

    [Header("Background")]
    [SerializeField] private UICornersGradient bg_Gradient;
    [SerializeField] private CanvasGroup whiteScreen;

    #region UI
    [Header("UI_Button")]
    [SerializeField] private List<GameObject> button_Arrow;
    [SerializeField] private List<GameObject> button_Roll;
    [Header("UI_Header")]
    [SerializeField] private Slider slider_EnemyHP;
    [SerializeField] private Image image_EnemyIcon;
    [SerializeField] private Image image_EnemyAtt;
    [SerializeField] private Text text_Serif;
    [SerializeField] private GameObject obj_Status;
    [Header("UI_Footer")]
    [SerializeField] private List<GameObject> obj_CharaIcon = new List<GameObject>();
    [SerializeField] private Slider slider_PlayerHP;
    [SerializeField] private Text text_PlayerHP;
    [SerializeField] private Text text_TileEffect;
    [Header("UI_Clear")]
    [SerializeField] private CanvasGroup cg_GameFin;
    #endregion

    [Header("Object")]
    [SerializeField] private List<GameObject> columns = new List<GameObject>();
    [SerializeField] private List<Color> color_Tiles = new List<Color>();
    [SerializeField] private List<Sprite> sprite_TileIcon = new List<Sprite>();
    private List<GameObject> tiles = new List<GameObject>();
    private Dictionary<int, TileCondition> setTiles = new Dictionary<int, TileCondition>();

    private int player_TotalHP = 0;
    private int player_Hp = 0;
    private int dicePower;
    private Data_Character phaseBoss;
    private int phaseNum;
    private int enemy_TotalHP = 0;
    private int enemy_HP = 0;

    #region プレイヤーステータス
    [SerializeField] private GameObject moveChara;
    private int presentLocation;    //プレイヤーが現在いる位置の座標
    private Condition condition;    //プレイヤーの状態
    private int conditionTurn = 0;  //状態異常時の残りターン
    private int rollPower;          //出目残り
    private int playUnit = -1;      //操作キャラ
    private int turn = 0;
    #endregion

    [SerializeField] private List<AudioClip> condition_SE = new List<AudioClip>();
    [SerializeField] private AudioSource sourceSE;

    public enum Condition
    {
        none,
        burn,
        paralysis,
        ice,
        poison,
    }
    public enum TileCondition
    {
        None,
        Damage,
        HeavyDamage,
        Burn,
        Paralysis,
        Ice,
        Poison
    }

    private void Awake()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn()) SceneManager.LoadScene("TitleScene");
    }
    private async void Start()
    {
        #region 画面初期設定
        cg_GameFin.alpha = 0;
        cg_GameFin.gameObject.SetActive(false);

        turn = 0;

        //タイル登録
        for(var i = 0;i < columns.Count; i++)
        {
            for(var j = 0;j < columns[i].transform.childCount; j++)
            {
                tiles.Add(columns[i].transform.Find($"{j}").gameObject);
            }
        }
        for (var i = 0; i < tiles.Count; i++) setTiles.Add(i, TileCondition.None);

        //バトル用キャラセット(プレイヤー)
        for (var i = 0; i < DataList_Battle.unit.Count; i++)
        {
            var unit = DataList_Battle.unit[i];
            var md_Unit1 = DataList.mcDatas.Find(x => x.mc_Id == unit.c_Id);

            var flame = obj_CharaIcon[i].transform.Find("Flame").GetComponent<UICornersGradient>();
            var image = obj_CharaIcon[i].transform.Find("Image").GetComponent<Image>();
            var icon = obj_CharaIcon[i].transform.Find("Icon").GetComponent<Image>();

            if (md_Unit1.mc_Costume)
            {
                image.sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == unit.c_Id).img_Face[unit.c_CostumeId];
            }
            else
            {
                image.sprite = data_Character.dataLists.Find(x => x.img_Id == unit.c_Id).img_Face;
            }

            flame.m_topLeftColor = data_Attribute.dataBases[unit.c_Attribute].at_Color1;
            flame.m_topRightColor = data_Attribute.dataBases[unit.c_Attribute].at_Color2;
            flame.m_bottomLeftColor = flame.m_topRightColor;
            flame.m_bottomRightColor = flame.m_topLeftColor;

            icon.sprite = data_Attribute.dataBases[unit.c_Attribute].at_Icon;

            player_TotalHP += unit.c_HP;
            dicePower += unit.c_DicePower;
        }

        player_Hp = player_TotalHP;
        dicePower = Mathf.FloorToInt(dicePower / 2);

        text_PlayerHP.text = $"{player_Hp}/{player_TotalHP}";
        slider_PlayerHP.maxValue = player_TotalHP;
        slider_PlayerHP.value = player_TotalHP;

        condition = Condition.none;

        foreach (var i in button_Arrow) i.SetActive(false);
        foreach (var i in button_Roll) i.SetActive(false);

        phaseNum = 1;
        SetEnemy(phaseNum);

        var md_Unit = DataList.mcDatas.Find(x => x.mc_Id == DataList_Battle.unit[0].c_Id);

        if (md_Unit.mc_Costume)
        {
            moveChara.GetComponent<SpriteRenderer>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == md_Unit.mc_Id).img_Face[DataList_Battle.unit[0].c_CostumeId];
        }
        else
        {
            moveChara.GetComponent<SpriteRenderer>().sprite = data_Character.dataLists.Find(x => x.img_Id == md_Unit.mc_Id).img_Face;
        }

        //テキスト
        text_Serif.text = $"～　バトル開始！　～";

        ResetTile();

        whiteScreen.gameObject.SetActive(true);
        whiteScreen.alpha = 1;
        whiteScreen.DOFade(0, 3);
        await UniTask.Delay(3000);
        whiteScreen.gameObject.SetActive(false);

        var pos = moveChara.transform.position;
        pos.x = 0;
        pos.y = 0;
        pos.z = 7.8f;
        moveChara.transform.position = pos;

        presentLocation = 24;
        #endregion

        SetTurn();
        text_Serif.text = $"右下のROLLボタンを押してダイスを振ろう！";
    }

    private void SetEnemy(int phaseValue)
    {
        Data_Character chara = new Data_Character();

        int bossId = 0;

        switch (phaseValue)
        {
            case 1:
                bossId = DataList.msDatas.Find(X => X.stage_Id == DataList_Battle.id_Quest).stage_First;
                break;
            case 2:
                bossId = DataList.msDatas.Find(X => X.stage_Id == DataList_Battle.id_Quest).stage_Second;
                break;
            case 3:
                bossId = DataList.msDatas.Find(X => X.stage_Id == DataList_Battle.id_Quest).stage_Third;
                break;
        }

        var charaData = DataList.mcDatas.Find(x => x.mc_Id == bossId);

        chara.c_Id = charaData.mc_Id;
        chara.c_Level = DataList.msDatas.Find(x => x.stage_Id == DataList_Battle.id_Quest).stage_Recommendation;
        chara.c_UniqueId = "eNeMy";

        chara.c_CostumeId = 0;
        chara.c_Uses = 0;

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

        phaseBoss = chara;
        enemy_TotalHP = phaseBoss.c_HP * 10;
        enemy_HP = enemy_TotalHP;

        slider_EnemyHP.maxValue = enemy_TotalHP;
        slider_EnemyHP.value = enemy_HP;
        image_EnemyIcon.sprite = data_Character.dataLists.Find(x => x.img_Id == phaseBoss.c_Id).img_Face;
        image_EnemyAtt.sprite = data_Attribute.dataBases[phaseBoss.c_Attribute].at_Icon;

        bg_Gradient.m_topLeftColor = data_Attribute.dataBases[chara.c_Attribute].at_Color1;
        bg_Gradient.m_topRightColor = data_Attribute.dataBases[chara.c_Attribute].at_Color2;
        bg_Gradient.m_bottomLeftColor = bg_Gradient.m_topRightColor;
        bg_Gradient.m_bottomRightColor = bg_Gradient.m_topLeftColor;
    }

    private void ResetTile()
    {
        setTiles = new Dictionary<int, TileCondition>();
        for(var i = 0;i < tiles.Count; i++)
        {
            setTiles.Add(i, TileCondition.None);
            tiles[i].transform.Find("Step Count").GetComponent<TextMesh>().text = "";
            var flame = tiles[i].transform.Find("Flame").GetComponent<SpriteRenderer>();
            var icon = tiles[i].transform.Find("Icon");
            icon.gameObject.SetActive(false);

            tiles[i].GetComponent<SpriteRenderer>().color = color_Tiles[0];
            flame.color = color_Tiles[0];
        }
    }

    private void SetTurn()
    {
        var stageData = DataList.msDatas.Find(x => x.stage_Id == DataList_Battle.id_Quest);

        for(var t = 0; t < 6; t++)
        {
            var tileNum = 0;
            var tileType = TileCondition.None;

            switch (t)
            {
                case 0:
                    tileNum = stageData.stage_Damage;
                    tileType = TileCondition.Damage;
                    break;
                case 1:
                    tileNum = stageData.stage_HeavyDamage;
                    tileType = TileCondition.HeavyDamage;
                    break;
                case 2:
                    tileNum = stageData.stage_Burn;
                    tileType = TileCondition.Burn;
                    break;
                case 3:
                    tileNum = stageData.stage_Paralysis;
                    tileType = TileCondition.Paralysis;
                    break;
                case 4:
                    tileNum = stageData.stage_Ice;
                    tileType = TileCondition.Ice;
                    break;
                case 5:
                    tileNum = stageData.stage_Poison;
                    tileType = TileCondition.Poison;
                    break;
            }

            for(var i = 0; i < tileNum; i++)
            {
                while (true)
                {
                    var r = Random.Range(0, 49);
                    
                    if (setTiles[r] == TileCondition.None)
                    {
                        setTiles[r] = tileType;
                        tiles[r].transform.Find("Step Count").GetComponent<TextMesh>().text = "";
                        var flame = tiles[r].transform.Find("Flame").GetComponent<SpriteRenderer>();
                        var icon = tiles[r].transform.Find("Icon");
                        icon.gameObject.SetActive(false);

                        if (t >= 2)
                        {
                            icon.gameObject.SetActive(true);
                            icon.GetComponent<SpriteRenderer>().sprite = sprite_TileIcon[t - 2];
                        }

                        tiles[r].GetComponent<SpriteRenderer>().color = color_Tiles[t + 1];
                        flame.color = color_Tiles[t + 1];
                        break;
                    }
                }
            }
        }

        switch (setTiles[presentLocation])
        {
            case TileCondition.None:
                text_TileEffect.text = "効果なし";
                break;
            case TileCondition.Damage:
                text_TileEffect.text = "敵からダメージを受ける";
                break;
            case TileCondition.HeavyDamage:
                text_TileEffect.text = "敵から大ダメージを受ける";
                break;
            case TileCondition.Burn:
                text_TileEffect.text = "【火傷状態】ターン開始時に極小ダメージを受け、\n与ダメージが半減する。継続3ターン。";
                break;
            case TileCondition.Paralysis:
                text_TileEffect.text = "【麻痺状態】ダイスを振った際に出目が、\n半減する。継続3ターン。";
                break;
            case TileCondition.Ice:
                text_TileEffect.text = "【氷状態】移動する際に通常の2倍出目を\n消費する。継続5ターン。";
                break;
            case TileCondition.Poison:
                text_TileEffect.text = "【毒状態】ターン開始時に小ダメージを受ける。\nまた、移動時に極小ダメージを受ける。継続4ターン。";
                break;
        }

        playUnit += 1;
        if (playUnit == 4) playUnit = 0;

        var md_Unit = DataList.mcDatas.Find(x => x.mc_Id == DataList_Battle.unit[playUnit].c_Id);

        if (md_Unit.mc_Costume)
        {
            moveChara.GetComponent<SpriteRenderer>().sprite = data_AnotherCostume.dataLists.Find(x => x.img_Id == md_Unit.mc_Id).img_Face[DataList_Battle.unit[playUnit].c_CostumeId];
        }
        else
        {
            moveChara.GetComponent<SpriteRenderer>().sprite = data_Character.dataLists.Find(x => x.img_Id == md_Unit.mc_Id).img_Face;
        }

        button_Roll[0].SetActive(true);
        button_Roll[0].transform.DOShakePosition(duration: 3, strength: 5f);
        button_Roll[0].transform.Find("Text_Num").GetComponent<Text>().text = $"1 ~ {6 + dicePower}";
    }

    /// <summary>
    /// 矢印表示
    /// </summary>
    private void DisplayArrow()
    {
        foreach (var i in button_Arrow) i.SetActive(true);

        if (moveChara.transform.position.z >= 15.6f) button_Arrow[0].SetActive(false);
        if (moveChara.transform.position.z <= 0) button_Arrow[3].SetActive(false);
        if (moveChara.transform.position.x <= -7.8f) button_Arrow[1].SetActive(false);
        if (moveChara.transform.position.x >= 7.8f) button_Arrow[2].SetActive(false);
    }

    public void Roll()
    {
        turn++;

        var r = Random.Range(1, 6 + dicePower + 1);
        button_Roll[1].transform.Find("Text_Power").GetComponent<Text>().text = $"威力:{r}";
        rollPower = r;
        button_Roll[0].SetActive(false);
        button_Roll[1].SetActive(true);

        float posX = tiles[presentLocation].transform.position.x;
        float posZ = tiles[presentLocation].transform.position.z;
        for (int i = 0; i < 49; i++)
        {
            var difference = 0;

            var tilePos = tiles[i].transform.position;
            var differenceX = Mathf.Abs((tilePos.x + 15.6f) - (posX + 15.6f));
            difference += (int)(differenceX / 2.6f);

            var differenceZ = Mathf.Abs(tilePos.z - posZ);
            difference += (int)System.Math.Round(differenceZ / 2.6f);

            if (difference <= rollPower)
            {
                tiles[i].transform.Find("Step Count").GetComponent<TextMesh>().text = $"-{difference}";
            }
            else
            {
                tiles[i].transform.Find("Step Count").GetComponent<TextMesh>().text = $"";
            }
        }

        if(conditionTurn > 0)
        {
            conditionTurn--;
        }
        switch (condition)
        {
            case Condition.none:
                break;
            case Condition.burn:
                var damage = Mathf.FloorToInt((player_TotalHP + phaseBoss.c_Attack) / 4);
                int defense = 0;
                foreach (var i in DataList_Battle.unit) defense += i.c_Defense;
                damage = damage * 100 / ((DataList_Battle.unit[playUnit].c_Defense / 10) + (defense / 1000)) / 100;
                DamageForPlayer(damage);
                obj_Status.transform.Find("Text").GetComponent<Text>().text = $"火傷　残り{conditionTurn}ターン";
                sourceSE.PlayOneShot(condition_SE[0]);
                break;
            case Condition.paralysis:
                button_Roll[1].transform.DOShakePosition(duration: 3, strength: 15f);
                rollPower = (int)System.Math.Round((float)rollPower, System.MidpointRounding.AwayFromZero);
                button_Roll[1].transform.Find("Text_Power").GetComponent<Text>().text = $"威力:{r}";
                obj_Status.transform.Find("Text").GetComponent<Text>().text = $"麻痺　残り{conditionTurn}ターン";
                sourceSE.PlayOneShot(condition_SE[1]);
                break;
            case Condition.ice:
                obj_Status.transform.Find("Text").GetComponent<Text>().text = $"氷　残り{conditionTurn}ターン";
                break;
            case Condition.poison:
                damage = Mathf.FloorToInt((player_TotalHP + phaseBoss.c_Attack) / 4);
                defense = 0;
                foreach (var i in DataList_Battle.unit) defense += i.c_Defense;
                damage = damage * 100 / ((DataList_Battle.unit[playUnit].c_Defense / 10) + (defense / 1000)) / 100;
                DamageForPlayer(damage);
                obj_Status.transform.Find("Text").GetComponent<Text>().text = $"毒　残り{conditionTurn}ターン";
                sourceSE.PlayOneShot(condition_SE[3]);
                break;
        }
        if (conditionTurn <= 0)
        {
            var anim = obj_Status.GetComponent<Animator>();
            anim.SetBool("IsC", false);
            condition = Condition.none;
        }

        DisplayArrow();
    }

    public async void MoveChara(int direction)
    {
        foreach (var i in button_Arrow) i.SetActive(false);
        foreach (var i in button_Roll) i.SetActive(false);

        rollPower--;
        if(condition == Condition.ice && rollPower > 0)
        {
            rollPower--;
            button_Roll[1].transform.DOShakePosition(duration: 3, strength: 15f);
            sourceSE.PlayOneShot(condition_SE[2]);
        }
        button_Roll[1].transform.Find("Text_Power").GetComponent<Text>().text = $"威力:{rollPower}";

        switch (direction)
        {
            case 0:
                moveChara.transform.DOMove(new Vector3(0, 0, 2.6f), 0.2f).SetRelative(true);
                presentLocation += 7;
                break;
            case 1:
                moveChara.transform.DOMove(new Vector3(-2.6f, 0, 0), 0.2f).SetRelative(true);
                presentLocation -= 1;
                break;
            case 2:
                moveChara.transform.DOMove(new Vector3(2.6f, 0, 0), 0.2f).SetRelative(true);
                presentLocation += 1;
                break;
            case 3:
                moveChara.transform.DOMove(new Vector3(0, 0, -2.6f), 0.2f).SetRelative(true);
                presentLocation -= 7;
                break;
        }

        await UniTask.Delay(200);

        switch (setTiles[presentLocation])
        {
            case TileCondition.None:
                text_TileEffect.text = "効果なし";
                break;
            case TileCondition.Damage:
                text_TileEffect.text = "敵からダメージを受ける";
                break;
            case TileCondition.HeavyDamage:
                text_TileEffect.text = "敵から大ダメージを受ける";
                break;
            case TileCondition.Burn:
                text_TileEffect.text = "【火傷状態】ターン開始時に小ダメージを受け、\n与ダメージが半減する。継続3ターン。";
                break;
            case TileCondition.Paralysis:
                text_TileEffect.text = "【麻痺状態】ダイスを振った際に出目が、\n半減する。継続3ターン。";
                break;
            case TileCondition.Ice:
                text_TileEffect.text = "【氷状態】移動する際に通常の2倍出目を\n消費する。継続5ターン。";
                break;
            case TileCondition.Poison:
                text_TileEffect.text = "【毒状態】ターン開始時に小ダメージを受ける。\nまた、移動時に極小ダメージを受ける。継続4ターン。";
                break;
        }

        if(condition == Condition.poison)
        {
            var damage = Mathf.FloorToInt((player_TotalHP + phaseBoss.c_Attack) / 8);
            int defense = 0;
            foreach (var i in DataList_Battle.unit) defense += i.c_Defense;
            damage = damage * 100 / ((DataList_Battle.unit[playUnit].c_Defense / 10) + (defense / 1000)) / 100;
            DamageForPlayer(damage);
            sourceSE.PlayOneShot(condition_SE[3]);
        }

        float posX = tiles[presentLocation].transform.position.x;
        float posZ = tiles[presentLocation].transform.position.z;
        for(int i = 0; i < 49; i++)
        {
            var difference = 0;

            var tilePos = tiles[i].transform.position;
            var differenceX = Mathf.Abs((tilePos.x + 15.6f) - (posX + 15.6f));
            difference += (int)(differenceX / 2.6f);

            var differenceZ = Mathf.Abs(tilePos.z - posZ);
            difference += (int)System.Math.Round(differenceZ / 2.6f);

            if(difference <= rollPower)
            {
                tiles[i].transform.Find("Step Count").GetComponent<TextMesh>().text = $"-{difference}";
            }
            else
            {
                tiles[i].transform.Find("Step Count").GetComponent<TextMesh>().text = $"";
            }
        }

        if (rollPower <= 0) TurnEnd();
        else DisplayArrow(); button_Roll[1].SetActive(true);
    }

    public async void Attack()
    {
        foreach (var i in button_Arrow) i.SetActive(false);
        foreach (var i in button_Roll) i.SetActive(false);

        var damage = DataList_Battle.unit[playUnit].c_Attack * rollPower;
        var random = Random.Range(0.85f, 1.00f);
        damage = Mathf.FloorToInt(damage * random);

        float correction = 1;
        var enemyAttribute = phaseBoss.c_Attribute;

        switch (DataList_Battle.unit[playUnit].c_Attribute)
        {
            case 0:
                if (enemyAttribute == 2) correction = 1.1f;
                if (enemyAttribute == 1) correction = 0.9f;
                break;
            case 1:
                if (enemyAttribute == 0) correction = 1.1f;
                if (enemyAttribute == 2) correction = 0.9f;
                break;
            case 2:
                if (enemyAttribute == 1) correction = 1.1f;
                if (enemyAttribute == 0) correction = 0.9f;
                break;

            case 3:
                if (enemyAttribute == 4) correction = 1.1f;
                if (enemyAttribute == 5) correction = 0.9f;
                break;
            case 4:
                if (enemyAttribute == 5) correction = 1.1f;
                if (enemyAttribute == 3) correction = 0.9f;
                break;
            case 5:
                if (enemyAttribute == 3) correction = 1.1f;
                if (enemyAttribute == 4) correction = 0.9f;
                break;
        }
        damage = Mathf.FloorToInt(damage * correction / 3);
        print($"ダメージ{damage}乱数{random}補正{correction}");

        text_Serif.text = $"{damage}ダメージの攻撃！";
        DamageForEnemy(damage);
        await UniTask.Delay(1500);
        rollPower = 0;

        TurnEnd();
    }
    public async void TurnEnd()
    {
        foreach (var i in button_Arrow) i.SetActive(false);
        foreach (var i in button_Roll) i.SetActive(false);
        button_Roll[1].SetActive(false);
        switch (setTiles[presentLocation])
        {
            case TileCondition.None:
                break;
            case TileCondition.Damage:
                text_Serif.text = $"敵の攻撃！";
                #region ダメージ計算式
                var damage = Mathf.FloorToInt(player_TotalHP / 2 + phaseBoss.c_Attack);
                var defense = 0;
                foreach (var i in DataList_Battle.unit) defense += i.c_Defense;
                damage = Mathf.FloorToInt(damage * 100 / ((DataList_Battle.unit[playUnit].c_Defense / 10) + (defense / 1000)));
                var random = Random.Range(0.85f, 1.00f);
                float correction = 1;
                var playerAttribute = DataList_Battle.unit[playUnit].c_Attribute;
                switch (phaseBoss.c_Attribute)
                {
                    case 0:
                        if (playerAttribute == 2) correction = 1.1f;
                        if (playerAttribute == 1) correction = 0.9f;
                        break;
                    case 1:
                        if (playerAttribute == 0) correction = 1.1f;
                        if (playerAttribute == 2) correction = 0.9f;
                        break;
                    case 2:
                        if (playerAttribute == 1) correction = 1.1f;
                        if (playerAttribute == 0) correction = 0.9f;
                        break;

                    case 3:
                        if (playerAttribute == 4) correction = 1.1f;
                        if (playerAttribute == 5) correction = 0.9f;
                        break;
                    case 4:
                        if (playerAttribute == 5) correction = 1.1f;
                        if (playerAttribute == 3) correction = 0.9f;
                        break;
                    case 5:
                        if (playerAttribute == 3) correction = 1.1f;
                        if (playerAttribute == 4) correction = 0.9f;
                        break;
                }
                damage = Mathf.FloorToInt(damage * random * correction / 10);
                #endregion
                print(damage);
                DamageForPlayer(damage);
                break;
            case TileCondition.HeavyDamage:
                text_Serif.text = $"敵の攻撃！";
                #region 大ダメージ計算式
                damage = Mathf.FloorToInt(player_TotalHP + phaseBoss.c_Attack);
                defense = 0;
                foreach (var i in DataList_Battle.unit) defense += i.c_Defense;
                damage = Mathf.FloorToInt(damage * 100 / ((DataList_Battle.unit[playUnit].c_Defense / 10) + (defense / 1000)));
                random = Random.Range(0.85f, 1.00f);
                correction = 1;
                playerAttribute = DataList_Battle.unit[playUnit].c_Attribute;
                switch (phaseBoss.c_Attribute)
                {
                    case 0:
                        if (playerAttribute == 2) correction = 1.1f;
                        if (playerAttribute == 1) correction = 0.9f;
                        break;
                    case 1:
                        if (playerAttribute == 0) correction = 1.1f;
                        if (playerAttribute == 2) correction = 0.9f;
                        break;
                    case 2:
                        if (playerAttribute == 1) correction = 1.1f;
                        if (playerAttribute == 0) correction = 0.9f;
                        break;

                    case 3:
                        if (playerAttribute == 4) correction = 1.1f;
                        if (playerAttribute == 5) correction = 0.9f;
                        break;
                    case 4:
                        if (playerAttribute == 5) correction = 1.1f;
                        if (playerAttribute == 3) correction = 0.9f;
                        break;
                    case 5:
                        if (playerAttribute == 3) correction = 1.1f;
                        if (playerAttribute == 4) correction = 0.9f;
                        break;
                }
                damage = Mathf.FloorToInt(damage * random * correction / 10);
                #endregion
                print(damage);
                DamageForPlayer(damage);
                break;
            case TileCondition.Burn:
                if (condition == Condition.none)
                {
                    text_Serif.text = $"火傷状態になってしまった！";
                    condition = Condition.burn;
                    conditionTurn = 3;
                    obj_Status.transform.Find("Text").GetComponent<Text>().text = $"火傷　残り{conditionTurn}ターン";
                    obj_Status.transform.Find("Image").GetComponent<Image>().sprite = sprite_TileIcon[0];
                    var anim = obj_Status.GetComponent<Animator>();
                    anim.SetBool("IsC", true);
                }
                else text_Serif.text = $"既に状態異常だ！";
                break;
            case TileCondition.Paralysis:
                if (condition == Condition.none)
                {
                    text_Serif.text = $"麻痺状態になってしまった！";
                    condition = Condition.paralysis;
                    conditionTurn = 3;
                    obj_Status.transform.Find("Text").GetComponent<Text>().text = $"麻痺　残り{conditionTurn}ターン";
                    obj_Status.transform.Find("Image").GetComponent<Image>().sprite = sprite_TileIcon[1];
                    var anim = obj_Status.GetComponent<Animator>();
                    anim.SetBool("IsC", true);
                }
                else text_Serif.text = $"既に状態異常だ！";
                break;
            case TileCondition.Ice:
                if (condition == Condition.none)
                {
                    text_Serif.text = $"氷状態になってしまった！";
                    condition = Condition.ice;
                    conditionTurn = 5;
                    obj_Status.transform.Find("Text").GetComponent<Text>().text = $"氷　残り{conditionTurn}ターン";
                    obj_Status.transform.Find("Image").GetComponent<Image>().sprite = sprite_TileIcon[2];
                    var anim = obj_Status.GetComponent<Animator>();
                    anim.SetBool("IsC", true);
                }
                else text_Serif.text = $"既に状態異常だ！";
                break;
            case TileCondition.Poison:
                if (condition == Condition.none)
                {
                    text_Serif.text = $"毒状態になってしまった！";
                    condition = Condition.poison;
                    conditionTurn = 4;
                    obj_Status.transform.Find("Text").GetComponent<Text>().text = $"毒　残り{conditionTurn}ターン";
                    obj_Status.transform.Find("Image").GetComponent<Image>().sprite = sprite_TileIcon[3];
                    var anim = obj_Status.GetComponent<Animator>();
                    anim.SetBool("IsC", true);
                }
                else text_Serif.text = $"既に状態異常だ！";
                break;
        }
        ResetTile();
        await UniTask.Delay(1000);
        if(player_Hp <= 0)
        {
            GamaOver();
        }
        else if (enemy_HP <= 0)
        {
            ChangePhase(phaseNum);
        }
        else
        {
            SetTurn();
        }
    }

    private async void ChangePhase(int phase)
    {
        if(phase < 3)
        {
            phaseNum++;

            whiteScreen.gameObject.SetActive(true);
            whiteScreen.alpha = 0;
            whiteScreen.DOFade(1, 1.5f);
            await UniTask.Delay(2000);

            SetEnemy(phaseNum);

            text_Serif.text = $"フェーズ{phaseNum - 1}クリア";

            ResetTile();

            whiteScreen.alpha = 1;
            whiteScreen.DOFade(0, 1.5f);
            await UniTask.Delay(1500);
            whiteScreen.gameObject.SetActive(false);

            SetTurn();
            text_Serif.text = $"右下のROLLボタンを押してダイスを振ろう！";
        }
        else //ゲームクリア
        {
            GameClear();
        }
    }

    private async void GameClear()
    {
        cg_GameFin.gameObject.transform.Find("Text").GetComponent<Text>().text = $"~ ゲームクリア ~";
        cg_GameFin.gameObject.transform.Find("Text_Conf").GetComponent<Text>().text = $"全ての敵を倒しました！";
        cg_GameFin.gameObject.transform.Find("Text_Turn").GetComponent<Text>().text = $"ターン数:{turn}";

        cg_GameFin.gameObject.SetActive(true);
        cg_GameFin.DOFade(1, 2);
        await UniTask.Delay(2000);
    }
    private async void GamaOver()
    {
        cg_GameFin.gameObject.transform.Find("Text").GetComponent<Text>().text = $"~ ゲームオーバー ~";
        cg_GameFin.gameObject.transform.Find("Text_Conf").GetComponent<Text>().text = $"全滅しました...";
        cg_GameFin.gameObject.transform.Find("Text_Turn").GetComponent<Text>().text = $"ターン数:{turn}";

        cg_GameFin.gameObject.SetActive(true);
        cg_GameFin.DOFade(1, 2);
        await UniTask.Delay(2000);
    }
    public async void LoadHome()
    {
        whiteScreen.gameObject.SetActive(true);
        whiteScreen.alpha = 0;
        whiteScreen.DOFade(1, 1.5f);
        await UniTask.Delay(1500);
        SceneManager.LoadScene("HomeScene");
    }

    private async void DamageForPlayer(int damage)
    {
        slider_PlayerHP.transform.DOShakePosition(duration: 0.5f, strength: 23);
        if (damage > 0)
        {
            for (var i = 0; i < 100; i++)
            {
                slider_PlayerHP.value -= damage / 90;
                text_PlayerHP.text = $"{Mathf.FloorToInt(slider_PlayerHP.value)}/{player_TotalHP}";
                await UniTask.Delay(1);
            }
            player_Hp -= damage;
            slider_PlayerHP.value = player_Hp;
            text_PlayerHP.text = $"{player_Hp}/{player_TotalHP}";
            if (player_Hp <= 0) text_PlayerHP.text = $"0/{player_TotalHP}";
            sourceSE.PlayOneShot(condition_SE[4]);
        }
    }
    private async void DamageForEnemy(int damage)
    {
        slider_EnemyHP.transform.DOShakePosition(duration: 0.5f, strength: 23);
        if(damage > 0)
        {
            for (var i = 0; i < 100; i++)
            {
                slider_EnemyHP.value -= damage / 90;
                await UniTask.Delay(1);
            }
            enemy_HP -= damage;
            slider_EnemyHP.value = enemy_HP;
        }
    }
}
