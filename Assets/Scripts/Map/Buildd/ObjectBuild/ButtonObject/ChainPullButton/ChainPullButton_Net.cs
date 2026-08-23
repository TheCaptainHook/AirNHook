using System.Collections;
using UnityEngine;
using Mirror;
using TMPro;

public class ChainPullButton_Net : ButtonEntity_Net
{
    [Space(20)]
    [Header("Save Data")]
    private ChainPullButton _cpb;
    private ChainPullButton _Main {get{_cpb ??= GetComponent<ChainPullButton>(); return _cpb;}}
    private int _maxChainLength => _Main._maxChainLength;
    private float _left_chain_condition_len=> _Main._left_chain_condition_len;
    private float _right_chain_condition_len=> _Main._right_chain_condition_len;

    [Space(10)]
    [SerializeField] private float _chainHailingPower = 2f;

#region Server
    [Space(30)]
    [ReadOnly]
    [SyncVar] public float _s_l_cur_chain_length;
    [ReadOnly]
    [SyncVar] public float _s_r_cur_chain_length;

    [Space(10)]
    [SerializeField] private float _recover_ChainSpeed;
    //================================================Recovery
    private Coroutine _l_recovery_coroutine;
    private Coroutine _r_recovery_coroutine;
    
    [SerializeField] private float _recoverDelay = 1f;
    [ReadOnly]
    public float _r_curRecoverDelay = 0;
    [ReadOnly]
    public float _l_curRecoverDelay = 0;
    private IEnumerator L_RecoveryCoroutine() //Server
    {
        while(true)
        {
            //====Air
            if(_Left_isAirGun_Attached)
            {
                _l_onRecovery = false;
                _l_recovery_coroutine = null;
                _l_curRecoverDelay = 0f;
                yield break;
            }
            //====Air

            _l_curRecoverDelay += Time.deltaTime;
            if(_l_curRecoverDelay < _recoverDelay)
            {
                yield return null;
                continue;
            }
            
            //=======Activation Or Deactivation Check
                CheckCondition();
            //=======Activation Or Deactivation Check
            
            if(_s_l_cur_chain_length > 0f)
            {
                _s_l_cur_chain_length = Mathf.MoveTowards(_s_l_cur_chain_length, 0f, _recover_ChainSpeed * Time.deltaTime);
                
            }else
            {
                _l_recovery_coroutine = null;
                _l_curRecoverDelay = 0f;
                _l_onRecovery = false;
                Rpc_Left_Chain_Reset();
                yield break;
            }

            yield return null;
        }
    }
    
    private IEnumerator R_RecoveryCoroutine() //Server
    {
        while(true)
        {
            //====Air
            if(_Right_isAirGun_Attached)
            {
                _r_onRecovery = false;
                _r_recovery_coroutine = null;
                _r_curRecoverDelay = 0f;
                yield break;
            }
            //====Air

            _r_curRecoverDelay += Time.deltaTime;
            if(_r_curRecoverDelay < _recoverDelay)
            {
                yield return null;
                continue;
            }
            
            //=======Activation Or Deactivation Check
                CheckCondition();
            //=======Activation Or Deactivation Check
            
            if(_s_r_cur_chain_length > 0f)
            {
                _s_r_cur_chain_length = Mathf.MoveTowards(_s_r_cur_chain_length, 0f, _recover_ChainSpeed * Time.deltaTime);
                
            }else
            {
                _r_recovery_coroutine = null;
                _r_curRecoverDelay = 0f;
                _r_onRecovery = false;
                Rpc_Right_Chain_Reset();
                yield break;
            }

            yield return null;
        }
    }
    //================================================Recovery

    

    private Coroutine _l_chain_coroutine;
    private Coroutine _r_chain_coroutine;

