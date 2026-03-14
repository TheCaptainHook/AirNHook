using System.Collections;
using Mirror;
using UnityEngine;

public class HomingMissile : NetworkBehaviour
{
  private Rigidbody2D rb;
  public Rigidbody2D RB {get{rb??=GetComponent<Rigidbody2D>(); return rb;} }

  [SerializeField] private float _moveSpeed = 10f;
  [SerializeField] private float _rotateSpeed = 200f;
  [SerializeField] private float _lifeTime = 5f;
  [SerializeField] private float _maxHomingDistance = 200f;

private SpriteRenderer _sr;
private SpriteRenderer SR{get{_sr??= GetComponent<SpriteRenderer>(); return _sr;}}

[SerializeField] private GameObject _boom_effect_obj;
[SerializeField] private GameObject _missile_effect_obj;
[SerializeField] private ParticleSystem _missile_Fire_effect;

  private bool _onTarget = false;
  private bool _isBoom = false;
  private Transform _target = null;
  private HomingTurret_Net _main;
    private bool _isFire = false;
    [SerializeField] private LayerMask _layer;

    [SerializeField] private float _maxTimer;
    [SerializeField] private float _curTimer;

    /// <summary>
    /// 0 : Init
    /// 1 : already Reset,
    /// </summary>
    private int _resetCount = 0;

    [ServerCallback]
    private void FixedUpdate()
    {
        if(_isBoom) return;
        if(!_onTarget) return;
        Launch();

         if(_onTarget) TargetCheckRay();
        _curTimer += Time.fixedDeltaTime;

        if(_curTimer >= _maxTimer)
        {
            //=============Boom
            _curTimer = 0;
            Rpc_Boom();
            //=============Boom
            return;
        }
    }

    [ServerCallback]
    private void Update()
    {
        if(_isBoom) return;
        if(!_onTarget) return;
        
        if(MapEditor.Instance._onMapTransition_Complete == false && _resetCount == 0)
        {
            _resetCount = 1;
            Rpc_Reset();
            return;
        }

        // if(_onTarget) TargetCheckRay();
        // _curTimer += Time.deltaTime;

        // if(_curTimer >= _maxTimer)
        // {
        //     //=============Boom
        //     _curTimer = 0;
        //     Rpc_Boom();
        //     //=============Boom
        //     return;
        // }
    }


    private void Launch()
    {
        //======Target Distance Check
        if(TargetDistanceCheck(_target))_onTarget = true;
        else _onTarget = false;
        //======Target Distance Check

        if(!_isFire)
        {
            _isFire = true;
            _missile_Fire_effect.Play();
        }

        if(_onTarget)
        {
            Homing(_target);
        }
        else
        {
            //=======일직선으로
            RB.velocity = transform.right * _moveSpeed;
            // RB.velocity = Vector2.zero;
        }
    }

    [SerializeField] private float _shakePower =1;
    private void Homing(Transform target)
    {
        Vector2 dir = ((Vector2)target.position - RB.position).normalized;

        float cross = Vector3.Cross(transform.right, dir).z;
        float noiseRot = Mathf.PerlinNoise(Time.time * 2f, 0f) - 0.5f; // -0.5 ~ 0.5 노이즈 값

        RB.angularVelocity = cross * _rotateSpeed + noiseRot * 40f;

        Vector2 forward = transform.right;
        Vector2 side = Vector2.Perpendicular(forward); //수직값

        float shake = (Mathf.PerlinNoise(Time.time * 6f, 1.3f) - 0.5f) * 2f; //대칭 진동 값

        RB.velocity =
            forward * _moveSpeed +
            side * shake * _shakePower;
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

    //===
    private void TargetCheckRay() //only Server
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 0.35f, _layer);
        if(hit.collider != null && hit.collider.gameObject != _main.gameObject)
        {
            if(hit.collider.TryGetComponent(out NetworkIdentity component))
            {
                Rpc_Target_Distroyed(component.netId);
            }
            
            Rpc_Boom();
        }
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
    [SerializeField] private float _boostPower = 50f;
    
    public void SetTarget(HomingTurret_Net main,Transform target)
    {
        _resetCount = 0;

        _target = target;
        _main = main;

        RB.AddForce(transform.right * _boostPower, ForceMode2D.Impulse);

        _onTarget = true;
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
    [SerializeField] private float _boom_release_delay = 1.5f;
    private WaitForSeconds _wfs;
    private IEnumerator Boom_Co()
    {
        _isBoom = true;

        _boom_effect_obj.SetActive(true);
        _missile_effect_obj.SetActive(false);
        SR.enabled = false;

        _curTimer = 0;
        _onTarget = false;
        _target = null;
        _main = null;
    
        RB.velocity = Vector2.zero;
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
        
        _curTimer = 0;
        _onTarget = false;
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
    private bool TargetDistanceCheck(Transform target)
    {
        if(target == null) return false;

        float sqrDist = (target.transform.position - transform.position).sqrMagnitude;

        if(sqrDist < _maxHomingDistance * _maxHomingDistance)
        {
            return true;
        }
        return false;
    }

    #endregion



}
