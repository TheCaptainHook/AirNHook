using Mirror;
using Mono.CompilerServices.SymbolWriter;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TransformMover : MonoBehaviour
{
    public LayerMask layer;

    public Vector3 rayStartOffset;
    public float rayDistance;
   
    private RaycastHit2D hit;
    
    [Space(20)]
    public MovingPlatform movingPlatform;

    NetworkIdentity identity;


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
                    transform.SetParent(movingPlatform.transform);
                }
            }
  
        }
        else
        {
            if(movingPlatform != null)transform.SetParent(null);
            movingPlatform = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position+rayStartOffset,Vector3.down*rayDistance);
    }

}
