using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain1 : MonoBehaviour
{
    private LineRenderer _line;
    private LineRenderer Line { get { _line ??= GetComponent<LineRenderer>(); return _line; } }

    public Transform _start; // 고정점
    public Transform _end;   // 중력 영향을 받는 끝점

    [Header("End Physics")]
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private float _fallAccelerationMultiplier = 3f;
    [SerializeField] private float _maxFallSpeed = 30f;
    private float _max_length = 0;
    public float _cur_length;
    // [SerializeField] public float _endDamping = 0.995f;

    [Header("Line")]
    private int _minSegmentCount = 2;
    private int _maxSegmentCount = 60;
    [SerializeField] private float _segmentLength = 0.25f;
    [SerializeField] private float _sagPower = .5f;
    [SerializeField] private float _recover_ChainSpeed = 5f;
    private Vector2 _endVelocity;

    private Rigidbody2D _rb;
    public Rigidbody2D Rb { get { _rb ??= _end.GetComponent<Rigidbody2D>(); return _rb; } }


    void Update()
    {
        Update_EndPhysics();
        Update_Line();
    }


    private void Update_EndPhysics()
    {
        if (_start == null || _end == null)
            return;
//====================RB없는 경우 자체 중력처리
        // float deltaTime = Time.deltaTime;

        // float acceleration = _gravity * _fallAccelerationMultiplier;
        // _endVelocity += Vector2.down * acceleration * deltaTime;

        // if (_endVelocity.y < -_maxFallSpeed)
        //     _endVelocity.y = -_maxFallSpeed;

        // _endVelocity *= _endDamping;
        // _end.position += (Vector3)(_endVelocity * deltaTime);
//====================RB없는 경우 자체 중력처리

        Vector2 startPos = _start.position;
        Vector2 endPos = _end.position;
        Vector2 startToEnd = endPos - startPos;
        float distance = startToEnd.magnitude;

        if (distance > _max_length)
        {
            Vector2 limitedEndPos = startPos + startToEnd.normalized * _max_length;
            _end.position = limitedEndPos;

            // 줄 방향으로 더 멀어지려는 속도는 제거해서 덜 튀게
            Vector2 ropeDir = startToEnd.normalized;
            float velocityAlongRope = Vector2.Dot(_endVelocity, ropeDir);

            if (velocityAlongRope > 0f)
                _endVelocity -= ropeDir * velocityAlongRope;
        }
    }
    
    private void Update_Line()
    {
        if (_start == null || _end == null)
            return;

        float distance = Vector2.Distance(_start.position, _end.position);
        int count = GetSegmentCountByLength(distance);

        Line.positionCount = count;

        Vector3 startPos = _start.position;
        Vector3 endPos = _end.position;

        // start-end 직선거리 자체가 max_length보다 길면 처질 수 없음 
        if (distance >= _max_length)
        {
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)(count - 1);
                Line.SetPosition(i, Vector3.Lerp(startPos, endPos, t));
            }

            return;
        } 

        _cur_length = Mathf.MoveTowards(_cur_length, distance, Time.deltaTime);

        float remainLength = Mathf.Max(0f, _cur_length - distance); 
        float sagAmount = remainLength * _sagPower;

        // 실제 라인 길이가 max_length를 넘지 않도록 sagAmount 조절
        sagAmount = FindValidSagAmount(startPos, endPos, sagAmount, count);

        for (int i = 0; i < count; i++)
        {
            float t = i / (float)(count - 1);

            Vector3 point = Vector3.Lerp(startPos, endPos, t);

            float sagCurve = Mathf.Sin(t * Mathf.PI);
            point += Vector3.down * sagAmount * sagCurve;

            Line.SetPosition(i, point);
        }
    }


    #region Utility
    public void SetMaxLength(float length)
    {
        _max_length = length;
    }
    public float Get_StartEndDistance()
    {
        if (_start == null || _end == null)
            return 0f;

        return Vector2.Distance(_start.position, _end.position);
    }
    private int GetSegmentCountByLength(float length)
    {
        float safeSegmentLength = Mathf.Max(0.01f, _segmentLength);
        int count = Mathf.CeilToInt(length / safeSegmentLength) + 1;
        return Mathf.Clamp(count, _minSegmentCount, _maxSegmentCount);
    }

    private float FindValidSagAmount(Vector3 startPos, Vector3 endPos, float targetSag, int count)
    {
        float low = 0f;
        float high = targetSag;

        for (int i = 0; i < 10; i++)
        {
            float mid = (low + high) * 0.5f;
            float length = CalculateLineLength(startPos, endPos, mid, count);

            if (length > _max_length)
                high = mid;
            else
                low = mid;
        }

        return low;
    }

    private float CalculateLineLength(Vector3 startPos, Vector3 endPos, float sagAmount, int count)
    {
        float length = 0f;
        Vector3 prev = startPos;

        for (int i = 1; i < count; i++)
        {
            float t = i / (float)(count - 1);

            Vector3 point = Vector3.Lerp(startPos, endPos, t);

            float sagCurve = Mathf.Sin(t * Mathf.PI);
            point += Vector3.down * sagAmount * sagCurve;

            length += Vector3.Distance(prev, point);
            prev = point;
        }

        return length;
    }
#endregion

}
