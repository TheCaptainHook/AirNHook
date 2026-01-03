using System.Collections;
using Mirror;
using UnityEngine;

public enum Missile_State
{
    SEARCH,
    TARGETTING,
    LAUNCH,
    RELOAD,
}

public class HomingTurret_Net : ActivatableObject_Net_Entity
{
    [SerializeField] private bool _onReady;
    [SerializeField] GameObject _target;

    [Header("Layer")]
    [SerializeField] LayerMask _detactLayer;
    [SerializeField] LayerMask _obstacleLayer;
    [Header("Parts")]
    [SerializeField] Transform _turretTopTR;
    // private Vector2 TopRight => _turretTopTR.right;
   
    [Header("Fire Point")]
    [SerializeField] Transform[] _firePoints;
    [SerializeField] bool[] _isLaunched;
    private float _minFirePositionOffset = 0.5f;
    private float _maxFirePositionOffset = 0.6f;

    [SerializeField] private Transform _rayPosition;
    //TESDT
    [SerializeField] private GameObject _missilePrefab;

    [Header("Launch")]
    // private bool _isCompleteRotate = false; //server
    private bool _onReload = false; //server
    
    // private WaitForSeconds _maxLaunchDelayWFS;

    [Header("Effect")]
    [SerializeField] private GameObject _greenLightEffect;
    [SerializeField] private GameObject _yellowLightEffect;
    [SerializeField] private GameObject _steam;
    [SerializeField] Animator _steam_1;
    [SerializeField] Animator _steam_2;

    void Awake()
    {
        _isLaunched = new bool[3];
    }

    protected override void Active() //RPC
    {
        _onReady = true;
        
    }
    protected override void Deactive() //RPC
    {
        _onReady = false;
    }


    [ServerCallback]
    void FixedUpdate()
    {
        if(!_onReady) return;
        // if(_onLunch) return;

        //UpdateTargetDetection();
        Server_Run_FSM();
    }

//================================ 260101 FSM
    private bool _onPrograss = false;
    private Missile_State _ms = Missile_State.SEARCH;
    private Missile_State _previous_ms;
    [Server]
    private void Server_Run_FSM()
    {
            Run_FSM(_ms);
    }

    private void Run_FSM(Missile_State ms) //server
    {
            switch (ms)
            {
                case Missile_State.SEARCH: Server_Searching();
                break;
                case Missile_State.TARGETTING: Server_Targetting();
                break;
                case Missile_State.LAUNCH: Server_Launch();
                break;
                case Missile_State.RELOAD: Server_Reloading();
                break;
            }   
    }
    [Server]
    private void Change_Ms(Missile_State ms)
    {
        _previous_ms = _ms;
        _ms = ms;
    }

#region  SEARCH
    [Server]
    private void Server_Searching()
    {
        if(_previous_ms == Missile_State.TARGETTING)
        {
            //RPC Stop Roate, Stop Marking
            //RPC Stop Roate, Stop Marking
        }

        //======State Initialize
        // _isCompleteRotate = false;
        _onRotateComplete = false;
        // _isRotate = false;
        //======State Initialize

         _s_target = DetectTargetInRange();
        if(_s_target != null) //타겟 발견
        {
            _cur_missingTargetCount = 0;
            if(!ObstacleCheck(_s_target)) //타겟이 장애물에 가려지면 미싱 타겟
            {
                // _s_target = null;
                Change_Ms(Missile_State.TARGETTING);
            }
        }
    }

    [SerializeField] float _detectRadius = 10f;
    private Collider2D[] _detectBuffer = new Collider2D[16];
    private GameObject DetectTargetInRange()
    {
        int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, _detectRadius, _detectBuffer,_detactLayer);
        if(hitCount == 0) return null;

        float closestSqrDist = float.MaxValue;
        GameObject closest = null;

        for(int i=0; i<hitCount; i++)
        {
            Collider2D col = _detectBuffer[i];
            if(col == null) continue;
            if(col.gameObject == gameObject) continue;
            if(!col.TryGetComponent(out NetworkIdentity _)) continue;

            
            float sqrDist = ((Vector2)col.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (sqrDist < closestSqrDist)
            {
                closestSqrDist = sqrDist;
                closest = col.gameObject;
            }
       
        }

        return closest;
    }
#endregion
#region TARGETTING
private bool _rotationRequested = false;

