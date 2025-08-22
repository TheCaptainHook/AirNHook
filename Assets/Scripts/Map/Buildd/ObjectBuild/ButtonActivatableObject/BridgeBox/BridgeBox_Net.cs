using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class BridgeBox_Net : ActivatableObject_Net_Entity
{
    [SerializeField] BoxCollider2D bridgeCollider;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject spriteObj;

    [Space(20)]
    [Header("Sync Data")]

    public float bridgeLength;
    public Vector2 connectionPoint;
    public Vector2 position;

    // [SyncVar]public bool onActive;



    // private BridgeBox Main => GetComponent<BridgeBox>();
    private BoxCollider2D Collider => GetComponent<BoxCollider2D>();


    #region Init



    protected override void SetData(ButtonActivatableObjectStruct data)
    {
        base.SetData(data);

        bridgeLength = data.bridgeLength;
        connectionPoint = data.connectionPoint;
        position = data.position;

        CreateBridge();

    }


   

    #endregion



    // [Server]
    // public override void Server_ChangeOnActive(bool onOff)
    // {
    //     onActive = onOff;
    //    Rpc_ChangeOnActive(onOff);
    // }

    // [ClientRpc]
    // protected override void Rpc_ChangeOnActive(bool onOff)
    // {
    //     if(onOff)
    //     {
    //         Active();
    //     }
    //     else
    //     {
    //         Deactive();
    //     }
    // }



    #region  ---------------------------------------Server_Util

    public void CreateBridge()
    {
        CreateConnectionObject();
        SetBridgeCollider();
    }

    private void CreateConnectionObject()
    {
        GameObject spO = Instantiate(spriteObj);
        spO.name = "Connect Object";
        spO.transform.SetParent(transform);
        spO.transform.position = transform.right * bridgeLength + transform.position;

        spO.transform.localRotation = Quaternion.Euler(0, 0, 0);
        spO.transform.localScale = new Vector3(-1, 1, 1);

        BoxCollider2D bcol = spO.AddComponent<BoxCollider2D>();
        bcol.offset = Collider.offset;
        bcol.size = Collider.size;



        spO.layer = transform.gameObject.layer;

    }


    private void SetBridgeCollider()
    {

        Vector2 a = lineRenderer.gameObject.transform.position;
        Vector2 b = connectionPoint + GetOffset();

        Vector2 mid = (a + b) / 2;
        float distance = Vector2.Distance(a, b);
        Vector2 dir = (b - a).normalized;

        bridgeCollider.size = new Vector2(distance, bridgeCollider.size.y);
        bridgeCollider.transform.position = mid;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bridgeCollider.transform.rotation = Quaternion.Euler(0, 0, angle);

        bridgeCollider.gameObject.layer = LayerMask.NameToLayer("Ground/NotHookable");

        bridgeCollider.enabled = false;

        DrawLine();

    }
    private Vector2 GetOffset()
    {
        //return transform.position - lineRenderer.transform.position;
        return (Vector3)position - lineRenderer.transform.position;
    }
    #endregion



    protected override void Active()
    {
        lineRenderer.enabled = true;
        bridgeCollider.enabled = true;
        //DrawLine();
    }
    protected override void Deactive()
    {
        lineRenderer.enabled = false;
        bridgeCollider.enabled = false;
    }


     private void DrawLine()
     {
        lineRenderer.positionCount =2;
        lineRenderer.SetPosition(0, lineRenderer.transform.position);
        lineRenderer.SetPosition(1,connectionPoint+GetOffset());
    }



}
