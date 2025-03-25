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
        try{
            if (typeof(T) == typeof(ButtonActivatableObjectStruct))
            {
            ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
            ButtonActivatedObjectStruct = objData;
            }

        }
        catch
        {
             Debug.Log($"ERROR,{typeof(T)}");
        }
            
        if(Application.isPlaying)
        {
            //Network Sync
            Net.Server_InitSync();
            //Network Sync


            Util util  = new Util();
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
        //if(openOrCloseDoorCoroutine != null) StopCoroutine(openOrCloseDoorCoroutine);
        //openOrCloseDoorCoroutine = StartCoroutine(OpenOrClose(true));
    }

    private void CloseDoor()
    {
        Animator.SetBool(Open, false);
        // if(openOrCloseDoorCoroutine != null) StopCoroutine(openOrCloseDoorCoroutine);
        //openOrCloseDoorCoroutine = StartCoroutine(OpenOrClose(false));
    }
    //    private float percent;
    //    public float animationSpeed;
    //    IEnumerator OpenOrClose(bool onOff)
    //    {
    //        float val = onOff ? Time.fixedDeltaTime: -Time.fixedDeltaTime;

    //        while(0<=percent && percent <=1)
    //        {
    //            percent += val * animationSpeed;
    //            /**
    //            animator.SetFloat(XXX,percent);
    //            **/
    //            yield return null;
    //        }

    //        percent = Mathf.Clamp01(percent);
    //    }

}


/**
    1. Init Sync
        - ButtonActivatableObjectStruct
    2. Open, Close Function
    3. 



    NetworkAnimation,


**/
