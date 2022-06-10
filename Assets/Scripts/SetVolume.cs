using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetLevel(float _sliderValue)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(_sliderValue) * 20);
    }
}
