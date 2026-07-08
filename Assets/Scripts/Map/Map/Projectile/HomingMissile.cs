using System.Collections;
using Mirror;
using UnityEngine;

public class HomingMissile : NetworkBehaviour
{
    private Rigidbody2D rb;
    public Rigidbody2D RB {get{rb??=GetComponent<Rigidbody2D>(); return rb;} }

    private SpriteRenderer _sr;
    private SpriteRenderer SR{get{_sr??= GetComponent<SpriteRenderer>(); return _sr;}}

    [Header("추진")]
    [SerializeField] private float _thrustForce = 10f;
    [SerializeField] private float _boostPower = 10f;
    [SerializeField] private float _rotateSpeed = 200f;
    [SerializeField] private float _maxAngularVelocity = 150f;
    [SerializeField] private float _shakePower = 2f;
    [SerializeField] private float _armTime = 0.15f;

    [Header("수명")]
    [SerializeField] private float _lifeTime = 10f;
    [SerializeField] private float _lostLifeTime = 5f;

    [Header("공기총")]
    [SerializeField] private float _pushBackForce = 15f;
    [SerializeField] private float _torqueImpulse = 80f;
    [SerializeField] private float _thrustDrag = 0.5f;
    [SerializeField] private float _thrustResumeDelay = 0.4f;

    [Header("이펙트")]
    [SerializeField] private GameObject _boom_effect_obj;
    [SerializeField] private GameObject _missile_effect_obj;
    [SerializeField] private ParticleSystem _missile_Fire_effect;
    [SerializeField] private float _boom_release_delay = 1.5f;

    [SerializeField] private LayerMask _layer;

    /// <summary>
    /// 0 : Init
    /// 1 : already Reset,
    /// </summary>
    private int _resetCount = 0;

    private HomingTurret_Net _main;
    private Transform _target;
    private bool _isBoom = false;
    private bool _isFire = false;
    private bool _homingEnabled = true;   // false = 유도 영구 상실
    private bool _thrustEnabled = true;   // 피격 직후 일시 차단
    private bool _isHit = false;
    private float _elapsedTime = 0f;
    private float _lostTime = 0f;


    private void FixedUpdate()
    {
        if(_isBoom) return;

        _elapsedTime += Time.fixedDeltaTime;
 
        
        if (_elapsedTime >= _lifeTime)
        {
            Rpc_Boom();
            return;
        }
 
        Launch();
    }

    [ServerCallback]
    private void Update()
    {
         if (_isBoom) return;
 
        // 맵 전환 중이면 미사일 리셋 (중복 방지)
        if (MapEditor.Instance._onMapTransition_Complete == false && _resetCount == 0)
        {
            _resetCount = 1;
            Rpc_Reset();
        }
    }


    private void Launch()
    {
        if(!_isFire)
        {
            _isFire = true;
            _missile_Fire_effect.Play();
        }

        if (!_homingEnabled)
        {
            _lostTime += Time.fixedDeltaTime;

            if (_lostTime >= _lostLifeTime)
            {
                Rpc_Reset();
                return;
            }

            // 피격 후 일정 시간 지나면 추진 재개 + 감속 해제
            if (!_thrustEnabled && _lostTime >= _thrustResumeDelay)
            {
                _thrustEnabled = true;
                RB.drag = 0f;
                RB.angularDrag = 0f;
            }

            AlignToVelocity();
        }
        else if (_elapsedTime >= _armTime && _target != null)
        {
            Homing(_target);
            return;
        }
 
        if (_thrustEnabled)
            RB.AddForce(transform.right * _thrustForce, ForceMode2D.Force);

        //if (_lostTarget)
        //{
        //    if (_appliedLostImpulse)
        //    {
        //        RB.drag = _thrustDrag;
        //        RB.angularDrag = 2f;
//
        //        RB.AddForce(_lostDir * _pushBackForce, ForceMode2D.Impulse);
//
        //        float torqueDir = _upside ? -1f : 1f;
        //        RB.AddTorque(_cachedTorque * torqueDir, ForceMode2D.Impulse);
//
        //        _appliedLostImpulse = false;
        //    }
//
        //    // 추진력 매 프레임 적용
        //    RB.AddForce(transform.right * _thrustForce, ForceMode2D.Force);
//
        //    if (_lostTime >= 5f)
        //    {
        //        Rpc_Reset();
        //    }
//
        //    _lostTime += Time.fixedDeltaTime;
        //    return;
        //}
//
        //RB.drag = 0f;
        //RB.angularDrag = 0f;
        //_lostTime = 0f;
//
        //if (_onTarget)
        //{
        //    Homing(_target);
        //}
        //else
        //{
        //    RB.AddForce(transform.right * _thrustForce, ForceMode2D.Force);
        //}
    }

