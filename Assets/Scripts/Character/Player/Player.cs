using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using static UnityEngine.RigidbodyConstraints2D;

public class Player : NetworkBehaviour, IDamageable
{
    private PlayerMovement _movement;
    private PlayerInput _input;
    
    //애니메이션 위함
    private Animator _animator;
    private NetworkAnimator _networkAnimator;
    private Collider2D _collider2D;
    private Rigidbody2D _rigidbd;
    
    //사망 체크
    [FormerlySerializedAs("isDead")] [SerializeField] public bool _isDead = false;
    
    //이모트
    private bool _emoteOnCoolDown;

    #region StringCache
    private static readonly int IsDead = Animator.StringToHash("IsDead");
    private static readonly int IsRespawning = Animator.StringToHash("IsRespawning");
    private static readonly int OnRespawnEnd = Animator.StringToHash("OnRespawnEnd");
    #endregion
    
    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _rigidbd = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _movement = GetComponent<PlayerMovement>();
        _networkAnimator = GetComponent<NetworkAnimator>();
        _input = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        if(!isLocalPlayer) return;

        _input.uiActions.Option.started += OptionStart;
        _input.playerActions.Emote.started += EmoteStart;
    }

    private void OnDestroy()
    {
        if (!isLocalPlayer && Managers.Network.isNetworkActive) return;
        
        _input.uiActions.Option.started -= OptionStart;
        _input.playerActions.Emote.started -= EmoteStart;
    }

    #region Animations
    // 사망 메서드
    public void TakeDamage()
    {
        if(!isLocalPlayer)
            return;
        
        _networkAnimator.SetTrigger(IsDead);
        if (!_isDead)
        {
            Debug.Log("사망하였습니다.");
            // 여기에 필요한 사망 처리
            // _animator.SetTrigger(IsDead);
            _isDead = true;
            _movement.IsDead = true;
            _rigidbd.constraints = FreezeAll;
            _collider2D.enabled = false;
        }
    }
    
    private void Respawning()
    {
        Debug.Log("리스포닝");
        
        transform.position = Managers.Network.startPos[0].position;
        //_animator.SetTrigger(IsRespawning);
        //_networkAnimator.SetTrigger(IsRespawning);
        _animator.SetTrigger(IsRespawning);
        CmdDoRespawn();
    }

    [Command(requiresAuthority = false)]
    private void CmdDoRespawn()
    {
        RpcDoRespawn();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcDoRespawn()
    {
        _animator.SetTrigger(IsRespawning);
    }

    private void RespawnEnd()
    {
        Debug.Log("리스폰끝");
        _animator.SetTrigger(OnRespawnEnd);
        CmdDoRespawnEnd();
        // _networkAnimator.SetTrigger(OnRespawnEnd);
        _isDead = false;
        _movement.IsDead = false;
        _collider2D.enabled = true;
        _rigidbd.constraints = None;
        _rigidbd.freezeRotation = true;
    }

    [Command(requiresAuthority = false)]
    private void CmdDoRespawnEnd()
    {
        RpcDoRespawnEnd();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcDoRespawnEnd()
    {
        _animator.SetTrigger(OnRespawnEnd);
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
        var go = Instantiate(prefab, gameObject.transform.position, Quaternion.identity);
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

    #region Input
    private void OptionStart(InputAction.CallbackContext context)
    {
        ShowOptionUI();
    }
    
    private void EmoteStart(InputAction.CallbackContext context)
    {
        ShowEmoteWheel();
    }
    #endregion
}
