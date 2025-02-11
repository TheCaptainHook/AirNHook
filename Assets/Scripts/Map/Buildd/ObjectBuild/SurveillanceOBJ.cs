using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveillanceOBJ : MonoBehaviour
{
    [Header("Detection Settings")]
    public LayerMask layerMask; // 플레이어 감지 레이어
    public LayerMask obstacleLayerMask; // 장애물 감지 레이어
    public float radius = 10f; // 감시 범위 반지름

    [Header("Eye Movement Settings")]
    public float moveSpeed = 2.0f; // _eye 이동 속도
    [SerializeField] private GameObject _eye;
    [SerializeField] private float eyeMoveLimit = 0.15f; // _eye가 이동할 수 있는 최대 거리

    [Header("Random Movement Settings")]
    [SerializeField] private float wonderingSpeed = 0.2f; // 랜덤 이동 속도
    [SerializeField] private float wonderingChangeTime = 2f; // 랜덤 이동 방향 변경 시간

    [Header("Rotation Settings")]
    [SerializeField] private bool rotateEnabled = false; // 회전 기능 활성화 여부 (토글)

    private Vector3 wonderTarget; // 랜덤 이동 목표 지점
    private float wonderTimer;

    private void Start()
    {
        SetRandomWonderTarget();
    }

    private void Update()
    {
        MoveEye();
    }

    private void MoveEye()
    {
        Transform nearestPlayer = FindNearestVisiblePlayer();

        if (nearestPlayer != null)
        {
            // **감지된 플레이어 방향으로 이동**
            Vector2 direction = (nearestPlayer.position - transform.position).normalized;
            Vector3 targetPosition = transform.position + (Vector3)direction * eyeMoveLimit;
            _eye.transform.position = Vector3.Lerp(_eye.transform.position, targetPosition, Time.deltaTime * moveSpeed);

            // **회전 기능이 활성화된 경우만 플레이어 방향으로 회전**
            if (rotateEnabled)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                _eye.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
        else
        {
            // **랜덤 이동 모드 (플레이어가 감지되지 않을 때)**
            WonderAround();
        }
    }

    private Transform FindNearestVisiblePlayer()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        Transform nearestPlayer = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            Vector2 playerPosition = collider.transform.position;
            float distance = Vector2.Distance(transform.position, playerPosition);

            // **Raycast로 장애물 확인 (obstacleLayerMask에 포함된 경우만 장애물로 인식)**
            RaycastHit2D hit = Physics2D.Raycast(transform.position, playerPosition - (Vector2)transform.position, distance, obstacleLayerMask);

            if (!hit.collider) // 장애물이 없는 경우
            {
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPlayer = collider.transform;
                }
            }
        }

        return nearestPlayer;
    }

    private void WonderAround()
    {
        wonderTimer -= Time.deltaTime;

        if (wonderTimer <= 0)
        {
            SetRandomWonderTarget();
        }

        // 목표 지점으로 이동
        _eye.transform.position = Vector3.Lerp(_eye.transform.position, wonderTarget, Time.deltaTime * wonderingSpeed);
    }

    private void SetRandomWonderTarget()
    {
        // **새로운 랜덤 목표 지점 설정 (반경 내에서)**
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        wonderTarget = transform.position + (Vector3)(randomDirection * eyeMoveLimit);

        // **다음 변경 시간 초기화**
        wonderTimer = wonderingChangeTime;
    }

    // **Gizmo를 사용해 감시 범위를 에디터에서 시각화**
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
