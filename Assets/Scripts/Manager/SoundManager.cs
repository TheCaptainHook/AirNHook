using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioType
{
    None,
    Jump,
    Land,
    Death,
    UI_Click,
    UI_Popup,
    Title,
    Lobby,
    Tutorial, //튜토리얼 1~3까지는 같은 배경음
    Stage1_Normal, //스테이지1-1~4
    Stage1_Final, //스테이지1-5
}

public enum AudioMixerGroupType
{
    Effects,
    BGM,
}

public class SoundManager
{
    public AudioMixer audioMixer { get; private set; }
    private Dictionary<AudioType, AudioClip> _audioClipDict = new();
    private Queue<AudioSource> _deactivatedAudioSources = new();
    private AudioSource _bgmAudioSource;
    
    private WaitForSeconds _waitForSeconds = new(1f);
    private Dictionary<string, AudioMixerGroup> _audioMixerGroups = new();
    private const string AUDIO_SOURCE_PATH = "Prefabs/Sound/AudioSource";

    #region SetUpMethod
    public void SetUp()
    {
        AudioMixSetUp();
        AudioClipSetUp();
        AddAudioSources(8);
    }

    private void AudioMixSetUp()
    {
        // load audioMixer
        audioMixer = ResourceManager.Load<AudioMixer>("Sounds/AudioMixer");
        var audioMixerGroupArray = audioMixer.FindMatchingGroups(string.Empty);
        foreach (var audioMixerGroup in audioMixerGroupArray)
            _audioMixerGroups.Add(audioMixerGroup.name, audioMixerGroup);
        
        audioMixer.SetFloat("MasterParam",GetAudioMixVolume(PlayerPrefs.GetFloat("MasterVolume", 1f)));
        audioMixer.SetFloat("BGMParam", GetAudioMixVolume(PlayerPrefs.GetFloat("BGMVolume", 1f)));
        audioMixer.SetFloat("EffectsParam", GetAudioMixVolume(PlayerPrefs.GetFloat("EffectsVolume", 1f)));
    }

    private float GetAudioMixVolume(float volume)
    {
        return Mathf.Log10(volume) * 20;
    }
    
    private void AudioClipSetUp()
    {
        // audioClip save form scriptable object
        var audioClipSO = ResourceManager.Load<AudioClipSO>("Audio/ScriptableObject/AudioClipSO");
        foreach (var audioClipData in audioClipSO.audioList)
            _audioClipDict.Add(audioClipData.audioType, audioClipData.audioClip);
    }

    private void AddAudioSources(int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            var audioSource = ResourceManager.Instantiate(AUDIO_SOURCE_PATH).GetComponent<AudioSource>();
            Object.DontDestroyOnLoad(audioSource);
            audioSource.gameObject.SetActive(false);
            _deactivatedAudioSources.Enqueue(audioSource);
        }
    }
    #endregion

    #region PlaySoundMethod
    /// <summary>
    /// 사운드 재생. 배경음악은 PlayBgm()으로 실행할 것.
    /// </summary>
    /// <param name="audioType"> 오디오 타입 설정 </param>
    /// <param name="audioMixerGroupType"> Mixer Group 설정 </param>
    /// <param name="isLoop"> 반복 체크. default is false. </param>
    /// <param name="volume"> 소리 조절. default is 1f. </param>
    public void PlaySound(AudioType audioType, AudioMixerGroupType audioMixerGroupType, bool isLoop = false, float volume = 1f)
    {
        GetAudioSource(out var audioSource);
        
        PlayAudioClip(audioSource, audioType, audioMixerGroupType, isLoop, volume);
    }

    /// <summary>
    /// BGM재생. 다른 사운드의 경우 PlaySound()로 재생.
    /// </summary>
    /// <param name="audioType"> 오디오 타입 설정. </param>
    /// <param name="audioMixerGroupType"> Mixer Group 설정 </param>
    /// <param name="isLoop"> 반복 체크. default is false. </param>
    /// <param name="volume"> 소리 조절. default is 1f. </param>
    public void PlayBGM(AudioType audioType, AudioMixerGroupType audioMixerGroupType, bool isLoop = false, float volume = 1f)
    {
        AudioSource audioSource;
        
        //TODO Fade out Fade IN ?
        if (_bgmAudioSource is null)
            GetAudioSource(out audioSource);
        else
            audioSource = _bgmAudioSource;
        
        PlayAudioClip(audioSource, audioType, audioMixerGroupType, isLoop, volume);
    }
    
    private void GetAudioSource(out AudioSource audioSource)
    {
        if (_deactivatedAudioSources.TryDequeue(out audioSource)) return;
        
        AddAudioSources(5);
        audioSource = _deactivatedAudioSources.Dequeue();
    }

    // 오디오 클립 재생. 윗쪽 Method에서 호출함.
    private void PlayAudioClip(AudioSource audioSource, AudioType audioType, AudioMixerGroupType audioMixerGroupType, bool isLoop, float volume)
    {
        var audioClip = _audioClipDict[audioType];
        audioSource.outputAudioMixerGroup = _audioMixerGroups[audioMixerGroupType.ToString()];
        audioSource.loop = isLoop;
        audioSource.volume = volume;
        audioSource.gameObject.SetActive(true);
        audioSource.clip = audioClip;
        audioSource.Play();
        if(!isLoop)
            Managers.Instance.StartCoroutine(CollectSoundSource(audioSource, audioClip.length));
    }
    #endregion

    #region MemoryManage
    // sound source collector
    private IEnumerator CollectSoundSource(AudioSource audioSource, float clipLength)
    {
        var time = 0f;
        while (time <= clipLength)
        {
            yield return _waitForSeconds;
            time += 1f;
        }

        audioSource.gameObject.SetActive(false);
        _deactivatedAudioSources.Enqueue(audioSource);
    }
    #endregion
}
