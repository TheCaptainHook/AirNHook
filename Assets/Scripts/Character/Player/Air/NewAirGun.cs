using System;
using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using System.Diagnostics; // 
using Debug = UnityEngine.Debug;
public class NewAirGun
{
    private AirSM _air;
    private Animator _animator;
    private Transform _transform;
    private Transform _armPivot;
    private Transform _charPivot;
    private Collider2D _collider;
    private Vector3 _airOffset = new(0, 0.5f);
    private bool _canControl => _air.canControl;
    private bool _canAction => _air.canAction;
    private PlayerInput _playerInput => Managers.Game.playerInput;
    private Camera _mainCamera;
    private Vector2 _mousePosition;
    private bool _rightClick;
    
    // TargetDetections
    private Transform _weaponPoint;
    private Transform _inhalingPoint;
    private ParentConstraint _targetConstraint;
    private ConstraintSource _targetConstraintSource;
    private Collider2D _closestTarget = null;
    private Collider2D _latestTarget = null;
    private LayerMask _objectMask;
    private LayerMask _obstacleMask;
    private float _airGunDistance;
    private float _shortestDistance = float.MaxValue;
    private Coroutine _keepGrapplingCheckCoroutine;
    private bool _sendAuthority;

    // InhaleAction
    private ShakingEffectOnAirGun _shakingEffectOnAirGun => _air.shakingEffectOnAirGun;
    private Rigidbody2D _rigidbody2D => _air.rigidbody2D;
    private Collider2D _inhaleTarget;
    private bool _isAttached;
    public bool _inhaling;
    private GameObject _inhalePermissionObject;
    private bool _inhalingPlayer;
    private bool _delay;
    private float _delayTimer;
    private float _inhalePower = 100f;
    private bool _isIhaleTargetOwned = false;
    private bool _isAttachedToHook;
    private bool _isInhaledHook;
    
    // ShootAction
    private Transform _crossHair;
    private LineRenderer _lineRenderer => _air.lineRenderer;
    private LayerMask _floorLayerMask;
    private LayerMask _predictLineLayerMask;
    private float _shootPower;
    private float _minShootPower;
    private float _maxShootPower;
    private Vector3[] _positions;
    private Vector2 _checkBoxSize = new Vector2(0.2f, 0.2f);
    private LayerMask _halfTileLayerMask;
    private int _numberOfPoints;
    private float _spaceBetweenPoints;
    private float _latestTargetGravityScale;
    private Coroutine _chargingCoroutine;
    private Coroutine _waitForStopCoroutine;
    private WaitForSeconds _waitFor1Seconds = new(0.1f);
    private bool _canInhale = true;
    
    // FlyAction
    private float _flyPower;
    private HookSM _hook;
    private bool _canStick;
    private bool _isStick;
    public bool sticking;
    
    // HookInteraction
    private float _stickToHookSpeed;
    private Vector3 _offset = new(0, -1f);
    private Vector3 _weaponOffset = new(0, 1f);
    private Coroutine _stickToHookCoroutine;
    private WaitForFixedUpdate _waitForFixedUpdate = new();

    // ParticleSystems
    private ParticleSystem _inhaleParticles;
    private ParticleSystem _exhaleParticles;
    
    public NewAirGun(AirSM air)
    {
        _air = air;
        Start();
    }

    private void Start()
    {
        _animator = _air.animator;
        _transform = _air.transform;
        _armPivot = _air.armPivot;
        _charPivot = _air.charPivot;
        _crossHair = _air.crossHair;
        _collider = _air.collider2D;
        _mainCamera = Camera.main;
        _weaponPoint = _air.weaponPoint;
        _inhalingPoint = _air.InhalingPoint;
        _targetConstraintSource = new ConstraintSource
        {
            sourceTransform = _weaponPoint,
            weight = 1f
        };
        _inhaleParticles = _air.inhaleParticle;
        _exhaleParticles = _air.exhaleParticle;
        _halfTileLayerMask = LayerMask.GetMask("Ground/HalfPlatform");
        
        var airData = (AirDataSO)_air.playerData;
        
        _objectMask = airData.objectLayerMask;
        _obstacleMask = airData.obstacleLayerMask;
        _airGunDistance = airData.AirGunDistance;

        _minShootPower = airData.minShootPower;
        _maxShootPower = airData.maxShootPower;
        _numberOfPoints = airData.numberOfPoints;
        _spaceBetweenPoints = airData.spaceBetweenPoints;
        _flyPower = airData.flyPower;
        _stickToHookSpeed = airData.stickToHookSpeed;
        _floorLayerMask = airData.floorLayerMask;
        _predictLineLayerMask = airData.predictLineLayerMask;
        _inhaleTarget = null;

        _positions = new Vector3[_numberOfPoints];
        
        if (!_air.isLocalPlayer) return;

        Managers.Command.itemInhaleCallback += GetPermissionForInhaling;
        SubscribeInput();
    }

