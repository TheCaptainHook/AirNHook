using System;
using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerSM : NetworkBehaviour, IDamageable
{
    [field: Header("PlayerData")]
    [field: SerializeField] public PlayerDataSO playerData { get; protected set; }
    public bool canControl;
    public bool canMovable;
    public bool invincible;
    [field: SerializeField] public Transform charPivot { get; private set; }
    protected float _coyoteTime => playerData.coyoteTime;
    public float coyoteTimeCount;
    private bool _emoteOnCoolDown;
    protected RaycastHit2D _hit;
    public bool isGround { get; protected set; }
    protected LayerMask _defaultForceReceiveLayer;
    [field: SerializeField] protected LayerMask _halfPlatformLayer;
    public bool isHalfPlatform;
    public bool isDownThroughPlatform;
    
    public CharacterType characterType => playerData.characterType;
    protected PlayerStateMachine stateMachine;

    [field: SerializeField] public new Rigidbody2D rigidbody2D;
    [field: SerializeField] public new Collider2D collider2D;
    [field: SerializeField] public NetworkRigidbodySync networkRigidbodySync { get; private set; }
    private Vector2 _velocity => networkRigidbodySync.velocity;

    [field: Header("Interaction")]
    protected PlayerInput input => Managers.Game.playerInput;
    [field: SerializeField] protected Transform grabPoint { get; private set; }
    protected LayerMask interactableLayerMask => playerData.interactableLayerMask;
    protected LayerMask obstacleMask => playerData.obstacleLayerMask;
    protected float detectDistance => playerData.detectDistance;
    protected Collider2D latestTarget;

    [field: Header("Particles")]
    [field: SerializeField] public ParticleSystem jumpParticle { get; private set; }
    [field: SerializeField] public ParticleSystem landParticle { get; private set; }
    
    [field: Header("Animation")]
    [field: SerializeField] public Animator animator { get; private set; }
    public PlayerAnimationData animationData { get; protected set; }
    private bool _isSuicideActive;
    
    #region Setup
    protected virtual void Awake()
    {
        stateMachine = new PlayerStateMachine(this);
        animationData = new PlayerAnimationData();
    }

    protected virtual void Start()
    {
        if (!isLocalPlayer)
        {
            Managers.Game.OtherPlayer = gameObject;
            return;
        }

        canControl = true;
        canMovable = true;
        _defaultForceReceiveLayer = collider2D.forceReceiveLayers;
        collider2D.forceReceiveLayers = ~ _halfPlatformLayer;
        stateMachine.SubscribeInput();
        SubscribeInput();
        StartCoroutine(DetectInteraction());
    }

    protected virtual void OnDisable()
    {
        stateMachine.UnsubscribeInput();
        StopAllCoroutines();
        UnsubscribeInput();
    }
    #endregion

    #region Update
    protected virtual void Update()
    {
        if (!isLocalPlayer || !canControl) return;

        GroundCheck();
        stateMachine.HandleInput();
        stateMachine.Update();
    }

    protected virtual void FixedUpdate()
    {
        if (!isLocalPlayer || !canControl) return;
        
        stateMachine.PhysicsUpdate();
    }
    #endregion

    #region Ground
    protected virtual void GroundCheck()
    {
        for (var i = -1; i < 2; i++)
        {
            _hit = Physics2D.Raycast(transform.position + (Vector3.right * (0.4f * i)) + (Vector3.up * 0.2f), Vector2.down, 0.4f, playerData.floorLayerMask);
            if (!_hit) continue;

            isHalfPlatform = _halfPlatformLayer == (_halfPlatformLayer | (1 << _hit.transform.gameObject.layer));
            //isHalfPlatform = (1 << _hit.transform.gameObject.layer) == _halfPlatformLayer;
            if (isHalfPlatform && !isDownThroughPlatform)
                collider2D.forceReceiveLayers = _defaultForceReceiveLayer;
            else
                collider2D.forceReceiveLayers = ~_halfPlatformLayer;
            
            if (isGround) return;
            
            CmdLandParticlePlay();
            isGround = true;
            coyoteTimeCount = _coyoteTime;
            return;
        }
        isHalfPlatform = false;
        collider2D.forceReceiveLayers = ~_halfPlatformLayer;
        isGround = false;
        coyoteTimeCount -= Time.deltaTime;
    }

    public void DownThroughHalfPlatform()
    {
        collider2D.forceReceiveLayers =~ _halfPlatformLayer;
    }
    #endregion
    
    #region Interaction
    protected virtual IEnumerator DetectInteraction()
    {
        var shortestDistance = float.MaxValue;
        var offset = new Vector3(0, 0.45f);
        Collider2D closestTarget = null;

        while (true)
        {
            yield return null;

            var collisions =
                Physics2D.OverlapCircleAll(transform.position + offset, detectDistance, interactableLayerMask);

            if (collisions.Length == 0)
            {
                if (latestTarget is null) continue;

                latestTarget = null;
                Managers.UI.HideUI<UI_ShowEButton>();
                continue;
            }

            closestTarget = null;

            foreach (var collision in collisions)
            {
                if (collision.TryGetComponent<IInteractable>(out var interactable) && !interactable.CanInteract()) continue;

                if (interactable is not null && interactable.GetObjectType() == ObjectTypeEnum.Grab) continue;

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
                if(latestTarget is not null)
                    Managers.UI.HideUI<UI_ShowEButton>();

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

    protected virtual void Interaction()
    {
        if (latestTarget is null) return;
        if (!latestTarget.TryGetComponent<IInteractable>(out var interactable)) return;
        if (interactable.GetObjectType() == ObjectTypeEnum.Grab) return;
        interactable.Interaction(transform);
    }
    #endregion

    #region Dead
    // ReSharper disable Unity.PerformanceAnalysis
    //0323
    public event Action deathEvent;
    //0323
    public virtual void TakeDamage(DamageType damageType = DamageType.Default)
    {
        if (!isLocalPlayer || !canControl) return;

        canControl = false;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        collider2D.enabled = false;
        Camera.main.GetComponent<PlayerCameraView>()._CameraGlobalVolumeController.DeathVignette(true);
        stateMachine.ChangeState(stateMachine.IdleState);
        
        // 애니메이션 처리
        PlayDeathAnimation(damageType);

        // 카메라 효과 처리
        HandleDeathCameraEffects(damageType);
        // 플레이어 죽었을 때 처리
        Managers.AcManager.CallPlayerDeath();
        
        deathEvent?.Invoke();
    }

    //private void TakeSuicideDamage()
    //{
    //    //애니메이션 트리거 용도
    //    TakeDamage(DamageType.Suicide);
    //}

    private void PlayDeathAnimation(DamageType damageType)
    {
        // DamageType에 따른 애니메이션 트리거 실행
        int triggerHash = animationData.DeathParameterHashes.ContainsKey(damageType)
            ? animationData.DeathParameterHashes[damageType]
            : animationData.DeathParameterHashes[DamageType.Default];

        animator.SetTrigger(triggerHash);

        // 카메라 이미지 효과 애니메이션
        Camera.main.GetComponent<PlayerCameraView>()._CameraImageEffects.animator.SetTrigger(triggerHash);
    }

    private void HandleDeathCameraEffects(DamageType damageType)
    {
        // 카메라 흔들림 처리
        (float duration, float intensity) shakeParam = damageType switch
        {
            DamageType.Boom or DamageType.Suicide => (0.35f, 10f),
            DamageType.Electric or DamageType.Fire => (0.4f, 7f),
            DamageType.Default => (0.3f, 6f),
        };

        if (shakeParam.duration > 0)
        {
            Managers.Game.cameraShake.RequestShake(gameObject, shakeParam.intensity, shakeParam.duration);
            //StartCoroutine(CameraShake.instance.Co_Shake(shakeParam.duration, shakeParam.intensity));
        }
    }
    
    public virtual void Respawning()
    {
        rigidbody2D.velocity = Vector2.zero;
        transform.position = Managers.Network.startPos[0].position;
        animator.SetTrigger(animationData.RespawningParameterHash);
        Camera.main.GetComponent<PlayerCameraView>()._CameraGlobalVolumeController.DeathVignette(false);
    }
    
    public void RespawnEnd()
    {
        animator.SetTrigger(animationData.RespawnEndParameterHash);
        canControl = true;
        collider2D.enabled = true;
        rigidbody2D.constraints = RigidbodyConstraints2D.None;
        rigidbody2D.freezeRotation = true;
        stateMachine.Initialize();
    }
    #endregion
    
    #region Emote
    private void ShowEmoteWheel()
    {
        if (_emoteOnCoolDown) return;

        if (Managers.UI.IsActive<UI_EmoteWheel>()) return;

        Managers.UI.ShowUI<UI_EmoteWheel>();
    }

    private void HideEmoteWheel()
    {
        if (!Managers.UI.IsActive<UI_EmoteWheel>()) return;
        
        Managers.UI.HideUI<UI_EmoteWheel>();
    }
    
    public void UsingEmote()
    {
        _emoteOnCoolDown = true;
        StartCoroutine(EmoteCoolDown());
    }
    
    private IEnumerator EmoteCoolDown()
    {
        yield return new WaitForSeconds(3.5f);
        _emoteOnCoolDown = false;
    }

    [Command(requiresAuthority = false)]
    public void CmdEmote(string emoteName)
    {
        var prefab = Managers.Network.spawnPrefabDict[emoteName];
        var go = Instantiate(prefab, gameObject.transform.position + Vector3.up * 0.6f,Quaternion.identity);
        NetworkServer.Spawn(go);
        go.name = prefab.name;
        RpcEmote(go);
    }
    
    [ClientRpc]
    private void RpcEmote(GameObject go)
    {
        var sort = go.GetComponent<SortingGroup>();
        sort.sortingOrder = isLocalPlayer ? 8 : 7;
        go.transform.parent = gameObject.transform;
    }
    #endregion

    #region Particles
    [Command(requiresAuthority = false)]
    public void CmdLandParticlePlay()
    {
        landParticle.Play();
        RpcLandParticlePlay();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcLandParticlePlay()
    {
        landParticle.Play();
    }

    [Command(requiresAuthority = false)]
    public void CmdJumpParticlePlay()
    {
        jumpParticle.Play();
        RpcJumpParticlePlay();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcJumpParticlePlay()
    {
        jumpParticle.Play();
    }
    #endregion
    
    #region Input
    private void ShowEmote(InputAction.CallbackContext context)
    {
        if (!canControl) return;
        
        ShowEmoteWheel();
    }

    private void HideEmote(InputAction.CallbackContext context)
    {
        HideEmoteWheel();
    }
    
    private void DoInteraction(InputAction.CallbackContext context)
    {
        if (!canControl) return;
        
        Interaction();
    }
    
    private void TrySuicide(InputAction.CallbackContext context)
    {
        if (!canControl) return;
        
        stateMachine.ChangeState(stateMachine.SuicideState);
    }

    private void Suicided()
    {
        if (!canControl) return;
        
        TakeDamage(DamageType.Suicide);
    }
    
    private void SubscribeInput()
    {
        input.playerActions.Emote.started += ShowEmote;
        input.playerActions.Emote.canceled += HideEmote;
        input.playerActions.Interaction.started += DoInteraction;
        input.playerActions.Suicide.started += TrySuicide;
    }
    
    private void UnsubscribeInput()
    {
        input.playerActions.Emote.started -= ShowEmote;
        input.playerActions.Emote.canceled -= HideEmote;
        input.playerActions.Interaction.started -= DoInteraction;
        input.playerActions.Suicide.started -= TrySuicide;
    }
    #endregion


}
