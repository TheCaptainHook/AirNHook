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
    private bool _isClick;

    //흡입액션 종료 bool값
    private bool _isAttached;

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
    private float _shootPower = 10f;


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
        playerInput.playerActions.Action.performed += PlayerActionPerformed;
        playerInput.playerActions.Action.canceled += PlayerActionCanceled;
        playerInput.playerActions.SubAction.performed += PlayerSubActionPerformed;
        playerInput.playerActions.SubAction.canceled += PlayerSubActionCanceled;
    }

    private void Update()
    {
        RotateArm();
        //클릭 했을때
        if (_isClick)
        {
            //붙지 않았을때
            if (!_isAttached)
            {
                ObjectCheck();
                SlerpTarget();
            }
            ////붙었을때
            else
            {
                Attached();
            }
        }
    }
    //에어 액션 시작
    private void PlayerActionPerformed(InputAction.CallbackContext context)
    {
        _isClick = true;
    }

    //에어 액션 끝
    private void PlayerActionCanceled(InputAction.CallbackContext context)
    {
        _isClick = false;

        if (_isAttached == true)
        {
            ShootObject();
        }
        else if (_latestTarget != null)
        {
            _latestTarget.GetComponent<Rigidbody2D>().gravityScale = 3;
            _latestTarget.GetComponent<CircleCollider2D>().isTrigger = false;
            _latestTarget = null;
        }
    }

    private void PlayerSubActionPerformed(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }

    private void PlayerSubActionCanceled(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
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
                _latestTarget.GetComponent<CircleCollider2D>().isTrigger = false;
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

            //두 오브젝트의 위치가 0.1보다 가깝다면
            if(Vector2.Distance(_latestTarget.transform.position,_weaponPoint.position) <= 0.1)
            {
                if ((_objectMask & (1 << _latestTarget.transform.gameObject.layer)) != 0)
                {
                    Debug.Log("0.1보다 가까워요");
                    //오브젝트위치를 총구위치에 고정
                    _latestTarget.transform.position = _weaponPoint.position;
                    _latestTarget.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

                    //_latestTarget의 트리거체크를 켠다.(충돌방지)
                    _latestTarget.GetComponent<CircleCollider2D>().isTrigger = true;

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
        _latestTarget.GetComponent<CircleCollider2D>().isTrigger = true;

        //오브젝트위치를 총구위치에 고정
        _latestTarget.transform.position = _weaponPoint.position;
        _latestTarget.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
    }

    //오브젝트 발사하는코드
    private void ShootObject()
    {
        //발사하면 트리거와 중력작용 둘다 켜야함
        _latestTarget.GetComponent<CircleCollider2D>().isTrigger = false;
        _latestTarget.GetComponent<Rigidbody2D>().gravityScale = 3;

        //임시코드
        //마우스 방향(rotZ) 을 벡터로 받아서 날려줘야함.
        _latestTarget.GetComponent<Rigidbody2D>().AddForce(_weaponPoint.right * _shootPower, ForceMode2D.Impulse);

        //발사하면 _latestTarget값을 잃는다.
        _latestTarget = null;
    }

    //포물선 그려주는 코드

    // Gizmos로 OverlapCircle범위 확인
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_weaponPoint.position, detectionDistance);
    }
}
