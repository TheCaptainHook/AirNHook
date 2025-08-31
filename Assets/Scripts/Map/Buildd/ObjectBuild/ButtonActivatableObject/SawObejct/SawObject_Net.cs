using System.Collections;
using UnityEngine;

public class SawObject_Net : ActivatableObject_Net_Entity
{
    [SerializeField] Animator animator;
    [SerializeField] ParticleSystem particle;
    [SerializeField] GameObject cameraShakeObject;
    private readonly int _ONACTIVE = Animator.StringToHash("onActive");

    private AudioSourceController audioSourceController;
    private Coroutine soundCoroutine;
    protected override void Active()
    {
        if (!cameraShakeObject.activeSelf) cameraShakeObject.SetActive(true);

        Col.enabled = true;
        animator.SetBool(_ONACTIVE, true);
        particle.Play();
        if (audioSourceController == null)
        {
            audioSourceController = Managers.Sound.PlaySound3D(GlobalText.SAW_SOUND_LOOP, transform.position, 1, true);
            return;
        }

        if (soundCoroutine != null)
        {
            StopCoroutine(soundCoroutine);
        }

        audioSourceController.GetAudioSource().volume = 1;
            
      
    }
    protected override void Deactive()
    {
        if (cameraShakeObject.activeSelf) cameraShakeObject.SetActive(false);
        
        Col.enabled = false;
        animator.SetBool(_ONACTIVE, false);
        particle.Stop();

        if (audioSourceController == null) return;
        soundCoroutine = StartCoroutine(FadeOut());
        
    }

    IEnumerator FadeOut()
    {
        var source = audioSourceController.GetAudioSource();
        float elis = source.volume;
        while (elis > 0)
        {
            elis = Mathf.MoveTowards(elis, 0, Time.deltaTime);
            source.volume = elis;
            yield return null;
        }
        source.volume = 0;
        Managers.Sound.StopSound(audioSourceController);
        audioSourceController = null;
    }
}