    [Server]
    private void Server_Targetting()
    {
        //======Obstacle Check, Distance Check
        if(_s_target == null || !DistanceCheck(_s_target) || ObstacleCheck(_s_target))
        {
             Change_Ms(Missile_State.SEARCH);
             return;
        }
        //======Obstacle Check, Distance Check
        // GameObject obj = NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity) ? identity.gameObject : null;
        //======Targetting
        //======Targetting

        //======Rotate
        if(!_rotationRequested)
        {
            _rotationRequested = true;
            float angle = CalcTargetAngle(_s_target);
            Rpc_RotateTurret(angle);
        }
        
        if(_onRotateComplete)
        {
            _onRotateComplete = false;
            _rotationRequested = false;
            Change_Ms(Missile_State.LAUNCH);
        }
    }
    private float CalcTargetAngle(GameObject target)
    {
        Vector2 dir = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;

        Transform basis = _turretTopTR.parent;
        Vector2 dirLocal = basis.InverseTransformDirection(dir);
        
        float angle = Vector2.SignedAngle(Vector2.right,dirLocal);
        angle = Mathf.Clamp(angle, _min_rotate_z, _max_rotate_z);
        
        return angle;
    }


    [ClientRpc]
    private void Rpc_Targetting(uint netID)
    {
        
    }

#region Rotate
    private bool _onRotateComplete = false;
    private Coroutine _rotate_coroutine;
    private float _max_rotate_z = 135;
    private float _min_rotate_z = 45;
    [SerializeField] private float _rotate_tolerance = 5;
    // private bool _isRotate = false;
    [ClientRpc]
    private void Rpc_RotateTurret(float angle)
    {
        _onRotateComplete = false;

        if(_rotate_coroutine != null)
        {
            StopCoroutine(_rotate_coroutine);
            _rotate_coroutine = null;
        }
        float delta = Mathf.Abs(Mathf.DeltaAngle(_turretTopTR.localEulerAngles.z,angle));
        if(delta < _rotate_tolerance)
        {
            _onRotateComplete = true;
            return;
        }

        _rotate_coroutine = StartCoroutine(Rotate_Co(angle));    
    }
    [SerializeField] private float _top_parts_rotate_speed=5;
    private IEnumerator Rotate_Co(float targetZ)
    {
        // Quaternion startRot = _turretTopTR.localRotation;
        Quaternion targetRot = Quaternion.Euler(0, 0, targetZ);

        while (Quaternion.Angle(_turretTopTR.localRotation, targetRot) > 0.1f)
        {
            _turretTopTR.localRotation = Quaternion.RotateTowards(_turretTopTR.localRotation,
                                                             targetRot,
                                                             _top_parts_rotate_speed * Time.deltaTime * 100);
            yield return null;
        }
        _turretTopTR.localRotation = targetRot;
        _rotate_coroutine = null;
        
    
        if(NetworkServer.active) Cmd_ChangeRoateComplete();
    
    }
    [Command]
    private void Cmd_ChangeRoateComplete()
    {
        _onRotateComplete = true;        
    }


#endregion//Rotate
#endregion//Targetting

#region LAUNCH
    [Server]
    private void Server_Launch()
    {
        if(_cur_FireDelay <= 0)
            {
                _cur_FireDelay = _max_fireDelay;
                
                if(_cur_fireCount >= _max_fireCount)
                {
                    //=====Reloading
                    Change_Ms(Missile_State.RELOAD);
                    //=====Reloading
                    return;
                }
                
                Server_LaunchMissile(_s_target,_cur_fireCount);
                _cur_fireCount++;
            }else
            {
                _cur_FireDelay -= Time.deltaTime;
            }
    }

    [Server]
    private void Server_LaunchMissile(GameObject obj,int count)
    {
        if(_isLaunched[count]) return;
        if(obj.TryGetComponent(out NetworkIdentity identity)) Rpc_LaunchMissile(identity.netId,count);        
    }


    [ClientRpc]
    private void Rpc_LaunchMissile(uint netId,int count)
    {
        GameObject obj = NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity) ? identity.gameObject : null;
        if(obj == null) return;
        
