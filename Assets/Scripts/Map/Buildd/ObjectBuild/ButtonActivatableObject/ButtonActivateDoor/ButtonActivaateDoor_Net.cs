
using System.Collections;
using UnityEngine;

public class ButtonActivaateDoor_Net : ActivatableObject_Net_Entity
{
    protected override void Active()
    {
        Open();
    }
    protected override void Deactive()
    {
        Close();
    }


    #region  Main Logic
    [SerializeField] Transform topDoor;
    [SerializeField] Transform bottomDoor;

    private float operateDoorRate =1;
    private Vector3 closeSet = new Vector3(1,1,0);
    private Vector3 openSet = new Vector3(1,0,0);
    private Coroutine operateDoorCoroutine;

    public void Open()
    {
        if(operateDoorCoroutine != null) StopCoroutine(operateDoorCoroutine);
        Col.enabled = false;
        operateDoorCoroutine = StartCoroutine(Operation(true));
    }
    public void Close()
    {
        if(operateDoorCoroutine != null) StopCoroutine(operateDoorCoroutine);
        Col.enabled = true;
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
        return y / 1;
    }
    #endregion

    #region  Clean
    public override void Clean_Value()
    {
        StopAllCoroutines();
        onActive = false;
        operateDoorCoroutine = null;
        Col.enabled = true;

        topDoor.localScale = closeSet;
        bottomDoor.localScale = closeSet;
    }
    #endregion
}
