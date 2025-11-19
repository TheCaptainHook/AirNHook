using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Rendering;

public class PlayerSM : NetworkBehaviour, IDamageable
{
    [field: Header("PlayerData")]
    [field: SerializeField] public PlayerDataSO playerData { get; protected set; }
    public bool canControl;
    public bool canAction;
    public bool canMovable;
    public bool invincible;
    public bool canInteract;
    [SyncVar] public bool isDead;
    [SyncVar] public bool doNotTouch;
    public bool isControlObj;
    [field: SerializeField] public Transform charPivot { get; private set; }
    [field: SerializeField] public List<SortingGroup> sortingGroup { get; private set; }
    [field: SerializeField] public PlayerTalkingSprite talkingSprite { get; private set; }
    [field: SerializeField] public GameObject spriteMask { get; private set; }
    protected float _coyoteTime => playerData.coyoteTime;
    public float coyoteTimeCount;
    private bool _emoteOnCoolDown;
    private bool _pingOnCoolDown;
    private int _pingCount;
    protected RaycastHit2D _hit;
    public bool isGround { get; protected set; }
    protected LayerMask _defaultForceReceiveLayer;
    [field: SerializeField] public LayerMask halfPlatformLayer;
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
    public ConstraintSource grabSource;
    protected ConstraintSource characterConstraintSource;
    protected ParentConstraint characterConstraint;
    protected LayerMask interactableLayerMask => playerData.interactableLayerMask;
    protected LayerMask obstacleMask => playerData.obstacleLayerMask;
    protected float detectDistance => playerData.detectDistance;
    [SerializeField] protected Collider2D latestTarget;

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
        characterConstraint = gameObject.GetComponent<ParentConstraint>();

        if (!isLocalPlayer)
        {
            Managers.Game.OtherPlayer = gameObject;
            return;
        }

        canControl = true;
        canAction = true;
        canMovable = true;
        canInteract = true;
        isDead = false;
        doNotTouch = false;
        _defaultForceReceiveLayer = collider2D.forceReceiveLayers;
        collider2D.forceReceiveLayers = ~halfPlatformLayer;
        stateMachine.SubscribeInput();
        SubscribeInput();
        StartCoroutine(DetectInteraction());