    void LateUpdate()
    {
        Condition_UI_Update();
    }
    [ServerCallback]
    private void Update()
    {
        //=======Left Safety Code
        if(_Left_isAirGun_Attached && IsChainLengthOverLimit &&_l_Chain.ParentConstraint.sourceCount > 0)
        {
            _s_l_CurSafety_code_delay+=Time.deltaTime;
            if(_s_l_CurSafety_code_delay>= _s_MaxSafety_code_delay)
            {
                Debug.Log("left Safety Code");
                _Left_isAirGun_Attached = false;
                Rpc_Air_L_ResetSubAction();
                Start_L_Recovery();
                _s_l_CurSafety_code_delay = 0;
            }
        }
        //=======Left Safety Code
        //=======Right Safety Code
        if(_Right_isAirGun_Attached && IsChainLengthOverLimit &&_r_Chain.ParentConstraint.sourceCount > 0)
        {
            _s_r_CurSafety_code_delay+=Time.deltaTime;
            if(_s_r_CurSafety_code_delay>= _s_MaxSafety_code_delay)
            {
                Debug.Log("right Safety Code");
                _Right_isAirGun_Attached = false;
                Rpc_Air_R_ResetSubAction();
                Start_R_Recovery();
                _s_r_CurSafety_code_delay = 0;
            }
        }
        //=======Right Safety Code
    }
#endregion
#region Chain
    [Space(20)]
    [ReadOnly]
    [SerializeField] private Transform _r_chain_handle; //red
    [ReadOnly]
    [SerializeField] private Transform _l_chain_handle; //blue

    [SerializeField] private GameObject _r_chain_end_prefab;
    [SerializeField] private GameObject _l_chain_end_prefab;
    [SerializeField] private Chain1 _l_Chain;
    [SerializeField] private Chain1 _r_Chain;
    
    [Space(20)]
    public GameObject _r_chain_end;
    public GameObject _l_chain_end;
    [Space(20)]

    // public bool _onSync;
    [ReadOnly]
    [SyncVar] public uint _r_chain_end_netId;
    [ReadOnly]
    [SyncVar] public uint _l_chain_end_netId;

    private AudioSourceController _audioSourceController;

#region Sound
    [ClientRpc]
    private void Rpc_Active_Sound()
    {
        
    }
    [ClientRpc]
    private void Rpc_Deactive_Sound()
    {
        
    }
#endregion

#endregion


#region Sync
    public void CreateChainEnd() //Server
    {
        if(_onSync) return;

        _r_chain_end = Managers.Stage.ServerBatchObejct("N_R_End");
        _l_chain_end = Managers.Stage.ServerBatchObejct("N_L_End");

        _r_chain_end_netId = GetNetId(_r_chain_end);
        _l_chain_end_netId = GetNetId(_l_chain_end);
    
    }
  
    private void ParentSync(uint l,uint r)
    {
        if(_onSync) return;

        GameObject left = NetworkClient.spawned.TryGetValue(l, out NetworkIdentity ll) ? ll.gameObject : null;
        GameObject right = NetworkClient.spawned.TryGetValue(r, out NetworkIdentity rr) ? rr.gameObject : null;

        if(left == null || right == null)
        {
            return;
        }
        ///======Left
        left.transform.SetParent(_l_chain_handle);
        left.transform.localPosition = Vector3.zero;
        left.transform.localScale = Vector3.one;
        left.transform.rotation = Quaternion.identity;
        _l_Chain._end = left.transform;
        ///======Left
        
        
        // =====Right
        right.transform.SetParent(_r_chain_handle);
        right.transform.localPosition = Vector3.zero;
        right.transform.localScale = Vector3.one;
        right.transform.rotation = Quaternion.identity;
        _r_Chain._end = right.transform;
        // =====Right
        

    }

    private uint GetNetId(GameObject obj)
    {
        if (obj.TryGetComponent(out NetworkIdentity identity))
        {
            return identity.netId;
        }

        return 999999;
    }
    protected override void Server_Sync_OtherValue()
    {
        if(_onSync) return;
        CreateChainEnd();

    }
    protected override void Set_Value(ButtonObjectStruct data)
    {
        transform.position = data.position;
        transform.rotation = data.quaternion;
        _Main._left_chain_condition_len = data.l_chain_len;
        _Main._right_chain_condition_len = data.r_chain_len;
        _Main._maxChainLength = data.max_chain_len;

        ParentSync(_l_chain_end_netId, _r_chain_end_netId);
    }
  
#endregion
#region Debug
    [Space(20)]
    [Header("Server Debug")]
    [SyncVar] public bool _Left_isHaling;
    [SyncVar] public bool _Left_isAirGun_Attached;
    [SyncVar] public bool _Left_isGrapping;
    [SyncVar] public bool _Right_isHaling;
    [SyncVar] public bool _Right_isAirGun_Attached;
    [SyncVar] public bool _Right_isGrapping;
    
