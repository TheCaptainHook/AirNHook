using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager
{
    public AudioMixer audioMixer { get; private set; }
    private Dictionary<string, AudioClip> _audioClipDict = new();
    private Queue<AudioSourceController> _deactivatedAudioSources = new();
    private List<AudioSourceController> _ambientAudioSources = new();
    private AudioSourceController _bgmAudioSource;

    private WaitForSeconds _waitForSeconds = new(0.2f);
    private Dictionary<string, AudioMixerGroup> _audioMixerGroups = new();

    private const int INITIAL_AUDIO_SOURCE_COUNT = 10; // 초기 오디오 소스 개수
    private const int MAX_ADDITIONAL_AUDIO_SOURCE_COUNT = 40; // 최대 추가 생성 가능한 오디오 소스 개수
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
            var audioSource = ResourceManager.Instantiate(GlobalText.AUDIO_SOURCE_PATH).GetComponent<AudioSourceController>();
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
    
    public AudioSourceController GetAudioSource()
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
    public AudioSourceController PlaySound(string audioName, float volume = 1f, bool isLoop = false)
    {
        return PlayAudioClip(audioName, volume, isLoop);
    }

    /// <summary>
    /// 해당 위치에 3d effect sound 재생.
    /// </summary>
    /// <param name="audioName">음악 이름(Global Text사용)</param>
    /// <param name="position">재생 위치</param>
    /// <param name="volume">volume 0~1, default : 1</param>
    /// <param name="isLoop">반복(default : false)</param>
    /// <param name="distance">사운드가 들리는 최대 거리(default : 10)</param>
    /// <param name="isRandomPitch">사운드 마다 랜덤 pitch 조정(default : false)</param>
    public AudioSourceController PlaySound3D(string audioName, Vector3 position, float volume = 1f, bool isLoop = false, int distance = 10, bool isRandomPitch = false)
    {
        return PlayAudioClip(audioName, position, volume, isLoop, distance, isRandomPitch);
    }

    /// <summary>
    /// 오브젝트를 따라가는(자식 느낌) 3d effect sound 재생.
    /// </summary>
    /// <param name="audioName">음악 이름(Global Text사용)</param>
    /// <param name="obj">따라갈 오브젝트의 transform</param>
    /// <param name="volume">volume 0~1, default : 1</param> 
    /// <param name="isLoop">반복(default : false)</param>
    /// <param name="destroyWhenParentsDestroyed">따라갈 오브젝트가 Destroy될 시, 사운드도 사라지게하기</param>
    /// <param name="distance">사운드가 들리는 최대 거리(default : 10)</param>
    /// <param name="isRandomPitch">사운드 마다 랜덤 pitch 조정(default : false)</param>
    public AudioSourceController PlaySound3D(string audioName, Transform obj, float volume = 1f, bool isLoop = false, bool destroyWhenParentsDestroyed = false, int distance = 10, bool isRandomPitch = false)
    {
        return PlayAudioClip(audioName, obj, volume, destroyWhenParentsDestroyed, isLoop, distance, isRandomPitch);
    }

    /// <summary>
    /// BGM 재생
    /// </summary>
    /// <param name="audioName">음악 이름(Global Text사용)</param>
    /// <param name="volume">volume 0~1, default : 1</param> 
    public AudioSourceController PlayBGM(string audioName, float volume = 1f)
    {
        return PlayBackgroundAudioClip(audioName, volume);
    }

    private bool GetAudioSource(out AudioSourceController audioSource)
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
    private AudioSourceController PlayAudioClip(string audioName, float volume, bool loop)
    {
        if (!GetAudioSource(out var audioSourceController)) return null;

        var audioClip = _audioClipDict[audioName];
        var audioSource = audioSourceController.GetAudioSource();

        audioSource.outputAudioMixerGroup = _audioMixerGroups[GlobalText.EFFECTS_STRING];
        SetAudioSource(audioSourceController, audioClip, volume, 0, loop);
        audioSourceController.Initialize(loop, false, false);

        if (loop)
            _ambientAudioSources.Add(audioSourceController);

        return audioSourceController;
    }

    // 3D effect sound용
    private AudioSourceController PlayAudioClip(string audioName, Vector3 position, float volume, bool loop, int distance, bool isRandomPitch)
    {
        if (!GetAudioSource(out var audioSourceController)) return null;

        var audioClip = _audioClipDict[audioName];
        var audioSource = audioSourceController.GetAudioSource();

        audioSource.outputAudioMixerGroup = _audioMixerGroups[GlobalText.EFFECTS_STRING];
        SetAudioSource(audioSourceController, audioClip, volume, 1f, loop, distance, null, isRandomPitch);
        audioSourceController.Initialize(loop, position, true, distance);

        if (loop)
            _ambientAudioSources.Add(audioSourceController);

        return audioSourceController;
    }

    // 3D effect sound용
    private AudioSourceController PlayAudioClip(string audioName, Transform obj, float volume, bool destroyWhenParentDestroyed, bool loop, int distance, bool isRandomPitch)
    {
        if (!GetAudioSource(out var audioSourceController)) return null;

        var audioClip = _audioClipDict[audioName];
        var audioSource = audioSourceController.GetAudioSource();

        audioSource.outputAudioMixerGroup = _audioMixerGroups[GlobalText.EFFECTS_STRING];
        SetAudioSource(audioSourceController, audioClip, volume, 1f, loop, distance, obj, isRandomPitch);
        audioSourceController.Initialize(loop, obj, destroyWhenParentDestroyed, true, distance);

        return audioSourceController;
    }

    public void StopSound(AudioSourceController audioSource)
    {
        if (audioSource == null) return;

        if (_ambientAudioSources.Contains(audioSource))
            _ambientAudioSources.Remove(audioSource);

        audioSource.gameObject.SetActive(false);
        _deactivatedAudioSources.Enqueue(audioSource);
    }

    // BGM 용
    private AudioSourceController PlayBackgroundAudioClip(string audioName, float volume)
    {
        AudioSourceController audioSourceController;

        if (_bgmAudioSource is null)
        {
            if (!GetAudioSource(out audioSourceController)) return null;
        }
        else
        {
            audioSourceController = _bgmAudioSource;
        }

        if (!_audioClipDict.TryGetValue(audioName, out var audioClip))
        {
            Debug.LogWarning(audioName + " audio name is not in dictionary.");
            return null;
        }

        audioSourceController.gameObject.SetActive(false);
        var audioSource = audioSourceController.GetAudioSource();

        audioSource.outputAudioMixerGroup = _audioMixerGroups[GlobalText.BGM_STRING];
        SetAudioSource(audioSourceController, audioClip, volume, 0, true);
        audioSourceController.Initialize(true, true, false);

        _bgmAudioSource = audioSourceController;

        return audioSourceController;
    }

    private void SetAudioSource(AudioSourceController audioSourceController, AudioClip clip, float volume, float spatialBlend, bool loop, int distance = 10, Transform targetTransform = null, bool isRandomPitch = false)
    {
        var audioSource = audioSourceController.GetAudioSource();

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = spatialBlend;
        audioSource.loop = loop;
        audioSource.maxDistance = distance;
        audioSource.pitch = isRandomPitch ? Random.Range(0.9f, 1.1f) : 1f;

        audioSourceController.gameObject.SetActive(true);
    }
    #endregion

    #region MemoryManage
    public void Recycle(AudioSourceController audioSource)
    {
        audioSource.gameObject.SetActive(false);
        _deactivatedAudioSources.Enqueue(audioSource);
    }

    public void CollectAmbientSoundSource()
    {
        foreach (var audioSource in _ambientAudioSources)
        {
            if (audioSource.gameObject.activeSelf)
            {
                audioSource.gameObject.SetActive(false);
                _deactivatedAudioSources.Enqueue(audioSource);
            }
        }
        
        _ambientAudioSources.Clear();
    }
    #endregion
}
