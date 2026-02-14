using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics; // 

using Debug = UnityEngine.Debug;

public class BlinkingButton_Inhalable_chack_collider : MonoBehaviour,IInhalable
{
    BlinkingButton_Net _net;
    BlinkingButton_Net Net {get { _net ??= GetComponent<BlinkingButton_Net>(); return _net; } }


   #region IInhalable
    public void Inhalation(Transform accessor)
    {
        Net.Cmd_Active();
        Debug.Log($"InHal,Blink, {accessor.name}");
    //에어 사용 중, 후크 상호작용 불가
    //타겟위치 확인해서 왼쪽인지 오른쪽인지 파악
    }

    public void StopInhale(GameObject accessor)
    {
        Debug.Log("Stop InHal,Blink");
        //1. 애니메이션 종료
        //2. 에어 사용중 해제
        //3. 초기화 
    }

    public void Fixed(bool value)
    {
        
    }

    public bool Inhaling(bool value, GameObject player)
    {
        return false;
    }

    public void Shooting(Vector2 force)
    {
        return;
    }

    public bool CanInhale()
    {
        return true;
    }
    #endregion
}
