using System.Collections;
using System.Runtime.InteropServices;
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
    public void OpenSound()
    {
        if (audioSourceController == null)
        {

        }
    }
    // private IEnumerator OpenSoundCo(AudioSourceController audioSourceController)
    // {

    // }
    public void CloseSound()
    {

    }
    
   #endregion
}


