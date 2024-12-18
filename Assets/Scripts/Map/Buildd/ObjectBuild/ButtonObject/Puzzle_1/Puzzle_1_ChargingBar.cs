using GoogleSheet.Type;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle_1_ChargingBar : MonoBehaviour
{
    [SerializeField] Image bar_Image;

    [SerializeField] GameObject container;
    
    private float recoverSpeed = 2;
    public bool onCharging;

    private bool onPrograss;

    private bool onFullBar;


    public Puzzle_1 puzzle;
    #region Components
    [SerializeField] Animator animator;
    #endregion
    //charging -> full bar -> fillamount =0

    readonly int shaking = Animator.StringToHash("onShaking");

    #region Main
    public void Charging()
    {
        if (onPrograss) return;
        if (!container.activeSelf) container.SetActive(true);

        onCharging = true;
        bar_Image.fillAmount += Time.deltaTime;

        if (bar_Image.fillAmount >= 1) 
        {
            puzzle.Power();
            StartCoroutine(FullBarCo()); 

        }

    }
    #endregion


    private void Update()
    {
        if (!onFullBar && !onCharging && !onPrograss) Recover();
    }

    private void Recover()
    {
        if (bar_Image.fillAmount <= 0) return;

        bar_Image.fillAmount -= Time.deltaTime * recoverSpeed;

        if (bar_Image.fillAmount <= 0)
        {
            bar_Image.fillAmount = 0;
            container.SetActive(false);
        }

    }

   IEnumerator FullBarCo()
    {  
        onPrograss = true;

        animator.SetBool(shaking, true);
        yield return new WaitForSeconds(1f);
       
        while (bar_Image.fillAmount >0)
        {
            bar_Image.fillAmount -= Time.deltaTime;
            yield return null;
        }

        animator.SetBool(shaking, false);

        onPrograss = false;
        onFullBar = false;
        container.SetActive(false);
        bar_Image.fillAmount = 0;
    }


    public void BarReset()
    {
        onFullBar = false;
        bar_Image.fillAmount=0;
    }
}
