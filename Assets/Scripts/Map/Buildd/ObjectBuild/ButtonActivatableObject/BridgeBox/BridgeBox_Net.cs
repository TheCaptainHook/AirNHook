using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BridgeBox_Net : NetworkBehaviour
{

    [SerializeField] GameObject spriteObj;

    [Space(20)]
    [Header("Sync Data")]

    [SyncVar] public float bridgeLength;
    [SyncVar] public Vector2 connectionPoint;

    [SyncVar] public GameObject bridge;

    private BoxCollider2D Collider => GetComponent<BoxCollider2D>();
    private LineRenderer lineRenderer;
    private LineRenderer LineRenderer 
    {
        get
        {
            if(!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
            return lineRenderer;
        }
    }



   [Server]
   public void Server_SetData(float bridgeLength,Vector2 connectionPoint)
   {
        this.bridgeLength = bridgeLength;
        this.connectionPoint = connectionPoint;

        Rpc_BridgeSetting();
   }


    [ClientRpc]
    private void Rpc_BridgeSetting()
    {
        CreateConnectionObject();
        SetBridgeCollider();
    }

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
            bridge = new GameObject("bridge");
            BoxCollider2D boxCol = bridge.AddComponent<BoxCollider2D>();
            
            Vector2 a = LineRenderer.gameObject.transform.position;
            Vector2 b = connectionPoint + GetOffset();

            Vector2 mid = (a+b)/2;
            float distance = Vector2.Distance(a,b);
            Vector2 dir = (b-a).normalized;

            boxCol.size = new Vector2(distance,boxCol.size.y);
            bridge.transform.position = mid;
            float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
            bridge.transform.rotation = Quaternion.Euler(0,0,angle);
            
            bridge.layer = LayerMask.NameToLayer("Ground/NotHookable");
            bridge.transform.SetParent(transform);

            // bridgeCol.enabled = false;
            bridge.SetActive(false);
    }
    private Vector2 GetOffset()
    {
        return transform.position - LineRenderer.transform.position;
    }
    #endregion





    [Command]
    public void Cmd_Activation()
    {
        Rpc_Activation();
    }
    [ClientRpc]
    private void Rpc_Activation()
    {
        bridge.SetActive(false);
        DrawLine();
    }


    [Command]
    public void Cmd_Deactivated()
    {
        Rpc_Deactivated();
    }
    [ClientRpc]
    private void Rpc_Deactivated()
    {
        LineRenderer.positionCount = 0;
        bridge.SetActive(false);
    }


     private void DrawLine()
     {
            LineRenderer.positionCount =2;
            LineRenderer.SetPosition(0,LineRenderer.transform.position);
            LineRenderer.SetPosition(1,connectionPoint+GetOffset());
    }
}