    public void OnDisable()
    {
        Managers.Command.itemInhaleCallback -= GetPermissionForInhaling;
        UnSubscribeInput();
    }

    public void Reset()
    {
        StopInhale();
        StopSticking();
        _inhaling = false;
        _isIhaleTargetOwned = false;
        _isAttached = false;
        _canStick = false;
        _isInhaledHook = false;
        _latestTarget = null;
        _inhaleTarget = null;
        _lineRenderer.enabled = false;
        _shortestDistance = float.MaxValue;
        try
        {
            if (_targetConstraint is not null && _targetConstraint.sourceCount != 0)
            {
                _targetConstraint.weight = 0f;
                _targetConstraint.constraintActive = false;
                _targetConstraint.locked = false;
                _targetConstraint.RemoveSource(0);
            }
        }
        catch (Exception) { }
        StopInhaleParticle();
    }

    #region UpdateMethod
    public void Update()
    {
        if (!_air.isLocalPlayer) return;

        if (!_canControl) return;

        RotateGun();
        DetectObject();
        ObjectCheck();
        AnimationPlay();
    }

    public void FixedUpdate()
    {
        if (!_air.isLocalPlayer) return;

        if (!_canControl) return;

        Inhaling();
    }
    #endregion

    #region ObjectCheckForAirGun
    private void DetectObject()
    {
        if (!_rightClick || _isAttached || sticking || !_canInhale || _isInhaledHook || !_canControl || !_canAction)
        {
            _animator.SetBool(GlobalText.INHAILING_ANIMATION_STRING, false);
            StopInhaleParticle();
            _shakingEffectOnAirGun.StopShaking();
            return;
        }

        if (_delay)
        {
            if (_delayTimer <.1f)
            {
                StopInhale();
                _isIhaleTargetOwned = false;
                _inhalePermissionObject = null;
                _sendAuthority = false;
                _inhaling = false;
                _isAttached = false;
                _isInhaledHook = false;
                _latestTarget = null;
                _inhaleTarget = null;
                _delayTimer += Time.deltaTime;
                return;
            }

            _delay = false;
        }

        _animator.SetBool(GlobalText.INHAILING_ANIMATION_STRING, true);
        PlayInhaleParticle();
        _shakingEffectOnAirGun.StartShaking();

        var collisions = Physics2D.OverlapCircleAll(_inhalingPoint.position, _airGunDistance, _objectMask);

        if (collisions.Length <= 1)
        {
            if (_latestTarget == null) return;

            StopInhale();
            _isIhaleTargetOwned = false;
            _inhalePermissionObject = null;
            _sendAuthority = false;
            _inhaling = false;
            _isAttached = false;
            _isInhaledHook = false;
            _latestTarget = null;
            _inhaleTarget = null;
            return;
        }
        
        _closestTarget = null;

        foreach (var collision in collisions)
        {
            if (collision.Equals(_collider)) continue;
            
            var targetDistance = Vector2.Distance(_inhalingPoint.position, collision.transform.position);
            
            if (targetDistance > _shortestDistance) continue;
            
            if (!collision.TryGetComponent<IInhalable>(out var inhalable)) continue;
            
            if (!inhalable.CanInhale()) continue;
            
            if (targetDistance <= 0.8f)
            {
                _closestTarget = collision;
                _shortestDistance = targetDistance;
            }
            else
            {
                var objectVector = (collision.transform.position - _inhalingPoint.position).normalized;
                var weaponVector = _inhalingPoint.transform.right;
                
                var angle = Vector2.Angle(weaponVector, objectVector);
                
                if (angle > 47.5) continue;
                
                var hit = Physics2D.Raycast(_inhalingPoint.position, objectVector, targetDistance, _obstacleMask);
                
                if (Vector2.Distance(_inhalingPoint.position, hit.point) < targetDistance) continue;
                
                _closestTarget = collision;
                _shortestDistance = targetDistance;
            }
        }
        
        if (_closestTarget == null)
        {
            if (_latestTarget == null) return;

            StopInhale();
            _isIhaleTargetOwned = false;
            _inhalePermissionObject =  null;
            _sendAuthority = false;
            _inhaling = false;
            _isAttached = false;
            _isInhaledHook = false;
            _latestTarget = null;
            _inhaleTarget = null;
            return;
        }
        
        if (ReferenceEquals(_latestTarget, _closestTarget))
        {
            if (ReferenceEquals(Managers.Game.OtherPlayer, _latestTarget.gameObject))
            {
                _shortestDistance = float.MaxValue;
                return;
            }
            else if (_inhaleTarget != null && ReferenceEquals(_inhaleTarget, _latestTarget) && _shortestDistance < 0.3f)
            {
                if(_inhaleTarget.TryGetComponent<IInhalable>(out var inhalable))
                    FixInhaleTarget();
            }

            _shortestDistance = float.MaxValue;
            return;
        }
        
        StopInhale();
        _isIhaleTargetOwned = false;
        _inhalePermissionObject = null;
        _sendAuthority = false;
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
        if (!_rightClick || _latestTarget == null || _isAttached || sticking || !_canInhale) return;
        
        if (_hook == null && ReferenceEquals(_latestTarget.gameObject, Managers.Game.OtherPlayer))
        {
            if (_latestTarget.TryGetComponent(out _hook) && _hook.isSwinging)
            {
                if (_keepGrapplingCheckCoroutine != null) return;

                _canStick = false;
                _keepGrapplingCheckCoroutine = _air.StartCoroutine(KeepGrapplingCheck());
            }
        }
        else if (_hook != null && _hook.isSwinging)
        {
            if (!_canStick) return;
            
            StickToHook();
        }
        else
        {
            _hook = null;
            if (_keepGrapplingCheckCoroutine != null)
            {
                _air.StopCoroutine(_keepGrapplingCheckCoroutine);
                _keepGrapplingCheckCoroutine = null;
            }
            _canStick = false;
            StartInhale();
        }
    }
    #endregion

