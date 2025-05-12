using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager
{
    public AudioMixer audioMixer { get; private set; }
    private Dictionary<string, AudioClip> _audioClipDict = new();
    private Queue<AudioSource> _deactivatedAudioSources = new();
    private List<AudioSource> _ambientAudioSources = new();
    private AudioSource _bgmAudioSource;

    private WaitForSeconds _waitForSeconds = new(0.2f);
    private Dictionary<string, AudioMixerGroup> _audioMixerGroups = new();

    private const int INITIAL_AUDIO_SOURCE_COUNT = 10; // 초기 오디오 소스 개수
    private const int MAX_ADDITIONAL_AUDIO_SOURCE_COUNT = 30; // 최대 추가 생성 가능한 오디오 소스 개수
    private int _additionalAudioSourceCount = 0;

    #region SetUpMethod
    public void SetUp()
    {
        AudioMixSetUp();
        AudioClipSetUp();
        AddAudioSources(INITIAL_AUDIO_SOURCE_COUNT);
    }
    
    private void AudioMixSetUp()
    {
        // load audioMixer
        audioMixer = ResourceManager.Load<AudioMixer>(GlobalText.AUDIOMIXER_PATH);
        var audioMixerGroupArray = audioMixer.FindMatchingGroups(string.Empty);
        foreach (var audioMixerGroup in audioMixerGroupArray)
            _audioMixerGroups.Add(audioMixerGroup.name, audioMixerGroup);

        audioMixer.SetFloat(GlobalText.MASTER_PARAMETER_STRING, GetAudioMixVolume(PlayerPrefs.GetFloat(GlobalText.MASTER_VOLUME_STRING, 1f)));
        audioMixer.SetFloat(GlobalText.BGM_PARAMETER_STRING, GetAudioMixVolume(PlayerPrefs.GetFloat(GlobalText.BGM_VOLUME_STRING, 1f)));
        audioMixer.SetFloat(GlobalText.EFFECT_PARAMETER_STRING, GetAudioMixVolume(PlayerPrefs.GetFloat(GlobalText.EFFECT_VOLUME_STRING, 1f)));
    }

    private float GetAudioMixVolume(float volume)
    {
        return Mathf.Log10(volume) * 20;
    }

    private void AudioClipSetUp()
    {
        // audioClip save form scriptable object
        var audioClipSO = ResourceManager.Load<AudioClipSO>(GlobalText.AUDIO_CLIP_SO_PATH);
        foreach (var audioClipData in audioClipSO.audioList)
        {
            _audioClipDict.Add(audioClipData.name, audioClipData); 
        }
    }

    private void AddAudioSources(int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            var audioSource = ResourceManager.Instantiate(GlobalText.AUDIO_SOURCE_PATH).GetComponent<AudioSource>();
            Object.DontDestroyOnLoad(audioSource);
            audioSource.gameObject.SetActive(false);
            _deactivatedAudioSources.Enqueue(audioSource);
        }
    }
    
    public AudioClip GetAudioClip(string audioName)
    {
        return _audioClipDict[audioName];
    }
    
    public AudioMixerGroup GetAudioMixerGroup(string name)
    {
        return _audioMixerGroups[name];
    }
    
    public AudioSource GetAudioSource()
    {
        return _deactivatedAudioSources.Dequeue();
    }
    #endregion

    #region PlaySoundMethod
    /// <summary>
    /// 2D effect sound 재생
    /// </summary>
    /// <param name="audioName">음악 이름(Global Text사용)</param>
    /// <param name="volume">volume 0~1, default : 1</param>
    /// <param name="isLoop">반복(default : false)</param>
    public void PlaySound(string audioName, float volume = 1f, bool isLoop = false)
    {
        PlayAudioClip(audioName, volume, isLoop);
    }

    /// <summary>
    /// 해당 위치에 3d effect sound 재생.
    /// </summary>
    /// <param name="audioName">음악 이름(Global Text사용)</param>
    /// <param name="position">재생 위치</param>
    /// <param name="volume">volume 0~1, default : 1</param>
    /// <param name="isLoop">반복(default : false)</param>
    public void PlaySound3D(string audioName, Vector3 position, float volume = 1f, bool isLoop = false)
    {
        PlayAudioClip(audioName, position, volume, isLoop);
    }
    
    /// <summary>
    /// 오브젝트를 따라가는(자식 느낌) 3d effect sound 재생.
    /// </summary>
    /// <param name="audioName">음악 이름(Global Text사용)</param>
    /// <param name="obj">따라갈 오브젝트의 transform</param>
    /// <param name="volume">volume 0~1, default : 1</param> 
    /// <param name="isLoop">반복(default : false)</param>
    /// <param name="destroyWhenParentsDestroyed">따라갈 오브젝트가 Destroy될 시, 사운드도 사라지게하기</param>
    public void PlaySound3D(string audioName, Transform obj, float volume = 1f, bool isLoop = false, bool destroyWhenParentsDestroyed = false)
    {
        PlayAudioClip(audioName, obj, volume, destroyWhenParentsDestroyed, isLoop);
    }

    /// <summary>
    /// BGM 재생
    /// </summary>
    /// <param name="audioName">음악 이름(Global Text사용)</param>
    /// <param name="volume">volume 0~1, default : 1</param> 
    public void PlayBGM(string audioName, float volume = 1f)
    {
        PlayBackgroundAudioClip(audioName, volume);
    }

    private bool GetAudioSource(out AudioSource audioSource)
    {
        if (_deactivatedAudioSources.TryDequeue(out audioSource))
            return true;

        if (_additionalAudioSourceCount < MAX_ADDITIONAL_AUDIO_SOURCE_COUNT)
        {
            AddAudioSources(5);
            _additionalAudioSourceCount += 5;
            audioSource = _deactivatedAudioSources.Dequeue();
            return true;
        }

        audioSource = null;
        Debug.LogWarning("No available audio source to play sound.");
        return false;
    }

    // 2D effect sound용
    private void PlayAudioClip(string audioName, float volume, bool loop)
    {
        if (!GetAudioSource(out var audioSource)) return;

        var audioClip = _audioClipDict[audioName];
        audioSource.outputAudioMixerGroup = _audioMixerGroups[GlobalText.EFFECTS_STRING];
        SetAudioSource(audioSource, audioClip, volume, 0, loop);
        
        if (loop)
            _ambientAudioSources.Add(audioSource);
        else
            Managers.Instance.StartCoroutine(CollectSoundSource(audioSource, audioClip.length));
    }
    //------------TEST 0512
    //  public AudioSource PlayAudioClip_(string audioName, float volume, bool loop)
    // {
    //     if (!GetAudioSource(out var audioSource)) return null;

    //     var audioClip = _audioClipDict[audioName];
    //     audioSource.outputAudioMixerGroup = _audioMixerGroups[GlobalText.EFFECTS_STRING];
    //     SetAudioSource(audioSource, audioClip, volume, 0, loop);
        
    //     if (loop)
    //         _ambientAudioSources.Add(audioSource);
    //     else
    //         Managers.Instance.StartCoroutine(CollectSoundSource(audioSource, audioClip.length));

    //     return audioSource;
    // }
    // public void ShutDownAudioSource(AudioSource audioSource)
    // {
    //     audioSource.Stop();
    //     audioSource.gameObject.SetActive(false);
    //     _deactivatedAudioSources.Enqueue(audioSource);
    // }
    //------------TEST 0512
    

    // 3D effect sound용
    private void PlayAudioClip(string audioName, Vector3 position, float volume, bool loop)
    {
        if (!GetAudioSource(out var audioSource)) return;

        var audioClip = _audioClipDict[audioName];
        audioSource.outputAudioMixerGroup = _audioMixerGroups[GlobalText.EFFECTS_STRING];
        audioSource.transform.position = position;
        SetAudioSource(audioSource, audioClip, volume, 1, loop);
        
        if (loop)
            _ambientAudioSources.Add(audioSource);
        else
            Managers.Instance.StartCoroutine(CollectSoundSource(audioSource, audioClip.length));
    }

    // 3D effect sound용
    private void PlayAudioClip(string audioName, Transform obj, float volume, bool destroyWhenParentDestroyed, bool loop)
    {
        if (!GetAudioSource(out var audioSource)) return;

        var audioClip = _audioClipDict[audioName];
        audioSource.outputAudioMixerGroup = _audioMixerGroups[GlobalText.EFFECTS_STRING];
        audioSource.transform.position = obj.position;
        SetAudioSource(audioSource, audioClip, volume, 1, loop);

        Managers.Instance.StartCoroutine(CollectSoundSource(audioSource, audioClip.length, obj, destroyWhenParentDestroyed));
    }

    // BGM 용
    private void PlayBackgroundAudioClip(string audioName, float volume)
    {
        AudioSource audioSource;

        if (_bgmAudioSource is null)
        {
            if (!GetAudioSource(out audioSource)) return;
        }
        else
        {
            audioSource = _bgmAudioSource;
        }

        if (!_audioClipDict.TryGetValue(audioName, out var audioClip))
        {
            Debug.LogWarning(audioName + " audio name is not in dictionary.");
            return;
        }
        audioSource.outputAudioMixerGroup = _audioMixerGroups[GlobalText.BGM_STRING];
        SetAudioSource(audioSource, audioClip, volume, 0, true);
        _bgmAudioSource = audioSource;
    }

    private void SetAudioSource(AudioSource audioSource, AudioClip clip, float volume, float spatialBlend, bool loop)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = spatialBlend;
        audioSource.loop = loop;
        audioSource.gameObject.SetActive(true);
        audioSource.Play();
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
            time += 0.2f;
        }

        audioSource.gameObject.SetActive(false);
        _deactivatedAudioSources.Enqueue(audioSource);
    }

    // moving position and collect sound
    private IEnumerator CollectSoundSource(AudioSource audioSource, float clipLength, Transform obj, bool destroy)
    {
        var time = 0f;
        while (time <= clipLength)
        {
            yield return null;
            
            time += Time.deltaTime;


            if (obj is not null)
            {
                audioSource.transform.position = obj.position;
                continue;
            }

            if (destroy) break;
        }

        audioSource.gameObject.SetActive(false);
        _deactivatedAudioSources.Enqueue(audioSource);
    }

    public void CollectAmbientSoundSource()
    {
        foreach (var audioSource in _ambientAudioSources)
        {
            audioSource.gameObject.SetActive(false);
            _deactivatedAudioSources.Enqueue(audioSource);
        }
        
        _ambientAudioSources.Clear();
    }
    #endregion
}
