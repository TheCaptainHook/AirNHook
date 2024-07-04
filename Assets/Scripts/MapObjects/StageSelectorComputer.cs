using System;
using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using UnityEngine;

public class StageSelectorComputer : MonoBehaviour
{
    #region StringCache
    private static readonly int Talk = Animator.StringToHash("Talk");
    private static readonly int Left = Animator.StringToHash("Left");
    private static readonly int Right = Animator.StringToHash("Right");
    private static readonly int Surprise = Animator.StringToHash("Surprise");
    private static readonly int Reset = Animator.StringToHash("Reset");
    private static readonly int Line = Animator.StringToHash("Line");
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

    //private void Update() //테스트용
    //{
    //    //Click();
    //    //KeySpawn();
    //    //Talking();
    //}




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

    //private void Talking()
    //{
    //    _animator.SetBool(IsTalking, _isTalking);
    //}



    public void Talking()
    {
        _animator.SetTrigger(Talk);
    }
    public void LineIdle()
    {
        _animator.SetTrigger(Line);
    }
    public void Right_()
    {
        _animator.SetTrigger(Right);
    }
    public void Left_()
    {
        _animator.SetTrigger(Left);
    }

    public void Surprise_()
    {
        _animator.SetTrigger(Surprise);
    }



    public void SpawnKey()
    {
        ExitPointObj obj = MapEditor.Instance.exitDoorObjectTransform.GetChild(0).gameObject.GetComponent<ExitPointObj>();
        if (obj.nextMapId != string.Empty)
        {
            if (_key == null)
            {
                _key = Managers.Stage.CmdBatchObject("Key");
            }

            ObjectData data = MapEditor.Instance.CurMap.FindObjectData(1000);
            _key.transform.position = data.position;
            Vector2 launchDirection = new Vector2(-1, 1).normalized;
            _animator.SetTrigger(Left);
            _key.GetComponent<Rigidbody2D>().AddForce(launchDirection * 5f, ForceMode2D.Impulse);
            
        }

    }



}
