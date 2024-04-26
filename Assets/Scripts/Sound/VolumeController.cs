using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    private AudioMixer _audioMixer;
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _effectsSlider;
    
    private const float defualtMasterVolume = 100;
    private const float defualtBGMVolume = 30;
    private const float defualtFXVolume = 50;

    private void Awake()
    {
        _masterSlider.onValueChanged.AddListener(SetMasterVolume);
        _bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        _effectsSlider.onValueChanged.AddListener(SetEffectsVolume);
    }

    public void Start()
    {
        _audioMixer = Managers.Sound.audioMixer;
        _masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        _bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        _effectsSlider.value = PlayerPrefs.GetFloat("EffectsVolume", 1f);
    }
    
    private float GetAudioMixVolume(float volume)
    {
        return Mathf.Log10(volume) * 20;
    }

    private void SetMasterVolume(float value)
    {
        _audioMixer.SetFloat("MasterParam", GetAudioMixVolume(value));
        PlayerPrefs.SetFloat("MasterVolume", _masterSlider.value);
    }

    private void SetBGMVolume(float value)
    {
        _audioMixer.SetFloat("BGMParam", GetAudioMixVolume(value));
        PlayerPrefs.SetFloat("BGMVolume", _bgmSlider.value);
    }

    private void SetEffectsVolume(float value)
    {
        _audioMixer.SetFloat("EffectsParam", GetAudioMixVolume(value));
        PlayerPrefs.SetFloat("EffectsVolume", _effectsSlider.value);
    }
}
