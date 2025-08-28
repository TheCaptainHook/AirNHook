using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class AirSM : PlayerSM
{
    [field: Header("PlayerData")]
    public AirDataSO airdata => (AirDataSO)playerData;

    [field: Header("AirGun")]
    public NewAirGun airGun;
    public ShakingEffectOnAirGun shakingEffectOnAirGun;

    public Transform armPivot;
    public Transform weaponPoint;
    public Transform InhalingPoint;
    public Transform crossHair;
    public LineRenderer lineRenderer;
    
    [field: Header("AirGun Particles")]
    [field: SerializeField] public ParticleSystem inhaleParticle { get; private set; }
    [field: SerializeField] public ParticleSystem exhaleParticle { get; private set; }
    [SyncVar] public bool isInhaleParticleOn;

    private IInteractable _airGunMountObj = null;

    protected override void Awake()
    {
        stateMachine = new AirStateMachine(this);
        animationData = new AirAnimationData();
    }

    protected override void Start()
    {
        base.Start();
        airGun = new NewAirGun(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        airGun.OnDisable();
    }

    #region UpdateMethod
    protected override void Update()
    {
        base.Update();
        airGun.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        airGun.FixedUpdate();
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

            if (_airGunMountObj != null || isControlObj)
                continue;

            var collisions =
                Physics2D.OverlapCircleAll(transform.position + offset, detectDistance, interactableLayerMask);

            if (collisions.Length == 0)
            {
                if (latestTarget == null) continue;

                latestTarget = null;
                Managers.UI.HideUI<UI_ShowEButton>();
                continue;
            }

            closestTarget = null;

            foreach (var collision in collisions)
            {
                if (collision.TryGetComponent<IInteractable>(out var interactable) && !interactable.CanInteract()) continue;
                if (interactable != null)
                {
                    if (interactable.GetObjectType() == ObjectTypeEnum.Grab) continue;
                    //if (interactable.GetObjectType() == ObjectTypeEnum.Mount) continue;
                }

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
            if (closestTarget == null)
            {
                if (latestTarget != null)
                    Managers.UI.HideUI<UI_ShowEButton>();

                latestTarget = null;
                shortestDistance = float.MaxValue;
                continue;
            }

            if (latestTarget != null)
            {
                if (ReferenceEquals(latestTarget, closestTarget))
                {
                    shortestDistance = float.MaxValue;
                    continue;
                }

                Managers.UI.HideUI<UI_ShowEButton>();
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
        if (_airGunMountObj != null)
        {
            _airGunMountObj.Interaction(transform);
            _airGunMountObj = null;
            return;
        }

        if (latestTarget == null)
            return;

        if (!latestTarget.TryGetComponent<IInteractable>(out var interactable))
            return;

        if (interactable.GetObjectType() == ObjectTypeEnum.AirGun)
            _airGunMountObj = interactable;

        if (interactable.GetObjectType() == ObjectTypeEnum.Control)
            isControlObj = !isControlObj;

        interactable.Interaction(transform);
    }

    public bool IsStick()
    {
        var airStateMachine = (AirStateMachine)stateMachine;

        return airStateMachine.CurrentState == airStateMachine.StickAtHookState;
    }
    
    public void StickToHook()
    {
        stateMachine.ChangeState(((AirStateMachine)stateMachine).StickAtHookState);
    }

    public void StopSticking()
    {
        airGun.Reset();
    }

    public void StickJump()
    {
        stateMachine.ChangeState(((AirStateMachine)stateMachine).StickJumpState);
    }
    #endregion

    #region Dead
    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        base.TakeDamage(damageType);
        StopGun();
    }

    protected override void TrySuicide(InputAction.CallbackContext context)
    {
        if (!canControl) return;

        canMovable = false;
        armPivot.localRotation = Quaternion.Euler(0f, 0f, 0f);
        stateMachine.ChangeState(stateMachine.SuicideState);
    }
    #endregion

    #region AirGun
    public void StopGun()
    {
        airGun.Reset();
    }

    public void HookAttached()
    {
        airGun.HookAttached();
    }
    #endregion

    #region NetworkCommand
    [Command(requiresAuthority = false)]
    public void CmdInhalePlayer()
    {
        RpcInhalePlayer();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcInhalePlayer()
    {
        Managers.Game.Player.GetComponent<IInhalable>().Inhalation(weaponPoint);
    }

    [Command(requiresAuthority = false)]
    public void CmdStopInhalePlayer()
    {
        RpcStopInhalePlayer();
    }
    
    [ClientRpc(includeOwner = false)]
    private void RpcStopInhalePlayer()
    {
        Managers.Game.Player.GetComponent<IInhalable>().StopInhale(gameObject);
    }
    
    [Command(requiresAuthority = false)]
    public void CmdShootPlayer(GameObject obj, Vector2 power)
    {
        if (!obj.TryGetComponent<IInhalable>(out var inhalable)) return;
    
        if (ReferenceEquals(Managers.Game.Player, obj) || ReferenceEquals(Managers.Game.OtherPlayer, obj))
        {
            RpcShootPlayer(obj, power);
            return;
        }
    }
    
    [ClientRpc(includeOwner = false)]
    private void RpcShootPlayer(GameObject player, Vector2 power)
    {
        player.GetComponent<IInhalable>().Shooting(power);
    }
    #endregion

    #region Particles
    public void PlayInhaleParticle()
    {
        inhaleParticle.Play();
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayInhaleParticle()
    {
        RpcPlayInhaleParticle();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcPlayInhaleParticle()
    {
        inhaleParticle.Play();
    }

    [Command(requiresAuthority = false)]
    public void CmdStopInhaleParticle()
    {
        RpcStopInhaleParticle();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcStopInhaleParticle()
    {
        inhaleParticle.Stop();
    }
    
    public void PlayExhaleParticle()
    {
        exhaleParticle.Play();
    }
    #endregion
}
