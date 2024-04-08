using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : BuildObj
{
    Pooling pool;

    RaycastHit2D hit;

    public LayerMask layerMask;
    private bool isShot;
    public float cooltime;
    private float curtime;
    
    private Animator _animator;
    public float radius = 10f; // 스피어 캐스트의 반지름
    
    
    #region StringCache
    private static readonly int IsFiring = Animator.StringToHash("IsFiring");
    private static readonly int FireEnd = Animator.StringToHash("FireEnd");
    #endregion
  
    private void Start()
    {
        pool = GetComponent<Pooling>();
        pool.CreatePoolItem(MapEditor.Instance.poolingContainer);
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (isShot)
        {
            curtime -= Time.deltaTime;
            if(curtime <= 0)
            {
                isShot = false;
                _animator.SetTrigger(FireEnd);
            }
        }
        RotateTrap();
    }


    private void FixedUpdate()
    {
        hit = Physics2D.Raycast(transform.position, transform.right, 100f, layerMask);

        if (hit)
        {
            if (!isShot)
            {
                _animator.SetTrigger(IsFiring);
                isShot = true;
                curtime = cooltime;


                float z = Mathf.Atan2(transform.right.y, transform.right.x) + Random.Range(-5,5);
                GameObject obj = pool.GetPoolItem("Arrow");
                obj.transform.right = transform.right;
                obj.transform.Rotate(transform.forward * z);
                obj.GetComponent<Projectile_Arrow>().Reset();
                obj.transform.position = transform.position;
                obj.SetActive(true);
                pool.Destroy(obj, 10f);
            }
          
        }
    }

    // private void RotateTrap()
    // {
    //     Vector3 targ = 
    //     targ.z = 0f;
    //
    //     Vector3 objectPos = transform.position;
    //     targ.x = targ.x - objectPos.x;
    //     targ.y = targ.y - objectPos.y;
    //
    //     float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
    //     transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    // }

    private void RotateTrap()
    {
        // 스피어 캐스트를 통해 주변에 있는 플레이어 레이어를 가진 모든 오브젝트 탐지
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        Transform nearestPlayer = null;
        float nearestDistance = Mathf.Infinity;

        // 주변에 있는 모든 플레이어 레이어를 가진 오브젝트에 대해 반복
        foreach (Collider2D collider in colliders)
        {
            // 현재 탐지된 오브젝트와의 거리 계산
            float distance = Vector2.Distance(transform.position, collider.transform.position);

            // 가장 가까운 오브젝트를 찾음
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPlayer = collider.transform;
            }
        }

        // 가장 가까운 오브젝트를 향해 회전
        if (nearestPlayer != null)
        {
            Vector2 direction = nearestPlayer.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // 스피어 캐스트를 그리기 위해 씬 상에 범위를 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