    [Space(10)]
    public bool _r_onRecovery;
    public bool _l_onRecovery;
    [Space(10)]
    public TextMeshProUGUI _text_ui;
    private void Condition_UI_Update()
    {
        float result_right;
        float result_left;
        result_right = Mathf.Floor(_s_r_cur_chain_length/ _right_chain_condition_len * 1000)/1000;
        result_left = Mathf.Floor(_s_l_cur_chain_length/ _left_chain_condition_len * 1000)/1000;

        _text_ui.text = $"{result_left.ToString("F3")} / {result_right.ToString("F3")}";
    }
    [Space(10)]
    [SerializeField] private float _s_MaxSafety_code_delay = 0.2f;
    [ReadOnly]
    public float _s_l_CurSafety_code_delay;
    [ReadOnly]
    public float _s_r_CurSafety_code_delay;

    public bool IsChainLengthOverLimit => _s_l_cur_chain_length + _s_r_cur_chain_length > _maxChainLength;
    
    /// <summary>
    /// Save the left and right chain length constraint and the total chain length as data
    /// </summary>
    private void CheckCondition() //Server
    {
        if(_s_l_cur_chain_length >=_left_chain_condition_len && _s_r_cur_chain_length >=_right_chain_condition_len)
        {
            if(!_isActive)
            {
                _isActive =true;
                Rpc_Active_Sound();

                Main.Activation();
            }
        }else
        {
            if(_isActive)
            {
                _isActive = false;
                Rpc_Deactive_Sound();  

                Main.Deactivated();
            }   
        }
    }

#endregion Debug


#region Left
    [ClientRpc]
    private void Rpc_Left_Chain_Reset()
    {
        _l_Chain.Reset();
    }
    public void Start_L_Recovery()
    {
        _l_onRecovery = true;
        if(_l_recovery_coroutine != null)
        {
            StopCoroutine(_l_recovery_coroutine);
        }

        _l_recovery_coroutine = StartCoroutine(L_RecoveryCoroutine());
    }
    public void Stop_L_Recovery()
    {
        _l_onRecovery = false;
        _l_curRecoverDelay = 0f;
        if(_l_recovery_coroutine != null)
        {
            StopCoroutine(_l_recovery_coroutine);
            _l_recovery_coroutine = null;
        }
    }
    private IEnumerator L_ChainCoroutine(Transform target) //Server
    {
        Transform player = target;
        // Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        PlayerSM sm = player.GetComponent<PlayerSM>();

        while(true)
        {
            //=======Activation Or Deactivation Check
            CheckCondition();
            //=======Activation Or Deactivation Check

            if(!IsChainLengthOverLimit)
            {
                if(sm.characterType == CharacterType.Air)
                {
                    AirSM air  = player.GetComponent<AirSM>();
                    if(_Left_isAirGun_Attached)
                    {
                        _s_l_cur_chain_length = Vector2.Distance(_l_Chain._start.position, air.shakingEffectOnAirGun.transform.position);
                    }else
                    {
                        _s_l_cur_chain_length += Time.deltaTime * _chainHailingPower;
                    }
                }else if(sm.characterType == CharacterType.Hook)
                {
                   _s_l_cur_chain_length = Vector2.Distance(_l_Chain._start.position, sm.transform.position);
                }
                
            }else
            {
                if(_Left_isAirGun_Attached)
                {
                    Rpc_Air_L_ResetSubAction();
                    yield break;
                }

                if(_Left_isGrapping)
                {
                    HookSM hook = player.GetComponent<HookSM>();
                    hook.ReleaseItem();
                    Left_RPC_RemoveGrabSource();
                    Cmd_Stop_Track_L();
                    yield break;
                }
               
            }
            yield return null;
        }
    }

