using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class DoorOpeningAnim : NetworkBehaviour
{
    [Header("State")] 
    //private bool _isClear = false;

    [SerializeField] private GameObject _lockGameObject;
    [SerializeField] private Rigidbody2D _lockRigidbody2D;
    [SerializeField] private Collider2D _lockCollider2D;
    
    private Animator _animator;

    public event Action OnUnlockAnimation;

    #region StringCache
    private static readonly int IsUnlocking = Animator.StringToHash("IsUnlocking");
    #endregion

    private void Awake()
    {
        _lockRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        _lockCollider2D.enabled = false;
        _animator = GetComponent<Animator>();
        OnUnlockAnimation += SetTriggerUnlocking;
    }

    //private void Update()
    //{
    //    if (_isClear)
    //    {
    //        _animator.SetTrigger(IsUnlocking);
    //    }
    //}


    private void SetTriggerUnlocking()
    {
        //_isClear = true;
        _animator.SetTrigger(IsUnlocking);
    }
    public void CallOnUnlockAnimation()
    {
        OnUnlockAnimation?.Invoke();
    }


    public void UnlockingAnim()
    {
        Debug.Log("UnlockingLock");
        _lockRigidbody2D.constraints = RigidbodyConstraints2D.None;
        _lockCollider2D.enabled = true;
        // 랜덤한 방향으로 힘을 가함
        Vector2 forceDirection = Random.insideUnitCircle.normalized;
        float forceMagnitude = Random.Range(10f, 20f); // 힘의 크기를 랜덤으로 지정
        _lockRigidbody2D.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
    }

    // [Server]
    // public void DestroyLock()
    // {
    //     NetworkServer.Destroy(_lockGameObject);
    // }
    
    [Command(requiresAuthority = false)]
    public void CmdMoveNextStage(string nextMapId)
    {
        RpcMoveNextStage(nextMapId);
    }
    
    [ClientRpc]
    public void RpcMoveNextStage(string nextMapId)
    {
        Managers.Stage.stageName = nextMapId;
        if (string.IsNullOrEmpty(nextMapId))
        {
            if(MapEditor.Instance.curMap.mapID != "Lobby")
            {
                if (MapEditor.Instance.curMap.stageLevel == Managers.Game.stageLevel) //todo 0419
                {
                    Managers.Game.stageLevel++;
                }

                //Managers.Game.StageLevelPlus()

                var player = Managers.Game.Player.GetComponent<Player>();
                if (player.isServer)
                {
                    player.CmdChangeStage("Lobby");
                }
            }
            Debug.Log("Stage Clear");
        }
        else
        {
            Managers.Game.CurrentState = GameState.Game;
            MapEditor.Instance.MoveNextStage(nextMapId, MapType.Main);
        }
    }
}
