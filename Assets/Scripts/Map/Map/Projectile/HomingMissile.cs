using System.Collections;
using UnityEngine;

public class HomingMissile : MonoBehaviour
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

  private bool _onTarget = false;
  private bool _isBoom = false;
  private Transform _target = null;
  private GameObject _main;

    [SerializeField] private LayerMask _layer;

    [SerializeField] private float _maxTimer;
    [SerializeField] private float _curTimer;

    private void FixedUpdate()
    {
        if(_isBoom) return;

        if(_curTimer >= _maxTimer)
        {
            //=============Boom
            Clean();
            //=============Boom
            return;
        }
        _curTimer += Time.fixedDeltaTime;
        Launch();
    }

    private void Update()
    {
        if(_onTarget) TargetCheckRay();
    }


    private void Launch()
    {
        //======Target Distance Check
        if(TargetDistanceCheck(_target))_onTarget = true;
        else _onTarget = false;
        //======Target Distance Check

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
    private void Homing(Transform target)
    {
        Vector2 dir = ((Vector2)target.position - RB.position).normalized;
        float noise = Mathf.PerlinNoise(Time.time * 2f,0f) - 0.5f;
        float cross = Vector3.Cross(transform.right,dir).z;

        if(cross != 0) RB.angularVelocity =(cross * _rotateSpeed) + noise * 50f;
        else RB.angularVelocity = 0f;
        RB.velocity = transform.right * _moveSpeed;
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
    private void TargetCheckRay()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 0.35f, _layer);
        if(hit.collider != null && hit.collider.gameObject != _main)
        {
            if(hit.collider.TryGetComponent(out IDamageable component))
            {
                component.TakeDamage(DamageType.Boom);
            }
            
            Boom();
        }
    }
    [SerializeField] private float _boostPower = 50f;
    
    public void SetTarget(GameObject main,Transform target)
    {
        _target = target;
        _main = main;

        RB.AddForce(transform.right * _boostPower, ForceMode2D.Impulse);

        _onTarget = true;
    }

    private Coroutine _boom_corotuine;
    private void Boom()
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
        Managers.Pooling.D_ReleaseToPool(gameObject);
    }

    private void Clean()
    {
        _isBoom = false;
        _boom_effect_obj.SetActive(false);
        _missile_effect_obj.SetActive(true);
        SR.enabled = true;
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
