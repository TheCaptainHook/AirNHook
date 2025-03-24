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

            //Network Sync


            Util util  = new Util();
            await util.Delay(()=>{CheckActiveRequirAmount();});
        }
    }
    #endregion


    private Coroutine openOrCloseDoorCoroutine;
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
        if(openOrCloseDoorCoroutine != null) StopCoroutine(openOrCloseDoorCoroutine);
        openOrCloseDoorCoroutine = StartCoroutine(OpenOrClose(true));
    }

    private void CloseDoor()
    {
         if(openOrCloseDoorCoroutine != null) StopCoroutine(openOrCloseDoorCoroutine);
        openOrCloseDoorCoroutine = StartCoroutine(OpenOrClose(false));
    }
    private float percent;
    public float animationSpeed;
    IEnumerator OpenOrClose(bool onOff)
    {
        float val = onOff ? Time.fixedDeltaTime: -Time.fixedDeltaTime;

        while(0<=percent && percent <=1)
        {
            percent += val * animationSpeed;
            /**
            animator.SetFloat(XXX,percent);
            **/
            yield return null;
        }

        percent = Mathf.Clamp01(percent);
    }

}


/**
    1. Init Sync
        - ButtonActivatableObjectStruct
    2. Open, Close Function
    3. 



    NetworkAnimation,


**/
