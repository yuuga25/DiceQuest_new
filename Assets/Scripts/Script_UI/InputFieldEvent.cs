using UnityEngine;
using UnityEngine.UI;

public class InputFieldEvent : MonoBehaviour
{
    public void LineBreakCancellation()
    {
        InputField inputField = this.gameObject.GetComponent<InputField>();
        string value = inputField.text;
        if(value.IndexOf("\n") != -1)
        {
            value = value.Replace("\n", "");
            inputField.text = value;
        }
    }
}
