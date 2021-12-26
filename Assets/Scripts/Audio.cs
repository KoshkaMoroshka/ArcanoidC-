using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Audio : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderEffect;

    private string nameMusic = "MusicVolume";
    private string nameEffect = "EffectsVolume";

    private void Start()
    {
        audioMixer.GetFloat(nameMusic, out float musicValue);
        audioMixer.GetFloat(nameEffect, out float effectValue);
        sliderMusic.value = musicValue;
        sliderEffect.value = effectValue;
        sliderMusic.onValueChanged.AddListener(SetMusicVolume);
        sliderEffect.onValueChanged.AddListener(SetEffectVolume);
    }

    public void SetMusicVolume(float volume) 
    {
        audioMixer.SetFloat(nameMusic, volume);
    }

    public void SetEffectVolume(float volume)
    {
        audioMixer.SetFloat(nameEffect, volume);
    }
}
