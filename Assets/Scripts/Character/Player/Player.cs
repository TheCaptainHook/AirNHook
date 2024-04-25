using System;
using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.RigidbodyConstraints2D;

public class Player : NetworkBehaviour, IDamageable
{
    public CharacterType characterType;
    protected PlayerMovement _movement;
    private PlayerInput _input;
    
    //애니메이션 위함
    protected Animator _animator;
    protected Collider2D _collider2D;
    protected Rigidbody2D _rigidbd;
    private SortingGroup _sortingGroup;
    
    //사망 체크
    [SerializeField] public bool isDead = false;
    
    //이모트
    private bool _emoteOnCoolDown;

    #region StringCache
    protected static readonly int IsDead = Animator.StringToHash("IsDead");
    private static readonly int IsRespawning = Animator.StringToHash("IsRespawning");
    private static readonly int OnRespawnEnd = Animator.StringToHash("OnRespawnEnd");
    #endregion
    
    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _rigidbd = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _movement = GetComponent<PlayerMovement>();
        _input = GetComponent<PlayerInput>();
        _sortingGroup = GetComponent<SortingGroup>();
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            Managers.Game.OtherPlayer = gameObject;
            return;
        }

        StartCoroutine(Co_DetectInteraction());
        _input.uiActions.Option.started += OptionStart;
        _input.playerActions.Emote.started += EmoteStart;
        _input.playerActions.Interaction.started += InteractionStart;
        _sortingGroup.sortingLayerID = SortingLayer.NameToID("PlayerFore");
    }

    public void OnDisable()
    {
        _input.uiActions.Option.started -= OptionStart;
        _input.playerActions.Emote.started -= EmoteStart;
        _input.playerActions.Interaction.started -= InteractionStart;
    }

    #region ExternalCommandSync
    [Command(requiresAuthority = false)]
    public void CmdChangeStage(string value)
    {
        RpcChangeStage(value);
    }

    [ClientRpc]
    private void RpcChangeStage(string value)
    {
        Managers.Stage.stageName = value;
        if (value.Equals("Lobby"))
        {
            Managers.Game.CurrentState = GameState.Lobby;
            MapEditor.Instance.MoveNextStage(value);
        }
        else
        {
            Managers.Game.CurrentState = GameState.Game;
            MapEditor.Instance.MoveNextStage(value);
        }
    }

    public Action<string, bool> stageCheckCallback;

    [Command(requiresAuthority = false)]
    public void CmdStageDataCheck(string value)
    {
        RpcStageDataCheck(value);
    }

    [ClientRpc(includeOwner = false)]
    private void RpcStageDataCheck(string value)
    {
        var isMapExist = Managers.Data.mapData.mapAllDictionary.ContainsKey(value);
        CmdStageDataChecked(value, isMapExist);
        
        if (!isMapExist) return;
        var stageUI = (UI_StageSelect)Managers.UI.GetUI<UI_StageSelect>();
        stageUI.MapSelected(value, true);
    }

    [Command(requiresAuthority = false)]
    private void CmdStageDataChecked(string mapID, bool value)
    {
        stageCheckCallback?.Invoke(mapID, value);
    }
    #endregion

    #region Animations
    // 사망 메서드
    public virtual void TakeDamage()
    {
        if(!isLocalPlayer)
            return;
        
        _animator.SetTrigger(IsDead);
        
        if (isDead) return;
        Debug.Log("사망하였습니다.");
        // 여기에 필요한 사망 처리
        // _animator.SetTrigger(IsDead);
        CmdIncreaseDeathCount();
        isDead = true;
        _movement.IsDead = true;
        _rigidbd.constraints = FreezeAll;
        _collider2D.enabled = false;
    }

    [Command(requiresAuthority = false)]
    protected void CmdIncreaseDeathCount()
    {
        RpcIncreaseDeathCount();
    }
    
    [ClientRpc]
    private void RpcIncreaseDeathCount()
    {
        Managers.Game.IncreaseDeathCount(isLocalPlayer);
    }
    
    public virtual void Respawning()
    {
        Debug.Log("리스포닝");
        
        _rigidbd.velocity = Vector2.zero;
        transform.position = Managers.Network.startPos[0].position;
        //_animator.SetTrigger(IsRespawning);
        //_networkAnimator.SetTrigger(IsRespawning);
        _animator.SetTrigger(IsRespawning);
    }

    private void RespawnEnd()
    {
        _animator.SetTrigger(OnRespawnEnd);
        isDead = false;
        _movement.IsDead = false;
        _collider2D.enabled = true;
        _rigidbd.constraints = None;
        _rigidbd.freezeRotation = true;
    }
    #endregion

    #region Emote
    private void ShowOptionUI()
    {
        if(!Managers.UI.IsActive<UI_Option>())
        {
            Managers.UI.ShowUI<UI_Option>();
        }
        else
        {
            Managers.UI.HideUI<UI_Option>();
        }
    }

    private void ShowEmoteWheel()
    {
        if (_emoteOnCoolDown == false)
        {
            if(!Managers.UI.IsActive<UI_EmoteWheel>())
            {
                Managers.UI.ShowUI<UI_EmoteWheel>();
            }
            else
            {
                Managers.UI.HideUI<UI_EmoteWheel>();
            }
        }
    }

    public void UsingEmote()
    {
        _emoteOnCoolDown = true;
        StartCoroutine(C0_EmoteCoolDown());
    }
    
    private IEnumerator C0_EmoteCoolDown()
    {
        yield return new WaitForSeconds(3.5f);
        _emoteOnCoolDown = false;
        Debug.Log("EmoteCOEnds");
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
    public void RpcEmote(GameObject go)
    {
        var sort = go.GetComponent<SortingGroup>();
        sort.sortingOrder = isLocalPlayer ? 8 : 7;
        go.transform.parent = gameObject.transform;
    }
    #endregion

    #region Interaction
    [SerializeField] protected Transform _grabPoint;
    [SerializeField] protected LayerMask _interactableLayer;
    protected Collider2D _latestTarget;
    protected readonly float _detectDistance = 1f;
    
    protected virtual IEnumerator Co_DetectInteraction()
    {
        var shortestDistance = float.MaxValue;
        var offset = new Vector3(0, 0.45f);
        Collider2D closestTarget = null;
        
        while (true)
        {
            yield return null;
            
            var collisions = Physics2D.OverlapCircleAll(transform.position + offset, _detectDistance, _interactableLayer);

            if (collisions.Length == 0)
            {
                if (_latestTarget is null) continue;
                
                _latestTarget.GetComponent<IInteractable>().HideEButton();
                _latestTarget = null;
                continue;
            }

            foreach (var collision in collisions)
            {
                //TODO 벽에 가로막혔을 경우 체크
                var targetDistance = Vector2.Distance(transform.position + offset, collision.transform.position);
                if (targetDistance < shortestDistance)
                {
                    // 후크가 잡고 있는 물체 처리
                    if (collision.TryGetComponent<IInteractable>(out var inhalable) && !inhalable.CanInteract()) continue;
                    
                    shortestDistance = targetDistance;
                    closestTarget = collision;
                }
            }

            if (_latestTarget is not null)
            {
                if (ReferenceEquals(_latestTarget, closestTarget))
                {
                    shortestDistance = float.MaxValue;
                    continue;
                }

                _latestTarget.GetComponent<IInteractable>().HideEButton();
            }

            _latestTarget = closestTarget;
            _latestTarget.GetComponent<IInteractable>().ShowEButton();
            shortestDistance = float.MaxValue;
        }
    }

    protected virtual void Interaction()
    {
        if (_latestTarget is null || isDead) return;
        
        if(!_latestTarget.TryGetComponent<IInteractable>(out var interactable)) return;
        
        if (interactable.GetObjectType() == ObjectTypeEnum.Grab) return;
        
        interactable.Interaction(transform);
    }
    #endregion
    
    #region Input
    private void OptionStart(InputAction.CallbackContext context)
    {
        ShowOptionUI();
    }
    
    private void EmoteStart(InputAction.CallbackContext context)
    {
        ShowEmoteWheel();
    }

    private void InteractionStart(InputAction.CallbackContext context)
    {
        Interaction();
    }
    #endregion
}
