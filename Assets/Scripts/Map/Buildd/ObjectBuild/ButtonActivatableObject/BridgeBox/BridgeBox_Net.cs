using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.IO.Compression;
using Org.BouncyCastle.Crypto.Engines;

public class BridgeBox_Net : NetworkBehaviour
{
    [SerializeField] BoxCollider2D bridgeCollider;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject spriteObj;

    [Space(20)]
    [Header("Sync Data")]

    [SyncVar] public float bridgeLength;
    [SyncVar] public Vector2 connectionPoint;
    [SyncVar(hook = nameof(OnChangeActive))] public bool onActive;


    private BoxCollider2D Collider => GetComponent<BoxCollider2D>();


    // public void Awake()
    // {
    //     lineRenderer.positionCount =2;
    // }

    [Server]
    public void Server_ChangeOnActive()
    {
        onActive = !onActive;
    }
    private void OnChangeActive(bool old,bool newVal)
    {
        if (newVal) Active();
        else Deactive();
    }

    [Server]
   public void Server_SetData(float bridgeLength,Vector2 connectionPoint)
   {
        this.bridgeLength = bridgeLength;
        this.connectionPoint = connectionPoint;


        //Rpc_BridgeSetting();
        //CreateConnectionObject();
        //SetBridgeCollider();
    }

    [Command(requiresAuthority = false)]
    public void Cmd_SetOnActive()
    {
        if(isServer)
        Server_ChangeOnActive();
    }

    //[ClientRpc]
    //private void Rpc_BridgeSetting()
    //{
    //    CreateConnectionObject();
    //    SetBridgeCollider();
    //}

    #region  ---------------------------------------Server_Util
    private void CreateConnectionObject()
    {
            GameObject obj = new GameObject("Connect Object");
        
            GameObject spO = Instantiate(spriteObj);
            spO.transform.localScale = new Vector3(-1,1,1);
            spO.transform.SetParent(obj.transform);

            Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;
            rb.gravityScale = 0;

            BoxCollider2D bcol = obj.AddComponent<BoxCollider2D>();
            bcol.offset = Collider.offset;
            bcol.size  = Collider.size;
            
            obj.transform.rotation = transform.rotation;
            obj.transform.position = connectionPoint;

            obj.transform.SetParent(transform);
            obj.layer  = transform.gameObject.layer;

    }


    private void SetBridgeCollider()
    {
        // lineRenderer.SetPosition(0, lineRenderer.transform.position);

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

    }
    private Vector2 GetOffset()
    {
        return transform.position - lineRenderer.transform.position;
    }
    #endregion





    //[Command]
    //public void Cmd_Activation()
    //{
    //    Rpc_Activation();
    //}
    //[ClientRpc]
    //private void Rpc_Activation()
    //{
    //    bridgeCollider.enabled = true;
    //    DrawLine();
    //}


    //[Command]
    //public void Cmd_Deactivated()
    //{
    //    Rpc_Deactivated();
    //}
    //[ClientRpc]
    //private void Rpc_Deactivated()
    //{
    //    lineRenderer.positionCount = 0;
    //    bridgeCollider.enabled = false;
    //}

    private void Active()
    {
        // StartCoroutine(On());
        bridgeCollider.enabled = true;
        DrawLine();
    }
    private void Deactive()
    {
        // StartCoroutine(Off());
        lineRenderer.positionCount = 0;
        bridgeCollider.enabled = false;
    }

    //------Test
    // IEnumerator On()
    // {
    //     float percent = 0;
    //     bridgeCollider.enabled = true;

    //     Vector3 targetLine = connectionPoint + GetOffset();

    //     while(percent <1)
    //     {
    //         percent += Time.fixedDeltaTime;
    //         // bridgeCollider.transform.localScale = Vector3.Lerp(Vector3.zero,Vector3.one,percent);
    //         Vector3 line = Vector3.Lerp(lineRenderer.transform.position,targetLine,percent);

    //         lineRenderer.SetPosition(1,line);
    //         yield return null;
    //     }

    //     // bridgeCollider.transform.localScale = Vector3.one;
    //     lineRenderer.SetPosition(1,targetLine);
    // }
    // IEnumerator Off()
    // {
    //     float percent = 1;
    //     Vector3 targetLine = connectionPoint + GetOffset();
    //     while(percent>0)
    //     {
    //         percent -=Time.fixedDeltaTime;
    //         // bridgeCollider.transform.localScale = Vector3.Lerp(Vector3.one,Vector3.zero,percent);
    //         Vector3 line = Vector3.Lerp(lineRenderer.transform.position,targetLine,percent);
    //         lineRenderer.SetPosition(1,line);

    //         yield return null;
    //     }
    //     bridgeCollider.enabled = false;
    //     // bridgeCollider.transform.localScale = Vector3.zero;
    //     lineRenderer.SetPosition(1,lineRenderer.transform.position);
    // }


     private void DrawLine()
     {
        lineRenderer.positionCount =2;
        lineRenderer.SetPosition(0, lineRenderer.transform.position);
        lineRenderer.SetPosition(1,connectionPoint+GetOffset());
    }



    public override void OnStartClient()
    {
        base.OnStartClient();
        CreateConnectionObject();
        SetBridgeCollider();
        if (!isServer)
        {
            if (onActive) Active();
        }
    }
}