        _target = obj;
        LaunchMissile(_firePoints[count], _target);
        _isLaunched[count] = true;
    }

    private void LaunchMissile(Transform tr, GameObject target)
    {
        tr.gameObject.SetActive(false);

        var obj = Managers.Pooling.D_GetItem(_missilePrefab);
        obj.transform.position = tr.position;
        obj.transform.rotation = _turretTopTR.rotation;

        obj.SetActive(true);

        if(obj.TryGetComponent(out HomingMissile missile))
        {
            missile.SetTarget(gameObject,target.transform);
        }

        Managers.Sound.PlaySound3D(GlobalText.MISSILE_TURRET_FIRE,transform);
    }

#region Reload
    [Server]
    private void Server_Reloading()
    {
        if(!_onReload)
        {
            _onReload = true;
            _onReloadComplete = false;
            _cur_FireDelay = _max_fireDelay;
            Rpc_Reloading();
        }

        if(_onReloadComplete)
        {
            _onReload = false;
            Change_Ms(Missile_State.SEARCH);
        }
    }
    private Coroutine _reloadingCoroutine;
    private bool _onReloadComplete = false;
    [ClientRpc]
    private void Rpc_Reloading()
    { 
        if(_reloadingCoroutine != null)
        {
             StopCoroutine(_reloadingCoroutine);
             _reloadingCoroutine = null;
        }
        _reloadingCoroutine = StartCoroutine(Reloading());
    }
    private IEnumerator Reloading()
    {
        _greenLightEffect.SetActive(false);
        _yellowLightEffect.SetActive(true);

        _steam.SetActive(true);
        int count = 0;
        //Reset Launch
        for(int i = 0;i<_isLaunched.Length; i++)
        {
            if(_isLaunched[i])
            {
                count++;
                StartCoroutine(Reload(_firePoints[i], i));
            }
        }
        yield return new WaitForSeconds(count);

        _cur_fireCount = 0;
        _cur_FireDelay = 0;

        _greenLightEffect.SetActive(true);
        _yellowLightEffect.SetActive(false);
        // _onReload = false;
        // _onReloadComplete = true;
        if(NetworkServer.active) Cmd_ReloadComplete();
        // if(NetworkServer.active) Change_Ms(Missile_State.SEARCH);
    }
    [Command]
    private void Cmd_ReloadComplete()
    {
        _onReloadComplete = true;
    }
    private IEnumerator Reload(Transform point,int index)
    {
        //==1. 생성
        point.localPosition = new Vector3(0,point.localPosition.y, 0);
        point.gameObject.SetActive(true);
        //==1

        float percent = 0;
        while(percent < 1f)
        {
            percent += Time.deltaTime;
            point.localPosition = new Vector3(Mathf.Lerp(0,_maxFirePositionOffset, percent),point.localPosition.y, 0);
            yield return null;
        }
        percent = 0;
        while(percent < 1f)
        {
            percent += Time.deltaTime;
            point.localPosition = new Vector3(Mathf.Lerp(_maxFirePositionOffset, _minFirePositionOffset, percent),point.localPosition.y, 0);
            yield return null;
        }

        point.localPosition = new Vector3(_minFirePositionOffset,point.localPosition.y, 0);
        _isLaunched[index] = false;

    }
#endregion//Reload
#endregion//Lauch

#region MISSING
    // [Server]
    // private void Server_MissiongTarget()
    // {
    //     _cur_missingTargetCount += Time.deltaTime;

    //     _s_target = DetectTargetInRange();
    //     if(_s_target != null) //타겟 발견
    //     {
    //         if(!ObstacleCheck(_s_target))
    //         {
    //             Change_Ms(Missile_State.SEARCH);
    //             return;
    //         }
    //     }
        
    //     if(_cur_missingTargetCount >= _max_missingTargetCount && _cur_fireCount > 0)
    //     {
    //         if(_cur_fireCount >0)
    //         {
    //             _cur_missingTargetCount = 0;
    //             // Server_Reloading();
    //             Change_Ms(Missile_State.RELOAD);
    //         }
    //         else
    //         {
                
    //         }
    //     }
        
    // }
    // [ClientRpc]
    // private void Rpc_MissiongTarget()
    // {
    //     /**
    //     1. Tartting Stop Coroutine
    //     2. Rotate Stop Coroutine

    //     **/

    //     //========================= Rotate
    //     _isCompleteRotate = false;
    //     if(_rotate_coroutine != null)
    //     {
    //         StopCoroutine(_rotate_coroutine);
    //         _rotate_coroutine = null;

    //     }
    //     //========================= Rotate
    // }