    #region AirGun
    #region Inhaling
    private void StartInhale()
    {
        if (!_canControl || !_canAction) return;

        if (!_canInhale || (_inhaling && ReferenceEquals(_latestTarget, _inhaleTarget))) return;

        if (!_latestTarget.TryGetComponent<IInhalable>(out var inhalable) && !inhalable.CanInhale()) return;

        _inhaleTarget = _latestTarget;
        _inhaling = true;

        if (ReferenceEquals(Managers.Game.OtherPlayer, _inhaleTarget.gameObject)) return;

        // NetworkIdentity identity = _inhaleTarget.transform.root.TryGetComponent(out NetworkIdentity networkIdentity) ? networkIdentity : _inhaleTarget.TryGetComponent(out NetworkIdentity networkIdentity2) ? networkIdentity2 : null;
        NetworkIdentity identity = _inhaleTarget.TryGetComponent(out NetworkIdentity component) ? component : null;
        
        if(identity == null) return;
        
        Managers.Command.TryInhaleItem(_air.gameObject, identity.netId);
    }

    private void GetPermissionForInhaling(GameObject permissionObject, bool value)
    {
        if (!_inhaling)
        {
            _inhalePermissionObject = null;
            return;
        }

        if (value)
        {
            _inhalePermissionObject = permissionObject;
        }
        else
        {
            _inhaling = false;
            StopInhale();
        }
    }

