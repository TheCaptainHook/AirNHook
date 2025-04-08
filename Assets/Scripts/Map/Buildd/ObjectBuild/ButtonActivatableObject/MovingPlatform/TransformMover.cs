
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections;
using DG.Tweening.Core.Easing;

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
        var main = NetworkClient.spawned.TryGetValue(mainID, out NetworkIdentity main_identity) ? main_identity : null;
        var platform = NetworkClient.spawned.TryGetValue(platformID, out NetworkIdentity platform_identity) ? platform_identity : null;

        if (platform == null)
        {
            main.transform.SetParent(null, true);
            movingPlatform = null;

        }
        else
        {
            //parent = transform.parent;
            movingPlatform = platform.gameObject.TryGetComponent(out MovingPlatform component) ? component : null;
            main.transform.SetParent(platform.transform, true);
        }

        //Rpc_SetTransform(mainID, platformID);
    }

    //[ClientRpc]
    //private void Rpc_SetTransform(uint mainID, uint platformID)
    //{
    //    var main = NetworkClient.spawned.TryGetValue(mainID, out NetworkIdentity main_identity) ? main_identity : null;
    //    var platform = NetworkClient.spawned.TryGetValue(platformID, out NetworkIdentity platform_identity) ? platform_identity : null;

    //    //var networkRd = main.TryGetComponent(out NetworkRigidbodyUnreliable2D netRb) ? netRb : null;

    //    try
    //    {
    //        if (platform == null)
    //        {
    //            //if (networkRd)
    //            //{
    //            //    DelaySet(main.transform, null, networkRd);
    //            //}

    //            main.transform.SetParent(null, true);
    //            movingPlatform = null;

    //        }
    //        else
    //        {
    //            //parent = transform.parent;
    //            movingPlatform = platform.gameObject.TryGetComponent(out MovingPlatform component) ? component : null;
    //            main.transform.SetParent(platform.transform, true);


    //            //if (networkRd)
    //            //{
    //            //    DelaySet(main.transform, platform.transform, networkRd);
    //            //}
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.Log(e);
    //    }
    //}

    //Coroutine delayCo;
    //private void DelaySet(Transform main, Transform parent, NetworkRigidbodyUnreliable2D rb)
    //{
    //    if (delayCo != null) StopCoroutine(delayCo);
    //    delayCo = StartCoroutine(Delay(main, parent, rb));
    //}

    //IEnumerator Delay(Transform main,Transform parent,NetworkRigidbodyUnreliable2D rb)
    //{
    //    rb.enabled = false;

    //    yield return new WaitForEndOfFrame();
    //    rb.enabled = true;
    //}
    #endregion



}
