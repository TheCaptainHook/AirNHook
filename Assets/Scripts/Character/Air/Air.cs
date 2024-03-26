using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Air : MonoBehaviour
{
    //에어 무기 회전
    [SerializeField] private SpriteRenderer _armRenderer;
    [SerializeField] private Transform _armPivot;
    [SerializeField] private SpriteRenderer _characterRenderer;
    [SerializeField] private SpriteRenderer _weaponSprite;

    //총구위치
    [SerializeField] private Transform _weaponPoint;

    //마우스가 움직인 값
    private Vector2 _mouseDelta;
    private Camera _camera;

    //마우스 클릭체크
    private bool _isRightButtonClick;
    private bool _isLeftButtonClick;

    //흡입액션 bool값
    private bool _isAttached;

    //흡입가능한지 아닌지 체크하는 bool값
    private bool _isInhale = true;

    //발사액션 후 흡입액션 쿨타임
    private float _coolDown;

    //private float _coolDownCount = 0f;

    //감지거리
    [SerializeField] public float detectionDistance = 3f;
    [SerializeField] private LayerMask _objectMask;  //이것은 열쇠등등 오브젝트
    [SerializeField] private LayerMask _hookMask;  //이걸 후크레이어로 설정해둬야할듯
    private LayerMask _collisionLayerMask;  //두개 레이어 
    private float _shortestDistance;

    //가장 가까운 객체
    private Collider2D _closestTarget;
    private Collider2D _latestTarget;

    //흡입한 오브젝트 날리는 파워
    [SerializeField] private float _minShootPower = 5f;
    [SerializeField] private float _maxShootPower = 20f;

    [SerializeField] private float _shootPower;

    //차징시간체크
    private float _chargingTime;

    //코루틴 변수선언
    private Coroutine _chargingCoroutine;

    //땅체크
    [SerializeField] private LayerMask _floorLayer;

    //유도선
    [SerializeField] private GameObject _point;
    [SerializeField] private GameObject[] _points;
    //위의 두코드는 삭제해야함
    [SerializeField] private int _numberOfPoints;
    [SerializeField] private float _spaceBetweenPoints;
    [SerializeField] LineRenderer _lineRenderer;
    private GameObject _pointParent;

    private void Awake()
    {
        _camera = Camera.main;
        _closestTarget = null;
        _shortestDistance = float.MaxValue;
        _collisionLayerMask = _objectMask | _hookMask;
    }

    public PlayerInput playerInput { get; private set; }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        playerInput.playerActions.Look.performed += Look;
        playerInput.playerActions.Action.started += PlayerActionStarted;
        playerInput.playerActions.Action.canceled += PlayerActionCanceled;
        playerInput.playerActions.SubAction.started += PlayerSubActionStarted;
        playerInput.playerActions.SubAction.canceled += PlayerSubActionCanceled;

        _lineRenderer = GetComponent<LineRenderer>();

        _points = new GameObject[_numberOfPoints];
        _pointParent = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
        for (int i = 0;i <_numberOfPoints; i++)
        {
            _points[i] = Instantiate(_point, _pointParent.transform);
        }
        _pointParent.SetActive(false);
    }

    
    private void Update()
    {
        RotateArm();

        //오른쪽마우스클릭 했을때
        if (_isRightButtonClick)
        {
            //붙지 않았을때
            if (!_isAttached)
            {
                //쿨타임코루틴제작해서 감싸야함
                if (_isInhale == true)
                {
                    ObjectCheck();
                    SlerpTarget();
                }
            }
            //붙었을때
            else
            {
                Attached();
            }
        }

        if (_pointParent.activeSelf)
        {
            for (int i = 0; i < _numberOfPoints; i++)
            {
                _points[i].transform.position = PointPosition(i * _spaceBetweenPoints);
            }
        }
    }

    //발사 게이지차징
    private void PlayerActionStarted(InputAction.CallbackContext context)
    {
        //차징액션 함수 제작하여 넣기
        if (_isAttached)
        {
            _isLeftButtonClick = true;
            _pointParent.SetActive(true);
            Charging();
        }
    }

    //오브젝트 발사
    private void PlayerActionCanceled(InputAction.CallbackContext context)
    {
        if (_isLeftButtonClick)
        {
            _isLeftButtonClick = false;
            ShootObject();
            _pointParent.SetActive(false);
        }
    }

    //흡입액션 시작 (우클릭)
    private void PlayerSubActionStarted(InputAction.CallbackContext context)
    {
        _isRightButtonClick = true;
    }

    //흡입액션 취소
    private void PlayerSubActionCanceled(InputAction.CallbackContext context)
    {
        _isRightButtonClick = false;
        
        if(_isAttached == true)
        {
            Vector2 target = new Vector2(_latestTarget.transform.position.x, _latestTarget.transform.position.y);
        }
        if (_latestTarget != null)
        {
            _latestTarget.GetComponent<Rigidbody2D>().gravityScale = 3;
            _latestTarget.GetComponent<CircleCollider2D>().excludeLayers = 0;
            _latestTarget = null;
            _isAttached = false;
        }
    }

    //에어 시선
    public void Look(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }

    //에어 무기회전 및 방향전환
    private void RotateArm()
    {
        Vector2 worldPos = _camera.ScreenToWorldPoint(_mouseDelta);
        Vector2 newAim = worldPos - (Vector2)_armPivot.position;

        float rotZ = Mathf.Atan2(newAim.y, newAim.x) * Mathf.Rad2Deg;

        //총구 각도제한
        if(rotZ < -10f && rotZ > -90f)
        {
            rotZ = -10f;
        }
        else if(rotZ > -170f && rotZ <= -90f)
        {
            rotZ = -170f;
        }

        _armRenderer.flipY = Mathf.Abs(rotZ) > 90f;
        _characterRenderer.flipX = _armRenderer.flipY;
        _weaponSprite.flipX = _armRenderer.flipY;

        _armPivot.rotation = Quaternion.AngleAxis(rotZ, Vector3.forward);
    }

    //일정거리이내 오브젝트 체크하는 코드
    private void ObjectCheck()
    {
        var collisions = Physics2D.OverlapCircleAll(_weaponPoint.position, detectionDistance, _collisionLayerMask);

        //타겟 초기화
        if(collisions.Length == 0)
        {
            if (_latestTarget != null) 
            {
                _latestTarget.GetComponent<Rigidbody2D>().gravityScale = 3;
            }
            _latestTarget = null;
        }

        //현재 범위안에 object or Hook 레이어를 가진 물체가있으면 거리를 비교하고 가까운녀석만 가져옴
        else
        {
            _closestTarget = null;
            //최단거리 물체 가져오기
            for(var i = 0; i < collisions.Length; i++)
            {
                var targetDistance = Vector2.Distance(_weaponPoint.position, collisions[i].transform.position);

                if (targetDistance < _shortestDistance)
                {
                    if (targetDistance <= 0.15f)
                    {
                        _closestTarget = collisions[i];
                        _shortestDistance = targetDistance;
                    }
                    else
                    {
                        Vector2 objectVector = (collisions[i].transform.position - _weaponPoint.position).normalized;
                        Vector2 weaponVector = _weaponPoint.transform.right;

                        float angle = Vector2.Angle(weaponVector, objectVector);

                        if (angle < 40)
                        {
                            _closestTarget = collisions[i];
                            _shortestDistance = targetDistance;
                        }
                    }
                }
            }

            if(_latestTarget != null && _latestTarget != _closestTarget)
            {
                _latestTarget.GetComponent<Rigidbody2D>().gravityScale = 3;
                _latestTarget.GetComponent<CircleCollider2D>().excludeLayers = 0;
            }

            if (_closestTarget == null)
            {
                _latestTarget = null;
                return;
            }

            _latestTarget = _closestTarget;
            
            if (_latestTarget != null)
            {
                //현재 가까운 타겟과 closestTarget이 같으면 초기화
                if(ReferenceEquals(_latestTarget, _closestTarget))
                {
                    _shortestDistance = float.MaxValue;
                }
                _latestTarget.GetComponent<Rigidbody2D>().gravityScale = 0;
            }
            _shortestDistance = float.MaxValue;
        }
    }

    //오브젝트 끌고오는 함수
    private void SlerpTarget()
    {
        if (_latestTarget == null)
        {
            Debug.Log("_latestTarget Is Null");
        }
        else
        {
            Vector2 target = new Vector2(_latestTarget.transform.position.x, _latestTarget.transform.position.y);

            //오브젝트의 중력소실되게해서 끌어와야함
            _latestTarget.GetComponent<Rigidbody2D>().gravityScale = 0;
            _latestTarget.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            //Slerp = (현재위치, 목표, 속도) / 이곳에 베지어곡선코드를 넣어야한다.
            _latestTarget.transform.position = Vector3.Slerp(target, _weaponPoint.position, 0.05f);

            //_latestTarget의 트리거체크를 켠다.(충돌방지)
            //_latestTarget.GetComponent<CircleCollider2D>().isTrigger = true;
            // 이걸 ture곳에 넣어주고 false인곳은 0을 넣어주면된다.
            _latestTarget.GetComponent<CircleCollider2D>().excludeLayers = (1 << gameObject.layer);

            //두 오브젝트의 위치가 0.1보다 가깝다면
            if (Vector2.Distance(_latestTarget.transform.position,_weaponPoint.position) <= 0.1)
            {
                if ((_objectMask & (1 << _latestTarget.transform.gameObject.layer)) != 0)
                {
                    Debug.Log("0.1보다 가까워요");
                    //오브젝트위치를 총구위치에 고정
                    _latestTarget.transform.position = _weaponPoint.position;
                    _latestTarget.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

                    //흡입액션이 끝나도록 ObjectCheck()에 bool값 한개 주기
                    //SlerpTarget()을 멈추면서 오브젝트의 위치를 총구에 고정시킴
                    _isAttached = true;
                }
            }
        }
    }

    //붙었을때 코드
    private void Attached()
    {
        if (_latestTarget == null) 
        {
            _isAttached = false;
            return;
        }
        //_latestTarget의 트리거체크를 켠다.(충돌방지)
        _latestTarget.GetComponent<CircleCollider2D>().excludeLayers = (1 << gameObject.layer);

        //오브젝트위치를 총구위치에 고정
        _latestTarget.transform.position = _weaponPoint.position;
        _latestTarget.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
    }

    //오브젝트 발사하는코드
    private void ShootObject()
    {
        if (_chargingCoroutine != null)
        {
            StopCoroutine(_chargingCoroutine);
            Debug.Log(_shootPower);
            _chargingCoroutine = null;
        }
        StartCoroutine(Co_CoolDown());

        //붙어있는 상태 해제
        _isAttached = false;

        //발사하면 트리거와 중력작용 둘다 켜야함
        _latestTarget.GetComponent<CircleCollider2D>().excludeLayers = 0;
        _latestTarget.GetComponent<Rigidbody2D>().gravityScale = 3;

        //임시코드
        //마우스 방향(rotZ) 을 벡터로 받아서 날려줘야함.
        _latestTarget.GetComponent<Rigidbody2D>().AddForce(_weaponPoint.right * _shootPower, ForceMode2D.Impulse);
        Debug.Log("_shootPower의 힘은 = " + _shootPower);
        //발사하면 _latestTarget값을 잃는다.
        _latestTarget = null;
    }

    //차징하는 코드
    private void Charging()
    {
        _shootPower = _minShootPower;
        _chargingCoroutine = StartCoroutine(Co_PowerCharging());
        //코루틴 사용 / 코루틴을 변수로 선언하고 이곳에서 코루틴 시작 / 마우스 캔슬드되면 코루틴 정지
    }

    //차징파워 올려주는 코루틴
    private IEnumerator Co_PowerCharging()
    {
        _chargingTime = 0f;
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (_chargingTime <= 1f)
        {
            _chargingTime += Time.fixedDeltaTime;
            Debug.Log(_chargingTime);

            _shootPower = (_chargingTime * (_maxShootPower - _minShootPower)) + _minShootPower;
            yield return waitForFixedUpdate;
        }
        _shootPower = _maxShootPower;
    }

    //쿨타임 코루틴
    private IEnumerator Co_CoolDown()
    {
        _isInhale = false;

        yield return new WaitForSeconds(1.5f);

        _isInhale = true;
    }

    //포물선 그려주는 코드
    //호출된 점 위치에 대한 벡터를 반환하는 함수 / 위치잡는 코드
    private Vector2 PointPosition(float t)
    {
        //핵심코드들
        Vector2 worldPos = _camera.ScreenToWorldPoint(_mouseDelta);
        Vector2 newAim = worldPos - (Vector2)_armPivot.position;
    
        Vector2 position = (Vector2)_weaponPoint.position + (newAim.normalized * _shootPower * t) + (0.5f * Physics2D.gravity * (t * t) * 3);
        
        return position;
    }

    // Gizmos로 OverlapCircle범위 확인
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(_weaponPoint.position, detectionDistance);
    }
}
