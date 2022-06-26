using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "ScriptableObject/ImageData_AnotherCostume", fileName = "ImageData_AnotherCostume")]
public class ImageData_AnotherCostume : ScriptableObject
{
    public List<DataBase> dataLists = new List<DataBase>();

    [Serializable]
    public class DataBase
    {
        public string img_CharaName;
        public int img_Id;
        public List<Sprite> img_Face = new List<Sprite>();
        public List<Sprite> img_Stand = new List<Sprite>();
    }
}