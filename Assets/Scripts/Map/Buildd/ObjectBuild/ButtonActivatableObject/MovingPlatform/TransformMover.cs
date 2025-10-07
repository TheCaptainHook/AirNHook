
using DG.Tweening.Core.Easing;
using Mirror;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;



public class TransformMover : NetworkBehaviour
{

    [Space(20)]
    [ReadOnly]
    public WDMP_Net _wdmp_Net;


    //Refs
    private Collider2D col;
    private Rigidbody2D rb;
    private NetworkRigidbodyUnreliable2D netRb;
    //Platform Detect
    [SerializeField] LayerMask movingPlatformMask = 1 << 15;
    [SerializeField] float skin = 0.2f;
    private RaycastHit2D[] _hits = new RaycastHit2D[2];
    private ContactFilter2D _filter;

    [SyncVar] public uint _platformId; // 입/퇴장 시점 공유용(옵션)
    private MovingPlatform _platform;
 
    private void Awake()
    {
        col =GetComponent<Collider2D>();    
        rb= GetComponent<Rigidbody2D>();
        netRb = GetComponent<NetworkRigidbodyUnreliable2D>();

        _filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = movingPlatformMask,
            useTriggers = true
        };
    }

    RaycastHit2D hit;
    // Vector3 offset;


    public bool startSync;


    private uint preMpId;

    //-----0813
    void FixedUpdate()
    {
        // 권한 없는 쪽은 직접 위치 보정하지 않음 (클라 권한식 가정)
        if (!isOwned) return;

        float dist = col.bounds.extents.y + skin;
        int count = Physics2D.Raycast(col.bounds.center, Vector2.down, _filter, _hits, dist);

        if (count > 0)
        {
            if (_hits[0].collider.TryGetComponent(out MovingPlatform mp))
            {
                rb.position += mp.dir;
                return;
            }
            if(_hits[0].collider.TryGetComponent(out WDMP_Net wdmp))
            {
                rb.position +=  wdmp._c_dir * wdmp._c_curStep;
                return;
            }

        }

    }

    [Command(requiresAuthority = true)]
    void Cmd_SetPlatform(uint platformId)
    {
        _platformId = platformId;
    }

    //-----0813

    //    private void FixedUpdate()
    //    {
    //        // Vector3 offset = new Vector3(0, col.bounds.extents.y, 0);
    //        float offset = col.bounds.extents.y + 0.2f;

    //        // hit = Physics2D.Raycast(transform.position - offset + layOffset, -Vector2.up, 0.6f, movingPlatformLayer);
    //        hit = Physics2D.Raycast(col.bounds.center, Vector2.down,offset, movingPlatformLayer);
    //#if UNITY_EDITOR
    //        Debug.DrawRay(col.bounds.center,-Vector2.up * offset, Color.green);
    //#endif
    //        if (hit.collider != null)
    //        {
    //           if (hit.collider.TryGetComponent(out MovingPlatform component))
    //           {
    //               if(!startSync)
    //               {
    //                  if(isServer)
    //                   {
    //                       Rpc_MovingPlatformNetRbEnable(GetNetId(component.GetComponent<NetworkIdentity>()),true);
    //                   }
    //               }

    //               rb.position += component.dir;
    //           }


    //        }else
    //        {
    //           if (startSync)
    //           {
    //             if(isServer)
    //              {
    //                   Rpc_MovingPlatformNetRbEnable(preMpId, false);
    //               }
    //           }
    //        }

    //    }
    private uint GetNetId(NetworkIdentity identity)
    {
        return identity.netId;
    }
    //[ClientRpc]
    //private void Rpc_MovingPlatformNetRbEnable(uint id,bool onoff)
    //{
    //    var item = NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity) ? identity : null;
    //    if (item != null)
    //    {
    //        var mp = item.GetComponent<MovingPlatform>();
    //        preMpId = onoff ? id : 9999;
    //        startSync = onoff;
    //        mp.netRb.enabled = onoff;
    //    }
       
    //}

    private bool ClientToServer()
    {
        if (netRb.syncDirection == SyncDirection.ServerToClient) return false;
        else return true;
    }



    //#region Set Transform Network
    //[Command(requiresAuthority = false)]
    //private void Cmd_SetTransform(uint platformID,Vector2 position)
    //{
    //    Rpc_SetTransform(platformID,position);
    //}

    

    //[ClientRpc]
    //private void Rpc_SetTransform( uint platformID,Vector2 position)
    //{
    //    var platform = NetworkClient.spawned.TryGetValue(platformID, out NetworkIdentity platform_identity) ? platform_identity : null;

    //    if (platform == null)
    //    {
    //        movingPlatform = null;
    //    }
    //    else
    //    {
    //        movingPlatform = platform.gameObject.TryGetComponent(out MovingPlatform component) ? component : null;

    //        //--------Sync using ping
    //            //if(!identity.isOwned)
    //            //{
    //            //    float ping = Managers.UI.GetUI<UI_PingAlways>().GetComponent<UI_PingAlways>().ping;
    //            //    transform.position +=  (Vector3)movingPlatform.dir* (ping / 20);
    //            //    Debug.Log(ping / 20);
    //            //}
    //        //--------Sync using ping
    //    }

    //}
 
    //#endregion



}
