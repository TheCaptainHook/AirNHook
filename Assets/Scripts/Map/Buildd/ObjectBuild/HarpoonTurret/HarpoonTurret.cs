
using UnityEngine;

public class HarpoonTurret : BuildObj
{

    RaycastHit2D hit;

    public LayerMask layerMask;


    public bool isShot;


    //public float cooltime;
    //private float curtime;
    [SerializeField] private GameObject _holder;
    [SerializeField] private ParticleSystem _shellParticle;
    [SerializeField] private Animator _topSteam;
    [SerializeField] private Animator _bottomSteam;
    private Animator _animator;
    public float radius = 10f; // 스피어 캐스트의 반지름

    [SerializeField] GameObject arrowPrefab;
    
    #region StringCache
    private static readonly int IsFiring = Animator.StringToHash("IsFiring");
    private static readonly int IsReloadingFinished = Animator.StringToHash("IsReloadingFinished");
    private static readonly int IsDestroyed = Animator.StringToHash("IsDestroyed");
    private static readonly int IsTurnedOff = Animator.StringToHash("IsTurnedOff");
    #endregion

    private Vector2 orgDirRight;

    private void Start()
    {
        _animator = GetComponent<Animator>();

        orgDirRight = transform.right;

    }
    private void Update()
    {
        RotateTrap();
    }
    #region  Get,Set
    HarpoonTurret_Net Net => GetComponent<HarpoonTurret_Net>();
    public override void SetData<T>(T data)
    {
        base.SetData(data);
        transform.position = ObjectData.position;
        transform.rotation = ObjectData.quaternion;
        Net.onSync = true;

        Net.Server_InitSync();

    }
    #endregion

    private float shotCooldown = 4f; // 재발사까지의 딜레이
    private float lastShotTime = -1f;
    private void FixedUpdate()
    {
        if (Time.time - lastShotTime < shotCooldown) return;

        hit = Physics2D.Raycast(transform.position, _holder.transform.right, 10f, layerMask);
        if (hit)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground/AllAllowed")) return;
            if (!isShot)
            {
                isShot = true;
                lastShotTime = Time.time;

                _animator.SetTrigger(IsFiring);
                //curtime = cooltime;
                if(hit.collider.TryGetComponent(out PlayerSM component))
                {
                    Shot();
                }

            }
          
        }
    }
    private void Shot(){
        Projectile_Arrow arrow = Managers.Pooling.D_GetItem(arrowPrefab).GetComponent<Projectile_Arrow>();
        arrow.Setting(transform.position, _holder.transform.right,gameObject);
        arrow.gameObject.SetActive(true);

    }

    //--------------------------Animation Trigger
    private void Reloaded()
    {
        _animator.SetTrigger(IsReloadingFinished);
    }

    private void ShotReady()
    {
        isShot = false;
    }
    //--------------------------Animation Trigger


    private void RotateTrap()
    {
        // 스피어 캐스트를 통해 주변에 있는 플레이어 레이어를 가진 모든 오브젝트 탐지
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        Transform nearestPlayer = null;
        //Vector2 nearestPlayer;
        float nearestDistance = Mathf.Infinity;

        // 주변에 있는 모든 플레이어 레이어를 가진 오브젝트에 대해 반복
        foreach (Collider2D collider in colliders)
        {
            //Vector2 targetPosition = collider is TilemapCollider2D ? (Vector2)collider.bounds.center : (Vector2)collider.transform.position;
            Vector2 targetPosition =  (Vector2)collider.transform.position;


            // 현재 탐지된 오브젝트와의 거리 계산
            //float distance = Vector2.Distance(transform.position, collider.transform.position);
            float distance = Vector2.Distance(transform.position, targetPosition);
            //semiCircle Detection
            Vector2 dir = collider.gameObject.transform.position - transform.position;
            float angleToObj = Vector2.Angle(orgDirRight,dir);
            // 가장 가까운 오브젝트를 찾음

            if (angleToObj <=90f && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPlayer = collider.transform;

            }
        }

        // 가장 가까운 오브젝트를 향해 회전
        if (nearestPlayer != null)
        {
            Vector2 direction = nearestPlayer.position + new Vector3(0,0.5f) - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _holder.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    private void PlayEffects()
    {
        _shellParticle.Play();
    }

    private  void SteamOn(){
        IsAnimationPlaying("Steam");
    }
    
    private void IsAnimationPlaying(string animationName)
    {
        AnimatorStateInfo stateInfo1 = _topSteam.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo stateInfo2 = _bottomSteam.GetCurrentAnimatorStateInfo(0);

        if(!stateInfo1.IsName(animationName)){
            _topSteam.Play(animationName);
        }
        if(!stateInfo2.IsName(animationName)){
            _bottomSteam.Play(animationName);
        }

    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        // Gizmos.DrawWireSphere(transform.position, radius);

        // 반원 그리기
        int segments = 20; // 반원을 그릴 세그먼트 수 (조절 가능)
        float angleStep = 180f / segments; // 각 세그먼트 간의 각도 차이

        Vector3 startPoint = transform.position + Quaternion.Euler(0, 0, -90) * orgDirRight * radius;
        Vector3 previousPoint = startPoint;

        for (int i = 1; i <= segments; i++)
        {
            float angle = -90 + i * angleStep;
            Vector3 nextPoint = transform.position + Quaternion.Euler(0, 0, angle) * orgDirRight * radius;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }

        // transform의 오른쪽 방향을 나타내는 선
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)orgDirRight * radius);
    }
#endif
}
