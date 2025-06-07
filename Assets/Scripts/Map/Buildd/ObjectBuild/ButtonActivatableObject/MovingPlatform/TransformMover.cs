
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



    // uint nullNetID = 99999;


    [SerializeField] LayerMask movingPlatformLayer;
    [SerializeField] Vector3 layOffset;


    public Collider2D col;

    private void Awake()
    {
        movingPlatformLayer = 1 << 15;
        col =GetComponent<Collider2D>();    

    }

    RaycastHit2D hit;
    Vector3 offset;

    private void FixedUpdate()
    {
        //if (movingPlatform && identity.isOwned)
        //{     
        //    transform.position += (Vector3)movingPlatform.dir;
        //}
        //Bounds bounds = GetComponent<Collider2D>().bounds;

        Vector3 offset = new Vector3(0, col.bounds.extents.y, 0);

        hit = Physics2D.Raycast(transform.position - offset + layOffset, -Vector2.up, 0.1f, movingPlatformLayer);
        Debug.DrawRay(transform.position -offset + layOffset,-Vector2.up * 0.1f, Color.green);
        //Debug.DrawRay(hit);

        if (hit.collider != null && hit.collider.TryGetComponent(out MovingPlatform component) && Identity.isOwned)
        {
            transform.position += (Vector3)component.dir;
        }




        //if (movingPlatform != null && Identity.isOwned)
        //{
        //    transform.position += (Vector3)movingPlatform.dir;
        //}

    }

//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {

//        Bounds bounds = GetComponent<Collider2D>().bounds;

//        Vector3 offset = new Vector3(0, transform.position.y + bounds.extents.y, 0);

//        Gizmos.color = Color.red;
//        Gizmos.DrawRay(transform.position - offset, -Vector3.up * 0.1f);

//    }
//#endif


    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.TryGetComponent(out MovingPlatform movingPlatform) && identity.isOwned)
    //    {
    //        var platformID = movingPlatform.TryGetComponent(out NetworkIdentity identity) ? identity.netId : nullNetID;
    //        Cmd_SetTransform(platformID,transform.position);
    //    }

    //}
    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (movingPlatform)
    //    {
    //        Cmd_SetTransform(nullNetID, transform.position);
    //    }

    //}




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
