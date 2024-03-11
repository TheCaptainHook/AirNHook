using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    private VolumeController _volumeController;
    
    private AudioMixer _audioMixer;
    
    private AudioSource _musicAudioSource;
    public AudioClip clip;

    private void Awake()
    {
        gameObject.AddComponent<AudioSource>();

        _volumeController = GetComponent<VolumeController>();
        _musicAudioSource = GetComponent<AudioSource>();
        _audioMixer = Resources.Load<AudioMixer>("Sounds/AudioMixer");
        AudioMixerGroup[] audioMixGroup = _audioMixer.FindMatchingGroups("BG_Sound");

        _musicAudioSource.outputAudioMixerGroup = audioMixGroup[0];
        _musicAudioSource.loop = true;
        
        _volumeController.Init();
    }


    public void ChangeBGM(AudioClip clip)
    {
        _musicAudioSource.Stop();
        _musicAudioSource.clip = clip;
        _musicAudioSource.Play();

    }
    //ToDO: 예전 프로젝트에서 만든 매니저라서 리소스 매니저를 통해 오브젝트 풀링을 했었음 바뀌게 된다면 다시 구현할 생각으로 주석화
    // public void PlayClip(AudioClip clip)
    // {
    //     SoundSource soundSource = Managers.RM.Instantiate("Sounds/FX_SoundSource").GetComponent<SoundSource>();
    //     soundSource.Play(clip);
    // }

    public void BGMStop()
    {
        _musicAudioSource.Stop();
    }
}
