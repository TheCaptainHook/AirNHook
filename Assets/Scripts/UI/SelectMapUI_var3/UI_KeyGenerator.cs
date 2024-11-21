using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_KeyGenerator : MonoBehaviour
{
    [SerializeField] Animator animator;
    private static readonly int KeyPrinting = Animator.StringToHash("KeyPrinting");



    public void KeyPrintingAni()
    {
        Managers.Sound.PlaySound(GlobalText.KET_PRINTING_SOUND, 0.35f);
        animator.SetTrigger(KeyPrinting);
    }
}
