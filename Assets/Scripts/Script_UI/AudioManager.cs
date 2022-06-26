using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Slider slider_Master;
    public Slider slider_BGM;
    public Slider slider_SE;

    private const float audioVolume_Master = 1;
    private const float audioVolume_BGM = 0.15f;
    private const float audioVolume_SE = 0.15f;

    private void Start()
    {
        slider_Master.maxValue = audioVolume_Master;
        slider_BGM.maxValue = audioVolume_BGM;
        slider_SE.maxValue = audioVolume_SE;

        slider_Master.value = PlayerPrefs.GetFloat("Audio_Master", 1);
        slider_BGM.value = PlayerPrefs.GetFloat("Audio_BGM", 0.15f);
        slider_SE.value = PlayerPrefs.GetFloat("Audio_SE", 0.15f);
    }

    public void AudioVolumeChange(int audioType)
    {
        switch (audioType)
        {
            case 0:
                AudioController.AudioVolume_Master = slider_Master.value;
                break;
            case 1:
                AudioController.AudioVolume_BGM = slider_BGM.value;
                break;
            case 2:
                AudioController.AudioVolume_SE = slider_SE.value;
                break;
        }
    }

    public void SaveAudio(int audioType)
    {
        switch (audioType)
        {
            case 0:
                PlayerPrefs.SetFloat("Audio_Master", AudioController.AudioVolume_Master);
                break;
            case 1:
                PlayerPrefs.SetFloat("Audio_BGM", AudioController.AudioVolume_BGM);
                break;
            case 2:
                PlayerPrefs.SetFloat("Audio_SE", AudioController.AudioVolume_SE);
                break;
        }

        PlayerPrefs.Save();
    }
}