    [ClientRpc]
    private void Left_RPC_RemoveGrabSource()
    {
        _l_Chain.CCC.Constranint_Reset();
    }
#endregion
#region Right
    [ClientRpc]
    private void Rpc_Right_Chain_Reset()
    {
        _r_Chain.Reset();
    }
    public void Start_R_Recovery()
    {
        _r_onRecovery = true;
        if(_r_recovery_coroutine != null)
        {
            StopCoroutine(_r_recovery_coroutine);
        }

        _r_recovery_coroutine = StartCoroutine(R_RecoveryCoroutine());
    }
    public void Stop_R_Recovery()
    {
        _r_onRecovery = false;
        _r_curRecoverDelay = 0f;
        if(_r_recovery_coroutine != null)
        {
            StopCoroutine(_r_recovery_coroutine);
            _r_recovery_coroutine = null;
        }
    }
    private IEnumerator R_ChainCoroutine(Transform target) //Server
    {
        Transform player = target;
        // Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        PlayerSM sm = player.GetComponent<PlayerSM>();

        while(true)
        {
            //=======Activation Or Deactivation Check
            CheckCondition();
            //=======Activation Or Deactivation Check

            if(!IsChainLengthOverLimit)
            {
                if(sm.characterType == CharacterType.Air)
                {
                    AirSM air  = player.GetComponent<AirSM>();
                    if(_Right_isAirGun_Attached)
                    {
                        _s_r_cur_chain_length = Vector2.Distance(_r_Chain._start.position, air.shakingEffectOnAirGun.transform.position);
                    }else
                    {
                        _s_r_cur_chain_length += Time.deltaTime * _chainHailingPower;
                    }
                }else if(sm.characterType == CharacterType.Hook)
                {
                   _s_r_cur_chain_length = Vector2.Distance(_r_Chain._start.position, sm.transform.position);
                }
                
            }else
            {
                if(_Right_isAirGun_Attached)
                {
                    Rpc_Air_R_ResetSubAction();
                    yield break;
                }

                if(_Right_isGrapping)
                {
                    HookSM hook = player.GetComponent<HookSM>();
                    hook.ReleaseItem();
                    Right_RPC_RemoveGrabSource();
                    Cmd_Stop_Track_R();
                    yield break;
                }
               
            }
            yield return null;
        }
    }
     [ClientRpc]
    private void Right_RPC_RemoveGrabSource()
    {
        _r_Chain.CCC.Constranint_Reset();
    }
#endregion

#region Air
    [ClientRpc]
    private void Rpc_Air_L_ResetSubAction()
    {
        _s_l_CurSafety_code_delay = 0;
        _Left_isAirGun_Attached = false;
        _l_Chain.DeletConstraint();
        AirSM air = Managers.Game.Player.TryGetComponent(out AirSM sm) ? sm : null;
        if(air != null)
        {
            _l_Chain.CCC.StopInhale(gameObject);
            Managers.Game.playerInput.playerActions.SubAction.Disable();
            Managers.Game.playerInput.playerActions.SubAction.Enable(); 
        }
    }
    private void Rpc_Air_R_ResetSubAction()
    {
        _s_r_CurSafety_code_delay = 0;
        _Right_isAirGun_Attached = false;
        _r_Chain.DeletConstraint();
        AirSM air = Managers.Game.Player.TryGetComponent(out AirSM sm) ? sm : null;
        if(air != null)
        {
            _r_Chain.CCC.StopInhale(gameObject);
            Managers.Game.playerInput.playerActions.SubAction.Disable();
            Managers.Game.playerInput.playerActions.SubAction.Enable(); 
        }
    }
    // [Command(requiresAuthority = false)]
    // public void Cmd_Start_Hailing_Track_L(uint targetNetID)
    // {
    //     Stop_L_Recovery();

    //     GameObject target = NetworkClient.spawned.TryGetValue(targetNetID, out NetworkIdentity identity) ? identity.gameObject : null;
    //     if(target == null) return;

    //     AirSM player = target.GetComponent<AirSM>();

    //     _Left_isHaling = true;

    //     if(_l_chain_coroutine != null)
    //         StopCoroutine(_l_chain_coroutine);
    //     _l_chain_coroutine = StartCoroutine(L_ChainCoroutine(player.shakingEffectOnAirGun.transform));
    // }
    // [Command(requiresAuthority = false)]
    // public void Cmd_Start_Hailing_Track_R(uint targetNetID)
    // {
    //     Stop_R_Recovery();
    //     GameObject target = NetworkClient.spawned.TryGetValue(targetNetID, out NetworkIdentity identity) ? identity.gameObject : null;
    //     if(target == null) return;

    //     AirSM player = target.GetComponent<AirSM>();

    //     _Right_isHaling = true;

