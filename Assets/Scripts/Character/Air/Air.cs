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

    //감지거리
    [SerializeField] public float detectionDistance = 3f;
    //이것은 열쇠등등 오브젝트
    [SerializeField] private LayerMask _objectMask;
    //오브젝트, 플레이어 제외한 레이어
    [SerializeField] private LayerMask _obstacleMask;
    private float _shortestDistance;

    //가장 가까운 객체
    private Collider2D _closestTarget;
    private Collider2D _latestTarget;

    //총구와 타겟사이에 땅이있는지 체크
    private bool _isBetween;

    //타겟의 중력을 저장하는변수
    private float _latestTargetGravityScale;

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
    [SerializeField] private int _numberOfPoints;
    [SerializeField] private float _spaceBetweenPoints;
    private GameObject _pointParent;

    private void Awake()
    {
        _camera = Camera.main;
        _closestTarget = null;
        _shortestDistance = float.MaxValue;
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

        _points = new GameObject[_numberOfPoints];
        _pointParent = new GameObject();
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
        //발사하기전 우클릭을 해제했을경우
        _isLeftButtonClick = false;
        _pointParent.SetActive(false);

        if (_isAttached == true)
        {
            Vector2 target = new Vector2(_latestTarget.transform.position.x, _latestTarget.transform.position.y);
        }
        if (_latestTarget != null)
        {
            _latestTarget.GetComponent<Rigidbody2D>().gravityScale = _latestTargetGravityScale;
            //_latestTarget.GetComponent<Collider2D>().excludeLayers = 0;
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
        //detectionDistance 안에있는 물체 찾기
        var collisions = Physics2D.OverlapCircleAll(_weaponPoint.position, detectionDistance, _objectMask);

        //타겟 초기화
        if(collisions.Length == 0)
        {
            if (_latestTarget != null) 
            {
                _latestTarget.GetComponent<Rigidbody2D>().gravityScale = _latestTargetGravityScale;
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
                            //오브젝트와 플레이어 사이에 벽이 존재할경우 끌고오지 못함
                            RaycastHit2D hit = Physics2D.Raycast(_weaponPoint.position, collisions[i].transform.position - _weaponPoint.position, targetDistance);

                            if(ReferenceEquals(hit.collider, collisions[i]))
                            {
                                _closestTarget = collisions[i];
                                _shortestDistance = targetDistance;
                            }
                        }
                    }
                }
            }

            if (_closestTarget == null)
            {
                _latestTarget = null;
                return;
            }

            _latestTarget = _closestTarget;

            if (_latestTarget != null)
            {
                var gravityScale = _latestTarget.GetComponent<Rigidbody2D>().gravityScale;
                if (gravityScale > 0)
                {
                    _latestTargetGravityScale = gravityScale;
                }
            }

            if (_latestTarget != null && _latestTarget != _closestTarget)
            {
                _latestTarget.GetComponent<Rigidbody2D>().gravityScale = _latestTargetGravityScale;
                //_latestTarget.GetComponent<Collider2D>().excludeLayers = 0;
            }

            if (_latestTarget != null)
            {
                //현재 가까운 타겟과 closestTarget이 같으면 초기화
                if(ReferenceEquals(_latestTarget, _closestTarget))
                {
                    _shortestDistance = float.MaxValue;
                }
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
            var latestTargetRB = _latestTarget.GetComponent<Rigidbody2D>();
            latestTargetRB.gravityScale = 0;
            latestTargetRB.velocity = new Vector2(0, 0);

            //흡입상태가 되면 플레이어와 충돌이 일어나지않도록
            //_latestTarget.GetComponent<Collider2D>().excludeLayers = (1 << gameObject.layer);

            //Slerp = (현재위치, 목표, 속도) / 이곳에 베지어곡선코드를 넣어야한다.
            _latestTarget.transform.position = Vector3.Slerp(target, _weaponPoint.position, 0.04f);

            //두 오브젝트의 위치가 0.1보다 가깝다면
            if (Vector2.Distance(_latestTarget.transform.position,_weaponPoint.position) <= 0.1)
            {
                if ((_objectMask & (1 << _latestTarget.transform.gameObject.layer)) != 0)
                {
                    Debug.Log("0.1보다 가까워요");
                    //오브젝트위치를 총구위치에 고정
                    _latestTarget.transform.position = _weaponPoint.position;
                    _latestTarget.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

                    //SlerpTarget()을 멈추면서 오브젝트의 위치를 총구에 고정시킴
                    _isAttached = true;
                }
            }
            else
            {
                _latestTarget.GetComponent<Rigidbody2D>().gravityScale = _latestTargetGravityScale;
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
        //_latestTarget.GetComponent<Collider2D>().excludeLayers = (1 << gameObject.layer);

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
            _chargingCoroutine = null;
        }
        StartCoroutine(Co_CoolDown());

        //붙어있는 상태 해제
        _isAttached = false;

        //발사하면 트리거와 중력작용 둘다 켜야함
        //_latestTarget.GetComponent<Collider2D>().excludeLayers = 0;
        _latestTarget.GetComponent<Rigidbody2D>().gravityScale = _latestTargetGravityScale;

        //날리는코드 / 발사할때 총구앞에 벽이있으면 총구위치가 아닌 플레이어 몸에서 발사가 되도록
        //오브젝트가 총구에 붙어있을때 OverlapBo체크되지않도록 해야함.
        //_latestTarget.GetComponent<Collider2D>().excludeLayers = (1 << gameObject.layer);
        Collider2D hit = Physics2D.OverlapBox(_weaponPoint.position, new Vector2(0.9f,0.9f), 0, _obstacleMask);
        if (hit != null)
        {
            _latestTarget.transform.position = _armPivot.transform.position;
            Debug.Log(hit.transform.name);
        }
        //발사 코드
        _latestTarget.GetComponent<Rigidbody2D>().AddForce(_weaponPoint.right * _shootPower, ForceMode2D.Impulse);

        //발사하면 _latestTarget값을 잃는다.
        _latestTarget = null;
    }

    //차징하는 코드
    private void Charging()
    {
        _shootPower = _minShootPower;
        _chargingCoroutine = StartCoroutine(Co_PowerCharging());
    }

    //차징파워 올려주는 코루틴
    private IEnumerator Co_PowerCharging()
    {
        _chargingTime = 0f;
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (_chargingTime <= 1f)
        {
            _chargingTime += Time.fixedDeltaTime;

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
        Vector2 worldPos = _camera.ScreenToWorldPoint(_mouseDelta);
        Vector2 newAim = worldPos - (Vector2)_armPivot.position;
    
        Vector2 position = (Vector2)_weaponPoint.position + (newAim.normalized * _shootPower * t) + (0.5f * Physics2D.gravity * (t * t) * _latestTargetGravityScale);
        
        return position;
    }

    // Gizmos로 OverlapCircle범위 확인
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(_weaponPoint.position, detectionDistance);
        Gizmos.DrawWireCube(_weaponPoint.position, new Vector2(1, 1));
    }
}
