using Mirror;
using System.Collections;
using UnityEngine;

public class ButtonActivatedDoor : ActivatableObjectEntity
{
    [CustomHeader("ButtonActivatedDoor")]
    // [SerializeField] private BoxCollider2D _collider;
    [SerializeField] NetworkAnimator _animator;

    #region StringCache
    private static readonly int UnlockTrigger = Animator.StringToHash("UnlockTrigger");
    private static readonly int LockTrigger = Animator.StringToHash("LockTrigger");
    #endregion

    public bool onOpen;

    public bool onPrograss;

    private void Awake(){
        _collider = GetComponent<Collider2D>();
    }
    protected override void Activation()
    {
        if (onPrograss) return;
        if (onOpen) return;
        onOpen = true;
        _collider.enabled = false;

        //Door open
        // _animator.SetTrigger(UnlockTrigger);
        //250124
        // Open();
        
        door_Net.HandleSetState(true);


    }

    // test
    //public void Update(){
    //    if(Input.GetKeyDown(KeyCode.Q))
    //    {
    //        ApplyActive(1);
    //    }
    //    if(Input.GetKeyDown(KeyCode.W))
    //    {
    //        ApplyActive(-1);
    //    }
    //}


    protected override void Deactivated()
    {
          if (onPrograss) return;
        if (!onOpen) return;

        onOpen = false;
        _collider.enabled = true;

        //Door close
        // _animator.SetTrigger(LockTrigger);
        //250124
        // Close();
        door_Net.HandleSetState(false);

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

        operateDoorCoroutine = StartCoroutine(Operation(true));
    }
    public void Close()
    {
        if(operateDoorCoroutine != null) StopCoroutine(operateDoorCoroutine);

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