    //     if(_r_chain_coroutine != null)
    //         StopCoroutine(_r_chain_coroutine);
    //     _r_chain_coroutine = StartCoroutine(R_ChainCoroutine(player.shakingEffectOnAirGun.transform));
    // }
    //====
    [Command(requiresAuthority = false)]
    public void Cmd_Start_Track_R(uint targetNetID)
    {
        Stop_R_Recovery();

        PlayerSM target = NetworkClient.spawned.TryGetValue(targetNetID, out NetworkIdentity identity) ? 
            identity.TryGetComponent(out PlayerSM sm) ? 
                sm : null : null;
        if(target == null) return;

        if(_r_chain_coroutine != null) StopCoroutine(_r_chain_coroutine);

        if(target.characterType == CharacterType.Air)
        {
            AirSM player = target.GetComponent<AirSM>();
            _Right_isHaling = true;
            _r_chain_coroutine = StartCoroutine(R_ChainCoroutine(player.shakingEffectOnAirGun.transform));

        }else if(target.characterType == CharacterType.Hook)
        {
            PlayerSM player = target.GetComponent<HookSM>();
            _Right_isGrapping = true;
            _r_chain_coroutine = StartCoroutine(R_ChainCoroutine(player.gameObject.transform));
        }
    }
    [Command(requiresAuthority = false)]
    public void Cmd_Start_Track_L(uint targetNetID)
    {
        Stop_L_Recovery();

        PlayerSM target = NetworkClient.spawned.TryGetValue(targetNetID, out NetworkIdentity identity) ? 
            identity.TryGetComponent(out PlayerSM sm) ? 
                sm : null : null;
        if(target == null) return;

        if(_l_chain_coroutine != null) StopCoroutine(_l_chain_coroutine);

        if(target.characterType == CharacterType.Air)
        {
            AirSM player = target.GetComponent<AirSM>();
            _Left_isHaling = true;
            _l_chain_coroutine = StartCoroutine(L_ChainCoroutine(player.shakingEffectOnAirGun.transform));

        }else if(target.characterType == CharacterType.Hook)
        {
            PlayerSM player = target.GetComponent<HookSM>();
            _Left_isGrapping = true;
            // _l_chain_coroutine = StartCoroutine(L_ChainCoroutine(player.grabSource.sourceTransform)); 
             _l_chain_coroutine = StartCoroutine(L_ChainCoroutine(player.gameObject.transform)); 
        }
    }
    //====



   
   [Command(requiresAuthority = false)]
    public void Cmd_Stop_Track_L()
    {
        //==Reset
        _Left_isAirGun_Attached = false;
        _Left_isHaling = false;
        _Left_isGrapping = false;

        if(_l_chain_coroutine != null)
        {
            StopCoroutine(_l_chain_coroutine);
            _l_chain_coroutine = null;
        }
        //==Reset
        Debug.Log("Stpo L");
        Start_L_Recovery();
    }
    [Command(requiresAuthority = false)]
    public void Cmd_Stop_Track_R()
    {
        //==Reset
        _Right_isAirGun_Attached = false;
        _Right_isHaling = false;
        _Right_isGrapping = false;
        
        if(_r_chain_coroutine != null)
        {
            StopCoroutine(_r_chain_coroutine);
            _r_chain_coroutine = null;
        }
        //==Reset
        Start_R_Recovery();
    }
    #endregion

#region Hook
// [Command(requiresAuthority = false)]
// public void Cmd_R_Hook_Interaction(uint id)
// {
//     GameObject player = NetworkServer.spawned.TryGetValue(id,out NetworkIdentity identity) ? identity.gameObject : null;
//     if(player == null) return;

//     // ParentConstraint pc;
// }
// [Command(requiresAuthority = false)]
// public void Cmd_L_Hook_Interaction(uint id)
// {
    
// }

#endregion

#region Clean

#endregion

    protected override void Rpc_Clean()
    {
        if(_l_chain_coroutine != null)
        {
            StopCoroutine(_l_chain_coroutine);
            _l_chain_coroutine = null;
        }
        if(_r_chain_coroutine != null)
        {
            StopCoroutine(_r_chain_coroutine);
            _r_chain_coroutine = null;
        }
        
        if(NetworkServer.active)
        {
            NetworkServer.Destroy(_r_chain_end);
            NetworkServer.Destroy(_l_chain_end);
        }

        base.Rpc_Clean();
    }
   
}
