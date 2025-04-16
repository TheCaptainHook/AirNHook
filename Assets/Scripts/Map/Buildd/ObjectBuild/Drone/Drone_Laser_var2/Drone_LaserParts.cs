using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_LaserParts : MonoBehaviour
{
    [SerializeField] Animator laserAnimator;
    [SerializeField] Transform laserPoint;
    public Transform LaserPoint => laserPoint;
    [SerializeField] LineRenderer line;

    [SerializeField] DroneEntity main;

    private readonly int DIRECTION = Animator.StringToHash("Direction");


    [SerializeField] GameObject ammo;

    public void TrackingTarget(GameObject target)
    {
        float angle = ConvertVec_To_Angle(target.transform.position);
        laserAnimator.SetFloat(DIRECTION, angle);
        //StartCoroutine(Delay_AngleAdjust(target));
        Angle_Adjustment(target.transform.position);
    }


    


    #region Util
    private float ConvertVec_To_Angle(Vector2 target)
    {
        Vector2 toTarget = (target - (Vector2)main.transform.position).normalized;
        return Mathf.Floor(Vector2.SignedAngle(Vector2.right, toTarget));
    }
    private void Angle_Adjustment(Vector2 target)
    {
        Vector2 toTarget = (target - (Vector2)laserPoint.position).normalized;
        float angle = Vector2.SignedAngle(laserPoint.right, toTarget);
        transform.Rotate(0, 0, angle);

    }
    #endregion
}
