using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DRONE_LASER_STATE
{
    GUARD,
    TRACKING,
    RETURN,
    ATTACK
}
public class Drone_Laser_var2 : DroneEntity
{
    
 [ReadOnly]
 [SerializeField] DRONE_LASER_STATE state = DRONE_LASER_STATE.GUARD;

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

#region Guard
    private void State_Guard()
    {
        if(state != DRONE_LASER_STATE.GUARD)
        {
            state = DRONE_LASER_STATE.GUARD;
            //onStop = false;
        }
    }
#endregion

#region  Attack
    [SerializeField] GameObject ammo;
    float maxReloadingCount = 2; 
    float curReloadingCount = 0;
    int maxAmmoCount = 10;
    int curAmmoCount = 0;
    float maxFireRate  = 0.1f;
    float curFireRate = 0;
    bool onReloading;
    private void Shot()
    {
        Projectile_Shell shell = Managers.Pooling.D_GetItem(ammo).GetComponent<Projectile_Shell>();
        // shell.Setting(LaserPoint.position, LaserPoint.right, gameObject);
        shell.Setting(LaserPoint.position, RandomDir(LaserPoint.right), gameObject);
        shell.gameObject.SetActive(true);

        curAmmoCount++;
    }

    private Vector2 RandomDir(Vector2 dir)
    {
        //-5~5
        float randomRange = Random.Range(-5,5);
        return Quaternion.Euler(0,0,randomRange) * dir;
    }
   
    private void State_Attack()
    {
        if(state != DRONE_LASER_STATE.ATTACK)
        {
            //Warning Animation or Effect

            //Warning Animation or Effect
            state = DRONE_LASER_STATE.ATTACK;
            //onStop = true;
        } 

        if(curAmmoCount>=maxAmmoCount)
        {
            onReloading = true;
        }

        if(!onReloading)
        {
            curFireRate += Time.deltaTime;
            if(curFireRate >= maxFireRate)
            {
                //Shot
                Shot();
                curFireRate = 0;
                //Shot
            }
           
        }else{
            curReloadingCount += Time.deltaTime;
            if(curReloadingCount >= maxReloadingCount)
            {
                curAmmoCount = 0;
                curReloadingCount = 0;
                onReloading = false;
            }
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