    private void Inhaling()
    {

        if (!_canControl || !_canAction)  return;

        if (!_inhaling || _isAttached) return;

        if (Managers.Game.OtherPlayer != null && ReferenceEquals(Managers.Game.OtherPlayer, _inhaleTarget.gameObject))
        {
            if (_inhalingPlayer) return;

            _air.CmdInhalePlayer();
            _inhalingPlayer = true;
            return;
        }

        if (_inhalePermissionObject != null && !ReferenceEquals(_inhalePermissionObject, _inhaleTarget.gameObject)) return;

        if (!_isIhaleTargetOwned)
        {
        
            if (!_inhaleTarget.GetComponent<NetworkIdentity>().isOwned)
            {
                _isIhaleTargetOwned = false;
                return;
            }

            _isIhaleTargetOwned = true;
            _inhaleTarget.GetComponent<IInhalable>().Inhalation(_weaponPoint);
        
        }

        if (!_inhaleTarget.TryGetComponent<IInhalable>(out var inhalable)) return;

        var targetRigdbody = _inhaleTarget.GetComponent<Rigidbody2D>();
        var direction = (_weaponPoint.position - _inhaleTarget.transform.position).normalized;
        var power = 50 * _inhalePower * targetRigdbody.mass;

        targetRigdbody.drag = 10f;
        targetRigdbody.gravityScale = 0f;
        targetRigdbody.AddForce(direction * power * Time.fixedDeltaTime);
    }

    private void StopInhale()
    {
        _inhaling = false;
        _inhalePermissionObject = null;
        _shakingEffectOnAirGun.StopShaking();

        if (_chargingCoroutine != null)
        {
            _air.StopCoroutine(_chargingCoroutine);
            _lineRenderer.enabled = false;
        }
        
        _isAttached = false;
        _isAttachedToHook = false;
        _inhalingPlayer = false;
        _isInhaledHook = false;
        _lineRenderer.enabled = false;
        _crossHair.gameObject.SetActive(false);

        if (_inhaleTarget == null) return;

        try
        {
            if (_targetConstraint != null && _targetConstraint.sourceCount != 0)
            {
                _targetConstraint.weight = 0f;
                _targetConstraint.constraintActive = false;
                _targetConstraint.locked = false;
                _targetConstraint.RemoveSource(0);
            }
        }
        catch (Exception) { }

        try
        {
            if (ReferenceEquals(Managers.Game.OtherPlayer, _inhaleTarget.gameObject))
                _air.CmdStopInhalePlayer();
            else
                _inhaleTarget.GetComponent<IInhalable>().StopInhale(_air.gameObject);
        }
        catch (Exception)
        {
            _inhaleTarget = null;
        }
    }

    private void FixInhaleTarget()
    {
        //260223 BlinkingButton
        if(_inhaleTarget.TryGetComponent(out Rigidbody2D component)) if(component.isKinematic) return;
        //260223 BlinkingButton
        if (!_inhaling) return;

        if (!_inhaleTarget.GetComponent<NetworkIdentity>().isOwned) return;

        if (_isAttached) return;

        _isAttached = true;

        if (!_inhaleTarget.TryGetComponent(out _targetConstraint))
            _targetConstraint = _inhaleTarget.gameObject.AddComponent<ParentConstraint>();

        _targetConstraint.AddSource(_targetConstraintSource);
        _targetConstraint.translationAxis = Axis.X | Axis.Y | Axis.Z;
        _targetConstraint.rotationAxis = Axis.None;
        _targetConstraint.weight = 1f;
        _targetConstraint.locked = true;
        _targetConstraint.constraintActive = true;
        _inhaleTarget.GetComponent<IInhalable>().Fixed(true);
    }

    public void HookAttached()
    {
        if (_inhaleTarget == null || !ReferenceEquals(_inhaleTarget.gameObject, Managers.Game.OtherPlayer)) return;

        _isAttached = true;
        _isInhaledHook = true;
    }
    #endregion

    #region InteractionWithHook
    private void StickToHook()
    {
        _canStick = false;
        sticking = true;
        
        _stickToHookCoroutine = _air.StartCoroutine(Co_StickHook());
    }
    
