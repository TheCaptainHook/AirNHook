using System;
using System.Collections;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class HookSM : PlayerSM, IInhalable
{
    [field: Header("PlayerData")]
    private HookDataSO _hookData => (HookDataSO)playerData;

    [field: Header("Grapple")]
    public NewGrappling grappling;
    public Transform grabbedItem { get; private set; }
    [SyncVar] public bool isSwinging;
    [SyncVar] public bool grappleAttached;
    [SyncVar] public bool isAirAttached;
    
    [field: SerializeField] public GameObject hookAnchor { get; private set; }
    [field: SerializeField] public Transform hookSprite { get; private set; }
    [field: SerializeField] public Transform hookStartPos { get; private set; }
    [field: SerializeField] public Transform ropeStartPos { get; private set; }
    [field: SerializeField] public LineRenderer ropeRenderer { get; private set; }
    [field: SerializeField] public LayerMask hookLayerMask { get; private set; }
    
    [field: Header("Hook Particles")]
    [field: SerializeField] public ParticleSystem hookParticle { get; private set; }
    
    [field: Header("Inhale")]
    private Transform _fixedPoint;
    private Coroutine _inhaleCoroutine;
    private float _inhalePower;
    private float _gravityScale;
    [SyncVar] private bool _isFixed;
    private bool _isShot;
    [SyncVar] private bool _canInteract;
    private WaitForFixedUpdate _waitForFixedUpdate = new();
    
    protected override void Awake()
    {
        stateMachine = new HookStateMachine(this);
        animationData = new HookAnimationData();
    }

    protected override void Start()
    {
        base.Start();
        //grappling = new NewGrappling(this);
        
        _inhalePower = _hookData.inhalePower;
        _gravityScale = rigidbody2D.gravityScale;
        
        if (!isLocalPlayer) return;

        characterConstraintSource = new ConstraintSource
        {
            sourceTransform = transform,
            weight = 1f
        };
        Managers.Command.itemGrabCallback += GrabItemNet;
        //Managers.Command.itemReleaseCallback += ReleaseItemNet;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        grappling.OnDisable();
        
        Managers.Command.itemGrabCallback -= GrabItemNet;
        //Managers.Command.itemReleaseCallback -= ReleaseItemNet;
    }

    #region UpdateMethod
    protected override void Update()
    {
        base.Update();
        //grappling.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //grappling.PhysicsUpdate();
    }
    #endregion

    #region Ground
    protected override void GroundCheck()
    {
        if (!isSwinging)
        {
            for (var i = -1; i < 2; i++)
            {
                _hit = Physics2D.Raycast(transform.position + (Vector3.right * (0.4f * i)) + (Vector3.up * 0.2f), Vector2.down, 0.4f, playerData.floorLayerMask);
                if (!_hit) continue;

                isHalfPlatform = _halfPlatformLayer == (_halfPlatformLayer | (1 << _hit.transform.gameObject.layer));
                //isHalfPlatform = (1 << _hit.transform.gameObject.layer) == _halfPlatformLayer;
                if (isHalfPlatform && !isDownThroughPlatform)
                {
                    rigidbody2D.excludeLayers = 0;
                    collider2D.forceReceiveLayers = _defaultForceReceiveLayer;
                }
                else
                {
                    rigidbody2D.excludeLayers = _halfPlatformLayer;
                    collider2D.forceReceiveLayers = ~_halfPlatformLayer;
                }

                if (isGround) return;

                CmdLandParticlePlay();
                isGround = true;
                coyoteTimeCount = _coyoteTime;
                Physics2D.SyncTransforms();
                return;
            }
        }
        isHalfPlatform = false;
        rigidbody2D.excludeLayers = _halfPlatformLayer;
        collider2D.forceReceiveLayers = ~_halfPlatformLayer;
        isGround = false;
        coyoteTimeCount -= Time.deltaTime;
    }
    #endregion

    #region Interaction
    protected override IEnumerator DetectInteraction()
    {
        var shortestDistance = float.MaxValue;
        var offset = new Vector3(0, 0.45f);
        Collider2D closestTarget = null;
        
        while (true)
        {
            yield return null;
            if (grabbedItem is not null)
                continue;
            
            var collisions = Physics2D.OverlapCircleAll(transform.position + offset, detectDistance, interactableLayerMask);

            if (collisions.Length == 0)
            {
                if (latestTarget is null)
                {
                    Managers.UI.HideUI<UI_ShowEButton>();
                    continue;
                }

                try
                {
                    if (latestTarget.TryGetComponent<IInteractable>(out var none))
                        none.HideEButton();
                }
                catch (Exception)
                {
                    latestTarget = null;
                    Managers.UI.HideUI<UI_ShowEButton>();
                }
                
                latestTarget = null;
                continue;
            }
            
            closestTarget = null;

            foreach (var collision in collisions)
            {
                if (!collision.TryGetComponent<IInteractable>(out var inhalable)) continue;
                
                if (!inhalable.CanInteract()) continue;

                var pos = transform.position + offset;
                var objectVector = (collision.transform.position - pos).normalized;
                var targetDistance = Vector2.Distance(transform.position + offset, collision.transform.position);

                var hit = Physics2D.Raycast(pos, objectVector, targetDistance, obstacleMask);
                
                if (Vector2.Distance(pos, hit.point) < targetDistance - 0.2f) continue;

                if (targetDistance < shortestDistance)
                {
                    shortestDistance = targetDistance;
                    closestTarget = collision;
                }
            }

            if (closestTarget is null)
            { 
                try
                {
                    if (latestTarget is not null && latestTarget.TryGetComponent<IInteractable>(out var other))
                        other.HideEButton();
                }
                catch (MissingReferenceException)
                {
                    latestTarget = null;
                    Managers.UI.HideUI<UI_ShowEButton>();
                }
                latestTarget = null;
                shortestDistance = float.MaxValue;
                continue;
            }
            
            if (latestTarget is not null)
            {
                if (ReferenceEquals(latestTarget, closestTarget))
                {
                    shortestDistance = float.MaxValue;
                    continue;
                }

                try
                {
                    if (latestTarget.TryGetComponent<IInteractable>(out var other))
                        other.HideEButton();
                }
                catch (MissingReferenceException)
                {
                    latestTarget = null;
                    Managers.UI.HideUI<UI_ShowEButton>();
                }
            }

            latestTarget = closestTarget;
            try
            {
                if (latestTarget.TryGetComponent<IInteractable>(out var newTarget))
                    newTarget.ShowEButton();
            }
            catch (MissingReferenceException)
            {
                latestTarget = null;
                Managers.UI.HideUI<UI_ShowEButton>();
            }
            shortestDistance = float.MaxValue;
        }
    }
    
    protected override void Interaction()
    {
        if (grabbedItem is not null)
        {
            ReleaseItem();
        }
        else if (latestTarget is not null)
        {
            if (!latestTarget.TryGetComponent<IInteractable>(out var interactable)) return;

            if (interactable.GetObjectType() == ObjectTypeEnum.Grab)
                Managers.Command.TryGrabItem(gameObject, latestTarget.GetComponent<NetworkIdentity>().netId);
            else
                interactable.Interaction(transform);
        }
    }
    
    private void GrabItemNet(NetworkIdentity item, bool value)
    {
        if (!value) return;
        
        if (!item.TryGetComponent<IInteractable>(out var interactable)) return;

        if (interactable.GetObjectType() == ObjectTypeEnum.Grab)
        {
            grabbedItem = item.transform;
            var constraint = grabbedItem.GetComponent<ParentConstraint>();

            constraint.weight = 1f;
            constraint.AddSource(grabSource);
            constraint.translationAxis = Axis.X | Axis.Y | Axis.Z;
            constraint.rotationAxis = Axis.X | Axis.Y | Axis.Z;
            constraint.locked = true;
            constraint.constraintActive = true;

            animator.SetBool(GlobalText.GRABBING_ANIMATION_STRING, true);
        }

        interactable.Interaction(transform);
        interactable.HideEButton();
    }

    public void ReleaseItem()
    {
        var constraint = grabbedItem.GetComponent<ParentConstraint>();

        constraint.locked = false;
        constraint.constraintActive = false;
        constraint.weight = 0f;
        constraint.RemoveSource(0);

        grabbedItem.GetComponent<IInteractable>().Interaction(transform);
        var itemRigidbody = grabbedItem.GetComponent<Rigidbody2D>();
        itemRigidbody.velocity = rigidbody2D.velocity;
        itemRigidbody.angularVelocity = 0f;
        grabbedItem = null;
        animator.SetBool(GlobalText.GRABBING_ANIMATION_STRING, false);
    }

    public Transform GetGrabbedItem()
    {
        return grabbedItem;
    }
    #endregion

    #region Dead
    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        if (invincible) return;
        
        base.TakeDamage(damageType);

        grappling.Reset();

        if (grabbedItem is not null)
            Interaction();
    }
    #endregion

    #region Hook
    public void ThrowHook()
    {
        isSwinging = true;
        stateMachine.ChangeState(((HookStateMachine)stateMachine).GrapplingState);
        PlayHookParticle();
    }
    
    public void WithdrawHook()
    {
        isSwinging = false;
        stateMachine.ChangeState(((HookStateMachine)stateMachine).GrapplingJumpState);
    }
    #endregion

    #region Inhalable
    public void Inhalation(Transform accesor)
    {
        if (!isLocalPlayer) return;

        canControl = false;
        _fixedPoint = accesor;

        Debug.Log(_inhaleCoroutine);
        if (_inhaleCoroutine is not null) return;

        _inhaleCoroutine = StartCoroutine(Co_Inhale());
    }

    private IEnumerator Co_Inhale()
    {
        while (true)
        {
            Debug.Log("a");
            if (!_isFixed)
            {
                Debug.Log("b");
                yield return _waitForFixedUpdate;

                if (_fixedPoint is null) break;
                
                var direction = (_fixedPoint.position - transform.position).normalized;
                var power = _inhalePower * Time.fixedDeltaTime;
                rigidbody2D.gravityScale = 0f;
                rigidbody2D.AddForce(direction * power);

                if (Vector2.Distance(_fixedPoint.position, transform.position) > 0.3f) continue;
                
                _isFixed = true;
                characterConstraint.weight = 1f;
                characterConstraint.AddSource(grabSource);
                characterConstraint.translationAxis = Axis.X | Axis.Y | Axis.Z;
                characterConstraint.locked = true;
                characterConstraint.constraintActive = true;

                CmdHookAttachedToAir();
                stateMachine.ChangeState(((HookStateMachine)(stateMachine)).InhaledState);
            }
            else
            {
                Debug.Log("c");
                yield return null;
                rigidbody2D.drag = 0f;
                rigidbody2D.velocity = Vector2.zero;
                
                if (_fixedPoint is null) break;
                transform.position = _fixedPoint.position;
            }
        }

        Debug.Log("Inhale Coroutine End");
        _inhaleCoroutine = null;
    }
    
    public void StopInhale()
    {
        if (_inhaleCoroutine is not null)
        {
            StopCoroutine(_inhaleCoroutine);
            _inhaleCoroutine = null;
        }

        if (_isFixed && !_isShot)
            stateMachine.ChangeState(isGround ? stateMachine.IdleState : stateMachine.FallingState);

        if (characterConstraint.sourceCount != 0)
        {
            characterConstraint.weight = 0f;
            characterConstraint.constraintActive = false;
            characterConstraint.locked = false;
            characterConstraint.RemoveSource(0);
        }

        Fixed(false);
        _isShot = false;
        canControl = true;
        _fixedPoint = null;
        rigidbody2D.gravityScale = _gravityScale;
    }

    public void Fixed(bool value)
    {
        _isFixed = value;
        _canInteract = !value;
    }

    public void Inhaling(bool value)
    {
        _canInteract = !value;
    }

    public void Shooting(Vector2 force)
    {
        _isShot = true;
        StopInhale();
        stateMachine.ChangeState(((HookStateMachine)(stateMachine)).InhaledShotState);
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(force, ForceMode2D.Impulse);
    }

    public bool CanInhale()
    {
        return true;
    }
    #endregion

    #region NetworkCommand
    [Command(requiresAuthority = false)]
    public void CmdAirAttached(bool value)
    {
        RpcAirAttached(value);
        animator.SetBool(GlobalText.SWINGING_WITH_AIR_ANIMATION_STRING, value);
    }

    [ClientRpc]
    private void RpcAirAttached(bool value)
    {
        isAirAttached = value;
        
        animator.SetBool(GlobalText.SWINGING_WITH_AIR_ANIMATION_STRING, isAirAttached);
    }

    [Command(requiresAuthority = false)]
    public void CmdHookAttachedToAir()
    {
        RpcHookAttachedToAir();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcHookAttachedToAir()
    {
        Managers.Game.Player.GetComponent<AirSM>().HookAttached();
    }
    #endregion

    #region Particles
    public void PlayHookParticle()
    {
        hookParticle.Play();
    }
    #endregion
}
