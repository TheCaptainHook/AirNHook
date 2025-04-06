using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformMover : MonoBehaviour
{
    public LayerMask layer;

    public Vector3 rayStartOffset;
    public float rayDistance;
   
    private RaycastHit2D hit;
    
    [Space(20)]
    public MovingPlatform_Net movingPlatform_Net;
    

    Rigidbody2D rb;
    float mass;
    float drag;
    float gravityScale;
    float gravityY;
    NetworkIdentity identity;


    private void Awake()
    {
        identity = GetComponent<NetworkIdentity>();
        rb = GetComponent<Rigidbody2D>();
        mass = rb.mass;
        drag = rb.drag;
        gravityScale = rb.gravityScale;
        gravityY = Mathf.Abs(Physics2D.gravity.y);
    }

    private void FixedUpdate()
    {
        if (!identity.isOwned) return;

        hit = Physics2D.Raycast(transform.position, Vector3.down + rayStartOffset, rayDistance, layer);
        Debug.DrawRay(transform.position, (Vector3.down + rayStartOffset) * rayDistance, Color.red);
        if (hit)
        {
            if(hit.collider.TryGetComponent(out MovingPlatform_Net component))
            {
                if (movingPlatform_Net == null)
                {
                    movingPlatform_Net = component;
                }
                AddForce();
            }
  
        }
        else
        {
            movingPlatform_Net = null;
        }
    }

    public Vector2 dir;
    public Vector2 forcePower;
    public float power;
    private void AddForce()
    {
        if (movingPlatform_Net == null) return;

        //dir = movingPlatform_Net.dir;
        //forcePower = ForcePowerControl(dir);
        //rb.AddForce(forcePower, ForceMode2D.Force);
        rb.MovePosition(rb.position + movingPlatform_Net.dir);
    }

    //public Vector2 previousVelocity;// 이전 속도
    public Vector2 currentVelocity;//현재 속도

    //private Vector2 ForcePowerControl(Vector2 dir)
    //{
    //    //Vector2 direct = dir.normalized;

    //    //currentVelocity = movingPlatform_Net.dir / Time.fixedDeltaTime;
    //    //Vector2 acceleration = currentVelocity / Time.fixedDeltaTime; //가속도 
    //    ////previousVelocity = currentVelocity;

    //    //Vector2 baseForce = acceleration * mass; // 힘

    //    //Vector2 dragForce = dir * drag * mass; // drag 보정


    //    float gravityForceY = gravityY * gravityScale * mass;// y축 gravity scale 보정

    //    //Vector2 finalForce = baseForce + dragForce;
    //    //finalForce.y -= gravityForceY;
    //    Vector2 force = dir / Time.fixedDeltaTime * mass *power;
    //    if (dir.y >0)
    //    {
    //        force.y -= gravityForceY;
    //    }
    //    else
    //    {
    //        force.y += gravityForceY;
    //    }


    //    return force;
    //}
}
