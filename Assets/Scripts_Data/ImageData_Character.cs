using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "ScriptableObject/ImageData_Character", fileName ="ImageData_Character")]
public class ImageData_Character : ScriptableObject
{
    public List<DataBase> dataLists = new List<DataBase>();

    [Serializable]
    public class DataBase
    {
        public string img_CharaName;
        public int img_Id;
        public Sprite img_Face;
        public Sprite img_Stand;
    }
}
