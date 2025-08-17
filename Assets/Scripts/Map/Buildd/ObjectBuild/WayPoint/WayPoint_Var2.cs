using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WayPoint_Var2 : BuildObj
{
     #region Components
    private NetworkStartPosition networkStartPosition;
    private Animator animator;
    private Collider2D col;
    #endregion

    private WaitForSeconds wait;
    private void Awake()
    {
        networkStartPosition = GetComponent<NetworkStartPosition>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        wait = new WaitForSeconds(2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if(!onWait && !networkStartPosition.enabled)
            {
                Active();
                Debug.Log("OnTrigger Acitve");
            }
        }
    }


    // private void CheckOtherWayPoint()
    // {

    //     foreach(Transform tr in MapEditor.Instance.objectTransform)
    //     {
    //         WayPoint wp = tr.GetComponent<WayPoint>();
    //         if(wp != null)
    //         {
    //             // if (wp.onWayPoint)
    //             // {
    //             //     wp.EnableNetWorkStartPosition();
    //             // }
               
    //         }
    //     }
    //     // Managers.Network.startPos.Clear();

    //     // onWayPoint = true;
    //     // MapEditor.Instance.startPosition = transform.position;
    //     networkStartPosition.enabled = true;

    //     // Debug.Log(Managers.Network.startPos.Count);

    // }




    //-------------------------------------------------------------------------0410
    //main
    public bool onWait;
    
    private readonly int ISOPEN = Animator.StringToHash("IsOpened");

    private List<WayPoint_Var2> otherWayPoint;
    private List<WayPoint_Var2> OtherWayPointList 
    {
        get
        {
            if(otherWayPoint == null)
            {
                otherWayPoint = MapEditor.Instance.wayPointList; 
            }
            return otherWayPoint;
        }
    }
    private SpawnPointObj startPoint;
    private SpawnPointObj StartPoint 
    {
        get
        {
            if(startPoint == null) startPoint = MapEditor.Instance.startPositionObject.GetComponent<SpawnPointObj>();
            return startPoint;
        }
    }

    private void Active()
    {
        Managers.Sound.PlaySound3D(GlobalText.DOOR_SOUND_5, transform.position, 0.45f);
        if (StartPoint.onSpawn) StartPoint.EnableNetWorkStartPosition();
        //other StartWayPoint Deactive
        foreach(var point in OtherWayPointList){
            if(point == this) continue;
            point.EnableNetWorkStartPositionOff();
        } 
        networkStartPosition.enabled = true;

        StartCoroutine(OtherPointWaitDelayCo());
   
    }

   
    
    IEnumerator OtherPointWaitDelayCo()
    {
        OtherPoint_ChangeOnWait(true);
        animator.SetBool(ISOPEN,true);
        yield return wait;
        OtherPoint_ChangeOnWait(false);
    }
    private void OtherPoint_ChangeOnWait(bool onOff)
    {   
        foreach(var point in OtherWayPointList) 
        {
            if(point == this) continue;
            point.OnWait(onOff);
        }
    }


    #region  Util
    public void EnableNetWorkStartPositionOff()
    {
        if(networkStartPosition.enabled)
        {
            Debug.Log(Managers.Network.startPos.Count);
            networkStartPosition.enabled = false;
            animator.SetBool(ISOPEN,false);
        }    
    }

    private void OnWait(bool onOff)
    {
        onWait = onOff;
        col.enabled = !onOff;
    }
    #endregion
  
    //-------------------------------------------------------------------------0410
}
