using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;

/// <summary>
/// サーバーから取得したデータを一時的に保存しておくデータ
/// </summary>
public class DataList
{
    public static Data_User userData = new Data_User();
    public static string isTutorialCompleted;

    /// <summary>
    /// キャラクターのマスターデータ
    /// </summary>
    public static List<MasterData_Character> mcDatas = new List<MasterData_Character>();
    /// <summary>
    /// ステージのマスターデータ
    /// </summary>
    public static List<MasterData_Stage> msDatas = new List<MasterData_Stage>();
}

/// <summary>
/// ユーザーのデータ
/// </summary>
public class Data_User
{
    public string user_Name { get; set; }       //名前
    public int user_Rank { get; set; }          //ランク
    public string user_NickName0 { get; set; }  //二つ名　上
    public string user_NickName1 { get; set; }  //二つ名　下
    public Data_Character user_FavoriteChara { get; set; }  //お気に入りキャラ
    public int user_ExpPoint { get; set; }      //経験値
    public int user_UntilRankUp { get; set; }   //次のランクまで
    public int user_Money { get; set; }         //所持金
    public int user_MagicStone { get; set; }    //所持魔法石


    public string[] user_Formations = new string[4];        //編成リスト
    public List<Data_Character> user_Character = new List<Data_Character>();//所持キャラリスト

    public Data_Status user_Status = new Data_Status(); //ステータスデータ

    public List<int> user_PictureBook = new List<int>(); //図鑑

    public Data_AnotherCostume user_DataCostume = new Data_AnotherCostume(); //コスチュームの所持可否
}

/// <summary>
/// プレイ記録などのユーザーのデータ
/// </summary>
public class Data_Status
{
    public int plays { get; set; }  //プレイ数
    public int clears { get; set; } //クリア数
}

/// <summary>
/// キャラクターのマスタデータ
/// </summary>
public class MasterData_Character
{
    public int mc_Id { get; set; }       //ID
    public string mc_Name { get; set; }  //名前
    public int mc_Rarity { get; set; }   //レアリティ
    public bool mc_Costume { get; set; } //別衣装の可否
    public int mc_Attribute { get; set; }//属性
    public int mc_SkillId { get; set; }  //スキルID
    public int mc_BattleType { get; set; }//戦型
    public int mc_Attack { get; set; }   //攻撃力
    public int mc_Defense { get; set; }  //防御力
    public int mc_HP { get; set; }       //HP
    public int mc_RiseValue { get; set; }//上昇値
    public int mc_DicePower { get; set; }//ダイスパワー
    public int mc_Ability1 { get; set; } //アビリティ1
    public int mc_Ability2 { get; set; } //アビリティ2
    public int mc_Ability3 { get; set; } //アビリティ3
}

/*
 属性
 0 = 炎
 1 = 氷
 2 = 草
 3 = 雷
 4 = 光
 5 = 闇

 戦型
 0 = バランス
 1 = アタッカー
 2 = ディフェンス
 */

/// <summary>
/// キャラクターのデータ
/// </summary>
public class Data_Character
{
    public int c_Id { get; set; }           //ID
    public string c_UniqueId { get; set; }  //ユニークID
    public int c_Level { get; set; }        //レベル
    public int c_ExpPoint { get; set; }     //経験値
    public int c_UntilLevelUp { get; set; } //次のレベルまで
    public int c_Rarity { get; set; }       //レアリティ
    public int c_Attribute { get; set; }    //属性
    public int c_BattleType { get; set; }   //戦型
    public int c_Attack { get; set; }       //攻撃力
    public int c_Defense { get; set; }      //防御力
    public int c_HP { get; set; }           //HP
    public int c_DicePower { get; set; }    //ダイスパワー
    public int c_CostumeId { get; set; }    //コスチュームのID（デフォルト　0）
    public int c_Uses { get; set; }         //使用回数
}

public class Data_AnotherCostume
{
    public bool[] cos_Jormungand = new bool[2]; //ヨルムンガンドのコスチューム
    public bool[] cos_Percival = new bool[2];   //パーシヴァルのコスチューム
    public bool[] cos_Odin = new bool[4];       //オーディンのコスチューム
    public bool[] cos_Frist = new bool[2];      //フリストのコスチューム
    public bool[] cos_Valkyrie = new bool[5];   //ヴァルキリーのコスチューム
    public bool[] cos_Gerbera = new bool[2];    //ガーベラのコスチューム
}

public class MasterData_Stage
{
    public int stage_Id { get; set; }       //ステージID
    public string stage_Name { get; set; }  //ステージ名
    public int stage_CharaId { get; set; }  //ステージメインキャラID
    public int stage_First { get; set; }    //フェーズ1のキャラID
    public int stage_Second { get; set; }   //フェーズ2のキャラID
    public int stage_Third { get; set; }    //フェーズ3のキャラID
    public int stage_Recommendation { get; set; }   //ステージの推奨レベル
    public int stage_Damage { get; set; }   //ダメージ床の数
    public int stage_HeavyDamage { get; set; }  //大ダメージ床の数
    public int stage_Burn { get; set; }     //火傷床の数
    public int stage_Paralysis { get; set; }    //麻痺床の数
    public int stage_Ice { get; set; }      //氷床の数
    public int stage_Poison { get; set; }   //毒床の数
}
