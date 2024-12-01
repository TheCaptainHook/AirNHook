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
    //public event Action OnLockAnimation;//TODO 0729

    #region StringCache
    private static readonly int IsUnlocking = Animator.StringToHash("IsUnlocking");
    private static readonly int Reset = Animator.StringToHash("Reset");//TODO 0729
    #endregion

    private void Awake()
    {
        _lockRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        _lockCollider2D.enabled = false;
        _animator = GetComponent<Animator>();
        OnUnlockAnimation += SetTriggerUnlocking;
        //OnLockAnimation += Ani_Reset;
    }


    //private void Update()
    //{
    //    //TEST  
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        Ani_Reset();
    //    }
    //}

    //TODO 0729 Where is the logic of the lock object disappearing
    public void Ani_Reset()
    {
        _animator.SetTrigger(Reset);
    }

    private void Lock() // add event, in Reset animation
    {
        _lockRigidbody2D.velocity = Vector2.zero;
        _lockRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        _lockGameObject.transform.position = new Vector3(0, 2.05f, 0);
        _lockGameObject.transform.rotation = Quaternion.identity;
        _lockCollider2D.enabled = false;

        _lockGameObject.SetActive(true);
    }
    //TODO 0729

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

    #region Network
    [Command(requiresAuthority = false)]
    public void CmdMoveNextStage(string nextMapId)
    {
        RpcMoveNextStage(nextMapId);
    }
    
    [ClientRpc]
    private void RpcMoveNextStage(string nextMapId)
    {
        string curStage = Managers.Stage.stageName;

        Managers.Sound.CollectAmbientSoundSource();
        Managers.Stage.stageName = nextMapId;
        Camera.main.GetComponent<ParallaxCamera>().enabled = false;

        if (string.IsNullOrEmpty(nextMapId))
        {
            if(MapEditor.Instance.CurMap.mapID != "Lobby")
            {   
                if (MapEditor.Instance.CurMap.stageLevel == Managers.Game.stageLevel)
                {
                    //Managers.Game.stageLevel++;
                    //Managers.Data.saveData._SaveFileData._PlayerSaveData.curStageLevel = Managers.Game.stageLevel;
                    //Debug.Log("level++");
                    Managers.Game.StageClear(curStage,true);
                }

                //Managers.Game.StageLevelPlus()
                var player = Managers.Game.Player.GetComponent<PlayerSM>();
                if (player.isServer)
                {
                    Managers.Command.ChangeStage("Lobby");
                }
                

            }
            Debug.Log("Stage Clear");
            //Managers.Data.mapData.GetMainStageMapData(Managers.Game.stageLevel);
        }
        else
        {
            Managers.Game.CurrentState = GameState.Game;
            MapEditor.Instance.MoveNextStage(nextMapId);
            Managers.Game.StageClear(curStage);
        }

       
    }

    #endregion



}