    private void Homing(Transform target)
    {
        Vector2 dir = ((Vector2)target.position - RB.position).normalized;

        float cross = Vector3.Cross(transform.right, dir).z;
        float noiseRot = Mathf.PerlinNoise(Time.time * 2f, 0f) - 0.5f; // -0.5 ~ 0.5 노이즈 값

        float targetAngular = cross * _rotateSpeed + noiseRot * 40f;
        RB.angularVelocity = Mathf.Clamp(targetAngular, -_maxAngularVelocity, _maxAngularVelocity);

        Vector2 forward = transform.right;
        Vector2 side = Vector2.Perpendicular(forward); //수직값

        float shake = (Mathf.PerlinNoise(Time.time * 6f, 1.3f) - 0.5f) * 2f; //대칭 진동 값

        RB.velocity = forward * _thrustForce + side * shake * _shakePower;
    }

    private void AlignToVelocity()
    {
        if (RB.velocity.sqrMagnitude < 0.01f) return;
 
        float velocityAngle = Mathf.Atan2(RB.velocity.y, RB.velocity.x) * Mathf.Rad2Deg;
        float currentAngle = RB.rotation;
        float delta = Mathf.DeltaAngle(currentAngle, velocityAngle);
        float maxDelta = (_rotateSpeed * 0.5f) * Time.fixedDeltaTime;
 
        RB.MoveRotation(currentAngle + Mathf.Clamp(delta, -maxDelta, maxDelta));
    }

    public void OnAirGunHit(Vector2 airGunWorldPos)
    {
        if (_isHit || _isBoom || !_homingEnabled) return;
 
        // 호출 시점의 미사일 상태를 스냅샷으로 고정
        // 모든 클라이언트가 동일한 값으로 계산
        Vector2 missilePos = RB.position;
        float missileRot = RB.rotation;
        Vector2 missileVel = RB.velocity;
        float missileAngVel = RB.angularVelocity;

        ApplyAirGunHit(airGunWorldPos, missilePos, missileRot, missileVel, missileAngVel);
        CmdAirGunHit(airGunWorldPos, missilePos, missileRot, missileVel, missileAngVel);
    }

    private void ApplyAirGunHit(Vector2 airGunWorldPos, Vector2 missilePos, float missileRot, Vector2 missileVel, float missileAngVel)
    {
        // 이미 폭발한 미사일에는 스냅샷 적용 금지 (Rpc 도착 순서 역전 대비)
        if (_isBoom) return;

        _isHit = true;
        _homingEnabled = false;
        _lostTime = 0f;

        // 송신자의 스냅샷으로 상태 통일 (송신자 본인은 사실상 no-op)
        RB.position = missilePos;
        RB.rotation = missileRot;
        RB.velocity = missileVel;
        RB.angularVelocity = missileAngVel;

        // 공기총 반대 방향으로 순간 충격
        Vector2 pushDir = (missilePos - airGunWorldPos).normalized;
        RB.AddForce(pushDir * _pushBackForce, ForceMode2D.Impulse);

        // 각도에 비례한 회전 충격 (스냅샷 회전 기준 forward 계산)
        // 측면을 맞을수록 크고, 정면/후면에 가까울수록 작음
        float rad = missileRot * Mathf.Deg2Rad;
        Vector2 forward = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        Vector2 toGun = airGunWorldPos - missilePos;
        float sideAngle = Vector2.SignedAngle(forward, toGun); // -180 ~ 180
        float torqueAmt = Mathf.Lerp(0f, _torqueImpulse, Mathf.Abs(sideAngle) / 180f);
        float torqueDir = sideAngle > 0f ? 1f : -1f;
        RB.AddTorque(torqueDir * torqueAmt, ForceMode2D.Impulse);

        // 추진 일시 정지 (Drag로 감속 연출)
        _thrustEnabled = false;
        RB.drag = _thrustDrag;
        RB.angularDrag = 2f;
    }

