using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    public Grappling grappling;

    [Header("Layers Settings:")]
    public LayerMask layerMask;
    
    [Header("Main Camera:")]
    public Camera m_camera;
    
    [Header("Transform Ref:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;
    
    [Header("Physics Ref:")]
    public DistanceJoint2D distanceJoint2D;
    public Rigidbody2D rb;

    [SerializeField] private float launchSpeed = 1;
    
    [Header("Distance:")]
    [SerializeField] private float maxDistance = 5;
    [SerializeField] private float targetDistance = 3;
    
    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    private void Start()
    {
        grappling.enabled = false;
        distanceJoint2D.enabled = false;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetGrapplePoint();
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            if (grappling.enabled)
            {
                RotateGun(grapplePoint);
            }
            else
            {
                Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
                RotateGun(mousePos);
            }

            if (grappling.isGrappling)
            {
                Vector2 firePointDistance = firePoint.position - gunHolder.localPosition;
                Vector2 targetPos = grapplePoint - firePointDistance;
                gunHolder.position = Vector2.Lerp(gunHolder.position, targetPos, Time.deltaTime * launchSpeed);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            grappling.enabled = false;
            distanceJoint2D.enabled = false;
            rb.gravityScale = 1;
        }
        else
        {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            RotateGun(mousePos);
        }
    }

    void RotateGun(Vector3 lookPoint)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;
        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        
        gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void SetGrapplePoint()
    {
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        if (Physics2D.Raycast(firePoint.position, distanceVector.normalized))
        {
            RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized);
            if (_hit.transform.gameObject.layer == layerMask)
            {
                if (Vector2.Distance(_hit.point, firePoint.position) <= maxDistance)
                {
                    grapplePoint = _hit.point;
                    grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                    grappling.enabled = true;
                }
            }
        }
    }

    public void Grapple()
    {
        distanceJoint2D.autoConfigureDistance = true;
        
        distanceJoint2D.connectedAnchor = grapplePoint;
        distanceJoint2D.enabled = true;
        
        distanceJoint2D.autoConfigureDistance = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistance);
        }
    }
}
