using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public enum AudioType
    {
        BGM,
        SE
    }

    public AudioType audioType;

    private float masterVolume;

    public static float AudioVolume_Master;
    public static float AudioVolume_BGM;
    public static float AudioVolume_SE;

    private void Start()
    {
        masterVolume = AudioVolume_Master;
    }

    private void Update()
    {
        if(audioType == AudioType.BGM && this.gameObject.GetComponent<AudioSource>().volume != AudioVolume_BGM)
        {
            var audio = this.gameObject.GetComponent<AudioSource>();
            audio.volume = AudioVolume_BGM * AudioVolume_Master;
            masterVolume = AudioVolume_Master;
        }
        else if (audioType == AudioType.SE && this.gameObject.GetComponent<AudioSource>().volume != AudioVolume_SE)
        {
            var audio = this.gameObject.GetComponent<AudioSource>();
            audio.volume = AudioVolume_SE * AudioVolume_Master;
            masterVolume = AudioVolume_Master;
        }
    }
}
