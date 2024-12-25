using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle_1_Button : MonoBehaviour
{
    private Animator animator;
    
    private readonly int FULLNESS = Animator.StringToHash("Fullness");

    private AirSM air;

    private float curChargeRate;

    private bool onFullCharge;
    private bool onCharging;

    private float recoverRate = 1;
    private float curRecoverRate =1;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        
    }

    private void Update()
    {
        if (onCharging)
        {
            curRecoverRate -= Time.deltaTime;
            if (curRecoverRate <= 0)
            {
                UnCharging();
            }
        }
    }

    public bool Charging()
    {
        if (onFullCharge) return false;

        onCharging = true;
        curRecoverRate = 1;
        curChargeRate += Time.deltaTime;
        if(curChargeRate >= 1)
        {
            onFullCharge = true;
            animator.SetFloat(FULLNESS, 1);

            
            return true;
        }

        animator.SetFloat(FULLNESS, curChargeRate);
        return false;
    }

    private void UnCharging()
    {
        curChargeRate -= Time.deltaTime;
        if (curChargeRate <= 0)
        {
            onCharging = false;
            curChargeRate = 0;
        }
        animator.SetFloat(FULLNESS, curChargeRate);
    }

    public void Wrong()
    {
        //test
        curChargeRate = 0;
        onCharging = false;
        onFullCharge = false;
        animator.SetFloat(FULLNESS, curChargeRate);
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(collision != null)
    //    {
    //        if(collision.TryGetComponent(out AirSM component))
    //        {
    //            air = component;
    //        }
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision != null)
    //    {
    //        if (collision.TryGetComponent(out AirSM component))
    //        {
    //            air = null;
    //        }
    //    }
    //}

}
