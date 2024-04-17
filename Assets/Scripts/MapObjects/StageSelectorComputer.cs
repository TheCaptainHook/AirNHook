using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectorComputer : MonoBehaviour
{
    #region StringCache
    private static readonly int IsTalking = Animator.StringToHash("IsTalking");
    private static readonly int LeftClick = Animator.StringToHash("LeftClick");
    private static readonly int RightClick = Animator.StringToHash("RightClick");
    private static readonly int Surprise = Animator.StringToHash("Surprise");
    #endregion

    private Animator _animator;
    
    [SerializeField] private bool _isTalking = false;
    
    [SerializeField] private GameObject _key;
    [SerializeField] private Rigidbody2D _keyrb;
    
    //애니메이션 test용
    public bool IsLeftClicking;
    public bool IsRightClicking;
    public bool IsSpawningKey;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update() //테스트용
    {
        //Click();
        //KeySpawn();
        Talking();
    }

    //private void Click() // 테스트용
    //{
    //    if (IsLeftClicking)
    //    {
    //        _animator.SetTrigger(LeftClick);
    //        IsLeftClicking = false;
    //    }
            

    //    if (IsRightClicking)
    //    {
    //        _animator.SetTrigger(RightClick);
    //        IsRightClicking = false;
    //    }
    //}

    //private void KeySpawn() // 테스트용
    //{
    //    if (IsSpawningKey)
    //    {
    //        _animator.SetTrigger(Surprise);
    //        _key.SetActive(true);
    //        Vector2 launchDirection = new Vector2(-1, 1).normalized;
    //        _keyrb.AddForce(launchDirection * 5f, ForceMode2D.Impulse);
    //        IsSpawningKey = false;
    //    }
    //}

    private void Talking()
    {
        _animator.SetBool(IsTalking, _isTalking);
    }
    
}
