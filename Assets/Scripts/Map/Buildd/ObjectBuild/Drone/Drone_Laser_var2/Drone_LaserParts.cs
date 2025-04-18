using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;

public class Drone_LaserParts : MonoBehaviour
{
    [SerializeField] Animator laserAnimator;
    [SerializeField] Transform laserPoint;
    public Transform LaserPoint => laserPoint;
    [SerializeField] DroneEntity main;
    [SerializeField] Drone_Laser_TargetingMark mark;
    private readonly int DIRECTION = Animator.StringToHash("Direction");

    [ReadOnly]
    public GameObject target;
   
    public void TrackingTarget()
    {
        if(target == null) return;

        // float angle = ConvertVec_To_Angle(target.transform.position);
        // float curAngle = laserAnimator.GetFloat(DIRECTION);

        // curAngle = Mathf.Lerp(curAngle,angle,0.2f);        
        // laserAnimator.SetFloat(DIRECTION, angle);
        // laserAnimator.SetFloat(DIRECTION, curAngle);

        if(angleCoroutine == null) angleCoroutine = StartCoroutine(AngleCo());
        
        // Angle_Adjustment(target.transform.position);
        // mark.Targeting(target.transform.position);
    }

   
    public void LaserReset()
    {
        StopAllCoroutines();
        angleCoroutine= null;
        isShotReady = false;
        mark.Reset();
    }
    private Coroutine angleCoroutine;
    public bool isShotReady;
    private IEnumerator AngleCo()
    {
        float percent = 0;
        float angle = ConvertVec_To_Angle(target.transform.position);
        float curAngle = laserAnimator.GetFloat(DIRECTION);
        float rate = 0;
        while(percent < 0.5f)
        {
            percent += Time.deltaTime;
            rate = Mathf.Clamp01(percent / 0.5f);
            laserAnimator.SetFloat(DIRECTION,  Mathf.Lerp(curAngle,angle,rate));
            yield return null;
        }
        laserAnimator.SetFloat(DIRECTION, angle);
        Angle_Adjustment(target.transform.position);
        angleCoroutine = null;
        isShotReady = true;
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
