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

    //_on : true = Red, false = Blue
    public void Active()
    {
  
    }
    
    [ClientRpc]
    private void Rpc_Active(BLINKBOX_COLOR color)
    {
  
    }
}
