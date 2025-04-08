
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections;

public class TransformMover : NetworkBehaviour
{

    [Space(20)]
    [ReadOnly]
    public MovingPlatform movingPlatform;

    NetworkIdentity identity;

    public uint Main_NetID => identity.netId;
    public uint movingPlatform_NetId 
    {
        get 
        {
            if(movingPlatform != null)
            {
                if(movingPlatform.TryGetComponent(out NetworkIdentity identity))
                {
                    return identity.netId;
                }
            }

            return nullNetID;
        }
    }
    uint nullNetID = 99999;
    public Transform parent;

    #region Recover

    #endregion
    private void Awake()
    {
        identity = GetComponent<NetworkIdentity>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out MovingPlatform movingPlatform) && identity.isOwned)
        {
             var platformID = movingPlatform.TryGetComponent(out NetworkIdentity identity) ? identity.netId : nullNetID;
             Cmd_SetTransform(Main_NetID, platformID);
        }
     
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (movingPlatform)
        {
            Cmd_SetTransform(Main_NetID, nullNetID);
        }

    }
    #region Recover
 
    #endregion

    #region Set Transform Network
    [Command(requiresAuthority = false)]
    private void Cmd_SetTransform(uint mainID, uint platformID)
    {
        Rpc_SetTransform(mainID, platformID);
    }

    [ClientRpc]
    private void Rpc_SetTransform(uint mainID, uint platformID )
    {
        var main = NetworkClient.spawned.TryGetValue(mainID, out NetworkIdentity main_identity) ? main_identity : null;
        var platform = NetworkClient.spawned.TryGetValue(platformID, out NetworkIdentity platform_identity) ? platform_identity : null;

        var networkRd = main.TryGetComponent(out NetworkRigidbodyUnreliable2D netRb) ? netRb : null;

        try
        {
            if (platform == null)
            {
                if(identity.isOwned && networkRd) networkRd.enabled = false;
                
                main.transform.SetParent(parent,true);
                movingPlatform = null;

                if (identity.isOwned && networkRd) networkRd.enabled = true;

            }
            else
            {
                //parent = transform.parent;
                movingPlatform = platform.gameObject.TryGetComponent(out MovingPlatform component) ? component : null;

                if (identity.isOwned && networkRd) networkRd.enabled = false;
                main.transform.SetParent(platform.transform,true);
                if (identity.isOwned && networkRd) networkRd.enabled = true;


            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }


    }

 
    #endregion



}
