using Mirror;
using Mono.CompilerServices.SymbolWriter;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TransformMover : NetworkBehaviour
{
    public LayerMask layer;

    public Vector3 rayStartOffset;
    public float rayDistance;
   
    private RaycastHit2D hit;
    
    [Space(20)]
    public MovingPlatform movingPlatform;

    NetworkIdentity identity;
    uint Main_NetID => identity.netId;

    private void Awake()
    {
        identity = GetComponent<NetworkIdentity>();
    }

    private void FixedUpdate()
    {
        if (!identity.isOwned) return;

        hit = Physics2D.Raycast(transform.position+rayStartOffset, Vector3.down, rayDistance, layer);
        // Debug.DrawRay(transform.position+rayStartOffset, Vector3.down * rayDistance, Color.red);
        if (hit)
        {
            if(hit.collider.TryGetComponent(out MovingPlatform component))
            {
                if (movingPlatform == null)
                {
                    movingPlatform = component;
                    if(component.TryGetComponent(out NetworkIdentity identity))
                    {
                        Cmd_SetTransform(Main_NetID, identity.netId);
                    }
                }
            }
  
        }
        else
        {
            if(movingPlatform != null) Cmd_SetTransform(Main_NetID,99999);
            movingPlatform = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position+rayStartOffset,Vector3.down*rayDistance);
    }



    [Command(requiresAuthority = false)]
    private void Cmd_SetTransform(uint mainID,uint platformID)
    {
        Rpc_SetTransform(mainID,platformID);
    }
    [ClientRpc]
    private void Rpc_SetTransform(uint mainID, uint platformID)
    {
        var main = NetworkClient.spawned.TryGetValue(mainID,out NetworkIdentity main_identity) ? main_identity : null;
        var platform = NetworkClient.spawned.TryGetValue(platformID, out NetworkIdentity platform_identity) ? platform_identity : null;



        if (platform == null) main.transform.SetParent(null);
        else main.transform.SetParent(platform.transform); 

    }

}
