using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class NewGrappling : MonoBehaviour
{
    [field: SerializeField] private HookSM _hook;
    private DistanceJoint2D _distanceJoint2D;
    private Vector2 _targetPos;
    private bool _canControl => _hook.canControl;
    private bool _canAction => _hook.canAction;
    private bool grappleAttached
    {
        get => _hook.grappleAttached;
        set => _hook.grappleAttached = value;
    }
    private bool isAirAttached
    {
        get => _hook.isAirAttached;
        set => _hook.isAirAttached = value;
    }
    
    private float _climbSpeed;
    private float _ropeSpeed;
    private Vector2 _ropePosition;
    public float swingPower { get; private set; }
    public float swingJumpPower { get; private set; }
    private float _ropeMaxDistance;
    private bool _isCoolDown;
    private float _coolDown;

    private Coroutine _grappleCoolDown;
    private WaitForSeconds _grappleCoolTime;
    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput => Managers.Game.playerInput;
    private Camera _mainCamera;
    private Vector2 _mousePosition;
    private Vector2 _aimDirection;
    private float _vertical;

    public GameObject hookAnchor => _hook.hookAnchor;
    public Transform hookSprite => _hook.hookSprite;
    public Transform hookStartPos => _hook.hookStartPos;
    public Transform ropeStartPos => _hook.ropeStartPos;
    public LineRenderer ropeRenderer => _hook.ropeRenderer;
    public LayerMask hookLayerMask => _hook.hookLayerMask;
    private Rigidbody2D _hookAnchorRb;
    public Vector2 hookAnchorPos;
    public LayerMask hookableObjectMask;
    private LayerMask _obstacleMask;
    public ParentConstraint _hookAnchorConstraint;
    private ConstraintSource _hookAnchorSource;
    private float _latestDistance;
    private bool _distanceChange;
    

    //public NewGrappling(HookSM hook)
    //{
    //    _hook = hook;
    //    Start();
    //}

    private void Start()
    {
        _distanceJoint2D = _hook.GetComponent<DistanceJoint2D>();
        _distanceJoint2D.enabled = false;
        _hookAnchorRb = hookAnchor.GetComponent<Rigidbody2D>();
        _rigidbody = _hook.GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
        ropeRenderer.enabled = false;

        var hookDataSo = (HookDataSO)_hook.playerData;
        _climbSpeed = hookDataSo.climbSpeed;
        _ropeSpeed = hookDataSo.ropeSpeed;
        swingPower = hookDataSo.swingForce;
        swingJumpPower = hookDataSo.swingJumpForce;
        _ropeMaxDistance = hookDataSo.ropeMaxDistance;
        _coolDown = hookDataSo.coolDown;
        _obstacleMask = hookDataSo.obstacleLayerMask;
        _grappleCoolTime = new WaitForSeconds(_coolDown);
        _hookAnchorSource = new ConstraintSource
        {
            sourceTransform = null,
            weight = 1.0f
        };
        _latestDistance = -1f;

        if (!_hook.isLocalPlayer) return;
        
        SubscribeInput();
    }

    public void OnDisable()
    {
        UnSubscribeInput();
    }
    
    #region UpdateMethod
    private void Update()
    {
        if (_hook.isLocalPlayer && _hook.canControl)
            HandleRopeLength();
            
        UpdateRopePosition();
    }

    private void FixedUpdate()
    {
        if (!_hook.isLocalPlayer || !_hook.canControl) return;
        
        var mousePos = _mainCamera.ScreenToWorldPoint(new Vector3(_mousePosition.x, _mousePosition.y, 0f));
        var facingDirection = mousePos - _hook.transform.position;
        var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
            aimAngle = Mathf.PI * 2 + aimAngle;
        
        _aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
    }
    #endregion

    #region Grappling
    public void Reset()
    {
        HookAnchorConstraintFree();
        _hook.CmdHookAnchorConstraintFree();

        if (_grappleCoolDown != null)
            _hook.StopCoroutine(_grappleCoolDown);
        _grappleCoolDown = _hook.StartCoroutine(GrappleCoolDown());

        _distanceJoint2D.enabled = false;
        grappleAttached = false;
        if (isAirAttached)
            isAirAttached = false;
        _hook.isSwinging = false;
        _ropePosition = Vector2.negativeInfinity;
        _hookAnchorRb.bodyType = RigidbodyType2D.Kinematic;
        _latestDistance = -1f;
    }
    
    public void StopRope()
    {
        Reset();
        hookSprite.position = hookStartPos.position;
        hookSprite.rotation = Quaternion.identity;
        hookAnchor.transform.position = hookStartPos.position;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, ropeStartPos.position);
        ropeRenderer.SetPosition(1, ropeStartPos.position);
    }

    private IEnumerator GrappleCoolDown()
    {
        _isCoolDown = true;
        yield return _grappleCoolTime;
        _isCoolDown = false;
        _grappleCoolDown = null;
    }

    private void UpdateRopePosition()
    {
        if (!grappleAttached)
        {
            if (ropeRenderer.enabled)
            {
                if (_hook.isLocalPlayer)
                    hookSprite.position = _hookAnchorConstraint.sourceCount == 0 ? _targetPos : _hookAnchorSource.sourceTransform.position;
                ropeRenderer.SetPosition(0, ropeStartPos.position);
                _targetPos = Vector2.MoveTowards(_targetPos, ropeStartPos.position, _ropeSpeed * Time.deltaTime);
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
            if (Vector2.Distance(_targetPos, hookAnchorPos) > 0.1f)
                _targetPos = Vector2.MoveTowards(_targetPos, hookAnchorPos, _ropeSpeed * Time.deltaTime);
            else
                _targetPos = hookAnchorPos;

            if (_hook.isLocalPlayer)
                hookSprite.position = _hookAnchorConstraint.sourceCount == 0 ? _targetPos : _hookAnchorSource.sourceTransform.position;
            ropeRenderer.SetPosition(1, hookSprite.position);
        }
    }

    private void HandleRopeLength()
    {
        if (!grappleAttached) return;

        if (_hookAnchorConstraint.sourceCount == 0)
            hookAnchor.transform.position = hookAnchorPos;
        
        if (_vertical > 0f)
        {
            _distanceChange = true;
            _distanceJoint2D.distance -= Time.deltaTime * _climbSpeed;
        }
        else if (_vertical < 0f && _distanceJoint2D.distance < _ropeMaxDistance)
        {
            _distanceChange = true;
            var distance = _distanceJoint2D.distance + Time.deltaTime * _climbSpeed;
            _distanceJoint2D.distance = Mathf.Min(distance, _ropeMaxDistance);
        }
        else if (_latestDistance > 0)
        {
            _distanceChange = false;
            _distanceJoint2D.distance = _latestDistance;
        }

        if (_distanceChange)
            _latestDistance = _distanceJoint2D.distance;

        var playerDistance = Vector2.Distance(_hook.transform.position, hookAnchor.transform.position);
        if (Mathf.Abs(playerDistance - _distanceJoint2D.distance) >= 0.15f && _distanceChange)
        {
            _distanceJoint2D.distance = playerDistance;
        }
    }

    private void ThrowHook()
    {
        if (grappleAttached || _isCoolDown || !_hook.canControl) return;
        
        var hit = Physics2D.Raycast(_hook.transform.position, _aimDirection, _ropeMaxDistance, hookLayerMask);

        if (hit.collider == null) return;

        if (((1 << hit.collider.gameObject.layer) & _obstacleMask) != 0) return;

        grappleAttached = true;
        _latestDistance = -1f;
        _hook.ThrowHook();

        if (_ropePosition == hit.point) return;
        
        _ropePosition = hit.point;
        var targetVec = (_ropePosition - (Vector2)_hook.transform.position).normalized * 0.25f;

        hookAnchorPos = _ropePosition - targetVec;
        
        _distanceJoint2D.distance = Vector2.Distance(_hook.transform.position, hookAnchorPos);
        _latestDistance = _distanceJoint2D.distance;
        _distanceJoint2D.enabled = true;

        if (((1 << hit.collider.gameObject.layer) & hookableObjectMask) != 0)
        {
            _hookAnchorSource.sourceTransform = hit.collider.transform;
            _hookAnchorConstraint.AddSource(_hookAnchorSource);
            hookAnchor.transform.position = _hookAnchorConstraint.transform.position;
            _hook.CmdHookAnchorConstraintSync(hit.transform.gameObject);
        }
        else
        {
            hookAnchor.transform.position = hookAnchorPos;
        }
        _hookAnchorRb.bodyType = RigidbodyType2D.Static;
        
        var targetVector = (hookAnchorPos - (Vector2)hookSprite.position).normalized;
        hookSprite.Rotate(0, 0, -Vector2.SignedAngle(targetVector, hookSprite.up));
    }

    public void HookAnchorConstraintSync(GameObject gameObject)
    {
        _hookAnchorSource.sourceTransform = gameObject.transform;
        _hookAnchorConstraint.AddSource(_hookAnchorSource);
        hookAnchor.transform.position = _hookAnchorConstraint.transform.position;
    }

    public void HookAnchorConstraintFree()
    {
        if (_hookAnchorConstraint.sourceCount > 0)
        {
            _hookAnchorConstraint.RemoveSource(0);
        }
        _hookAnchorSource.sourceTransform = null;
    }

    private void WithdrawHook()
    {
        Reset();
        _hook.WithdrawHook();
    }
    #endregion
    
    #region Input
    private void SubscribeInput()
    {
        if (!_hook.isLocalPlayer) return;

        _playerInput.playerActions.Action.started += OnMainActionStarted;
        _playerInput.playerActions.Look.performed += OnLook;
        _playerInput.playerActions.SubAction.started += OnSubActionStarted;
        _playerInput.playerActions.VerticalMove.started += OnVerticalMove;
    }

    private void UnSubscribeInput()
    {
        _playerInput.playerActions.Action.started -= OnMainActionStarted;
        _playerInput.playerActions.Look.performed -= OnLook;
        _playerInput.playerActions.SubAction.started -= OnSubActionStarted;
        _playerInput.playerActions.VerticalMove.started -= OnVerticalMove;
    }

    private void OnMainActionStarted(InputAction.CallbackContext context)
    {
        if (!_canControl || !_canAction) return;
        
        ThrowHook();
    }
    
    private void OnSubActionStarted(InputAction.CallbackContext context)
    {
        if (!_canControl || !_canAction) return;
        
        if (!grappleAttached) return;
        
        WithdrawHook();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        _mousePosition = context.ReadValue<Vector2>();
    }

    private void OnVerticalMove(InputAction.CallbackContext context)
    {
        if (!_canControl || !grappleAttached)
        {
            _vertical = 0f;
            return;
        }
        
        _vertical = context.ReadValue<Vector2>().y;
    }
    #endregion
}
    