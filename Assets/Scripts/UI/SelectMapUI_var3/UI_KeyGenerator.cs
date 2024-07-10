using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_KeyGenerator : MonoBehaviour
{
    [SerializeField] Animator animator;
    private static readonly int KeyPrinting = Animator.StringToHash("KeyPrinting");



    public void KeyPrintingAni()
    {
        animator.SetTrigger(KeyPrinting);
    }
}
