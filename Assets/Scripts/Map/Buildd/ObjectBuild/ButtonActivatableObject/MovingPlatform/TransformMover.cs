
using DG.Tweening.Core.Easing;
using Mirror;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;



public class TransformMover : NetworkBehaviour
{

    [Space(20)]
    [ReadOnly]
    public MovingPlatform movingPlatform;

    NetworkIdentity identity;
    NetworkIdentity Identity { get { identity ??= GetComponent<NetworkIdentity>(); return identity; } }


    [SerializeField] LayerMask movingPlatformLayer;
    [SerializeField] Vector3 layOffset;

    public Collider2D col;
    public Rigidbody2D rb;
    public NetworkRigidbodyUnreliable2D netRb;

    private void Awake()
    {
        movingPlatformLayer = 1 << 15;
        col =GetComponent<Collider2D>();    
        rb= GetComponent<Rigidbody2D>();
        netRb = GetComponent<NetworkRigidbodyUnreliable2D>();
    }

    RaycastHit2D hit;
    Vector3 offset;


    public bool startSync;


    private uint preMpId;

    private void FixedUpdate()
    {
        Vector3 offset = new Vector3(0, col.bounds.extents.y, 0);

        hit = Physics2D.Raycast(transform.position - offset + layOffset, -Vector2.up, 0.6f, movingPlatformLayer);
#if UNITY_EDITOR
        Debug.DrawRay(transform.position -offset + layOffset,-Vector2.up * 0.6f, Color.green);
#endif
        //if (hit.collider != null)
        //{
        //    if (hit.collider.TryGetComponent(out MovingPlatform component))
        //    {
        //        if(!startSync)
        //        {
        //           if(isServer)
        //            {
        //                Rpc_MovingPlatformNetRbEnable(GetNetId(component.GetComponent<NetworkIdentity>()),true);
        //            }
        //        }

        //        rb.position += component.dir;
        //    }


        //}else
        //{
        //    if (startSync)
        //    {
        //      if(isServer)
        //       {
        //            Rpc_MovingPlatformNetRbEnable(preMpId, false);
        //        }
        //    }
        //}

    }
    private uint GetNetId(NetworkIdentity identity)
    {
        return identity.netId;
    }
    [ClientRpc]
    private void Rpc_MovingPlatformNetRbEnable(uint id,bool onoff)
    {
        var item = NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity) ? identity : null;
        if (item != null)
        {
            var mp = item.GetComponent<MovingPlatform>();
            preMpId = onoff ? id : 9999;
            startSync = onoff;
            mp.netRb.enabled = onoff;
        }
       
    }

    private bool ClientToServer()
    {
        if (netRb.syncDirection == SyncDirection.ServerToClient) return false;
        else return true;
    }



    #region Set Transform Network
    [Command(requiresAuthority = false)]
    private void Cmd_SetTransform(uint platformID,Vector2 position)
    {
        Rpc_SetTransform(platformID,position);
    }

    

    [ClientRpc]
    private void Rpc_SetTransform( uint platformID,Vector2 position)
    {
        var platform = NetworkClient.spawned.TryGetValue(platformID, out NetworkIdentity platform_identity) ? platform_identity : null;

        if (platform == null)
        {
            movingPlatform = null;
        }
        else
        {
            movingPlatform = platform.gameObject.TryGetComponent(out MovingPlatform component) ? component : null;

            //--------Sync using ping
                //if(!identity.isOwned)
                //{
                //    float ping = Managers.UI.GetUI<UI_PingAlways>().GetComponent<UI_PingAlways>().ping;
                //    transform.position +=  (Vector3)movingPlatform.dir* (ping / 20);
                //    Debug.Log(ping / 20);
                //}
            //--------Sync using ping
        }

    }
 
    #endregion



}
