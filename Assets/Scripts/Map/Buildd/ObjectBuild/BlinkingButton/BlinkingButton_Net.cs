using System.Collections;
using System.Collections.Generic;
using Mirror;
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

    private bool _server_bool = true; // server activation permission flag

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
    private Coroutine _blinkCoroutine;

    private void Active()
    {
        Animator.SetBool(On, _onOff);
        if(_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
        _blinkCoroutine = StartCoroutine(BlinkCoroutine());
    }
    private IEnumerator BlinkCoroutine()
    {
        AnimatorStateInfo info = Animator.GetCurrentAnimatorStateInfo(0);
       yield return new WaitForSeconds(info.length);

        //Sound
        //Managers.Sound.PlaySound3D(GlobalText., transform.position, 1, true);
        //Sound

        MapEditor.Instance.CallBlinkingBoxEvent_Red();
        MapEditor.Instance.CallBlinkingBoxEvent_Blue();

    }

    #region  Air
    // [Command(requiresAuthority = false)]
    // public void Cmd_Air_Active(bool rL)
    // {
    //     if(!_server_bool) return;
   
    //     _curCooltime = _maxCooltime;
    //     _server_bool = false;
    //     Rpc_Air_Active(rL);
    // }
    // /// <summary>
    // /// rL == true : Right -> Change Red
    // /// rL == false : Left -> Change Blue
    // /// </summary>
    // /// <param name="rL"></param>
    // [ClientRpc]
    // public void Rpc_Air_Active(bool rL)
    // {
    //     if(rL == _onOff) return;

    //     _onOff = !_onOff;
    //     Active();
    // } 
    #endregion
    #region Hook
    [Command(requiresAuthority = false)]
    public void Cmd_Active()
    {
       if(!_server_bool)return;

        _curCooltime = _maxCooltime;
        _server_bool = false;
        Rpc_Active();
        
    }
    
    [ClientRpc]
    private void Rpc_Active()
    {
        _onOff = !_onOff; // true = Red, false = Blue
        Active();
    }
    #endregion

    public void Clean()
    {
        if(_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
        _blinkCoroutine = null; 

        _onOff = false;
        Animator.SetBool(On, _onOff);
        
        Animator.Rebind();
        Animator.Update(0f);

        _curCooltime = 0f;
        _server_bool = true;
    }
}