    [Command(requiresAuthority = false)]
    private void CmdAirGunHit(Vector2 airGunWorldPos, Vector2 missilePos, float missileRot, Vector2 missileVel, float missileAngVel)
    {
        // 서버 기준으로 이미 폭발/피격된 히트는 무효 처리
        if (_isBoom || _isHit) return;

        RpcAirGunHit(airGunWorldPos, missilePos, missileRot, missileVel, missileAngVel, connectionToClient.identity.netId);
    }

    [ClientRpc]
    private void RpcAirGunHit(Vector2 airGunWorldPos, Vector2 missilePos, float missileRot, Vector2 missileVel, float missileAngVel, uint senderNetId)
    {
        if (NetworkClient.localPlayer.netId == senderNetId) return;

        ApplyAirGunHit(airGunWorldPos, missilePos, missileRot, missileVel, missileAngVel);
    }

    //===
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color =Color.red;
        Vector3 origin = transform.position;
        Vector3 dir = transform.right;   // 레이 방향
        float length = 0.35f;

        Gizmos.DrawRay(origin, dir * length);
    }
    #endif

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _main.gameObject) return;

        if (((1 << collision.gameObject.layer) & _layer) == 0) return;
        
        if (collision.TryGetComponent(out IDamageable component))
        {
            component.TakeDamage(DamageType.Boom);
        }

        Rpc_Boom();
    }

    [ClientRpc]
    private void Rpc_Target_Distroyed(uint netId)
    {
        _main.Cmd_Target_Distroyed();
        NetworkIdentity target = NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity) ? identity : null;
        if(target != null)
        {
            if(target.TryGetComponent(out IDamageable component))
            {
                component.TakeDamage(DamageType.Boom);
            }
        }
    }
    
    public void SetTarget(HomingTurret_Net main,Transform target)
    {
        _main = main;
        _target = target;
 
        // 상태 초기화
        _isBoom = false;
        _isFire = false;
        _homingEnabled = true;
        _thrustEnabled = true;
        _isHit = false;
        _elapsedTime = 0f;
        _lostTime = 0f;
        _resetCount = 0;
 
        RB.drag = 0f;
        RB.angularDrag = 0f;
        RB.velocity = Vector2.zero;
        RB.angularVelocity = 0f;
 
        Clean();
 
        // 발사 초기 부스트
        RB.AddForce(transform.right * _boostPower, ForceMode2D.Impulse);
    }

    private Coroutine _boom_corotuine;

    [ClientRpc]
    private void Rpc_Boom()
    {
        if(_boom_corotuine != null)
        {
            StopCoroutine(_boom_corotuine);
            _boom_corotuine = null;
        }

        _boom_corotuine = StartCoroutine(Boom_Co());
    }

    //[SerializeField] private float _boom_release_delay = 1.5f;
    private WaitForSeconds _wfs;
    private IEnumerator Boom_Co()
    {
        _isBoom = true;

        _boom_effect_obj.transform.rotation = Quaternion.identity;
        _boom_effect_obj.SetActive(true);
        _missile_effect_obj.SetActive(false);
        SR.enabled = false;

        _target = null;
        _main = null;

        RB.velocity = Vector2.zero;
        RB.angularVelocity = 0f;
        yield return _wfs ??= new WaitForSeconds(_boom_release_delay);
        Clean();
        // Managers.Pooling.D_ReleaseToPool(gameObject);
        if(isServer) Managers.Pooling.N_ReleaseToPool(gameObject);
        else gameObject.SetActive(false);
    }

    [ClientRpc]
    private void Rpc_Reset()
    {
        StopAllCoroutines();
        RB.velocity = Vector2.zero;
        
        _target = null;
        _main = null;
        
        Clean();
        if(isServer) Managers.Pooling.N_ReleaseToPool(gameObject);
        else gameObject.SetActive(false);
    }

    private void Clean()
    {
        _isBoom = false;
        _boom_effect_obj.SetActive(false);
        _missile_effect_obj.SetActive(true);
        SR.enabled = true;

        _isFire = false;
        _missile_Fire_effect.Stop();
    }


    #region  Util
    //private bool TargetDistanceCheck(Transform target)
    //{
    //    if(target == null) return false;
//
    //    float sqrDist = (target.transform.position - transform.position).sqrMagnitude;
//
    //    if(sqrDist < _maxHomingDistance * _maxHomingDistance)
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    #endregion



}
