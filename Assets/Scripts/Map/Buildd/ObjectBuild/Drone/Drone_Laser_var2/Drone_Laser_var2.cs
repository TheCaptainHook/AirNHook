
using Unity.VisualScripting;
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

    [SerializeField] Drone_Laser_var2_Net droneLaser_Net;

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

    void OnDisable()
    {
        Laser_Rapid_Fire_Sound(false);
    }

    #region  GuardVision
    [SerializeField] Drone_Laser_GuardVision guardVision;
    #endregion

    public void PreStateSetUp(bool isStop,bool message)
    {
        this.isStop = isStop;
        this.message.enabled = message;
        laserParts.LaserReset();
    }

    #region Guard
    private void State_Guard()
    {
        if(curAmmoCount >0)
        {
            Reloading();
        }
    }
    #endregion

#region Tracking
    // private void State_Tracking()
    // {

    //     float dis = Vector2.Distance(droneLaser_Net.trackingBeforePosition, _rb.position);
    //     if(dis > 3)
    //     {
    //         //return;
    //         if (NetworkServer.active) droneLaser_Net.Server_DroneLaserState(2);
    //         return;
    //     }

    //     Vector2 dir = (laserParts.transform.position-transform.position).normalized;
    //     Vector2 step = dir * moveSpeed * Time.fixedDeltaTime;
    //     transform.position += (Vector3)step;
    // }

    #endregion
    #region Return
    // public int returnIndex = 0;
    // private void State_Return()
    // {
    //     Debug.Log("Return");
    //     if (returnIndex >= droneLaser_Net.returnPath.Count)
    //     {
    //         if(NetworkServer.active)
    //         droneLaser_Net.Server_AfterReturn();
    //         return;
    //     }

    //     ReturnForward();

    // }
    // private void ReturnForward()
    // {
    //     Vector2 target = droneLaser_Net.returnPath[returnIndex];
    //     Vector2 step = (target - (Vector2)transform.position).normalized * Time.fixedDeltaTime * moveSpeed;

    //     if (Vector2.Distance(target, transform.position) < step.magnitude)
    //     {
    //         transform.position = target;
    //         returnIndex++;
    //         //Debug.Log($"Arrived at {target}, Next Index: {returnIndex}");
    //     }
    //     else
    //     {
    //         transform.position += (Vector3)step;
    //         Debug.Log($"Arrived at {target}, Next Index: {returnIndex}, {step}");
    //     }
    // }

    #endregion
    
    #region Drone Laser Field
    [Space(10)]
    [Header("Drone Laser Field")]
    public float laserPartsAngleRate = 0.5f;
    public float laserTargetingMarkMovingRate = 0.1f;
    #endregion


    #region  Attack
    [SerializeField] GameObject ammo;
    [Tooltip("Reloading Cooltime")]
    public float maxReloadingRate = 1; 
    public int maxAmmoCount = 50;
    [Tooltip("Shouts per second")]
    public float maxFireRate  = 0.05f;
    private float curReloadingCount = 0;
    private int curAmmoCount = 0;
    private float curFireRate = 0;
    private bool onReloading;
    private void Shot()
    {
        Projectile_Plasma plasma = Managers.Pooling.D_GetItem(ammo).GetComponent<Projectile_Plasma>();
        plasma.Setting(LaserPoint.position, RandomDir(LaserPoint.right), gameObject);
        // plasma.gameObject.SetActive(true);
        curAmmoCount++;
    }

    private Vector2 RandomDir(Vector2 dir)
    {
        //-5~5
        float randomRange = Random.Range(-10,10);
        return Quaternion.Euler(0,0,randomRange) * dir;
    }
    //------------------------------------------------------Sound
    private AudioSourceController _audioSourceController;
    private AudioSource AudioSource => _audioSourceController == null ? null : _audioSourceController.GetAudioSource();
    //------------------------------------------------------Sound
    private void State_Attack()
    {
        if (!laserParts.isShotReady) laserParts.TrackingTarget();
        if (curAmmoCount >= maxAmmoCount)
        {
            onReloading = true;
            droneLaser_Net.onAtack = false;
        }

        if (!onReloading)
        {
            curFireRate += Time.deltaTime;

            if (curFireRate >= maxFireRate && laserParts.isShotReady)
            {
                //Shot
                //------------------------Sound
                Laser_Rapid_Fire_Sound(true);
                //------------------------Sound
                Shot();
                curFireRate = 0;
                //Shot

            }

        }
        else
        {
            Reloading();
        }
    }

    private void Laser_Rapid_Fire_Sound(bool onOff)
    {
        if (!onOff)
        {
            if (_audioSourceController != null)
            {
                Managers.Sound.StopSound(_audioSourceController);
                _audioSourceController = null;
            }
            return;
        }

        if (_audioSourceController != null) return;
        _audioSourceController = Managers.Sound.PlaySound3D(GlobalText.DRONE_LASER_RAPID_FIRE_SOUND, LaserPoint.position, 1f, true);

    }

    private void Reloading()
    {
        //------------------------Sound Off
        Laser_Rapid_Fire_Sound(false);
        //------------------------Sound Off
        laserParts.isShotReady = false;
        curReloadingCount += Time.deltaTime;
        if (curReloadingCount >= maxReloadingRate)
        {
            curAmmoCount = 0;
            curReloadingCount = 0;
            onReloading = false;
        }
    }
    #endregion



    #region State Change
    public void StateChange(DRONE_LASER_STATE state, GameObject target = null)
    {
        switch (state)
        {
            case DRONE_LASER_STATE.GUARD:
                Laser_Rapid_Fire_Sound(false);
                State_Guard();
                break;
            case DRONE_LASER_STATE.TRACKING:
                Laser_Rapid_Fire_Sound(false);
                // State_Tracking();
                break;
            case DRONE_LASER_STATE.RETURN:
                Laser_Rapid_Fire_Sound(false);
                //State_Return();
                break;
            case DRONE_LASER_STATE.ATTACK:
                State_Attack();
                break;
        }
    }


#endregion




}
