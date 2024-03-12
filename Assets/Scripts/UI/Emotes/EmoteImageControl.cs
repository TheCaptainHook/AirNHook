using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteImageControl : MonoBehaviour
{
    private Animator _animator;
    private static readonly int MouseOver = Animator.StringToHash("MouseOver");

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void OnMouseEnter()
    {
        _animator.SetBool(MouseOver, true);
    }
    public void OnMouseExit()
    {
        _animator.SetBool(MouseOver, false);
    }
}
