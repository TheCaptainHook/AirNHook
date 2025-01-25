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

    [ReadOnly]
    public float curChargeRate;

    public bool onFullCharge;
    private bool onCharging;

    private float curRecoverRate =1;

    public bool onProgress;

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
        //curChargeRate += Time.fixedDeltaTime;
        //puzzle_1.SetButtonAnimation(curChargeRate);
        SyncAnimation(0.01f);

        if (puzzle_Net.chargingRate >= 1)
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
        curChargeRate += rate;
        float val = Mathf.Clamp01(rate);
        animator.SetFloat(FULLNESS, val);
    }
    #endregion



    private void UnCharging()
    {

        SyncAnimation(-0.01f);
        if (puzzle_Net.chargingRate <= 0)
        {
            onCharging = false;
            curChargeRate = 0;
        }
      
    }
    
    public bool onRecover;
    public void Wrong()
    {
        StartCoroutine(WrongAndRecover());
    }
    

    IEnumerator WrongAndRecover(){
        onProgress = true;
        onRecover = true;
        onCharging = false;
        curChargeRate = 0;
        animator.SetTrigger(EXPLODE);

        yield return new WaitForSeconds(2f);
        puzzle_Net.CmdReset();
        //animator.SetFloat(FULLNESS,curChargeRate);
        
        onFullCharge = false;
        onRecover = false;
        onProgress = false;
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
