using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerAnim : MonoBehaviour
{
    [SerializeField] private GameObject _disappearingObj; 
    private Animator _animator;
    
    #region StringCache
    private static readonly int Changing = Animator.StringToHash("Changing");
    #endregion
    
    public bool IsChanging = false;
    
    public event Action OnChangingAnimation;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        OnChangingAnimation += SetTriggerChanging;
    }
    private void SetTriggerChanging()
    {
        IsChanging = true;
        _animator.SetTrigger(Changing);
    }
    public void CallOnChangingAnimation()
    {
        OnChangingAnimation?.Invoke();
    }
    
    //이하 애니메이션 테스트용 코드
    
    private void Update()
    {
        if (IsChanging)
        {
            _animator.SetTrigger(Changing);
        }
    }

    public void DestroyGO()
    {
        Destroy(_disappearingObj);
    }
}
