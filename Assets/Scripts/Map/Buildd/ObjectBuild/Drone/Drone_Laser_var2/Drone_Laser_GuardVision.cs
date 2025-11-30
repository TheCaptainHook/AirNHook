using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_Laser_GuardVision : MonoBehaviour
{
    public float radius = 10f;
    public float attackRange = 8f;
    public LayerMask detectionLayer;
    public LayerMask ignoreLayer;

    [SerializeField] Drone_Laser_var2 main;
    [SerializeField] Drone_LaserParts parts;
    [SerializeField] Drone_Laser_var2_Net net;

    //public bool onAttack;
    private void Update()
    {
        if(!net.onSync) return;

        // if (net.droneLaserState == DRONE_LASER_STATE.RETURN) return; //RETURN

        if(parts.isShotReady) return;
        if (net.onAtack) return;

        Collider2D hit_1 = Physics2D.OverlapCircle(main.transform.position, radius, detectionLayer);
        //Debug
        DebugDrawCircle(main.transform.position, radius, Color.cyan); 
        //Debug
        
        if (hit_1) // - Detected Player
        {
            var dir = (hit_1.transform.position - main.transform.position).normalized;
            //-------------- Detect All layer, And Analyz/ 0416
            RaycastHit2D[] hits = Physics2D.RaycastAll(main.transform.position,dir,radius); //Detacting range : 10
            var result = AnalyzeHit(hits);
            //-------------- Detect All layer, And Analyz/ 0416

            if(result.onDetacted) //ATTACK
            {
                if (NetworkServer.active) 
                {
                    net.Server_SetTarget(result.player.GetComponent<NetworkIdentity>().netId);
                    net.Server_DroneLaserState(3);
                }

            }
            //else if(result.player) // TRACKING
            //{
            //    parts.target = result.player;
            //    //State -> Tracking
            //    if (NetworkServer.active) net.Server_DroneLaserState(1);
            //    //main.StateChange(DRONE_LASER_STATE.TRACKING);
            //    Debug.Log("Tracking");
            //}
            else
            {
                if (NetworkServer.active) net.Server_DroneLaserState(0);
            }
   
        }
        else
        {
            if (NetworkServer.active) net.Server_DroneLaserState(0);
        }
    }
    
 

    //-----------------------------------Debug
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
    //-----------------------------------Debug

    #region  Util
    
   private (bool onDetacted,GameObject player) AnalyzeHit(RaycastHit2D[] hits)
   {
        GameObject player = null;
        bool onDetacted = false;

        foreach (var hit in hits)
        {
            // Debug.Log($"name : {hit.collider.name}");
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                player = hit.collider.gameObject;
                break;
            }

            if ((ignoreLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
            {
                Debug.Log("Ignore Ray");
                return (onDetacted, player);
            }


        }

        if (player == null) return (false, null);
        //Check Distacne and angle
        var check = Check_DistanceAndAngle(player.transform.position);
        if (!check.a) player = null;
        return (check.d && check.a, player);
        //Check Distacne and angle

    }

    private (bool d,bool a) Check_DistanceAndAngle(Vector2 target)
    {
        bool d = Vector2.Distance(main.transform.position, target) <= attackRange;
        Vector2 toTarget = (target - (Vector2)main.transform.position).normalized;
        float a = Vector2.SignedAngle(Vector2.right, toTarget);
        bool aa = a < 10 && a > -170;
        return (d, aa);
    }

    #endregion
}
