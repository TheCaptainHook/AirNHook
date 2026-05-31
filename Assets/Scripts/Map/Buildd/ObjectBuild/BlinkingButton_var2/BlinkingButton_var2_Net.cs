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

    //============Server
    [SyncVar] public float _s_l_cur_chain_length;
    [SyncVar] public float _s_r_cur_chain_length;
    
    //============Server
    //============Local
    public float _l_l_cur_chain_length;
    public float _l_r_cur_chain_length;
    private bool L_IsChainLengthOverLimit => _l_l_cur_chain_length + _l_r_cur_chain_length > _maxChainLength;
    //============Local

    private IEnumerator L_ChainCoroutine(Transform target)
    {
        Transform player = target.root;
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        while(true)
        {
            float distance = Vector3.Distance(target.position ,_l_Chain._start.position);
            // Debug.Log($"{_l_Chain.Get_StartEndDistance()}, {_l_l_cur_chain_length}");
            if(!L_IsChainLengthOverLimit)
            {
                 _l_l_cur_chain_length += Time.deltaTime * _chainHailingPower;

                //엔드와스타트지점의 거리가 체인길이보다 작으면 _ㅣ_ㅣ_cur_chain_length를 줄이기.


                _l_Chain.SetMaxLength(_l_l_cur_chain_length);
            }else
            {
                if(distance >=_l_l_cur_chain_length)
                {
                    Vector2 startToPlayer = playerRb.position - (Vector2)_l_Chain._start.position;
                    Vector2 dir = startToPlayer.normalized;
                    float outwardspeed = Vector2.Dot(playerRb.velocity, dir);
                    if(outwardspeed>0f)
                    {
                        playerRb.velocity -= dir * outwardspeed;
                    }
                }
                
                
                // player.position = _l_Chain._start.position + dir * _maxChainLength;
            }

            Debug.Log(player.GetComponent<AirSM>().airGun._isAttached);

            
            yield return null;
        }
    }
    

    #region Util
    
    #endregion
    [Command(requiresAuthority = false)]
    public void Cmd_Start_Track_L(uint targetNetID)
    {
        GameObject target = NetworkClient.spawned.TryGetValue(targetNetID, out NetworkIdentity identity) ? identity.gameObject : null;
        if(target == null) return;

        AirSM player = target.GetComponent<AirSM>();

        if(_l_chain_coroutine != null)
            StopCoroutine(_l_chain_coroutine);
        _l_chain_coroutine = StartCoroutine(L_ChainCoroutine(player.shakingEffectOnAirGun.transform));
    }
   
   [Command(requiresAuthority = false)]
    public void Cmd_Stop_Track_L()
    {
        if(_l_chain_coroutine != null)
        {
            StopCoroutine(_l_chain_coroutine);
            _l_chain_coroutine = null;
        }
    }
//============Server
  
    private void Active()
    {
        // Animator.SetBool(On, _onOff);
        MapEditor.Instance.CallBlinkingBoxEvent_Red();
        MapEditor.Instance.CallBlinkingBoxEvent_Blue();
    }

    
    //private bool _server_bool = true; // server activation permission flag


    [ServerCallback]
    private void Update()
    {
        // if (_curCooltime > 0f && !_server_bool)
        // {
        //     _curCooltime -= Time.deltaTime;
        //     if(_curCooltime < 0f)
        //     {
        //         _server_bool = true;
        //     }
                
        // }
    }

#region  Air
   
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

        left.transform.SetParent(_l_chain_handle);
        left.transform.localPosition = Vector3.zero;
        left.transform.localScale = Vector3.one;
        _l_Chain._end = left.transform;

        right.transform.SetParent(_r_chain_handle);
        right.transform.localPosition = Vector3.zero;
        right.transform.localScale = Vector3.one;
        _r_Chain._end = right.transform;

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

#endregion//Air

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
