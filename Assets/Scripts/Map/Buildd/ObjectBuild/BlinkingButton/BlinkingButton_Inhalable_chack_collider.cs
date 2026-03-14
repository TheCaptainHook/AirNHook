using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingButton_Inhalable_chack_collider : MonoBehaviour,IInhalable
{
    BlinkingButton_Net _net;
    BlinkingButton_Net Net {get { _net ??= GetComponent<BlinkingButton_Net>(); return _net; } }


    private float _deadZone = 0.1f;
    private int Dot_Check(Transform accessor)
    {
        Vector2 toAcc = (Vector2)(accessor.position - transform.position);
        Vector2 right = transform.right;

        // dot > 0 : 오른쪽 반평면, dot < 0 : 왼쪽 반평면 (right 기준)
        float dot = Vector2.Dot(toAcc.normalized, right);
        
        if (dot > _deadZone)
        {
            return 1;
        }
        else if(dot<-_deadZone)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }





   #region IInhalable
    public void Inhalation(Transform accessor)
    {
        Debug.Log("Start InHal,Blink");
        
        if(Dot_Check(accessor) == 1)
        {
            Net.Cmd_Air_Active(true);
        }
        else if(Dot_Check(accessor) == -1)
        {
            Net.Cmd_Air_Active(false);
        }
        else
        {
            Debug.Log("Deadzone");
        }


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
        return;
    }

    public bool Inhaling(bool value, GameObject player)
    {
        return true;
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
