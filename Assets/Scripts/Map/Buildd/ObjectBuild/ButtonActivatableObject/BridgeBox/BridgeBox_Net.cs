using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class BridgeBox_Net : NetworkBehaviour
{
    [SerializeField] BoxCollider2D bridgeCollider;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject spriteObj;

    [Space(20)]
    [Header("Sync Data")]

    public float bridgeLength;
    public Vector2 connectionPoint;
    public Vector2 position;

    [SyncVar(hook = nameof(OnChangeActive))] public bool onActive;



    private BridgeBox Main => GetComponent<BridgeBox>();
    private BoxCollider2D Collider => GetComponent<BoxCollider2D>();




    #region Init

    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(Main.ButtonActivatedObjectStruct);
    }
    [ClientRpc]
    private void Rpc_InitSync(ButtonActivatableObjectStruct data)
    {
        if (onSync) return;
        SetData(data);
    }

    [Command]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }
    public void SetData(ButtonActivatableObjectStruct data)
    {
        bridgeLength = data.bridgeLength;
        connectionPoint = data.connectionPoint;
        position = data.position;

        transform.position = data.position;
        transform.rotation = data.quaternion;

        CreateBridge();
        onSync = true;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!onSync) StartCoroutine(Delay());
   
    }

    IEnumerator Delay()
    {
       while(!NetworkClient.ready) yield return null;
        Cmd_InitSync();
    }

    #endregion


    
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

   // [Server]
   //public void Server_SetData(float bridgeLength,Vector2 connectionPoint,Vector2 position)
   //{
   //     this.bridgeLength = bridgeLength;
   //     this.connectionPoint = connectionPoint;
   //     this.position = position;

   //     //Rpc_BridgeSetting();
   //     //CreateConnectionObject();
   //     //SetBridgeCollider();
   // }

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
    //private void OnChangeConnectionPoint(Vector2 old, Vector2 newVal)
    //{
    //    //CreateConnectionObject();
    //    //SetBridgeCollider();
    //    StartCoroutine(WaitforSync());
    //}

    //IEnumerator WaitforSync()
    //{

    //    while(bridgeLength <= 0 || connectionPoint == Vector2.zero || this.position == Vector2.zero)
    //    {
    //        Debug.Log("Sync Wait");
    //        yield return null;
    //    }
        
    //    transform.position = position;

    //    CreateConnectionObject();
    //    SetBridgeCollider();
    //}
    public void CreateBridge()
    {
        CreateConnectionObject();
        SetBridgeCollider();
    }

    private void CreateConnectionObject()
    {
        //GameObject obj = new GameObject("Connect Object");
        //obj.transform.SetParent(transform);

        //obj.transform.position = transform.right * bridgeLength + transform.position;
        //obj.transform.localRotation = Quaternion.Euler(0, 0, 0);


        GameObject spO = Instantiate(spriteObj);
        spO.name = "Connect Object";
        spO.transform.SetParent(transform);
        spO.transform.position = transform.right * bridgeLength + transform.position;

        //spO.transform.position = transform.right * bridgeLength + transform.position;

        //spO.transform.localPosition = Vector3.zero;
        spO.transform.localRotation = Quaternion.Euler(0, 0, 0);
        spO.transform.localScale = new Vector3(-1, 1, 1);

        BoxCollider2D bcol = spO.AddComponent<BoxCollider2D>();
        bcol.offset = Collider.offset;
        bcol.size = Collider.size;



        spO.layer = transform.gameObject.layer;

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
        Debug.Log($"Transform : {transform.position}, Line : {lineRenderer.transform.position}");

        //return transform.position - lineRenderer.transform.position;
        return (Vector3)position - lineRenderer.transform.position;
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



}
