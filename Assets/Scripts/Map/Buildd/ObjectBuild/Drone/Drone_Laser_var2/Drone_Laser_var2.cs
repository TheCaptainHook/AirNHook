using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DRONE_LASER_STATE
{
    GUARD = 0,
    TRACKING = 1,
    RETURN = 2,
    ATTACK = 3
}
public class Drone_Laser_var2 : DroneEntity
{

//TEST
    [SerializeField] SpriteRenderer  message;
//TEST

#region Laser Parts
    [CustomHeader("Drone Laser")]
    [SerializeField] Drone_LaserParts laserParts;
    private Transform laserPoint;
    private Transform LaserPoint 
    {
        get
        {
            if(laserPoint == null) laserPoint = laserParts.LaserPoint;
            return laserPoint;
        }
    }

#endregion

#region  GuardVision
    [SerializeField] Drone_Laser_GuardVision guardVision;
    #endregion

    public void PreStateSetUp(bool onOff)
    {
        isStop = onOff;
        message.enabled = onOff;
        laserParts.LaserReset();
    }

    #region Guard
    private void State_Guard()
    {
        //if(state != DRONE_LASER_STATE.GUARD)
        //{
        //    state = DRONE_LASER_STATE.GUARD;

        //    isStop = false; //Movement-related parameters
        //    message.enabled = false;
        //    laserParts.LaserReset();
        //}
        if(curAmmoCount >0)
        {
            Reloading();
        }
    }
    #endregion

#region Tracking
    private void State_Tracking()
    {
        //if (state != DRONE_LASER_STATE.TRACKING)
        //{
        //    state = DRONE_LASER_STATE.TRACKING;

        //    isStop = true; //Movement-related parameters
        //    message.enabled = true;
        //    laserParts.LaserReset();
            
        //}
        //addforce target direction, and check for obstacles.
        // If an object is detected, find a new path and update the direction.

    }

    #endregion

#region  Attack
    [SerializeField] GameObject ammo;
    float maxReloadingCount = 1; 
    float curReloadingCount = 0;
    int maxAmmoCount = 50;
    public int curAmmoCount = 0;
    float maxFireRate  = 0.05f;
    float curFireRate = 0;
    bool onReloading;
    private void Shot()
    {
        Projectile_Plasma plasma = Managers.Pooling.D_GetItem(ammo).GetComponent<Projectile_Plasma>();
        plasma.Setting(LaserPoint.position, RandomDir(LaserPoint.right), gameObject);
        plasma.gameObject.SetActive(true);

        curAmmoCount++;
    }

    private Vector2 RandomDir(Vector2 dir)
    {
        //-5~5
        float randomRange = Random.Range(-10,10);
        return Quaternion.Euler(0,0,randomRange) * dir;
    }
   
    private void State_Attack()
    {
        //if(state != DRONE_LASER_STATE.ATTACK)
        //{
        //    //Warning Animation or Effect

        //    //Warning Animation or Effect
        //    state = DRONE_LASER_STATE.ATTACK;

        //    isStop = true; //Movement-related parameters
        //    message.enabled = true;
        //    // laserParts.TrackingTarget();

        //} 
        if(!laserParts.isShotReady) laserParts.TrackingTarget();
        if(curAmmoCount>=maxAmmoCount)
        {
            onReloading = true;
        }

        if(!onReloading)
        {
            curFireRate += Time.deltaTime;
            if(curFireRate >= maxFireRate && laserParts.isShotReady)
            {
                //Shot
                Shot();
                curFireRate = 0;
                //Shot
            }
           
        }else{
            Reloading();

        }
    }
    private void Reloading()
    {
        laserParts.isShotReady =false;
        curReloadingCount += Time.deltaTime;
        if (curReloadingCount >= maxReloadingCount)
        {
            curAmmoCount = 0;
            curReloadingCount = 0;
            onReloading = false;
        }
    }
#endregion
    
    

#region State Change
   public void StateChange(DRONE_LASER_STATE state,GameObject target = null)
   {
    switch(state)
    {
        case DRONE_LASER_STATE.GUARD:
            State_Guard();
            break;
        case DRONE_LASER_STATE.TRACKING:
            State_Tracking();
            break;
        case DRONE_LASER_STATE.RETURN:
            break;
        case DRONE_LASER_STATE.ATTACK:
            State_Attack();
            break;
    }
   }


#endregion




}
