using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class AirGunNet : NetworkBehaviour
{
    // Input
    [field: SerializeField] private Transform _armPivot;
    [field: SerializeField] private Transform _charPivot;
    public bool canHandle;
    private PlayerInput _playerInput;
    private PlayerMovement _playerMovement;
    private Camera _mainCamera;
    private Vector2 _mouseDelta;
    private bool _rightClick;
    
    // TargetDetections
    [field: SerializeField] public Transform weaponPoint { get; private set; }
    private Collider2D _closestTarget = null;
    private Collider2D _latestTarget = null;
    [field: SerializeField] private LayerMask _objectMask;
    [field: SerializeField] private LayerMask _obstacleMask;
    [field: SerializeField] private float _detectionDistance;
    private float _shortestDistance = float.MaxValue;
    private Coroutine _keepGrapplingCheckCoroutine;
    
    // InhaleAction
    [field: SerializeField] private Rigidbody2D _rigidbody2D;
    private Collider2D _inhaleTarget;
    private bool _isAttached;
    private bool _inhaling;
    private bool _isAttachedToHook;
    private bool _isInhaledHook;
    
    // ShootAction
    private LineRenderer _lineRenderer;
    [field: SerializeField] private float _shootPower;
    [field: SerializeField] private float _minShootPower = 5f;
    [field: SerializeField] private float _maxShootPower = 20f;
    [field: SerializeField] private float _numberOfPoints;
    [field: SerializeField] private float _spaceBetweenPoints;
    private float _latestTargetGravityScale;
    private Coroutine _chargingCoroutine;
    private bool _canInhale = true;
    
    // FlyAction
    [field: SerializeField] private float _flyPower;
    private Grappling _grappling;
    private bool _canStick;
    private bool _isStick;
    private bool _sticking;

    // ParticleSystems
    [SerializeField] private ParticleSystem _inhaleParticles;
    [SerializeField] private ParticleSystem _exhaleParticles;

    // Animator
    private Animator _animator;
    #region AnimationTriggerStringCache
    private static readonly int IsHookInhaled = Animator.StringToHash("IsHookInhaled");
    private static readonly int IsAirAttached = Animator.StringToHash("IsAttached");
    private static readonly int IsInhaling = Animator.StringToHash("IsInhaling");
    private static readonly int IsExhaling = Animator.StringToHash("IsExhaling");
    private static readonly int IsFlying = Animator.StringToHash("IsFlying");
    #endregion
    
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _playerMovement = GetComponent<PlayerMovement>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        if(!isLocalPlayer) return;

        _mainCamera = Camera.main;
        canHandle = true;
        _playerInput.playerActions.Look.performed += Look;
        _playerInput.playerActions.Action.started += PlayerActionStarted;
        _playerInput.playerActions.Action.canceled += PlayerActionCanceled;
        _playerInput.playerActions.SubAction.started += PlayerSubActionStarted;
        _playerInput.playerActions.SubAction.canceled += PlayerSubActionCanceled;

        Managers.Command.itemInhaleCallback += Inhale;
        Managers.Command.fixItemCallback += FixInhaleTarget;
    }

    private void OnDisable()
    {
        _playerInput.playerActions.Look.performed -= Look;
        _playerInput.playerActions.Action.started -= PlayerActionStarted;
        _playerInput.playerActions.Action.canceled -= PlayerActionCanceled;
        _playerInput.playerActions.SubAction.started -= PlayerSubActionStarted;
        _playerInput.playerActions.SubAction.canceled -= PlayerSubActionCanceled;
        
        Managers.Command.itemInhaleCallback -= Inhale;
        Managers.Command.fixItemCallback -= FixInhaleTarget;
    }

    private void Update()
    {
        if(!isLocalPlayer) return;

        if (canHandle)
        {
            RotateGun();
            DetectObject();
            ObjectCheck();
        }
        AnimationParticlesChecks();
    }
    
    private void RotateGun()
    {
        if(!_rightClick || _sticking) return;
        
        var mousePos = _mainCamera.ScreenToWorldPoint(_mouseDelta);
        var newAim = mousePos - _armPivot.position;

        var rotZ = Mathf.Atan2(newAim.y, newAim.x) * Mathf.Rad2Deg;

        if (rotZ is < -10f and > -90f)
            rotZ = -10f;
        else if (rotZ is > -170f and < -90f)
            rotZ = -170f;
        
        _armPivot.rotation = Quaternion.AngleAxis(rotZ, Vector3.forward);
        
        if (Mathf.Abs(rotZ) > 90f) {
            rotZ = -rotZ;
            _charPivot.rotation = Quaternion.Euler(0f, 180f, 0f);
            _armPivot.rotation = Quaternion.Euler(-180f, 0f, rotZ);
        } 
        else
        {
            _charPivot.rotation = Quaternion.identity;
        }
    }
    
    public void Reset()
    {
        canHandle = false;
        StopInhaleTarget();
        StopSticking();
        _inhaling = false;
        _isAttached = false;
        _canStick = false;
        _sticking = false;
        _isInhaledHook = false;
        _latestTarget = null;
        _grappling = null;
        _lineRenderer.enabled = false;
        _shortestDistance = float.MaxValue;
        CmdInhaleParticlesStop();
    }
    
    #region ObjectCheck
    private void DetectObject()
    {
        if(!_rightClick || _isAttached || _sticking || !_canInhale || _isInhaledHook) return;
        
        var collisions = Physics2D.OverlapCircleAll(weaponPoint.position, _detectionDistance, _objectMask);

        // 검출 없을 시 예외처리
        if (collisions.Length == 0)
        {
            if(_latestTarget is null) return;

            StopInhaleTarget();
            _inhaling = false;
            _isAttached = false;
            _isInhaledHook = false;
            Debug.Log("a");
            _latestTarget = null;
            return;
        }

        _closestTarget = null;

        foreach (var collision in collisions)
        {
            var targetDistance = Vector2.Distance(weaponPoint.position, collision.transform.position);

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
                var objectVector = (collision.transform.position - weaponPoint.position).normalized;
                var weaponVector = weaponPoint.transform.right;

                var angle = Vector2.Angle(weaponVector, objectVector);

                if (angle > 45) continue;

                // 후크가 잡고 있는 물체 처리
                if (collision.TryGetComponent<IInhalable>(out var inhalable) && !inhalable.CanInhale()) continue;
                
                // 장애물 처리
                var hit = Physics2D.Raycast(weaponPoint.position, objectVector, targetDistance, _obstacleMask);
                if (!ReferenceEquals(hit.collider, collision)) continue;

                _closestTarget = collision;
                _shortestDistance = targetDistance;
            }
        }

        // 각도 밖의 오브젝트 예외처리
        if (_closestTarget is null)
        {
            if (_latestTarget is null) return;

            StopInhaleTarget();
            _inhaling = false;
            _isAttached = false;
            _isInhaledHook = false;
            _latestTarget = null;
            return;
        }
        
        // 같은 오브젝트 검출 예외처리
        if (ReferenceEquals(_latestTarget, _closestTarget))
        {
            if ( _inhaleTarget is not null && ReferenceEquals(_inhaleTarget, _latestTarget) && _shortestDistance <= 0.2f)
            {
                if (ReferenceEquals(Managers.Game.OtherPlayer, _inhaleTarget.gameObject))
                    _isInhaledHook = true;
                else
                    _isAttached = true;
                
                if(_inhaleTarget.TryGetComponent<IInhalable>(out var inhalable))
                    Managers.Command.TryFixInhaleItem(gameObject, _inhaleTarget.GetComponent<NetworkIdentity>().netId);
            }

            _shortestDistance = float.MaxValue;
            return;
        }

        StopInhaleTarget();
        _inhaling = false;
        _isAttached = false;
        _isInhaledHook = false;
        _latestTarget = _closestTarget;
        if(_latestTarget.attachedRigidbody.gravityScale > 0)
            _latestTargetGravityScale = _latestTarget.attachedRigidbody.gravityScale;
        
        _shortestDistance = float.MaxValue;
    }

    private void ObjectCheck()
    {
        if(!_rightClick || _latestTarget is null || _isAttached || _isStick || !_canInhale) return;
        
        if (_grappling is null && ReferenceEquals(_latestTarget.gameObject, Managers.Game.OtherPlayer))
        {
            if (_latestTarget.TryGetComponent(out _grappling) && _grappling.grappleAttached)
            {
                _canStick = false;
                _keepGrapplingCheckCoroutine = StartCoroutine(KeepGrapplingCheck());
            }
        }
        else if (_grappling is not null && _grappling.grappleAttached)
        {
            if(!_canStick) return;
            
            StickToHook();
        }
        else
        {
            _grappling = null;
            StartInhaleTarget();
        }
    }

    private IEnumerator KeepGrapplingCheck()
    {
        yield return new WaitForSeconds(1f);
        _canStick = true;
    }
    #endregion

    #region InhaleAction
    private void StartInhaleTarget()
    {
        if(!_canInhale || (_inhaling && ReferenceEquals(_latestTarget, _inhaleTarget))) return;

        _inhaleTarget = _latestTarget;
        _inhaling = true;
        Managers.Command.TryInhaleItem(gameObject, _latestTarget.GetComponent<NetworkIdentity>().netId);
    }

    private void Inhale(NetworkIdentity item, bool value)
    {
        if (!value)
        {
            _inhaleTarget = null;
            _inhaling = false;
            return;
        }

        if (ReferenceEquals(Managers.Game.OtherPlayer, item.gameObject))
        {
            CmdInhalePlayer();
            return;
        }

        if(!item.TryGetComponent<IInhalable>(out var inhalable)) return;
        
        inhalable.Inhalation(weaponPoint);
    }

    private void StopInhaleTarget()
    {
        if (_inhaleTarget is null || !_inhaling || _canStick) return;
        
        StopInhale();
    }

    private void StopInhale()
    {
        _inhaling = false;
        if (_chargingCoroutine is not null)
        {
            StopCoroutine(_chargingCoroutine);
            _lineRenderer.enabled = false;
        }
        
        if (_isInhaledHook)
        {
            CmdStopInhalePlayer();
            return;
        }

        _inhaleTarget.GetComponent<IInhalable>().StopInhale();
        
        _isAttached = false;
        _isInhaledHook = false;
        Managers.Command.StopInhaleItem(_inhaleTarget.GetComponent<NetworkIdentity>().netId);
    }

    private void FixInhaleTarget(bool value)
    {
        if (value && _isAttached) return;
        
        StopInhale();
    }
    #endregion
    
    #region HookInteraction
    [field: SerializeField] private float _stickToHookSpeed;
    private Vector3 _offset = new(0, -1f);
    private Coroutine _stickToHookCoroutine;
    private WaitForFixedUpdate _waitForFixedUpdate = new();
    
    private void StickToHook()
    {
        _canStick = false;

        _stickToHookCoroutine = StartCoroutine(Co_StickHook());
    }
    
    private IEnumerator Co_StickHook()
    {
        var stick = false;
        _sticking = true;
        _rigidbody2D.drag = 8f;
        while (true)
        {
            if (!stick)
            {
                yield return _waitForFixedUpdate;
                if (_playerMovement.canControl)
                {
                    _playerMovement.canControl = false;
                    _rigidbody2D.gravityScale = 0f;
                    _rigidbody2D.velocity = Vector2.zero;
                }
                
                if(!_grappling.grappleAttached)
                    StopSticking();

                var direction = (_grappling.transform.position + _offset - transform.position).normalized;

                _rigidbody2D.AddForce(direction * _stickToHookSpeed);

                var rotation = Quaternion.LookRotation(_grappling.transform.position + new Vector3(0, 0.5f) - _armPivot.position,
                    _armPivot.TransformDirection(Vector2.up));
                _armPivot.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
                
                if (Vector2.Distance(_grappling.transform.position, transform.position + new Vector3(0, 1.0f)) <= 0.2f)
                    stick = true;
            }
            else
            {
                yield return null;
                if(!_grappling.grappleAttached)
                    StopSticking();
                
                _rigidbody2D.velocity = Vector2.zero;
                transform.position = _grappling.transform.position + _offset;
                //_grappling.GetComponent<HookMovement>().isAirAttached = true;
                _rigidbody2D.drag = 0f;
                _grappling.isAirAttached = true;
                _isStick = true;
                _isAttachedToHook = true;
            }
        }
    }
    
    private void FlyAway()
    {
        _isStick = false;
        _canStick = false;
        _sticking = false;
        _isAttachedToHook = false;
        _rigidbody2D.drag = 0f;
        //_grappling.GetComponent<HookMovement>().isAirAttached = false;
        _grappling.isAirAttached = false;
        StopCoroutine(_stickToHookCoroutine);
        _stickToHookCoroutine = null;
        
        Vector2 mousePos = _mainCamera.ScreenToWorldPoint(_mouseDelta);
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;

        if (_chargingCoroutine is not null)
        {
            StopCoroutine(_chargingCoroutine);
            _chargingCoroutine = null;
        }
        _canInhale = false;
        StartCoroutine(Co_CoolDown());
        _grappling = null;

        _playerMovement.specificJump = true;
        _playerMovement.canControl = true;
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.gravityScale = 3f;
        _rigidbody2D.AddForce(dir * -_flyPower, ForceMode2D.Impulse);
    }

    private void StopSticking()
    {
        if (_keepGrapplingCheckCoroutine is not null)
        {
            StopCoroutine(_keepGrapplingCheckCoroutine);
            _keepGrapplingCheckCoroutine = null;
        }
        _isAttachedToHook = false;
        _rigidbody2D.drag = 0f;
        
        if (_stickToHookCoroutine is null) return;
        StopCoroutine(_stickToHookCoroutine);
        _stickToHookCoroutine = null;
        _isStick = false;
        _canStick = false;
        _sticking = false;
        _playerMovement.canControl = true;
        if(_grappling is not null)
            _grappling.isAirAttached = false;
        _grappling = null;
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.gravityScale = 3f;
    }

    [Command(requiresAuthority = false)]
    public void CmdStopSticking()
    {
        RpcStopSticking();
    }
    
    [ClientRpc(includeOwner = false)]
    private void RpcStopSticking()
    {
        StopSticking();
    }
    #endregion

    #region ShootingAction
    private void Charging()
    {
        if(!_isAttached || !canHandle) return;
        
        _chargingCoroutine = StartCoroutine(Co_PowerCharging());
    }
    
    private IEnumerator Co_PowerCharging()
    {
        _shootPower = _minShootPower;
        _lineRenderer.enabled = true;
        
        var chargingTime = 0f;
        while (true)
        {
            if (_shootPower < _maxShootPower)
            {
                chargingTime += Time.deltaTime;

                _shootPower = chargingTime * (_maxShootPower - _minShootPower) + _minShootPower;
            }
            else if (_shootPower > _maxShootPower)
            {
                _shootPower = _maxShootPower;
            }
            
            ProjectilePredict();
            yield return null;
        }
    }
    
    private void ShootObject()
    {
        if(!_isAttached || _shootPower <= 0f) return;

        if (_chargingCoroutine != null)
        {
            StopCoroutine(_chargingCoroutine);
            _chargingCoroutine = null;
        }
        StartCoroutine(Co_CoolDown());
        StopInhaleTarget();

        _lineRenderer.enabled = false;
        if (ReferenceEquals(_inhaleTarget.gameObject, Managers.Game.OtherPlayer))
            _isInhaledHook = false;
        
        Managers.Command.ShootObject(_inhaleTarget.gameObject, weaponPoint.right * _shootPower);
        _inhaleTarget = null;
        _isAttached = false;
        _inhaling = false;
        _shootPower = 0f;
    }
    
    private IEnumerator Co_CoolDown()
    {
        _canInhale = false;
        
        yield return new WaitForSeconds(1.5f);
        
        _canInhale = true;
    }

    private void ProjectilePredict()
    {
        for (var i = 0; i < _numberOfPoints; i++)
        {
            _lineRenderer.SetPosition(i, PointPosition(i * _spaceBetweenPoints));
        }
    }
    
    private Vector2 PointPosition(float t)
    {
        Vector2 worldPos = _mainCamera.ScreenToWorldPoint(_mouseDelta);
        var newAim = worldPos - (Vector2)_armPivot.position;
    
        var position = (Vector2)weaponPoint.position
                           + (newAim.normalized * (_shootPower * t))
                           + (Physics2D.gravity * (0.5f * (t * t) * _latestTargetGravityScale));
        
        return position;
    }
    #endregion
    
    #region Input
    private void Look(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }
    
    private void PlayerActionStarted(InputAction.CallbackContext context)
    {
        Charging();
    }
    
    private void PlayerActionCanceled(InputAction.CallbackContext context)
    {
        if(!canHandle) return;
        
        _animator.SetTrigger(IsExhaling);
        if (_isStick)
            FlyAway();
        else
            ShootObject();
    }
    
    private void PlayerSubActionStarted(InputAction.CallbackContext context)
    {
        _rightClick = true;
    }
    
    private void PlayerSubActionCanceled(InputAction.CallbackContext context)
    {
        _rightClick = false;
        StopInhaleTarget();
        StopSticking();
        _grappling = null;
    }
    #endregion

    #region Animation&Particles
    //파티클 및 애니메이션
    private void InhaleParticlesPlay()
    {
        _inhaleParticles.Play();
    }

    [Command(requiresAuthority = false)]
    private void CmdInhaleParticlesStop()
    {
        RpcInhaleParticlesStop();
    }
    
    [ClientRpc]
    private void RpcInhaleParticlesStop()
    {
        _inhaleParticles.Stop();
    }
    
    private void ExhaleParticlesPlay()
    {
        _exhaleParticles.Play();
    }

    private void AnimationParticlesChecks()
    {
        _animator.SetBool(IsInhaling, _rightClick && canHandle);
        _animator.SetBool(IsFlying, _sticking);
        _animator.SetBool(IsAirAttached, _isAttachedToHook);
        _animator.SetBool(IsHookInhaled, _isInhaledHook);
        if (!_rightClick && _inhaleParticles.isPlaying)
            CmdInhaleParticlesStop();
        if ((_isAttachedToHook || _isAttached) && _inhaleParticles.isPlaying)
            CmdInhaleParticlesStop();
    }
    #endregion

    #region Command
    [Command(requiresAuthority = false)]
    private void CmdInhalePlayer()
    {
        RpcInhalePlayer();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcInhalePlayer()
    {
        Managers.Game.Player.GetComponent<IInhalable>().Inhalation(weaponPoint);
    }

    [Command(requiresAuthority = false)]
    private void CmdStopInhalePlayer()
    {
        RpcStopInhalePlayer();
    }
    
    [ClientRpc(includeOwner = false)]
    private void RpcStopInhalePlayer()
    {
        Managers.Game.Player.GetComponent<IInhalable>().StopInhale();
    }
    #endregion
}
