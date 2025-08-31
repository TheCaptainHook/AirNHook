using System.Collections;
using UnityEngine;

public class BeamDoor : ActivatableObjectEntity
{

    #region Animation
    Animator Animator => GetComponent<Animator>();
    readonly int Open = Animator.StringToHash("OnOpen");
    #endregion

    #region Get,Set

    #endregion

    public override void Activation()
    {
        Animator.SetBool(Open, true);
    }
    public override void Deactivated()
    {
        Animator.SetBool(Open, false);
    }


    #region Animation Trigger
    private Coroutine soundCoroutine;
    private AudioSourceController audioSourceController;
    private float minPitch = 0.7f;
    private float maxPitch = 1f;
    public void CloseSound()
    {
        if (audioSourceController == null) audioSourceController = Managers.Sound.PlaySound3D(GlobalText.BEAMDOOR_HUMMING, transform.position, 1, true);
        if (soundCoroutine != null) StopCoroutine(soundCoroutine);
        soundCoroutine = StartCoroutine(CloseSoundCo(audioSourceController));
    }
    private IEnumerator CloseSoundCo(AudioSourceController audioSourceController)
    {
        var source = audioSourceController.GetAudioSource();
        var pitch = minPitch;
        while (pitch < maxPitch)
        {
            pitch = Mathf.MoveTowards(pitch, maxPitch, Time.deltaTime);
            source.pitch = pitch;
            yield return null;
        }

    }
    public void OpenSound()
    {
        if (audioSourceController == null) return;
        if (soundCoroutine != null) StopCoroutine(soundCoroutine);
        soundCoroutine = StartCoroutine(OpenSouncCo(audioSourceController));

    }
    private IEnumerator OpenSouncCo(AudioSourceController audioSourceController)
    {
        var source = audioSourceController.GetAudioSource();
        var pitch = source.pitch;
        while (pitch > minPitch)
        {
            pitch = Mathf.MoveTowards(pitch, minPitch, Time.deltaTime);
            source.pitch = pitch;
            yield return null;
        }

        Managers.Sound.StopSound(audioSourceController);
        source.pitch = 1;

        this.audioSourceController = null;
        soundCoroutine = null;
    }
   #endregion
}


