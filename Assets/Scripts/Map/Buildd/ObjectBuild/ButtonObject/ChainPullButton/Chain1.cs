
using UnityEngine;
using UnityEngine.Animations;

public class Chain1 : MonoBehaviour
{
    [SerializeField] private ChainPullButton_Net _net;

    private LineRenderer _line;
    private LineRenderer Line { get { _line ??= GetComponent<LineRenderer>(); return _line; } }

    public Transform _start; // 고정점
    public Transform _end;   // 중력 영향을 받는 끝점

    [Header("End Physics")]
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private float _fallAccelerationMultiplier = 3f;
    [SerializeField] private float _maxFallSpeed = 30f;
    [SerializeField] private float _max_length => Get_Max_Length();
    private float Get_Max_Length()
    {
        if(CCC.rl)//true : r, false : l
            return _net._s_r_cur_chain_length;
            else return _net._s_l_cur_chain_length;
    }
    // private float _min_length = 0.5f;
    [ReadOnly]
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

    private ChainPullButton_chack_collider _ccc;
    public ChainPullButton_chack_collider CCC {get{_ccc??= _end.GetComponent<ChainPullButton_chack_collider>(); return _ccc;}}

    private ParentConstraint _parentConstraint;
    public ParentConstraint ParentConstraint { get { _parentConstraint ??= _end.GetComponent<ParentConstraint>(); return _parentConstraint; } }

    public void DeletConstraint()
    {
        if(ParentConstraint.sourceCount > 0)
        {
            ParentConstraint.weight = 0;
            ParentConstraint.constraintActive = false;
            ParentConstraint.locked = false;
            ParentConstraint.RemoveSource(0);
        }

        Rb.gravityScale = 10;

    }

    void LateUpdate()
    {
        Update_EndPhysics();
        Update_Line();
    }

    private void Update_EndPhysics()
    {
        if (_start == null || _end == null)
            return;

        Vector2 startPos = _start.position;
        Vector2 endPos = _end.position;
        Vector2 startToEnd = endPos - startPos;
        if(startToEnd.sqrMagnitude < 0.0001f)
        {
            _end.localPosition = Vector3.zero;
            _endVelocity = Vector2.zero;
            return;
        }
            
        float distance = startToEnd.magnitude;

        if (distance > _max_length)
        {
            CCC.RemoveVelocity();

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

        UpdateChainSoundPitch(distance);

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

#region Sound
    private float _prev_distance = -1f; // -1은 "아직 초기화 안됨" 표시용
    private AudioSourceController _chainAudio;
    private float _minPitch = 0.5f;
    private float _maxPitch = 3.0f;
    private float _deltaToMaxPitch = 5f; 
    // [SerializeField] private float _pitchLerpSpeed = 10f;
    private float _sensitivity = 1f;
    private float _tolerance = 0.01f;

    private void UpdateChainSoundPitch(float currentDistance)
    {
        if (_prev_distance < 0f)
        {
            _prev_distance = currentDistance;
            return;
        }

        if(Mathf.Abs(_prev_distance - currentDistance) < _tolerance)
        {
            ReturnSound();
            return;
        } 
            
        //정규화 
        float deltaPerSecond = Mathf.Abs(currentDistance - _prev_distance) / Time.deltaTime;

        // 0~1 사이로 정규화
        float t = Mathf.Clamp01(deltaPerSecond / _deltaToMaxPitch);
        t = Mathf.Pow(t, 1f / _sensitivity);


        float targetPitch = Mathf.Lerp(_minPitch, _maxPitch, t);
        Debug.Log($"Target Pitch: {targetPitch}, prev_distance: {_prev_distance}, currentDistance: {currentDistance}, deltaPerSecond: {deltaPerSecond}, t: {t}");
        PlaySound(targetPitch);
        

        if (_chainAudio != null)
        {
            // _chainAudio.pitch = Mathf.Lerp(_chainAudio.pitch, targetPitch, Time.deltaTime * _pitchLerpSpeed);
        }

        _prev_distance = currentDistance;
    } 
    private void ReturnSound()
    {
        if(_chainAudio != null)
        {
            Managers.Sound.StopSound(_chainAudio);
            _chainAudio = null;
        }
    }
    private void PlaySound(float pitch)
    {
        if(_chainAudio == null)
        {
            _chainAudio = Managers.Sound.PlaySound3D(GlobalText.CHAIN_DRAGGING, _net.transform.position, 1,true);
        }
        _chainAudio.GetAudioSource().pitch = pitch;
        
    }

#endregion

    #region Utility
    
    public float Get_StartEndDistance()
    {
        if (_start == null || _end == null) return 0f; 
        
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


    public void Reset()
    {
        Rigidbody2D Rb = _end.GetComponent<Rigidbody2D>();
        Rb.velocity = Vector2.zero;
        Rb.angularVelocity = 0f;
        Rb.gravityScale = 0;
        _end.localRotation = Quaternion.identity;
        _end.localPosition = Vector3.zero;
        _endVelocity = Vector2.zero;
    }
#endregion



#region Debug

#endregion
}
