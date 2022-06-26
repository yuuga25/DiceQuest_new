using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "ScriptableObject/GachaData", fileName = "GachaData")]
public class Data_Gacha : ScriptableObject
{
    public List<DataBase> dataBases = new List<DataBase>();

    [Serializable]
    public class DataBase
    {
        public string dayOfWeek;
        public int picId_1;
        public int picId_2;
        public int picId_3;
        public Color color1;
        public Color color2;
    }
}
