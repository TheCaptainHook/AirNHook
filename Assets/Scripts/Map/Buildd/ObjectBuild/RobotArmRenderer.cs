using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArmRenderer : MonoBehaviour
{
    [SerializeField] private Transform parentTransform; // 몸통 (기준점)
    [SerializeField] private LineRenderer lineRenderer; // 라인 렌더러
    //[SerializeField] private SpringJoint2D springJoint; // 스프링 조인트

    [SerializeField] private bool enableSagging = false; // 처짐 효과 (Gravity Effect)
    [SerializeField] private float sagAmount = 0.3f; // 처짐 정도 (값이 클수록 더 처짐)

    private Vector3[] linePositions = new Vector3[7]; // 7개 세그먼트 고정

    private void Awake()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        /*if (springJoint == null) springJoint = GetComponent<SpringJoint2D>();

        if (springJoint != null)
        {
            springJoint.connectedBody = parentTransform.GetComponent<Rigidbody2D>();
        }*/
    }

    private void LateUpdate()
    {
        if (parentTransform == null || lineRenderer == null) return;

        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        Vector3 startPos = parentTransform.position;
        Vector3 endPos = transform.position;

        // 🌟 세그먼트 설정 (7개 고정)
        linePositions[0] = startPos;
        linePositions[6] = endPos;

        Vector3 midPoint = (startPos + endPos) * 0.5f;

        if (enableSagging)
        {
            // 중간(3번) 위치를 가장 낮은 지점으로 설정
            linePositions[3] = midPoint + Vector3.down * sagAmount;

            // 1번, 2번을 0번과 3번 사이에 배치
            linePositions[1] = Vector3.Lerp(linePositions[0], linePositions[3], 0.28f);
            linePositions[2] = Vector3.Lerp(linePositions[0], linePositions[3], 0.66f);

            // 4번, 5번을 3번과 6번 사이에 배치
            linePositions[4] = Vector3.Lerp(linePositions[3], linePositions[6], 0.28f);
            linePositions[5] = Vector3.Lerp(linePositions[3], linePositions[6], 0.66f);
        }
        else
        {
            // 처짐이 없을 경우 직선 형태로 설정
            for (int i = 1; i < 6; i++)
            {
                linePositions[i] = Vector3.Lerp(startPos, endPos, i / 6f);
            }
        }

        // 라인 렌더러 적용
        lineRenderer.positionCount = 7;
        lineRenderer.SetPositions(linePositions);
    }
}