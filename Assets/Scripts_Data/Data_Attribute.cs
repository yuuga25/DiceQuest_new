using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "ScriptableObject/AttributeData", fileName = "AttributeData")]
public class Data_Attribute : ScriptableObject
{
    public List<DataBase> dataBases = new List<DataBase>();

    [Serializable]
    public class DataBase
    {
        public string at_Name;
        public Sprite at_Icon;
        public Color at_Color1;
        public Color at_Color2;
    }
}
