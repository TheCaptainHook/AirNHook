using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grappling : NetworkBehaviour
{
    [Header("Player")]
    public HookMovement playerMovement;
    public DistanceJoint2D distanceJoint;
    private Vector2 _playerPosition;
    public float climbSpeed = 3f;
    public float swingJumpPower;

    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;
    private Vector2 _mousePosition;
    private Vector2 _aimDirection;
    private float _vertical;
    
    [SyncVar]
    private bool _grappleAttached;
    private bool _distanceSet;
    private bool _isActioning;

    [Header("Rope")]
    public Transform ropeStartPos;
    public LineRenderer ropeRenderer;
    private float _ropeMaxDistance = 5f;
    private Vector2 _ropePosition = Vector2.negativeInfinity;
    
    [Header("Hook")]
    public GameObject hookAnchor;
    public Transform hookStartPos;
    private Rigidbody2D _hookAnchorRb;
    public LayerMask hookLayerMask;
    
    private void Awake()
    {
        distanceJoint.enabled = false;
        _playerPosition = transform.position;
        _hookAnchorRb = hookAnchor.GetComponent<Rigidbody2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // 조작하는 플레이어 체크
        if(!isLocalPlayer) return;
        
        _playerInput = GetComponent<HookMovement>().playerInput;
        _playerInput.playerActions.Look.performed += OnLook;
        _playerInput.playerActions.Look.canceled += OnLook;
        _playerInput.playerActions.VerticalMove.started += OnVerticalMove;
        _playerInput.playerActions.Action.started += OnMainAction;
        _playerInput.playerActions.SubAction.started += OnSubAction;
    }

    private void OnDestroy()
    {
        if (!isLocalPlayer && Managers.Network.isNetworkActive) return;
        
        _playerInput.playerActions.Look.performed -= OnLook;
        _playerInput.playerActions.Look.canceled -= OnLook;
        _playerInput.playerActions.VerticalMove.started -= OnVerticalMove;
        _playerInput.playerActions.Action.started -= OnMainAction;
        _playerInput.playerActions.SubAction.started -= OnSubAction;
    }

    private void Update()
    {
        // 조작하는 플레이어 체크
        if(isLocalPlayer)
            HandleRopeLength();
        UpdateRopePositions();
    }

    private void FixedUpdate()
    {
        // 조작하는 플레이어 체크
        if (!isLocalPlayer) return;
        
        // 마우스 위치
        var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(_mousePosition.x, _mousePosition.y, 0f));
        var facingDirection = mousePos - transform.position;
        var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
            aimAngle = Mathf.PI * 2 + aimAngle;
        
        // 캐릭터 -> 마우스 방향
        _aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
        _playerPosition = transform.position;
    }
    
    // 로프 회수 코드
    public void ResetRope()
    {
        distanceJoint.enabled = false;
        _grappleAttached = false;
        CmdChangeGrappleState(false);
        playerMovement.isSwinging = false;
        playerMovement.swingJump = true;
        
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, ropeStartPos.position);
        ropeRenderer.SetPosition(1, ropeStartPos.position);
        ropeRenderer.enabled = false;
        
        _ropePosition = Vector2.negativeInfinity;
        _hookAnchorRb.bodyType = RigidbodyType2D.Kinematic;
        CmdChangeHookBody(RigidbodyType2D.Kinematic);
        hookAnchor.transform.position = hookStartPos.position;
    }
    
    private void UpdateRopePositions()
    {
        if (!_grappleAttached)
        {
            //TODO 현재 임시로 위치 조정 중
            hookAnchor.transform.position = hookStartPos.position;
            ropeRenderer.enabled = false;
            return;
        }

        // LineRenderer 그리기
        ropeRenderer.enabled = true;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, ropeStartPos.position);
        ropeRenderer.SetPosition(1, hookAnchor.transform.position);
    }
    
    private void HandleRopeLength()
    {
        if (!_grappleAttached) return;
        
        // 위치가 애매할 경우 회수
        if (Vector2.Distance(transform.position, hookAnchor.transform.position) <= 0.2f)
        {
            ResetRope();
        }
        // 플레이어의 위치가 갈고리보다 높을 경우 길이 줄어들게 하는 코드
        else if (transform.position.y > hookAnchor.transform.position.y)
        {
            Vector2 playerVector = transform.right;
            Vector2 targetVector = (hookAnchor.transform.position - transform.position).normalized;
            var angle = Vector2.Angle(playerVector, targetVector);
            if (angle is >= 20 or <= 160)
            {
                // 각도가 90도에 가까울 수록 더 빠르게 줄어들도록 보정치 설정
                var multiplier = angle > 90 ? (180 - angle) / 90 : angle / 90;
                var currentDistance = distanceJoint.distance;
                distanceJoint.distance = Mathf.Lerp(currentDistance, 0f, Time.deltaTime * 8.9f * 0.5f * multiplier);
            }
        }
        
        // 로프의 수직 이동 코드
        if (_vertical > 0f)
        {
            distanceJoint.distance -= Time.deltaTime * climbSpeed;
            
            // 플레이어가 움직이지 못할 때, distanceJoint.distance 재수정
            var playerDistance = Vector2.Distance(transform.position, hookAnchor.transform.position);
            if (playerDistance - distanceJoint.distance >= 0.1f)
            {
                distanceJoint.distance = playerDistance;
            }
        }
        else if (_vertical < 0f && distanceJoint.distance < _ropeMaxDistance)
        {
            var distance = distanceJoint.distance + Time.deltaTime * climbSpeed;
            distanceJoint.distance = Mathf.Min(distance, _ropeMaxDistance);
            
            // 플레이어가 움직이지 못할 때, distanceJoint.distance 재수정
            var playerDistance = Vector2.Distance(transform.position, hookAnchor.transform.position);
            if (distanceJoint.distance - playerDistance >= 0.1f)
            {
                distanceJoint.distance = playerDistance;
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdChangeGrappleState(bool value)
    {
        RpcChangeGrappleState(value);
    }

    [ClientRpc(includeOwner = false)]
    private void RpcChangeGrappleState(bool value)
    {
        _grappleAttached = value;
        ropeRenderer.enabled = true;
    }

    [Command(requiresAuthority = false)]
    private void CmdChangeHookBody(RigidbodyType2D type)
    {
        RpcChangeHookBody(type);
    }
    
    [ClientRpc(includeOwner = false)]
    private void RpcChangeHookBody(RigidbodyType2D type)
    {
        _hookAnchorRb.bodyType = type;
    }
    
    private void OnLook(InputAction.CallbackContext context)
    {
        // 마우스 위치
        _mousePosition = context.ReadValue<Vector2>();
    }
    
    private void OnVerticalMove(InputAction.CallbackContext context)
    {
        // 위 아래 움직임
        _vertical = context.ReadValue<Vector2>().y;
    }

    private void OnMainAction(InputAction.CallbackContext context)
    {
        if (_grappleAttached) return;

        var hit = Physics2D.Raycast(_playerPosition, _aimDirection, _ropeMaxDistance, hookLayerMask);

        if (hit.collider != null)
        {
            // 그래플링 연결 코드
            ropeRenderer.enabled = true;
            _grappleAttached = true;
            CmdChangeGrappleState(true);
            playerMovement.isSwinging = true;
            
            if (_ropePosition != hit.point)
            {
                _ropePosition = hit.point;
                playerMovement.ropeHook = _ropePosition;
                
                distanceJoint.distance = Vector2.Distance(_playerPosition, hit.point);
                distanceJoint.enabled = true;
                
                hookAnchor.transform.position = _ropePosition;
                _hookAnchorRb.bodyType = RigidbodyType2D.Static;
                CmdChangeHookBody(RigidbodyType2D.Static);
            }
        }
        else
        {
            ropeRenderer.enabled = false;
            _grappleAttached = false;
            CmdChangeGrappleState(false);
            playerMovement.isSwinging = false;
            distanceJoint.enabled = false;
        }
    }

    private void OnSubAction(InputAction.CallbackContext context)
    {
        if (_rigidbody.velocity.y > 0)
        {
            // 그래플링 swing action 종료시, 포물선 이동을 위한 보정치
            _rigidbody.AddForce(new Vector2(0, swingJumpPower * _rigidbody.velocity.magnitude * 0.1f), ForceMode2D.Impulse);
        }
        
        ResetRope();
    }
}
