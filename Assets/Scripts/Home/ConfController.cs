using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class ConfController : MonoBehaviour
{
    [SerializeField] private GameObject obj_Loading;
    [SerializeField] private GameObject obj_Error;

    [Header("NameSpace")]
    [SerializeField] private Text text_NameSpace;
    [SerializeField] private InputField inputField_NameSpace;
    [SerializeField] private Button button_ChangeName;
    [SerializeField] private Text text_Error;

    [Header("AudioSetting")]
    [SerializeField] private List<Slider> sliders_Audio;
    [SerializeField] private List<Text> texts_Audio;

    public void InitialSetting()
    {
        for (var i = 0; i < sliders_Audio.Count; i++) ChangeValueAudio(i);
        text_NameSpace.text = DataList.userData.user_Name;
    }
    #region 名前変更
    public void ChangeName()
    {
        button_ChangeName.gameObject.SetActive(false);
        inputField_NameSpace.gameObject.SetActive(true);
        inputField_NameSpace.Select();
    }
    public void InputChanged()
    {
        obj_Loading.SetActive(true);
        if(inputField_NameSpace.text == "" || inputField_NameSpace.text == null)
        {
            text_Error.text = "文字が入力されていません";
            text_Error.gameObject.SetActive(true);
            inputField_NameSpace.gameObject.SetActive(false);
            button_ChangeName.gameObject.SetActive(true);
            StartCoroutine(falseObject());
            return;
        }

        DataList.userData.user_Name = inputField_NameSpace.text;

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"PlayerData",  PlayFabSimpleJson.SerializeObject(DataList.userData)}
            }
        },
        result =>
        {
            obj_Loading.SetActive(false);
            GetComponent<HomeUIDisplay>().SetHomeMenuUI();
            inputField_NameSpace.gameObject.SetActive(false);
            button_ChangeName.gameObject.SetActive(true);
            text_Error.gameObject.SetActive(false);
            text_NameSpace.text = DataList.userData.user_Name;
        },
        error =>
        {
            obj_Error.SetActive(true);
            obj_Loading.SetActive(false);
            var button = obj_Error.transform.Find("Button").GetComponent<ButtonExtention>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => InputChanged());
        });

        IEnumerator falseObject()
        {
            yield return new WaitForSeconds(2);
            text_Error.gameObject.SetActive(false);
        }
    }
    #endregion
    #region オーディオUI
    public void ChangeValueAudio(int sliderId)
    {
        if(sliderId == 0)
        {
            texts_Audio[sliderId].text = Mathf.FloorToInt(sliders_Audio[sliderId].value * 100).ToString();
        }
        else
        {
            texts_Audio[sliderId].text = Mathf.FloorToInt((sliders_Audio[sliderId].value * 1000) / 150 * 100).ToString();
        }
    }
    #endregion
}
