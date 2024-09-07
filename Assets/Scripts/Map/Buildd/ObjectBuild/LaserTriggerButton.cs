using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Engines;
using UnityEngine;
using UnityEngine.UI;

public class LaserTriggerButton : ButtonEntity
{
    [ReadOnly]
    [CustomHeader("Laser Trigger Button")]
    public int chargingCount;
    private Coroutine chargingCoroutine;
    private float defChargingRate = 3f;
    [ReadOnly]
    [SerializeField]private float curChargingRate;


    [SerializeField] SpriteRenderer testSpriteRenderer;
    private Color color = new Color(0,0,0);

    private bool onActivate;

    #region State
    private bool onCharging;
    #endregion

    private void Update(){
        if(!onCharging && chargingCount != 0){
            chargingCount--;
            testSpriteRenderer.color = new Color(chargingCount/255f,0,0);
        }

        if(chargingCount >= 200){
            Activation();
        }else{
            Deactivated();
        }
    }


    protected override void Activation()
    {
        if(!Application.isPlaying){
            Debug.Log("Activate");
            return;
        }

        if(!onActivate){
            onActivate = true;
            PrograssButtonActivatedObject(true);
            Debug.Log("Activation");
        }
        
    }

    protected override void Deactivated()
    {
        if(!Application.isPlaying){
            Debug.Log("Deactivate");
            return;
        }

        if(onActivate){
            onActivate = false;
            PrograssButtonActivatedObject(false);
            Debug.Log("Deactivation");
        }
        
    }


    public void Charging()
    {
        Debug.Log("Charging");
        curChargingRate = defChargingRate;

        if(chargingCoroutine == null){
            chargingCoroutine = StartCoroutine(ChargingTimerCoroutine());
        }

        if(chargingCount <=200){
            chargingCount++;
            testSpriteRenderer.color = new Color(chargingCount/255f,0,0);

        }

        Debug.Log("Charging");
    }

    IEnumerator ChargingTimerCoroutine(){
        onCharging = true;

        while(curChargingRate > 0){
            curChargingRate -= Time.deltaTime;
            yield return null;

        }
        curChargingRate = 0;
        onCharging = false;
        chargingCoroutine = null;
    }

}
