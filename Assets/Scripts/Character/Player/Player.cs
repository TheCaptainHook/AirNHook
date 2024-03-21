using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.RigidbodyConstraints2D;

public class Player : NetworkBehaviour, IDamageable
{
    //애니메이션 위함
    private Animator _animator;
    private Collider2D _collider2D;
    private Rigidbody2D _rigidbd;
    
    //사망 체크
    [SerializeField] private bool _isDead = false;

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
    }
    // 사망 메서드
    public void TakeDamage()
    {
        if (!_isDead)
        {
            Debug.Log("사망하였습니다.");
            // 여기에 필요한 사망 처리
            _animator.SetTrigger(IsDead);
            _isDead = true;
            _rigidbd.constraints = FreezeAll;
            _collider2D.enabled = false;
        }
    }

    private void Respawning()
    {
        Debug.Log("리스포닝");
        
        transform.position = Managers.Network.startPos[0].position;
        _animator.SetTrigger(IsRespawning);
    }

    private void RespawnEnd()
    {
        Debug.Log("리스폰끝");
        _animator.SetTrigger(OnRespawnEnd);
        _isDead = false;
        _collider2D.enabled = true;
        _rigidbd.constraints = None;
        _rigidbd.freezeRotation = true;
    }

    [Command]
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

}
