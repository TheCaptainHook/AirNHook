using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_LaserParts : MonoBehaviour
{
    [SerializeField] Animator laserAnimator;
    [SerializeField] Transform laserPoint;
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


    #region Attack

    private void Shot(GameObject target)
    {
        Projectile_Shell shell = Managers.Pooling.D_GetItem(ammo).GetComponent<Projectile_Shell>();
        shell.Setting(laserPoint.position, laserPoint.right, main.gameObject);
        shell.gameObject.SetActive(true);

    }
    #endregion


    public void Recover()
    {

    }


    #region Util
    private float ConvertVec_To_Angle(Vector2 target)
    {
        Vector2 toTarget = (target - (Vector2)transform.position).normalized;

        return Vector2.SignedAngle(Vector2.right, toTarget);

    }
    private void Angle_Adjustment(Vector2 target)
    {
        Vector2 toTarget = (target - (Vector2)laserPoint.position).normalized;
        float angle = Vector2.SignedAngle(laserPoint.right, toTarget);
        transform.Rotate(0, 0, angle);

    }
    #endregion
}
