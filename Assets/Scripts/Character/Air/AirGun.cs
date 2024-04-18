using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class AirGun : NetworkBehaviour
{
    // Input
    [field: SerializeField] private Transform _armPivot;
    //[field: SerializeField] private SpriteRenderer _armRenderer;
    [field: SerializeField] private SpriteRenderer _weaponSprite;
    private PlayerInput _playerInput;
    private Camera _mainCamera;
    private Vector2 _mouseDelta;
    private bool _leftClick;
    private bool _rightClick;
    
    // TargetDetections
    [field: SerializeField] private Transform _weaponPoint;
    private Collider2D _closestTarget;
    private Collider2D _latestTarget;
    [field: SerializeField] private LayerMask _objectMask;
    [field: SerializeField] private float _detectionDistance;
    private float _shortestDistance = float.MaxValue;

    // InhaleAction
    [field: SerializeField] private Rigidbody2D _rigidbody2D;
    [field: SerializeField] private float _inhaleSpeed;
    private Collider2D _inhaleTarget;
    private bool _isAttached;
    [SyncVar] private bool _inhaling;

    // ShootAction
    private LineRenderer _projectilePredict;
    [field: SerializeField] private float _shootPower;
    [field: SerializeField] private float _numberOfPoints;
    [field: SerializeField] private float _spaceBetweenPoints;
    
    
    // FlyAction
    private Grappling _grappling;
    private bool _canStick;
    
    private void Awake()
    {
        _projectilePredict = GetComponent<LineRenderer>();
    }
    
    
private void Start()
    {
        if(!isLocalPlayer) return;

        _mainCamera = Camera.main;
        _playerInput = GetComponent<PlayerInput>();

        _playerInput.playerActions.Look.performed += Look;
        _playerInput.playerActions.Action.started += PlayerActionStarted;
        _playerInput.playerActions.Action.canceled += PlayerActionCanceled;
        _playerInput.playerActions.SubAction.started += PlayerSubActionStarted;
        _playerInput.playerActions.SubAction.canceled += PlayerSubActionCanceled;
    }

    private void OnDisable()
    {
        if (!ReferenceEquals(Managers.Game.Player, gameObject)) return;
        
        _playerInput.playerActions.Look.performed -= Look;
        _playerInput.playerActions.Action.started -= PlayerActionStarted;
        _playerInput.playerActions.Action.canceled -= PlayerActionCanceled;
        _playerInput.playerActions.SubAction.started -= PlayerSubActionStarted;
        _playerInput.playerActions.SubAction.canceled -= PlayerSubActionCanceled;
    }
    private void Update()
    {
        
        if(!isLocalPlayer) return;
        
        RotateGun();
        DetectObject();
        CheckObject();
    }

    private void RotateGun()
    {
        if(!_rightClick) return;
        
        var mousePos = _mainCamera.ScreenToWorldPoint(_mouseDelta);
        var newAim = mousePos - _armPivot.position;

        var rotZ = Mathf.Atan2(newAim.y, newAim.x) * Mathf.Rad2Deg;

        if (rotZ is < -10f and > -90f)
            rotZ = -10f;
        else if (rotZ is > -170f and < -90f)
            rotZ = -170f;
        
        _weaponSprite.flipX = Mathf.Abs(rotZ) > 90f;

        _armPivot.rotation = Quaternion.AngleAxis(rotZ, Vector3.forward);
    }

    #region InhaleAction
    private void DetectObject()
    {
        if(!_rightClick || _isAttached) return;
        
        var collisions = Physics2D.OverlapCircleAll(_weaponPoint.position, _detectionDistance, _objectMask);

        // 검출 없을 시 예외처리
        if (collisions.Length == 0)
        {
            if(_latestTarget is null) return;

            CmdStopInhaleTarget();
            _inhaling = false;
            _isAttached = false;
            _latestTarget = null;
            return;
        }

        _closestTarget = null;

        foreach (var collision in collisions)
        {
            var targetDistance = Vector2.Distance(_weaponPoint.position, collision.transform.position);

            if (targetDistance > _shortestDistance) continue;

            // 거리가 일정 이내 일 때는 각도 체크 없이 처리 
            if (targetDistance <= 0.5f)
            {
                _closestTarget = collision;
                _shortestDistance = targetDistance;
            }
            else
            {
                // 벡터를 사용해 각도 처리
                var objectVector = (collision.transform.position - _weaponPoint.position).normalized;
                var weaponVector = _weaponPoint.transform.right;

                var angle = Vector2.Angle(weaponVector, objectVector);

                if (angle > 45) continue;

                // 장애물 처리
                var hit = Physics2D.Raycast(_weaponPoint.position, objectVector, targetDistance);
                if (!ReferenceEquals(hit.collider, collision)) continue;

                _closestTarget = collision;
                _shortestDistance = targetDistance;
            }
        }

        // 각도 밖의 오브젝트 예외처리
        if (_closestTarget is null)
        {
            if (_latestTarget is null) return;

            CmdStopInhaleTarget();
            _inhaling = false;
            _isAttached = false;
            _latestTarget = null;
            return;
        }
        
        // 같은 오브젝트 검출 예외처리
        if (ReferenceEquals(_latestTarget, _closestTarget))
        {
            _shortestDistance = float.MaxValue;
            return;
        }

        _latestTarget = _closestTarget;
        
        _shortestDistance = float.MaxValue;
    }

    private void CheckObject()
    {
        if(!_rightClick || _latestTarget is null || _isAttached) return;

        if (_grappling is null && ReferenceEquals(_latestTarget.gameObject, Managers.Game.OtherPlayer))
        {
            if (_latestTarget.TryGetComponent(out _grappling) && _grappling.grappleAttached)
                StartCoroutine(KeepGrapplingCheck());
        }
        else if (_grappling is not null && _grappling.grappleAttached)
        {
            StickToHook();
        }
        else
        {
            if(_latestTarget.GetComponent<NetworkIdentity>().isOwned)
                CmdInhaleTarget(_latestTarget.GetComponent<NetworkIdentity>());
        }
    }

    private IEnumerator KeepGrapplingCheck()
    {
        _canStick = false;
        yield return new WaitForSeconds(1f);
        _canStick = true;
    }

    private void StickToHook()
    {
        if(!_canStick) return;
        
        var targetPos = _latestTarget.transform.position;
        _rigidbody2D.gravityScale = 0;

        if (Vector2.Distance(_weaponPoint.position, targetPos) <= 0.2f)
        {
            _rigidbody2D.velocity = Vector2.zero;
            _isAttached = true;
        }
        else
        {
            var direction = (_latestTarget.transform.position - transform.position).normalized;
            _rigidbody2D.MovePosition((transform.position + direction) * (Time.deltaTime * _inhaleSpeed));
        }
    }
    
    [Command(requiresAuthority = false)]
    private void CmdInhaleTarget(NetworkIdentity target)
    {
        if (target is null) return;
        
        if (!_inhaling)
        {
            _inhaling = true;
            _inhaleTarget = target.GetComponent<Collider2D>();
            
            var targetId = _inhaleTarget.GetComponent<NetworkIdentity>();
            targetId.RemoveClientAuthority();
            targetId.AssignClientAuthority(netIdentity.connectionToClient);
            
            _inhaleTarget.GetComponent<IInhalable>().Inhalation(_weaponPoint);
        }
        
        if (Vector2.Distance(_inhaleTarget.transform.position, _weaponPoint.position) > 0.1f) return;

        _isAttached = true;
        _inhaling = false;
    }

    [Command(requiresAuthority = false)]
    private void CmdStopInhaleTarget()
    {
        if (_inhaleTarget is null) return;
        
        _inhaleTarget.GetComponent<IInhalable>().StopInhale();
        _inhaling = false;
        _isAttached = false;
    }
    
    [ClientRpc]
    private void RpcStopInhaleTarget()
    {
        
    }
    #endregion
    
    private void RenderProjectilePredictLine()
    {
        for (var i = 0; i < _numberOfPoints; i++)
        {
            _projectilePredict.SetPosition(i, LinePointPosition(i * _spaceBetweenPoints));
        }
    }

    private Vector2 LinePointPosition(float t)
    {
        Vector2 mousePos = _mainCamera.ScreenToWorldPoint(_mouseDelta);
        Vector2 newAim = (mousePos - (Vector2)_armPivot.position).normalized;

        var position = (Vector2)_weaponPoint.position +
                        (newAim * (_shootPower * t)) + (0.5f * Physics2D.gravity * (t * t) * _latestTarget.attachedRigidbody.gravityScale);

        return position;
    }

    #region PlayerInput
    private void Look(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }
    
    private void PlayerActionStarted(InputAction.CallbackContext context)
    {
        _leftClick = true;
    }
    
    private void PlayerActionCanceled(InputAction.CallbackContext context)
    {
        _leftClick = false;
    }
    
    private void PlayerSubActionStarted(InputAction.CallbackContext context)
    {
        _rightClick = true;
    }
    
    private void PlayerSubActionCanceled(InputAction.CallbackContext context)
    {
        _rightClick = false;
        if (_inhaling || _isAttached)
        {
            CmdStopInhaleTarget();
        }
    }
    #endregion
}