#endregion
//================================

    private GameObject _s_target;
    
    private uint GetTargetNetID
    {
        get
        {
            if (_s_target == null)
                return 99999;
            return _s_target.TryGetComponent(out NetworkIdentity identity) ? identity.netId : 99999;
        }
    }

    #region  Detect
    [SyncVar] public bool _isFindTarget;
  
    //======= 1223
    [Tooltip("missile Firing Interval")]
    [SerializeField] private float _max_fireDelay = 0.5f;
    private float _cur_FireDelay=0;
    private int _max_fireCount = 3;
    private int _cur_fireCount = 0;
    //=======
    [Tooltip("missing Target Reload Delay")]
    [SerializeField] private float _max_missingTargetCount = 1;
    private float _cur_missingTargetCount = 0;

    // [Server]
    // private void UpdateTargetDetection()
    // {
    //     //========Reloading 중이면 return
    //     if(_onReload) return;
    //     //========Reloading

    //     _s_target = DetectTargetInRange();
    //     if(_s_target != null) //타겟 발견
    //     {
    //         _cur_missingTargetCount = 0;

    //         if(ObstacleCheck(_s_target)) //타겟이 장애물에 가려지면 미싱 타겟
    //         {
    //             _s_target = null;
    //             // _isFindTarget = false;
    //             Server_MissiongTarget();
    //             return;
    //         }

            
    //         //================Targetting
    //         /**
    //             1. Marking Coroutine   
    //                 - Marking Animation
    //                     - RPC_Marking
    //                 - Top Parts Rotation
    //                     - RPC_Rotation
    //             3. onTargetting Comp

    //         **/
    //         //================Targetting
    //         //================Rotate
    //         // Rpc_RotateTurret(GetTargetNetID);
    //         if(!_isCompleteRotate) return;
    //         //================Rotate

    //         if(_cur_FireDelay <= 0)
    //         {
    //             _cur_FireDelay = _max_fireDelay;
                
    //             if(_cur_fireCount >= _max_fireCount)
    //             {
    //                 //=====Reloading
    //                 Server_Reloading();
    //                 //=====Reloading
    //                 return;
    //             }
                
    //             Server_LaunchMissile(_s_target,_cur_fireCount);
    //             _cur_fireCount++;
    //         }else
    //         {
    //             _cur_FireDelay -= Time.deltaTime;
    //         }

    //     }
    //     else
    //     {
    //         _cur_missingTargetCount += Time.deltaTime;

    //         if(_cur_missingTargetCount >= _max_missingTargetCount && _cur_fireCount > 0)
    //         {
    //             _cur_missingTargetCount = 0;
    //             Server_Reloading();
    //         }
    //     }
    // }
   

   

 
   

    private bool ObstacleCheck(GameObject target)
    {
        Vector2 dir = ((Vector2)target.transform.position - (Vector2)_rayPosition.position).normalized;
        float dist = Vector2.Distance(target.transform.position, _rayPosition.position);

        RaycastHit2D hit = Physics2D.Raycast(_rayPosition.position, dir, dist, _obstacleLayer);
        if(hit.collider != null)
        {
            return true;
        }

        return false;
    }
    private bool DistanceCheck(GameObject target)
    {
        if(target == null) return false;

        float sqrDist = ((Vector2)target.transform.position - (Vector2)transform.position).sqrMagnitude;
        return sqrDist <= _detectRadius * _detectRadius;
    }

#endregion



#region  Clean
    private void Reset()
    {
        _s_target = null;
        
        if(_rotate_coroutine != null)
        {
            StopCoroutine(_rotate_coroutine);
            _rotate_coroutine = null;
        }
        if(_reloadingCoroutine != null)
        {
            StopCoroutine(_reloadingCoroutine);
            _reloadingCoroutine = null;
        }


        for(int i =0;i<_isLaunched.Length;i++)
        {
            _isLaunched[i]= false;
            _firePoints[i].localPosition = new Vector3(_minFirePositionOffset,_firePoints[i].position.y,0);
        }    

        _cur_fireCount = 0;
        _cur_FireDelay = 0;
        _greenLightEffect.SetActive(true);
        _yellowLightEffect.SetActive(false);
        _onReload = false;

    }

#endregion


}