        grabSource = new ConstraintSource
        {
            sourceTransform = grabPoint,
            weight = 1f
        };
    }

    protected virtual void OnDisable()
    {
        stateMachine.UnsubscribeInput();
        StopAllCoroutines();
        UnsubscribeInput();
    }

    public virtual void Reset()
    {

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

            isHalfPlatform = halfPlatformLayer == (halfPlatformLayer | (1 << _hit.transform.gameObject.layer));
            //isHalfPlatform = (1 << _hit.transform.gameObject.layer) == _halfPlatformLayer;
            if (isHalfPlatform && !isDownThroughPlatform)
                StopDownThroughHalfPlatform();
            else
                DownThroughHalfPlatform();

            if (isGround) return;

            landParticle.Play();
            CmdLandParticlePlay();
            isGround = true;
            coyoteTimeCount = _coyoteTime;
            return;
        }
        isHalfPlatform = false;
        DownThroughHalfPlatform();
        isGround = false;
        coyoteTimeCount -= Time.deltaTime;
    }

    public void StopDownThroughHalfPlatform()
    {
        collider2D.forceReceiveLayers = _defaultForceReceiveLayer;
    }

    public void DownThroughHalfPlatform()
    {
        collider2D.forceReceiveLayers = ~halfPlatformLayer;
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
        

            if (isControlObj)
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

                    if (interactable.GetObjectType() == ObjectTypeEnum.Mount) continue;

                    if (interactable.GetObjectType() == ObjectTypeEnum.AirGun) continue;
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

    protected virtual void Interaction()
    {
        if (latestTarget == null) return;
        if (!latestTarget.TryGetComponent<IInteractable>(out var interactable)) return;
        if (interactable.GetObjectType() == ObjectTypeEnum.Control)
            isControlObj = !isControlObj;
        interactable.Interaction(transform);
    }
    #endregion

    #region Dead
    // ReSharper disable Unity.PerformanceAnalysis
    //0323
    public event Action<DamageType> deathEvent;
    //0323
    public virtual void TakeDamage(DamageType damageType = DamageType.Default)
    {
        if (!isLocalPlayer || !canControl) return;

        Debug.Log($"TakeDamage {damageType}");
        canControl = false;
        isDead = true;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        collider2D.enabled = false;
        Camera.main.GetComponent<PlayerCameraView>()._CameraGlobalVolumeController.DeathVignette(true);
        stateMachine.ChangeState(stateMachine.IdleState);

        deathEvent?.Invoke(damageType);

        // 애니메이션 처리
        PlayDeathAnimation(damageType);

        // 카메라 효과 처리
        HandleDeathCameraEffects(damageType);
        // 플레이어 죽었을 때 처리
        Managers.AcManager.CallPlayerDeath(damageType);

        PlayDeathSound(damageType);
        CmdPlayDeathSound(damageType);
    }

    public void CallPlayerDeathEvent()
    {
        deathEvent?.Invoke(DamageType.Default);
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
        PlayRespawnSound();
        CmdPlayRespawnSound();
    }

    public void RespawnEnd()
    {
        animator.SetTrigger(animationData.RespawnEndParameterHash);
        canControl = true;
        isDead = false;
        collider2D.enabled = true;
        rigidbody2D.constraints = RigidbodyConstraints2D.None;
        rigidbody2D.freezeRotation = true;
        stateMachine.Initialize();
    }

    private void PlayDeathSound(DamageType damageType)
    {
        if (!GlobalText.DeathSoundDictionary.TryGetValue(damageType, out var sound)) return;

        Managers.Sound.PlaySound3D(sound, transform.position);
    }

    [Command(requiresAuthority = false)]
    private void CmdPlayDeathSound(DamageType damageType)
    {
        RpcPlayDeathSound(damageType);
    }

    [ClientRpc(includeOwner = false)]
    private void RpcPlayDeathSound(DamageType damageType)
    {
        PlayDeathSound(damageType);
    }

    private void PlayRespawnSound()
    {
        Managers.Sound.PlaySound3D(GlobalText.PLAYER_RESURRECT, transform.position);
    }

    [Command(requiresAuthority = false)]
    private void CmdPlayRespawnSound()
    {
        RpcPlayRespawnSound();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcPlayRespawnSound()
    {
        PlayRespawnSound();
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

        UI_EmoteWheel emoteWheel = Managers.UI.GetUI<UI_EmoteWheel>().GetComponent<UI_EmoteWheel>();
        emoteWheel.TryShowHoveredEmote();
        Managers.UI.HideUI<UI_EmoteWheel>();
    }

    public void UsingEmote()
    {
        _emoteOnCoolDown = true;
        DoVoice();
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
        var go = Instantiate(prefab, gameObject.transform.position + Vector3.up * 0.6f, Quaternion.identity);
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

    #region PingSystem
    private void ShowBasicPing()
    {
        if (_pingOnCoolDown) return;

        // CmdPing("Ping1", Camera.main.ScreenToWorldPoint(Input.mousePosition));
        CmdPing("Ping1", Camera.main.ScreenToWorldPoint(Input.mousePosition));
        UsingPing();
    }

    private void ShowPingWheel()
    {
        if (_pingOnCoolDown) return;

        if (Managers.UI.IsActive<UI_PingWheel>()) return;

        Managers.UI.ShowUI<UI_PingWheel>();
    }

    private void HidePingWheel()
    {
        if (!Managers.UI.IsActive<UI_PingWheel>()) return;

        UI_PingWheel pingWheel = Managers.UI.GetUI<UI_PingWheel>().GetComponent<UI_PingWheel>();
        pingWheel.TryShowHoveredPing();
        Managers.UI.HideUI<UI_PingWheel>();
    }

    [Command(requiresAuthority = false)]
    public void CmdPing(string pingName, Vector2 pos)
    {
        //var prefab = Managers.Network.spawnPrefabDict[pingName];
        //var go = Instantiate(prefab, pos, Quaternion.identity);
        //NetworkServer.Spawn(go);
        //-------------Ping mark 0728
        Rpc_Ping(pingName, pos);
        //-------------Ping mark 0728


    }
    //-------------Ping mark 0728
    [ClientRpc]
    private void Rpc_Ping(string pingName, Vector2 pos)
    {
        var prefab = Managers.Network.spawnPrefabDict[pingName];
        var go = Instantiate(prefab, pos, Quaternion.identity);
        go.GetComponent<PingWheel_Item_Marker>().Setting_PingPosition(pos);
        go.name = prefab.name;
    }
    //-------------Ping mark 0728


    public void UsingPing()
    {
        if (_pingCount > 4)
        {
            Debug.Log(_pingCount);
            _pingOnCoolDown = true;
            StartCoroutine(PingCoolDown());
            return;
        }

        _pingCount++;
        Debug.Log(_pingCount);
    }

    private IEnumerator PingCoolDown()
    {
        yield return new WaitForSeconds(2f);
        _pingOnCoolDown = false;
    }

    public void PingRemoved()
    {
        _pingCount--;

        if (_pingCount < 0)
            _pingCount = 0;
    }
    #endregion

    #region Sound
    private void DoVoice()
    {
        Managers.Sound.PlaySound(GlobalText.PLAYER_SPEAK);
        CmdVoice();
        TurnOnTalkingSprite();
    }

    [Command(requiresAuthority = false)]
    private void CmdVoice()
    {
        RpcVoice();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcVoice()
    {
        Managers.Sound.PlaySound3D(GlobalText.PLAYER_SPEAK, transform);
        TurnOnTalkingSprite();
    }

    private void TurnOnTalkingSprite()
    {
        if (talkingSprite.gameObject.activeSelf) talkingSprite.StartVoice();
        else talkingSprite.gameObject.SetActive(true);
    }

    public void JumpSoundPlay()
    {
        Managers.Sound.PlaySound(GlobalText.PLAYER_JUMP);
        CmdJumpSoundPlay();
    }

    [Command(requiresAuthority = false)]
    private void CmdJumpSoundPlay()
    {
        RpcJumpSoundPlay();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcJumpSoundPlay()
    {
        Managers.Sound.PlaySound3D(GlobalText.PLAYER_JUMP, transform.position);
    }
    #endregion

    #region Particles
    [Command(requiresAuthority = false)]
    public void CmdLandParticlePlay()
    {
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

        if (!canAction) return;

        ShowEmoteWheel();
    }

    private void HideEmote(InputAction.CallbackContext context)
    {
        HideEmoteWheel();
    }

    private void DoInteraction(InputAction.CallbackContext context)
    {
        if (!canControl) return;

        if (!canInteract) return;

        Interaction();
    }

    protected virtual void TrySuicide(InputAction.CallbackContext context)
    {
        if (!canControl) return;

        if (!canAction) return;

        canMovable = false;
        stateMachine.ChangeState(stateMachine.SuicideState);
    }

    private void Suicided()
    {
        if (!canControl) return;

        TakeDamage(DamageType.Suicide);
    }

    private void Voice(InputAction.CallbackContext context)
    {
        if (!canControl) return;

        if (!canAction) return;

        DoVoice();
    }

    private void ShowPing(InputAction.CallbackContext context)
    {
        if (!canControl) return;

        if (!canAction) return;

        if (context.interaction is TapInteraction)
            ShowBasicPing();
        else if (context.interaction is HoldInteraction)
            ShowPingWheel();
    }

    private void HidePing(InputAction.CallbackContext context)
    {
        HidePingWheel();
    }

    public virtual void FreezePlayerState(bool onOff)
    {
        canAction = !onOff;
        canMovable = !onOff;
        canInteract = !onOff;
        doNotTouch = onOff;

        if (onOff)
            rigidbody2D.velocity = Vector2.zero;
    }

    private void SubscribeInput()
    {
        input.playerActions.Emote.started += ShowEmote;
        input.playerActions.Emote.canceled += HideEmote;
        input.playerActions.Interaction.started += DoInteraction;
        input.playerActions.Suicide.started += TrySuicide;
        input.playerActions.Voice.started += Voice;
        input.playerActions.Ping.performed += ShowPing;
        input.playerActions.Ping.canceled += HidePing;
    }

    private void UnsubscribeInput()
    {
        input.playerActions.Emote.started -= ShowEmote;
        input.playerActions.Emote.canceled -= HideEmote;
        input.playerActions.Interaction.started -= DoInteraction;
        input.playerActions.Suicide.started -= TrySuicide;
        input.playerActions.Voice.started -= Voice;
        input.playerActions.Ping.performed -= ShowPing;
        input.playerActions.Ping.canceled -= HidePing;
    }
    #endregion
}
