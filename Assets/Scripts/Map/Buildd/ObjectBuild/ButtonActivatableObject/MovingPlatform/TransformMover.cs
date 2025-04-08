
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
    Transform parent;

    #region Recover

    #endregion
    private void Awake()
    {
        identity = GetComponent<NetworkIdentity>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out MovingPlatform movingPlatform))
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

        try
        {
            if (platform == null)
            {
                //main.transform.SetParent(parent);
                if (delayCoroutine != null) StopCoroutine(delayCoroutine);
                delayCoroutine = StartCoroutine(SetParentDelayed(null));
            }
            else
            {
                parent = transform.parent;
                movingPlatform = platform.gameObject.TryGetComponent(out MovingPlatform component) ? component : null;

                if(delayCoroutine != null)StopCoroutine(delayCoroutine);
                delayCoroutine = StartCoroutine(SetParentDelayed(platform.transform));

                //main.transform.SetParent(platform.transform,true);

            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }


    }

    Coroutine delayCoroutine;

   IEnumerator SetParentDelayed(Transform platformTr)
    {
        yield return new WaitForEndOfFrame();
        transform.SetParent(platformTr, true);

    }
    #endregion



}
