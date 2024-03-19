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


    //감지거리
    [SerializeField] public float detectionDistance = 3f;
    private LayerMask _objectMask;  //이것은 열쇠등등 오브젝트
    private LayerMask _hookMask;  //이걸 후크레이어로 설정해둬야할듯
    private LayerMask _collisionLayerMask;  //두개 레이어 
    private float _shortestDistance;

    //가장 가까운 객체
    private Collider2D _closestTarget;
    private Collider2D _latestTarget;


    private void Awake()
    {
        _camera = Camera.main;
        _closestTarget = null;
        _shortestDistance = float.MaxValue;
        _objectMask = LayerMask.GetMask("Object");
        _hookMask = LayerMask.GetMask("Hook");
        _collisionLayerMask = _objectMask | _hookMask;
    }

    public PlayerInput playerInput { get; private set; }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        playerInput.playerActions.Look.performed += Look;
        playerInput.playerActions.Action.performed += PlayerActionPerformed;
        playerInput.playerActions.Action.canceled += PlayerActionCanceled;
    }

    private void FixedUpdate()
    {
        RotateArm();
        if (_isClick)
        {
            ObjectCheck();
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


        _armRenderer.flipY = Mathf.Abs(rotZ) > 90f;
        _characterRenderer.flipX = _armRenderer.flipY;
        _weaponSprite.flipX = _armRenderer.flipY;

        _armPivot.rotation = Quaternion.AngleAxis(rotZ, Vector3.forward);
    }

    private void ObjectCheck()
    {
        var collisions = Physics2D.OverlapCircleAll(_weaponPoint.position, detectionDistance, _collisionLayerMask);

        if(collisions.Length == 0)
        {
            Debug.Log("없다");
        }

        //현재 범위안에 object or Hook레이어를 가진 물체가있으면 거리를 비교하고 가까운녀석만 가져옴
        else
        {
            //최단거리 물체 가져오기
            for(var i = 0; i < collisions.Length; i++)
            {
                var targetDistance = Vector2.Distance(_weaponPoint.position, collisions[i].transform.position);
                if (targetDistance < _shortestDistance)
                {
                    _shortestDistance = targetDistance;
                    _closestTarget = collisions[i];
                }
            }
            if(_latestTarget != null)
            {
                //현재 가까운 타겟과 closestTarget이 같으면 초기화
                if(ReferenceEquals(_latestTarget, _closestTarget))
                {
                    _shortestDistance = float.MaxValue;
                }
            }
            _latestTarget = _closestTarget;
            _shortestDistance = float.MaxValue;

            // 오브젝트벡터 - 총구위치벡터
            Vector2 objectVector = (_closestTarget.transform.position - _weaponPoint.position).normalized;
            Vector2 weaponVector = _weaponPoint.transform.right;

            // 인식되도록하는 각도
            float angle = Vector2.Angle(weaponVector, objectVector);
            // 범위안에 인식가능한 물체가 있으면
            if (angle < 40)
            {
                
            }
        }
    }

    // Gizmos로 OverlapCircle범위 확인
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_weaponPoint.position, detectionDistance);
    }
}
