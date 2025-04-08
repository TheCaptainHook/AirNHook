
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
            //transform.SetParent(movingPlatform.transform);
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
    private void Rpc_SetTransform(uint mainID, uint platformID)
    {
        var main = NetworkClient.spawned.TryGetValue(mainID, out NetworkIdentity main_identity) ? main_identity : null;
        var platform = NetworkClient.spawned.TryGetValue(platformID, out NetworkIdentity platform_identity) ? platform_identity : null;

        var netRb = main.GetComponent<NetworkRigidbodyUnreliable2D>();
        if (netRb && main.isOwned)
        {
            netRb.enabled = false;
        }

        if (platform == null)
        {
            main.transform.SetParent(null, true);
            movingPlatform = null;
            if (netRb && main.isOwned)
                main.StartCoroutine(ReenableNetworkRigidbody(netRb));
        }
        else
        {
            movingPlatform = platform.gameObject.TryGetComponent(out MovingPlatform component) ? component : null;
            main.transform.SetParent(platform.transform, true);
            if (netRb && main.isOwned)
                main.StartCoroutine(ReenableNetworkRigidbody(netRb));

        }


    }
    IEnumerator ReenableNetworkRigidbody(NetworkRigidbodyUnreliable2D netRb)
    {
        yield return new WaitForEndOfFrame(); // 또는 yield return null;
        netRb.enabled = true;
    }
    // NetworkRigidbody 가 동기화중, 메인 클라이언트에서 먼저 동기화 되면서 로컬포지션 동기화 -> 다른 클라이언트에서 동기화된 로컬 포지션값 동기화 후에 트렌스폼 세팅.


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
