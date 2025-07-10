
using DG.Tweening.Core.Easing;
using Mirror;
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

    private void FixedUpdate()
    {

        Vector3 offset = new Vector3(0, col.bounds.extents.y, 0);

        hit = Physics2D.Raycast(transform.position - offset + layOffset, -Vector2.up, 0.5f, movingPlatformLayer);
#if UNITY_EDITOR
        Debug.DrawRay(transform.position -offset + layOffset,-Vector2.up * 0.5f, Color.green);
#endif
        if (hit.collider != null)
        {
            if (ClientToServer())
            {
                if (hit.collider.TryGetComponent(out MovingPlatform component) && Identity.isOwned)
                {
                    rb.position += component.dir;
                }

                if (hit.collider.TryGetComponent(out WDMP_Net component2) && Identity.isOwned)
                {
                    rb.position += component2.moveDir * component2.step;
                }
            }
            else
            {
                if (hit.collider.TryGetComponent(out MovingPlatform component) && Identity.isServer)
                {
                    rb.position += component.dir;
                }

                if (hit.collider.TryGetComponent(out WDMP_Net component2) && Identity.isServer)
                {
                    rb.position += component2.moveDir * component2.step;
                }
            }

         
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
