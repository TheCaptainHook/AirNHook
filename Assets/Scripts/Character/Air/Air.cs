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
    private float _airChargingTime;

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

    //에어의 중력값 저장변수
    private float _airGravityScale;
    //후크가 매달려있는지 체크
    private bool _isFlyAway;
    //1초 차징이 끝났는지 체크
    private bool _isOneSecond;
    //에어가 후크에게 붙었을때 RotateArm이 동작하지 않도록 하는 bool
    [SerializeField] private bool _isHookAttached;

    //라인렌더러
    LineRenderer lr;

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
        lr = GetComponent<LineRenderer>();

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
        if(!_isHookAttached)
            RotateArm();
        if(!_isOneSecond)
            ObjectCheck();
        //오른쪽마우스클릭 했을때
        if (_isRightButtonClick)
        {
            _isFlyAway = false;
            //_latestTarget에 Garppling이 있을때
            if (_latestTarget != null && _latestTarget.TryGetComponent(out Grappling grappling))
            {
                //_latestTarget에 Garppling이 있고 grappleAttached이 false일때
                if (!grappling.grappleAttached)
                {
                    ObjectAttached();
                    ShowRoutePoint();
                    return;
                }
                //_latestTarget에 Garppling이 있고 grappleAttached이 true일때
                else
                {
                    _isFlyAway = true;
                    AirCharging();
                }
            }
            //_latestTarget에 Garppling이 없을때
            else if(_latestTarget != null)
            {
                ObjectAttached();
                ShowRoutePoint();
            }
        }
    }

    //오브젝트가 붙었는지 붙지않았는지 체크하는 함수
    private void ObjectAttached()
    {
        //붙지 않았을때
        if (!_isAttached)
        {
            //쿨타임
            if (_isInhale && !_isFlyAway)
            {
                InhaleTarget();
            }
            //에어가 후크에게 날아가는 코드
            else if (_isInhale && _isFlyAway && _isOneSecond)
            {
                AirFlyAway();
                LookAt();
            }
        }
        //붙었을때
        else
        {
            Attached();
        }
    }
    //포물선 보여주는 함수
    private void ShowRoutePoint()
    {
        if (_isLeftButtonClick && !_isFlyAway)
        {
            for (int i = 0; i < _numberOfPoints; i++)
            {
                Debug.Log("1111");
                lr.SetPosition(i, PointPosition(i * _spaceBetweenPoints));
            }
        }
        else if(_isLeftButtonClick && _isFlyAway)
        {
            for(int i = 0; i < _numberOfPoints; i++)
            {
                Debug.Log("2222");
                lr.SetPosition(i, AirPointPosition(i * _spaceBetweenPoints));
            }
        }
    }

    //발사 게이지차징
    private void PlayerActionStarted(InputAction.CallbackContext context)
    {
        //차징액션 함수 제작하여 넣기
        if (_isAttached && !_isFlyAway)
        {
            _isLeftButtonClick = true;
            lr.enabled = true;
            Charging();
        }
        
        else if(_isAttached && _isFlyAway)
        {
            Debug.Log("후크에게붙은 상태에서 좌클릭!");
            _isLeftButtonClick = true;
            lr.enabled = true;
            Charging();
        }
    }

    //오브젝트 발사
    private void PlayerActionCanceled(InputAction.CallbackContext context)
    {
        if (_isRightButtonClick && !_isFlyAway)
        {
            _isLeftButtonClick = false;
            lr.enabled = false;
            _isFlyAway = false;
            ShootObject();
        }
        else if (_isRightButtonClick && _isFlyAway)
        {
            Debug.Log("후크에게 붙은 상태에서 좌클릭떼기");
            _isHookAttached = false;
            _isLeftButtonClick = false;
            lr.enabled = false;
            _isFlyAway = false;
            ShootAir();
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
        _isLeftButtonClick = false;
        lr.enabled = false;
        _isOneSecond = false;
        _isHookAttached = false;
        _airChargingTime = 0;

        if (_latestTarget != null && _isFlyAway)
        {
            Debug.Log("@@#@#@#@#@#");
            transform.GetComponent<Rigidbody2D>().gravityScale = _airGravityScale;
            _latestTarget = null;
            _isAttached = false;
        }

        if (_latestTarget != null && !_isFlyAway)
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
        if (_isAttached && _isFlyAway)
        {
            return;
        }
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
        if(_isAttached == true)
        {
            return;
        }
        //detectionDistance 안에있는 물체 찾기
        var collisions = Physics2D.OverlapCircleAll(_weaponPoint.position, detectionDistance, _objectMask);

        //타겟 초기화
        if(collisions.Length == 0)
        {
            if (_latestTarget != null) 
            {
                _latestTarget.GetComponent<Rigidbody2D>().gravityScale = _latestTargetGravityScale;
                transform.GetComponent<Rigidbody2D>().gravityScale = _airGravityScale;
            }
            _latestTarget = null;
            _isFlyAway = false;
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

                //현재 가까운 타겟과 closestTarget이 같으면 초기화
                if (ReferenceEquals(_latestTarget, _closestTarget))
                {
                    _shortestDistance = float.MaxValue;
                }
            }

            if (_latestTarget != null && _latestTarget != _closestTarget)
            {
                _latestTarget.GetComponent<Rigidbody2D>().gravityScale = _latestTargetGravityScale;
                transform.GetComponent<Rigidbody2D>().gravityScale = _airGravityScale;
                //_latestTarget.GetComponent<Collider2D>().excludeLayers = 0;
            }

            //if (_latestTarget != null)
            //{
            //    //현재 가까운 타겟과 closestTarget이 같으면 초기화
            //    if(ReferenceEquals(_latestTarget, _closestTarget))
            //    {
            //        _shortestDistance = float.MaxValue;
            //    }
            //}
            _shortestDistance = float.MaxValue;
        }
    }

    //오브젝트 끌고오는 함수
    private void InhaleTarget()
    {
        if (_latestTarget == null)
        {
            Debug.Log("_latestTarget Is Null");
        }

        //물체가 에어에게 빨려들어오는 코드
        else
        {
            Debug.Log("2");
            Vector2 target = new Vector2(_latestTarget.transform.position.x, _latestTarget.transform.position.y);

            //오브젝트의 중력소실되게해서 끌어와야함
            var latestTargetRB = _latestTarget.GetComponent<Rigidbody2D>();
            latestTargetRB.gravityScale = 0;
            latestTargetRB.velocity = new Vector2(0, 0);

            //흡입상태가 되면 플레이어와 충돌이 일어나지않도록
            //_latestTarget.GetComponent<Collider2D>().excludeLayers = (1 << gameObject.layer);

            //Slerp = (현재위치, 목표, 속도) / 이곳에 베지어곡선코드를 넣어야한다.
            _latestTarget.transform.position = Vector3.Slerp(target, _weaponPoint.position, 0.05f);

            //두 오브젝트의 위치가 0.1보다 가깝다면
            if (Vector2.Distance(_latestTarget.transform.position, _weaponPoint.position) <= 0.1)
            {
                if ((_objectMask & (1 << _latestTarget.transform.gameObject.layer)) != 0)
                {
                    Debug.Log("0.1보다 가까워요");
                    //오브젝트위치를 총구위치에 고정
                    _latestTarget.transform.position = _weaponPoint.position;
                    latestTargetRB.velocity = new Vector2(0, 0);

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

    //후크가 벽에 붙었을때 에어가 후크에게 날아가는 코드
    private void AirFlyAway()
    {
        Debug.Log("@##@@#");
        //에어의 중력크기 저장
        var gravityScale = transform.GetComponent<Rigidbody2D>().gravityScale;
        _airGravityScale = gravityScale;

        // air = 에어위치 저장 , target = 후크의 위치 저장
        Vector3 air = transform.position;
        Vector3 target = _latestTarget.transform.position;

        //에어의 중력소실되게해서 끌어와야함
        var airRB = transform.GetComponent<Rigidbody2D>();
        airRB.gravityScale = 0;
        airRB.velocity = new Vector3(0, 0);

        //총구위치 고정하는 코드
        //90f이것을 벡터값을 사용해서 하던가 <- 후크가 바라보는 벡터or 후크 위쪽벡터와 총구의 벡터를 사용해서
        //벡터간의 간격을 사용해서 로테이선돌리던가
        //or Lookat함수? 사용해서 제작
        //_armPivot.rotation = Quaternion.AngleAxis(90f, Vector3.forward);

        //Slerp = (현재위치, 목표 , 속도)
        transform.position = Vector3.Lerp(air, target, 0.03f);

        //이곳에서 overlapCircle을 써서 범위에 포착이되면 순간적으로 에어의 위치를 이동시켜 부착시키도록?
        //Vector2.Distance(_weaponPoint.position, target) <= 0.3 / IsAttached()
        //후크의 높이가 일정거리이상 됬을때만 흡입액션이 동작하도록?
        if (Vector2.Distance(_weaponPoint.position, target) <= 0.3)
        {
            //이곳에서 좀더 부드럽게 움직이도록?
            if (((1 << transform.gameObject.layer) & _objectMask) != 0)
            {
                //오브젝트위치를 총구위치에 고정
                //_weaponPoint.position = target;
                
                airRB.velocity = new Vector2(0, 0);
            
                //SlerpTarget()을 멈추면서 오브젝트의 위치를 총구에 고정시킴
                _isAttached = true;
            }
        }
        else
        {
            airRB.gravityScale = _airGravityScale;
        }

        //이 코드가 실행되었을땐 무조건 후크에게 붙어야함
        //1초동안 후크를 바라보면서 액션을 지속한다면 후크에게 날아가 붙는데 중간에 캔슬이 불가능하다.
    }

    //후크에게 날아가는동안 에어의 시선
    private void LookAt()
    {
        _isHookAttached = true;
        Vector2 target = _latestTarget.transform.position;
        Vector2 air = transform.position;
        Vector2 airWeaponPos = _weaponPoint.transform.position;

        Vector2 look = (target - airWeaponPos).normalized;
        Vector2 look2 = (target - air).normalized;

        float rotz = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg;
        float rotz2 = Mathf.Atan2(look2.y, look2.x) * Mathf.Rad2Deg;
        float distance = Vector2.Distance(target, airWeaponPos);

        //두물체의 거리체크하는 코드를 넣어야함
        if (distance <= 1f)
        {
            _armPivot.rotation = Quaternion.AngleAxis(rotz2, Vector3.forward);
        }
        else
        {
            _armPivot.rotation = Quaternion.AngleAxis(rotz, Vector3.forward);
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
        //에어가 후크에게 붙었을때의 코드를 따로 작성해야한다.
        else if (_isFlyAway)
        {
            Debug.Log("@@@@");
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            _armPivot.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
            _weaponPoint.position = _latestTarget.transform.position;
            transform.position = new Vector2(_latestTarget.transform.position.x, _latestTarget.transform.position.y - 1.5f);
            //bool변수를 줘서 true일땐 update의 rotatearm이 동작하지 않도록
            _isHookAttached = true;
        }
        else
        {
            Debug.Log("@#$@#$%!@#%!#$%@$^#$^");
            //오브젝트위치를 총구위치에 고정
            _latestTarget.transform.position = _weaponPoint.position;
            _latestTarget.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
    }

    //오브젝트 발사하는코드
    private void ShootObject()
    {
        Debug.Log("1");
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
        }
        //발사 코드
        _latestTarget.GetComponent<Rigidbody2D>().AddForce(_weaponPoint.right * _shootPower, ForceMode2D.Impulse);

        //발사하면 _latestTarget값을 잃는다.
        _latestTarget = null;
    }

    //에어가 후크에게 붙어있을때 에어를 발사하는 코드
    private void ShootAir()
    {
        Debug.Log("3");
        _isAttached = false;
        Vector2 worldPos = _camera.ScreenToWorldPoint(_mouseDelta);
        Vector2 airPos = new Vector2(transform.position.x, transform.position.y + 0.5f);
        var direction = (worldPos - airPos).normalized;

        if (_chargingCoroutine != null)
        {
            StopCoroutine(_chargingCoroutine);
            _chargingCoroutine = null;
        }
        StartCoroutine(Co_CoolDown());

        transform.GetComponent<Rigidbody2D>().gravityScale = _airGravityScale;

        //발사코드
        transform.GetComponent<Rigidbody2D>().AddForce(direction * _shootPower, ForceMode2D.Impulse);
        
        _latestTarget = null;
    }

    //차징하는 코드
    private void Charging()
    {
        _shootPower = _minShootPower;
        _chargingCoroutine = StartCoroutine(Co_PowerCharging());
    }

    //에어가 후크를 바라보고 차징하는 시간
    private void AirCharging()
    {
        if(_airChargingTime <= 1f)
        {
            _airChargingTime += Time.deltaTime;
        }
        else
        {
            _isOneSecond = true;
            ObjectAttached();
            ShowRoutePoint();
        }
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

    //에어가 후크에게 매달려있을때 포물선그려주는함수
    private Vector2 AirPointPosition(float t)
    {
        Vector2 worldPos = _camera.ScreenToWorldPoint(_mouseDelta);
        Vector2 newAim = worldPos - (Vector2)_armPivot.position;
        Vector2 airPos = new Vector2(transform.position.x, transform.position.y + 0.5f);

        Vector2 position = airPos + (newAim.normalized * _shootPower * t) + (0.5f * Physics2D.gravity * (t * t) * _airGravityScale);
        return position;
    }

    //후크에게 매달린 상태에선 총구위치가아닌 몸통방향에서 발사해야함!(새로운 벡터함수제작)

    //에어의 총구위치에 오브젝트가 왔는지 판단하는 bool함수
    private bool IsAttached()
    {
        return Physics2D.OverlapCircle(_weaponPoint.transform.position, 0.01f, _objectMask);
    }
    // Gizmos로 OverlapCircle범위 확인
    private void OnDrawGizmos()
    {
        Vector2 worldPos = _camera.ScreenToWorldPoint(_mouseDelta);
        Vector2 airPos = new Vector2(transform.position.x, transform.position.y + 0.5f);
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(_weaponPoint.position, detectionDistance);
        //Gizmos.DrawWireCube(_weaponPoint.position, new Vector2(1, 1));
        Gizmos.DrawWireSphere(_weaponPoint.transform.position, 0.01f);
        Gizmos.DrawLine(airPos, worldPos);
    }
}
