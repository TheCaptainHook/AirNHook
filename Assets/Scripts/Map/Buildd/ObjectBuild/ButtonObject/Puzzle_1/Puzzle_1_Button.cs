using System.Collections;
using UnityEngine;


public class Puzzle_1_Button : MonoBehaviour
{
    //[SerializeField] Puzzle_1 puzzle_1;
    [SerializeField] Puzzle_1_Net puzzle_Net;
    private Animator animator;
    
    private readonly int FULLNESS = Animator.StringToHash("Fullness");
    private readonly int EXPLODE = Animator.StringToHash("Explode");
    private AirSM air;

    private float curChargeRate;

    private bool onFullCharge;
    private bool onCharging;

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
        //
        curChargeRate += Time.fixedDeltaTime;
        //puzzle_1.SetButtonAnimation(curChargeRate);
        SyncAnimation(curChargeRate);

        if (curChargeRate >= 1)
        {
            onFullCharge = true;
            return true;
        }

        return false;

    }

    #region NetWork
    public void SyncAnimation(float rate)
    {
        puzzle_Net.HandleSetRate(rate);
    }
    public void SetAnimation(float rate)
    {
        float val = Mathf.Clamp01(rate);
        animator.SetFloat(FULLNESS, val);
    }
    #endregion



    private void UnCharging()
    {
        curChargeRate -= Time.fixedDeltaTime;
        if (curChargeRate <= 0)
        {
            onCharging = false;
            curChargeRate = 0;
        }
        SyncAnimation(curChargeRate);
    }
    
    public bool onRecover;
    public void Wrong()
    {

        StartCoroutine(WrongAndRecover());
    }

    IEnumerator WrongAndRecover(){
        onRecover = true;
        curChargeRate = 0;
        onCharging = false;
        onFullCharge = false;
        animator.SetTrigger(EXPLODE);
        yield return new WaitForSeconds(1.5f);
        animator.SetFloat(FULLNESS,curChargeRate);
        onRecover = false;
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
