using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grappling : NetworkBehaviour
{
    [Header("Player")]
    public HookMovement playerMovement;
    public DistanceJoint2D distanceJoint;
    public float climbSpeed = 3f;
    public float swingJumpPower;
    [SerializeField] public float coolTime;
    private bool _isGround => playerMovement.isGround;

    private Coroutine Co_GrappleCoolDown;
    private WaitForSeconds _grappleCoolTime;
    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;
    private Camera _mainCamera;
    private Vector2 _mousePosition;
    private Vector2 _aimDirection;
    private float _vertical;
    
    [SyncVar] public bool grappleAttached;
    [SyncVar] private bool _isAirAttached;
    public bool isAirAttached
    {
        get => _isAirAttached;
        set => CmdChangeAirAttachedState(value);
    }

    public bool canControl;
    private bool _distanceSet;
    private bool _isActioning;
    private bool _isCoolTime;

    [Header("Rope")]
    public Transform ropeStartPos;
    public LineRenderer ropeRenderer;
    public float ropeSpeed;
    private float _ropeMaxDistance = 5f;
    private Vector2 _ropePosition = Vector2.negativeInfinity;
    private Vector2 _targetPos;

    [Header("Hook")]
    public GameObject hookAnchor;
    public Transform hookSprite;
    public Transform hookStartPos;
    public LayerMask hookLayerMask;
    private Rigidbody2D _hookAnchorRb;
    private Vector2 _hookAnchorPos;
    
    private void Awake()
    {
        distanceJoint.enabled = false;
        _hookAnchorRb = hookAnchor.GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _targetPos = ropeStartPos.position;
        
        // 조작하는 플레이어 체크
        if(!isLocalPlayer) return;

        canControl = true;
        _playerInput.playerActions.Look.performed += OnLook;
        _playerInput.playerActions.Look.canceled += OnLook;
        _playerInput.playerActions.VerticalMove.started += OnVerticalMove;
        _playerInput.playerActions.Action.started += OnMainAction;
        _playerInput.playerActions.SubAction.started += OnSubAction;
        _grappleCoolTime = new WaitForSeconds(coolTime);
    }

    private void OnDisable()
    {
        _playerInput.playerActions.Look.performed -= OnLook;
        _playerInput.playerActions.Look.canceled -= OnLook;
        _playerInput.playerActions.VerticalMove.started -= OnVerticalMove;
        _playerInput.playerActions.Action.started -= OnMainAction;
        _playerInput.playerActions.SubAction.started -= OnSubAction;
    }

    private void Update()
    {
        // 조작하는 플레이어 체크
        if(isLocalPlayer && canControl)
            HandleRopeLength();
        UpdateRopePositions();
    }

    private void FixedUpdate()
    {
        // 조작하는 플레이어 체크
        if (!isLocalPlayer || !canControl) return;
        
        // 마우스 위치
        var mousePos = _mainCamera.ScreenToWorldPoint(new Vector3(_mousePosition.x, _mousePosition.y, 0f));
        var facingDirection = mousePos - transform.position;
        var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
            aimAngle = Mathf.PI * 2 + aimAngle;
        
        // 캐릭터 -> 마우스 방향
        _aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
    }
    
    // 로프 회수 코드
    public void ResetRope()
    {
        if(Co_GrappleCoolDown != null)
            StopCoroutine(Co_GrappleCoolDown);
        Co_GrappleCoolDown = StartCoroutine(GrappleCoolDown());
        
        distanceJoint.enabled = false;
        grappleAttached = false;
        if(isAirAttached)
            isAirAttached = false;
        CmdChangeGrappleState(false);
        playerMovement.isSwinging = false;
        playerMovement.swingJump = true;
        
        _ropePosition = Vector2.negativeInfinity;
        _hookAnchorRb.bodyType = RigidbodyType2D.Kinematic;
        CmdChangeHookBody(RigidbodyType2D.Kinematic);
    }

    public void StopRope()
    {
        ResetRope();
        hookSprite.position = hookStartPos.position;
        hookSprite.rotation = Quaternion.identity;
        hookAnchor.transform.position = hookStartPos.position;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, ropeStartPos.position);
        ropeRenderer.SetPosition(1, ropeStartPos.position);
    }

    private IEnumerator GrappleCoolDown()
    {
        _isCoolTime = true;
        yield return _grappleCoolTime;
        _isCoolTime = false;
        Co_GrappleCoolDown = null;
    }
    
    private void UpdateRopePositions()
    {
        if (!grappleAttached)
        {
            if (ropeRenderer.enabled)
            {
                if(isLocalPlayer)
                    hookSprite.position = _targetPos;
                ropeRenderer.SetPosition(0, ropeStartPos.position);
                _targetPos = Vector2.MoveTowards(_targetPos, ropeStartPos.position, ropeSpeed * Time.deltaTime);
                ropeRenderer.SetPosition(1, hookSprite.position);

                if (Vector3.Distance(_targetPos, ropeStartPos.position) <= 0.1f)
                    ropeRenderer.enabled = false;
            }
            else
            {
                hookSprite.position = hookStartPos.position;
                hookSprite.rotation = Quaternion.identity;
                hookAnchor.transform.position = hookStartPos.position;
                ropeRenderer.positionCount = 2;
                ropeRenderer.SetPosition(0, ropeStartPos.position);
                ropeRenderer.SetPosition(1, ropeStartPos.position);
            }
            return;
        }

        // LineRenderer 그리기
        if (!ropeRenderer.enabled)
        {
            _targetPos = ropeStartPos.position;
            ropeRenderer.positionCount = 2;
            ropeRenderer.SetPosition(0, ropeStartPos.position);
            ropeRenderer.SetPosition(1, ropeStartPos.position);
            ropeRenderer.enabled = true;
        }
        else
        {
            ropeRenderer.SetPosition(0, ropeStartPos.position);
            if (Vector2.Distance(_targetPos, _hookAnchorPos) > 0.1f)
                _targetPos = Vector2.MoveTowards(_targetPos, _hookAnchorPos, ropeSpeed * Time.deltaTime);
            else
                _targetPos = _hookAnchorPos;

            if(isLocalPlayer)
                hookSprite.position = _targetPos;
            ropeRenderer.SetPosition(1, hookSprite.position);
        }
    }
    
    private void HandleRopeLength()
    {
        if (!grappleAttached) return;
        
        // 위치가 애매할 경우 회수
        if (Vector2.Distance(transform.position, hookAnchor.transform.position) <= 0.2f)
        {
            ResetRope();
            return;
        }
        
        hookAnchor.transform.position = _hookAnchorPos;
        
        // 플레이어의 위치가 갈고리보다 높을 경우 길이 줄어들게 하는 코드
        if (transform.position.y > hookAnchor.transform.position.y)
        {
            Vector2 playerVector = transform.right;
            Vector2 targetVector = (hookAnchor.transform.position - transform.position).normalized;
            
            var angle = Vector2.Angle(playerVector, targetVector);
            if (angle is >= 30 or <= 150)
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
        }
        else if (_vertical < 0f && distanceJoint.distance < _ropeMaxDistance)
        {
            var distance = distanceJoint.distance + Time.deltaTime * climbSpeed;
            distanceJoint.distance = Mathf.Min(distance, _ropeMaxDistance);
        }
        
        // 플레이어가 움직이지 못할 때, distanceJoint.distance 재수정
        var playerDistance = Vector2.Distance(transform.position, hookAnchor.transform.position);
        if (playerDistance - distanceJoint.distance >= 0.1f)
        {
            distanceJoint.distance = playerDistance;
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
        grappleAttached = value;
        ropeRenderer.enabled = value;
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

    [Command(requiresAuthority = false)]
    private void CmdChangeAirAttachedState(bool value)
    {
        _isAirAttached = value;
        playerMovement.isAirAttached = value;
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
        if (grappleAttached || _isCoolTime || _isGround) return;

        var hit = Physics2D.Raycast(transform.position, _aimDirection, _ropeMaxDistance, hookLayerMask);

        if (hit.collider != null)
        {
            grappleAttached = true;
            CmdChangeGrappleState(true);
            playerMovement.isSwinging = true;
            
            if (_ropePosition != hit.point)
            {
                _ropePosition = hit.point;
                var targetVec = (_ropePosition - (Vector2)transform.position).normalized * 0.25f;

                _hookAnchorPos = _ropePosition - targetVec;
                playerMovement.ropeHook = _hookAnchorPos;
                
                distanceJoint.distance = Vector2.Distance(transform.position, _hookAnchorPos);
                distanceJoint.enabled = true;
                
                hookAnchor.transform.position = _hookAnchorPos;
                _hookAnchorRb.bodyType = RigidbodyType2D.Static;
                CmdChangeHookBody(RigidbodyType2D.Static);
                
                var targetVector = (_hookAnchorPos - (Vector2)hookSprite.position).normalized;
                hookSprite.Rotate(0, 0, -Vector2.SignedAngle(targetVector, hookSprite.up));
            }
        }
    }

    private void OnSubAction(InputAction.CallbackContext context)
    {
        if(!grappleAttached)
            return;
        
        if (_rigidbody.velocity.y > 0)
        {
            // 그래플링 swing action 종료시, 포물선 이동을 위한 보정치
            _rigidbody.AddForce(new Vector2(0, swingJumpPower * _rigidbody.velocity.magnitude * 0.1f), ForceMode2D.Impulse);
        }
        
        ResetRope();
    }
}
