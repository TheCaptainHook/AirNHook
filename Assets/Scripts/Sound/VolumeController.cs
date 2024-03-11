using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _effectsSlider;
    
    
    // private void Start()
    // {
    //     Init();
    // }

    public void Init()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            Debug.Log("LoadVolume");
            LoadVolume();
        }
        else
        {
            SetMasterVolume();
            SetBGMVolume();
            SetEffectsVolume();
        }
    }

    private void SetMasterVolume()
    {
        float mastervolume = _masterSlider.value;
        _audioMixer.SetFloat("MasterParam", Mathf.Log10(mastervolume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", mastervolume);
    }

    private void SetBGMVolume()
    {
        float bgmvolume = _bgmSlider.value;
        _audioMixer.SetFloat("BGMParam", Mathf.Log10(bgmvolume) * 20);
        PlayerPrefs.SetFloat("BGMVolume", bgmvolume);
    }

    private void SetEffectsVolume()
    {
        float effectsVolume = _effectsSlider.value;
        _audioMixer.SetFloat("FXParam", Mathf.Log10(effectsVolume) * 20);
        PlayerPrefs.SetFloat("EffectsVolume", effectsVolume);
    }

    private void LoadVolume()
    {
        _masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        _audioMixer.SetFloat("MasterParam", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume")) * 20);
        
        _bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume");
        _audioMixer.SetFloat("BGMParam", Mathf.Log10(PlayerPrefs.GetFloat("BGMVolume")) * 20);
        
        _effectsSlider.value = PlayerPrefs.GetFloat("EffectsVolume");
        _audioMixer.SetFloat("EffectsParam", Mathf.Log10(PlayerPrefs.GetFloat("EffectsVolume")) * 20);
    }
}