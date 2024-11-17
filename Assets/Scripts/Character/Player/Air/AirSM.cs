using Mirror;
using UnityEngine;

public class AirSM : PlayerSM
{
    [field: Header("PlayerData")]
    public AirDataSO airdata => (AirDataSO)playerData;

    [field: Header("AirGun")]
    public NewAirGun airGun;

    public Transform armPivot;
    public Transform weaponPoint;
    public LineRenderer lineRenderer;
    
    [field: Header("AirGun Particles")]
    [field: SerializeField] public ParticleSystem inhaleParticle { get; private set; }
    [field: SerializeField] public ParticleSystem exhaleParticle { get; private set; }

    
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
    #endregion

    #region Interaction
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
    #endregion

    #region AirGun
    private void StopGun()
    {
        airGun.Reset();
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
        Managers.Game.Player.GetComponent<IInhalable>().StopInhale();
    }
    
    [Command(requiresAuthority = false)]
    public void CmdShootObject(GameObject obj, Vector2 power)
    {
        if (!obj.TryGetComponent<IInhalable>(out var inhalable)) return;

        var item = obj.GetComponent<NetworkIdentity>();

        if (ReferenceEquals(Managers.Game.Player, obj) || ReferenceEquals(Managers.Game.OtherPlayer, obj))
        {
            RpcShootPlayer(obj, power);
            return;
        }
        
        Managers.Command.AuthorityToServer(item.netId);
        
        inhalable.Shooting(power);
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
    public void CmdStopInhaleParticle()
    {
        RpcStopInhaleParticle();
    }

    [ClientRpc]
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