    private IEnumerator Co_StickHook()
    {
        var stick = false;
        sticking = true;
        _rigidbody2D.drag = 10f;
        while (true)
        {
            if (!stick)
            {
                yield return _waitForFixedUpdate;
                
                try
                {
                    _rigidbody2D.gravityScale = 0f;
                    _rigidbody2D.velocity = Vector2.zero;

                    var objectVector = (_hook.transform.position - _weaponPoint.position).normalized;
                    var targetDistance = Vector2.Distance(_weaponPoint.position, _hook.transform.position);

                    var hit = Physics2D.Raycast(_weaponPoint.position, objectVector, targetDistance - 0.1f, _obstacleMask);
                
                    if (hit || !_hook.isSwinging)
                        StopSticking();
                    
                    var direction = (_hook.transform.position + _offset - _transform.position).normalized;

                    _rigidbody2D.AddForce(direction * _stickToHookSpeed);
                    var dir = (_hook.transform.position + new Vector3(0, 0.5f) - _armPivot.position).normalized;
                    var rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                    _armPivot.rotation = Quaternion.AngleAxis(rotZ, Vector3.forward);

                    if (Mathf.Abs(rotZ) > 90f)
                    {
                        rotZ = -rotZ;
                        _charPivot.rotation = Quaternion.Euler(0f, 180f, 0f);
                        _armPivot.rotation = Quaternion.Euler(-190f, 0f, rotZ);
                    }
                    else
                    {
                        _charPivot.rotation = Quaternion.identity;
                    }
                
                    if (Vector2.Distance(_hook.transform.position, _transform.position + new Vector3(0, 1.0f)) <= 0.25f)
                        stick = true;
                }
                catch (NullReferenceException) { StopSticking(); }
            }
            else
            {
                yield return null;
                try
                {
                    if (!_hook.isSwinging)
                        StopSticking();

                    _rigidbody2D.velocity = Vector2.zero;
                    _transform.position = _hook.transform.position + _offset;
                    
                    if (_air.IsStick()) continue;

                    _air.StickToHook();
                    _rigidbody2D.drag = 0f;
                    _hook.CmdAirAttached(true);
                    _isStick = true;
                    _isAttachedToHook = true;
                }
                catch (NullReferenceException) { StopSticking(); }
            }
        }
    }
    
    private void FlyAway()
    {
        _isStick = false;
        _canStick = false;
        sticking = false;
        _isAttachedToHook = false;
        _rigidbody2D.drag = 0f;
        _hook.CmdAirAttached(false);
        _air.StopCoroutine(_stickToHookCoroutine);
        _stickToHookCoroutine = null;
        _air.StickJump();
        
        Vector2 mousePos = _mainCamera.ScreenToWorldPoint(_mousePosition);
        Vector2 dir = (mousePos - (Vector2)_transform.position).normalized;

        if (_chargingCoroutine != null)
        {
            _air.StopCoroutine(_chargingCoroutine);
            _chargingCoroutine = null;
        }
        _lineRenderer.enabled = false;
        _crossHair.gameObject.SetActive(false);

        _canInhale = false;
        _air.StartCoroutine(Co_CoolDown());
        _hook = null;

        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.gravityScale = 3f;
        _rigidbody2D.AddForce(dir * -_shootPower, ForceMode2D.Impulse);
    }

    private void StopSticking()
    {
        if (_keepGrapplingCheckCoroutine != null)
        {
            _air.StopCoroutine(_keepGrapplingCheckCoroutine);
            _keepGrapplingCheckCoroutine = null;
        }
        _isAttachedToHook = false;
        _rigidbody2D.drag = 0f;
        
        if (_stickToHookCoroutine == null) return;
        _air.StopCoroutine(_stickToHookCoroutine);
        _stickToHookCoroutine = null;
        _isStick = false;
        _canStick = false;
        sticking = false;

        if (_chargingCoroutine != null)
        {
            _air.StopCoroutine(_chargingCoroutine);
            _chargingCoroutine = null;
        }
        _lineRenderer.enabled = false;
        _crossHair.gameObject.SetActive(false);

        if (_hook != null)
            _hook.CmdAirAttached(false);
        else
            if (Managers.Game.OtherPlayer.TryGetComponent(out HookSM hook))
                hook.CmdAirAttached(false);
        _hook = null;
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.gravityScale = 3f;
    }
    
    private IEnumerator KeepGrapplingCheck()
    {
        yield return new WaitForSeconds(0.7f);
        _canStick = true;
        _keepGrapplingCheckCoroutine = null;
    }
    #endregion
    
