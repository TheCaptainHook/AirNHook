using System;
using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using static UnityEngine.RigidbodyConstraints2D;

public class Player : NetworkBehaviour, IDamageable
{
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
        StartCoroutine(Co_DetectInteraction());
        if (!isLocalPlayer)
        {
            Managers.Game.OtherPlayer = gameObject;
            return;
        }

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
            MapEditor.Instance.MoveNextStage(value, MapType.Scene);
        }
        else
        {
            Managers.Game.CurrentState = GameState.Game;
            MapEditor.Instance.MoveNextStage(value, MapType.Main);
        }
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
        isDead = true;
        _movement.IsDead = true;
        _rigidbd.constraints = FreezeAll;
        _collider2D.enabled = false;
    }
    
    public void Respawning()
    {
        Debug.Log("리스포닝");
        
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
    [SerializeField] private LayerMask _interactableLayer;
    protected Collider2D _latestTarget;
    private readonly float _detectDistance = 2f;
    private WaitForSeconds _waitForSeconds;
    
    private IEnumerator Co_DetectInteraction()
    {
        _waitForSeconds = new WaitForSeconds(0.2f);
        var shortestDistance = float.MaxValue;
        Collider2D closestTarget = null;
        
        while (true)
        {
            yield return _waitForSeconds;
            
            var collisions = Physics2D.OverlapCircleAll(transform.position, _detectDistance, _interactableLayer);

            if (collisions.Length == 0)
            {
                _latestTarget = null;
                continue;
            }

            foreach (var collision in collisions)
            {
                //TODO 벽에 가로막혔을 경우 체크
                var targetDistance = Vector2.Distance(transform.position, collision.transform.position);
                if (targetDistance < shortestDistance)
                {
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
            }

            _latestTarget = closestTarget;
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
