
using Mirror;
using Unity.VisualScripting;
using UnityEngine;



public class TransformMover : NetworkBehaviour
{

    [Space(20)]
    [ReadOnly]
    public MovingPlatform movingPlatform;

    NetworkIdentity identity;

    uint nullNetID = 99999;

    #region Recover

    #endregion
    private void Awake()
    {
        identity = GetComponent<NetworkIdentity>();
    }

    private void FixedUpdate()
    {
        if (movingPlatform && identity.isOwned)
        {     
            transform.position += (Vector3)movingPlatform.dir;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out MovingPlatform movingPlatform) && identity.isOwned)
        {
            var platformID = movingPlatform.TryGetComponent(out NetworkIdentity identity) ? identity.netId : nullNetID;
            Cmd_SetTransform(platformID,transform.position);
        }
     
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (movingPlatform)
        {
            Cmd_SetTransform(nullNetID, transform.position);
        }

    }
    #region Recover

    #endregion


 

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
