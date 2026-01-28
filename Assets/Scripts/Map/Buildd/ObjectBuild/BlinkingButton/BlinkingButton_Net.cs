using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 기본이 파랑색, 파랑색 켜져있고,
/// 빨간색은 꺼져있음
/// </summary>
public class BlinkingButton_Net : NetworkBehaviour
{
    private Animator _animator;
    private Animator Animator { get { _animator ??= GetComponent<Animator>(); return _animator; } }
    private int On = Animator.StringToHash("BlueOn");

    private bool _onOff = false; // true = Red, false = Blue
    [SerializeField] private float _maxCooltime;
    private float _curCooltime = 0f;

    private bool _server_bool = true;
    //_on : true = Red, false = Blue

    [ServerCallback]
    private void Update()
    {
        if (_curCooltime > 0f && !_server_bool)
        {
            _curCooltime -= Time.deltaTime;
            if(_curCooltime < 0f)
            {
                _server_bool = true;
            }
                
        }
    }

    public void Cmd_Active()
    {
        if(_server_bool)
        {
            _curCooltime = _maxCooltime;
            _server_bool = false;
            Rpc_Active();
        }
    }
    
    [ClientRpc]
    private void Rpc_Active()
    {
        _onOff = !_onOff; // true = Red, false = Blue
        Animator.SetBool(On, _onOff);

        MapEditor.Instance.CallBlinkingBoxEvent_Red();
        MapEditor.Instance.CallBlinkingBoxEvent_Blue();
    }


    public void Clean()
    {
        _onOff = false;
        Animator.SetBool(On, _onOff);
        
        Animator.Rebind();
        Animator.Update(0f);

        _curCooltime = 0f;
        _server_bool = true;
    }
}
