using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_Laser_GuardVision : MonoBehaviour
{
    public float radius = 10f;
    public LayerMask detectionLayer;

    [SerializeField] Drone_LaserParts parts;
    [SerializeField] DroneEntity_Net net;




    private void Update()
    {
        Collider2D hit_1 = Physics2D.OverlapCircle(transform.position, radius, detectionLayer);
        DebugDrawCircle(transform.position, radius, Color.cyan);

        if (hit_1)
        {
            var dir = (hit_1.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionLayer);

            if (hit)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    var check = Check_DistanceAndAngle(hit.transform.position);
                    //1. 거리 체크
                    if (check.d && check.a)
                    {
                        //State_Attack
                        parts.TrackingTarget(hit.collider.gameObject);
                    }
                    else
                    {
                        parts.Recover();
                        Debug.Log("Track");
                    }
                }
            }
            else
            {
                parts.Recover();
            }
        }
    }

    private (bool d,bool a) Check_DistanceAndAngle(Vector2 target)
    {
        bool d = Vector2.Distance(transform.position, target) <= 8;
        Vector2 toTarget = (target - (Vector2)transform.position).normalized;
        float a = Vector2.SignedAngle(transform.right, toTarget);
        bool aa = a < 5 && a > -160;
        return (d, aa);
    }



    void DebugDrawCircle(Vector2 center, float radius, Color color)
    {
        int segments = 32;
        float angle = 0f;
        Vector3 lastPoint = center + new Vector2(Mathf.Cos(0), Mathf.Sin(0)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            angle = i * 2 * Mathf.PI / segments;
            Vector3 nextPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Debug.DrawLine(lastPoint, nextPoint, color);
            lastPoint = nextPoint;
        }
    }
}
