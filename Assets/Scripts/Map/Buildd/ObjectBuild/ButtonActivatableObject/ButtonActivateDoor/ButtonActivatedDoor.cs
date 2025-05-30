using Mirror;
using System.Collections;
using UnityEngine;

public class ButtonActivatedDoor : ActivatableObjectEntity
{
    [CustomHeader("ButtonActivatedDoor")]

    public bool onOpen;

    //public bool onPrograss;

    #region Get,Set
    public override async void SetData<T>(T data)
    {
        if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
            ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
            ButtonActivatedObjectStruct = objData;

            if(Application.isPlaying)
            {
                door_Net.onSync = true;
                door_Net.Server_InitSync();

                await new Util().Delay(() => { CheckActiveRequirAmount(); });
            }
        

        }

     
    }
    #endregion

    protected override void Activation()
    {
        onOpen = true;
        door_Net.Server_ChangeDoorState(true);
    }



    protected override void Deactivated()
    {
        onOpen = false;
        door_Net.Server_ChangeDoorState(false);
    }

    

    //----------------------------------------------------------------Refactoring 250124
    [SerializeField]  ButtonActivaateDoor_Net door_Net;
    [SerializeField] Transform topDoor;
    [SerializeField] Transform bottomDoor;

    private float operateDoorRate =1;
    private Vector3 closeSet = new Vector3(1,1,0);
    private Vector3 openSet = new Vector3(1,0,0);
    private Coroutine operateDoorCoroutine;

    public void Open()
    {
        if(operateDoorCoroutine != null) StopCoroutine(operateDoorCoroutine);
        _collider.enabled = false;
        operateDoorCoroutine = StartCoroutine(Operation(true));
    }
    public void Close()
    {
        if(operateDoorCoroutine != null) StopCoroutine(operateDoorCoroutine);
        _collider.enabled = true;
        operateDoorCoroutine = StartCoroutine(Operation(false));
    }

    IEnumerator Operation(bool openOrClose)//true: open, false : close
    {
        Vector3 set = openOrClose ? openSet : closeSet;
        float percent = openOrClose ? 1 - topDoor.localScale.y : GetCurrentLocalScalePercent() ;
         
        while(percent < operateDoorRate)
        {
            percent += Time.deltaTime;
            topDoor.localScale = Vector3.Lerp(topDoor.localScale,set,percent);
            bottomDoor.localScale = Vector3.Lerp(topDoor.localScale,set,percent);
            yield return null;
        }

        topDoor.localScale = set;
        bottomDoor.localScale = set;

    } 


    private float GetCurrentLocalScalePercent()
    {
        float y = topDoor.localScale.y;
        return y/1;
    }
    //----------------------------------------------------------------Refactoring 250124

}






