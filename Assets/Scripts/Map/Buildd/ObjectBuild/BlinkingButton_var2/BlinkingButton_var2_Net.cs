using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BlinkingButton_var2_Net : NetworkBehaviour
{
    [SyncVar] private bool _onActive;

//============Server
    [SerializeField] private float _maxChainLength = 5f;
    private float _curCooltime = 0f;
    [SerializeField] private float _maxCooltime;

    private Coroutine _l_chain_coroutine;
    private Coroutine _r_chain_coroutine;

    public float _chainHailingPower = 2f;
#region Debug
    [Space(20)]
    [Header("Server Debug")]
    [SyncVar] public bool _Left_isHaling;
    [SyncVar] public bool _Left_isAirGun_Attached;
    [SyncVar] public bool _Right_isHaling;
    [SyncVar] public bool _Right_isAirGun_Attached;
    public bool _onRecovery;
#endregion Debug

#region  Server
    [Space(20)]
    [SyncVar] public float _s_l_cur_chain_length;
    [SyncVar] public float _s_r_cur_chain_length;

    [SerializeField] private float _recover_ChainSpeed;
    //Recovery
    private Coroutine _l_recovery_coroutine;
    private Coroutine _r_recovery_coroutine;
    private float _recoverDelay = 0.2f;
    public float _curRecoverDelay = 0;
    public bool IsChainLengthOverLimit => _s_l_cur_chain_length + _s_r_cur_chain_length > _maxChainLength;
    
    /**
    Callback server update
        - _isAirGun_Attached = true -> AirSm.StopGun,
    **/

    private float _s_MaxSafety_code_delay = 0.3f;
    public float _s_CurSafety_code_delay;
    
    [ServerCallback]
    private void Update()
    {
        if(_Left_isAirGun_Attached && IsChainLengthOverLimit &&_l_Chain.ParentConstraint.sourceCount > 0)
        {
            _s_CurSafety_code_delay+=Time.deltaTime;
            if(_s_CurSafety_code_delay>= _s_MaxSafety_code_delay)
            {
                Debug.Log("Safety Code");
                _Left_isAirGun_Attached = false;
                Rpc_Air_ResetSubAction();
                Start_L_Recovery();
                _s_CurSafety_code_delay = 0;
            }
        }
    }


    private IEnumerator L_RecoveryCoroutine() //Server
    {
        while(true)
        {
            //====Air
            if(_Left_isAirGun_Attached)
            {
                _onRecovery = false;
                _l_recovery_coroutine = null;
                _curRecoverDelay = 0f;
                yield break;
            }
            //====Air

            _curRecoverDelay += Time.deltaTime;
            if(_curRecoverDelay < _recoverDelay)
            {
                yield return null;
                continue;
            }

            if(_s_l_cur_chain_length > 0f)
            {
                _s_l_cur_chain_length = Mathf.MoveTowards(_s_l_cur_chain_length, 0f, _recover_ChainSpeed * Time.deltaTime);
            }else
            {
                _l_recovery_coroutine = null;
                _curRecoverDelay = 0f;
                _onRecovery = false;
                Rpc_Left_Chain_Reset();
                yield break;
            }

            yield return null;
        }
    }
    [ClientRpc]
    private void Rpc_Left_Chain_Reset()
    {
        _l_Chain.Reset();
    }
    
    public void Start_L_Recovery()
    {
        _onRecovery = true;
        if(_l_recovery_coroutine != null)
        {
            StopCoroutine(_l_recovery_coroutine);
        }

        _l_recovery_coroutine = StartCoroutine(L_RecoveryCoroutine());
    }
    public void Stop_L_Recovery()
    {
        _onRecovery = false;
        _curRecoverDelay = 0f;
        if(_l_recovery_coroutine != null)
        {
            StopCoroutine(_l_recovery_coroutine);
            _l_recovery_coroutine = null;
        }
    }
    
    //Recovery
    private IEnumerator L_ChainCoroutine(Transform target) //Server
    {
        Transform player = target.root;
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        AirSM air  = player.GetComponent<AirSM>();

        while(true)
        {
            if(!IsChainLengthOverLimit)
            {
                if(_Left_isAirGun_Attached)
                {
                    _s_l_cur_chain_length = Vector2.Distance(_l_Chain._start.position, air.shakingEffectOnAirGun.transform.position);
                }else
                {
                    _s_l_cur_chain_length += Time.deltaTime * _chainHailingPower;
                }
            }else
            {
                if(_Left_isAirGun_Attached)
                {
                    // _l_Chain.Test1();
                    // _Left_isAirGun_Attached = false;/

                    // air.CmdStopInhalePlayer();

                    Rpc_Air_ResetSubAction();

                    yield break;
                }
            }

            yield return null;
        }
    }
    [ClientRpc]
    private void Rpc_Air_ResetSubAction()
    {
        _s_CurSafety_code_delay = 0;
        _Left_isAirGun_Attached = false;
        _l_Chain.DeletConstraint();
        AirSM air = Managers.Game.Player.TryGetComponent(out AirSM sm) ? sm : null;
        if(air != null)
        {
            _l_Chain.BICCV.StopInhale(gameObject);
            Managers.Game.playerInput.playerActions.SubAction.Disable();
            Managers.Game.playerInput.playerActions.SubAction.Enable(); 
        }
    }


    private void StopHailing()
    {
        
    }
#endregion//Server
#region  Cmd
    [Command(requiresAuthority = false)]
    public void Cmd_Start_Hailing_Track_L(uint targetNetID)
    {
        Stop_L_Recovery();

        GameObject target = NetworkClient.spawned.TryGetValue(targetNetID, out NetworkIdentity identity) ? identity.gameObject : null;
        if(target == null) return;

        AirSM player = target.GetComponent<AirSM>();

        _Left_isHaling = true;

        if(_l_chain_coroutine != null)
            StopCoroutine(_l_chain_coroutine);
        _l_chain_coroutine = StartCoroutine(L_ChainCoroutine(player.shakingEffectOnAirGun.transform));
    }
   
   [Command(requiresAuthority = false)]
    public void Cmd_Stop_Track_L()
    {
        //==Air
        _Left_isAirGun_Attached = false;
        _Left_isHaling = false;

        if(_l_chain_coroutine != null)
        {
            StopCoroutine(_l_chain_coroutine);
            _l_chain_coroutine = null;
        }
        //==Air

        //==Hook
        //==Hook

        Start_L_Recovery();
    }
#endregion //Cmd
  
    private void Active()
    {
        // Animator.SetBool(On, _onOff);
        MapEditor.Instance.CallBlinkingBoxEvent_Red();
        MapEditor.Instance.CallBlinkingBoxEvent_Blue();
    }


#region Chain
    [Space(20)]
    [SerializeField] private Transform _r_chain_handle; //red
    [SerializeField] private Transform _l_chain_handle; //blue

    [SerializeField] private GameObject _r_chain_end_prefab;
    [SerializeField] private GameObject _l_chain_end_prefab;
    [SerializeField] private Chain1 _l_Chain;
    [SerializeField] private Chain1 _r_Chain;
    
    [Space(20)]
    public GameObject _r_chain_end;
    public GameObject _l_chain_end;
    [Space(20)]

    public bool _onSync;

    [SyncVar] public uint _r_chain_end_netId;
    [SyncVar] public uint _l_chain_end_netId;

#region  Sync
    private Coroutine _syncCoroutine;
    public void CreateChainEnd() //Server
    {
        _r_chain_end = Managers.Stage.ServerBatchObejct("N_R_End");
        _l_chain_end = Managers.Stage.ServerBatchObejct("N_L_End");

        _r_chain_end_netId = GetNetId(_r_chain_end);
        _l_chain_end_netId = GetNetId(_l_chain_end);

        // ParentSync(_l_chain_end_netId, _r_chain_end_netId);
        if(_syncCoroutine != null)
        {
            StopCoroutine(_syncCoroutine);
        }
        _syncCoroutine =StartCoroutine(SyncChainParentCoroutine());
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        // ParentSync(_l_chain_end_netId, _r_chain_end_netId);
        if(_syncCoroutine != null)
        {
            StopCoroutine(_syncCoroutine);
        }
        _syncCoroutine =StartCoroutine(SyncChainParentCoroutine());
    }
    private IEnumerator SyncChainParentCoroutine()
    {
        while(!_onSync)
        {
            ParentSync(_l_chain_end_netId, _r_chain_end_netId);
            yield return null;
        }
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
        
        
        /// =====Right
        // right.transform.SetParent(_r_chain_handle);
        // right.transform.localPosition = Vector3.zero;
        // right.transform.localScale = Vector3.one;
        // right.transform.rotation = Quaternion.identity;
        // _r_Chain._end = right.transform;
        /// =====Right
        /// 
        _onSync = true;

    }

    private uint GetNetId(GameObject obj)
    {
        if (obj.TryGetComponent(out NetworkIdentity identity))
        {
            return identity.netId;
        }

        return 999999;
    }
#endregion//Sync

#endregion//Chain


#region Clean

    [Command(requiresAuthority = false)]
    public void Cmd_Clean()
    {
        if(_l_chain_coroutine != null)
        {
            StopCoroutine(_l_chain_coroutine);
            _l_chain_coroutine = null;
        }

        Rpc_Clean();
    }
    [ClientRpc]
    private void Rpc_Clean()
    {
        //  _curCooltime = 0f;
        _r_chain_end_netId = 999999;
        _l_chain_end_netId = 999999;

        if(NetworkServer.active)
        {
            NetworkServer.Destroy(_r_chain_end);
            NetworkServer.Destroy(_l_chain_end);
        }
        _onSync = false;

    }

    public void Clean()
    {
        Cmd_Clean();
    }

#endregion

}
