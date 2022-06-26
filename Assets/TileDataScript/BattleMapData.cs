using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenu(menuName = "BattleScript/TilesData", fileName = "TilesData")]
public class BattleMapData : ScriptableObject
{
    [Header("7 = 障害物")][Space(-10)]
    [Header("3 = 毒, 4 = 麻痺, 5 = 炎, 6 = 氷")][Space(-10)]
    [Header("0 = なし, 1 = 大ダメージ, 2 = ダメージ")][Space(-10)]
    [Header("tiles早見表:Sizeは必ず'49'")]
    public float randomProbability = 0;
    public List<DataBase> dataBases = new List<DataBase>();

    [Serializable]
    public class DataBase
    {
        public BattleMapType.type type;
        [SmartBoolArray]
        public int[] tiles = new int[49];
    }
}

public class BattleMapType
{
    public enum type
    {
        none,
        first
    }
}
