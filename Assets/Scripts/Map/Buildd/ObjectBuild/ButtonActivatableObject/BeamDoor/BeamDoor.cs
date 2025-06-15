using System.Collections;

using UnityEngine;

public class BeamDoor : ActivatableObjectEntity
{
   [CustomHeader("Beam Door")]



    [Space(20)]
    [ReadOnly]
    public bool onOpen;
    [ReadOnly]
    public bool onPrograss;

    BeamDoor_Net Net => GetComponent<BeamDoor_Net>();


    #region Animation
    Animator Animator => GetComponent<Animator>();
    readonly int Open = Animator.StringToHash("OnOpen");
    #endregion

    #region Get,Set
    public override async void SetData<T>(T data)
    {
        base.SetData(data);
            
        if(Application.isPlaying)
        {
            //Network Sync
            Net.Server_InitSync();
            //Network Sync

            await util.Delay(()=>{CheckActiveRequirAmount();});
        }
    }
    #endregion

    protected override void Activation()
    {
        OpenDoor();
    }
    protected override void Deactivated()
    {
        CloseDoor();
    }


    private void OpenDoor()
    {

        Animator.SetBool(Open, true);
      
    }

    private void CloseDoor()
    {
        Animator.SetBool(Open, false);
     
    }
   

}


