using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraImageEffects : MonoBehaviour
{
    public Animator animator;
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        animator = GetComponent<Animator>();
    }
}