    #region ShootingAction
    private void RotateGun()
    {
        if (!_canControl || !_canAction) return;

        if (!_rightClick || sticking) return;
        
        var mousePos = _mainCamera.ScreenToWorldPoint(_mousePosition);
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
    
    private void Charging()
    {
        if (!_isAttachedToHook && (!_isAttached && !_isInhaledHook)) return;

        if (!_canControl) return;

        _chargingCoroutine = _air.StartCoroutine(Co_PowerCharging());
    }
    
    private IEnumerator Co_PowerCharging()
    {
        _shootPower = _minShootPower;
        _lineRenderer.enabled = true;
        var maxPower = _isAttachedToHook ? _flyPower : _maxShootPower;
        
        var chargingTime = 0f;
        while (true)
        {
            if (_shootPower < maxPower)
            {
                chargingTime += Time.deltaTime;

                _shootPower = chargingTime * (maxPower - _minShootPower) + _minShootPower;
            }
            else if (_shootPower > maxPower)
            {
                _shootPower = maxPower;
            }
            
            if (_isAttachedToHook)
                FlyPredict();
            else
                ProjectilePredict();
            yield return null;
        }
    }
    
    private void ShootObject()
    {
        if ((!_isAttached && !_isInhaledHook) || _shootPower <= 0f) return;
        
        if (_inhaleTarget == null)
        {
            StopInhale();
            return;
        }
        
        if (_chargingCoroutine != null)
        {
            _air.StopCoroutine(_chargingCoroutine);
            _chargingCoroutine = null;
        }
        _air.StartCoroutine(Co_CoolDown());
        StopInhale();

        _lineRenderer.enabled = false;
        _crossHair.gameObject.SetActive(false);

        Managers.Game.cameraShake.RequestShake(_air.gameObject, 5f, 0.2f);
        var force = _weaponPoint.right * _shootPower * _inhaleTarget.GetComponent<Rigidbody2D>().mass;

        if (ReferenceEquals(Managers.Game.OtherPlayer, _inhaleTarget.gameObject))
        {
            _air.CmdShootPlayer(Managers.Game.OtherPlayer, force);
        }
        else
        {
            if (Physics2D.OverlapBox(_inhaleTarget.transform.position, Vector2.one, 0f, _obstacleMask))
                _inhaleTarget.transform.position = _air.transform.position + (Vector3.up / 2);

            _inhaleTarget.GetComponent<IInhalable>().Shooting(force);
            _inhaleTarget.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        }
        _inhaleTarget = null;
        _isAttached = false;
        _isInhaledHook = false;
        _inhaling = false;
        _shootPower = 0f;
        Managers.Game.cameraShake.RequestShake(_air.gameObject, 7f, 0.18f);
    }
    
    private IEnumerator Co_CoolDown()
    {
        _canInhale = false;
        
        yield return new WaitForSeconds(1.5f);
        
        _canInhale = true;
    }
    
    private void ProjectilePredict()
    {
        var pos = _air.transform.position + _weaponOffset;
        var firstHit = Physics2D.Raycast(pos, _weaponPoint.position - pos, Vector2.Distance(pos, _weaponPoint.position), _obstacleMask);

        if (firstHit && !firstHit.collider.Equals(_inhaleTarget))
            return;

        int i;
        for (i = 0; i < _numberOfPoints; i++)
        {
            Vector3 point = PointPosition(i * _spaceBetweenPoints);
            var hit = Physics2D.OverlapBox(point, _checkBoxSize, 0, _predictLineLayerMask);

            if (hit && !hit.gameObject.Equals(_inhaleTarget.gameObject))
            {
                var isHalfTile = (_halfTileLayerMask & (1 << hit.gameObject.layer)) > 0;
                var isUpVector = i <= 0 || (point - _positions[i - 1]).y <= 0;

                if (!isHalfTile || isUpVector)
                    break;
            }

            _positions[i] = point;
        }

        if (i < 5)
        {
            _lineRenderer.positionCount = 0;
            _crossHair.gameObject.SetActive(false);
            return;
        }
        
        _lineRenderer.positionCount = i - 3;
        _lineRenderer.SetPositions(_positions);
        _crossHair.position = _positions[i - 1];
        _crossHair.gameObject.SetActive(true);
    }

    private void FlyPredict()
    {
        int i;
        var grabbedItem = Managers.Game.OtherPlayer.GetComponent<HookSM>().grabbedItem;
        for (i = 0; i < _numberOfPoints; i++)
        {
            Vector3 point = PointPosition(i * _spaceBetweenPoints, true);
            var hit = Physics2D.OverlapBox(point, _checkBoxSize, 0, _predictLineLayerMask);

            if (hit && (grabbedItem == null || !hit.gameObject.Equals(grabbedItem.gameObject)))
            {
                var isHalfTile = (_halfTileLayerMask & (1 << hit.gameObject.layer)) > 0;
                var isUpVector = i <= 0 || (point - _positions[i - 1]).y <= 0;

                if (!isHalfTile || isUpVector)
                    break;
            }

            _positions[i] = point;
        }

        if (i < 5)
        {
            _lineRenderer.positionCount = 0;
            _crossHair.gameObject.SetActive(false);
            return;
        }

        _lineRenderer.positionCount = i - 3;
        _lineRenderer.SetPositions(_positions);
        _crossHair.position = _positions[i - 1];
        _crossHair.gameObject.SetActive(true);
    }

    private Vector2 PointPosition(float t, bool isFly = false)
    {
        Vector2 dir;
        Vector2 position;

        if (isFly)
        {
            var mousePos = _mainCamera.ScreenToWorldPoint(_mousePosition);
            dir = (_air.transform.position - mousePos).normalized;

            position = (Vector2)(_air.transform.position + _airOffset)
                          + (dir * (_shootPower * t))
                          + (Physics2D.gravity * (0.5f * (t * t) * 3f));
        }
        else
        {
            dir = _weaponPoint.transform.right;

            position = (Vector2)_weaponPoint.position
                          + (dir * (_shootPower * t))
                          + (Physics2D.gravity * (0.5f * (t * t) * _latestTargetGravityScale));
        }
        
        return position;
    }
    #endregion

    private void WaitForStop()
    {
        if (_waitForStopCoroutine != null) return;

        _waitForStopCoroutine = _air.StartCoroutine(Co_WaitForStop());
    }

    private IEnumerator Co_WaitForStop()
    {
        yield return _waitFor1Seconds;

        DontWaitForStop();
        _waitForStopCoroutine = null;
    }

    private void DontWaitForStop()
    {
        StopInhaleParticle();
        StopInhale();
        StopSticking();
        _hook = null;
    }
    #endregion

    #region Animations
    private void AnimationPlay()
    {
        _animator.SetBool(GlobalText.FLYING_ANIMATION_STRING, sticking);
        _animator.SetBool(GlobalText.AIR_ATTACHED_ANIMATION_STRING, _isAttachedToHook);
        _animator.SetBool(GlobalText.HOOK_INHALED_ANIMATION_STRING, _isInhaledHook);
    }
    #endregion

    #region Particles
    private void PlayInhaleParticle()
    {
        if (_inhaleParticles.isPlaying) return;

        _inhaleParticles.Play();
        _air.CmdPlayInhaleParticle();
    }

    private void StopInhaleParticle()
    {
        if (!_inhaleParticles.isPlaying) return;
        
        _inhaleParticles.Stop();
        _air.CmdStopInhaleParticle();
    }
    #endregion
    
    #region Input
    private void SubscribeInput()
    {
        _playerInput.playerActions.Action.started += OnMainActionStarted;
        _playerInput.playerActions.Action.canceled += OnMainActionCanceled;
        _playerInput.playerActions.SubAction.started += OnSubActionStarted;
        _playerInput.playerActions.SubAction.canceled += OnSubActionCanceled;
        _playerInput.playerActions.Look.performed += OnLook;
    }

    private void UnSubscribeInput()
    {
        _playerInput.playerActions.Action.started -= OnMainActionStarted;
        _playerInput.playerActions.Action.canceled -= OnMainActionCanceled;
        _playerInput.playerActions.SubAction.started -= OnSubActionStarted;
        _playerInput.playerActions.SubAction.canceled -= OnSubActionCanceled;
        _playerInput.playerActions.Look.performed -= OnLook;
    }

    private void OnMainActionStarted(InputAction.CallbackContext context)
    {
        if (!_canControl || !_canAction) return;
        
        Charging();
    }

    private void OnMainActionCanceled(InputAction.CallbackContext context)
    {
        if (!_canControl || !_canAction) return;
        
        _animator.SetTrigger(GlobalText.EXHAILING_ANIMATION_STRING);
        if (_isStick)
            FlyAway();
        else
            ShootObject();
    }

    private void OnSubActionStarted(InputAction.CallbackContext context)
    {
        _delay = true;
        _delayTimer = 0f;
        _rightClick = true;
    }
    
    private void OnSubActionCanceled(InputAction.CallbackContext context)
    {
        _rightClick = false;

        if (_chargingCoroutine != null)
            WaitForStop();
        else
            DontWaitForStop();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        _mousePosition = context.ReadValue<Vector2>();
    }
    #endregion
}
