using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LaserTriggerButton : ButtonEntity
{
    [ReadOnly]
    [CustomHeader("Laser Trigger Button")]
    public int chargingCount;
    public int maxChargingCount = 200; //200
    //private Coroutine chargingCoroutine;
    //private float defChargingRate = 3f;
    //[ReadOnly]
    //[SerializeField]private float curChargingRate;

    //todo Shader 0914
    //[SerializeField] SpriteRenderer testSpriteRenderer;
    //private Color color = new Color(0,0,0);
    //todo Shader 0914


    //private bool onActivate;

    //[Header("Animation")]
    //[SerializeField] private Animator _animator;
    //[Header("Effect")]
    //[SerializeField] ParticleSystem particle;
    //[SerializeField] GameObject chargingSprite;

    #region StringCache
    private static readonly int IsActive = Animator.StringToHash("IsActive");
    #endregion
    
    #region State
    //private bool onCharging;
    #endregion

    //private void Update(){
    //    if(!onCharging && chargingCount != 0){
    //        chargingCount--;
    //        ChargingEffectIntensity();
    //    }

    //    if(chargingCount >= maxChargingCount){
    //        Activation();
    //    }else{
    //        Deactivated();
    //    }
    //}


    public void Net_Act()
    {
        Activation();
    }
    public void Net_Deact()
    {
        Deactivated();
    }
    protected override void Activation()
    {
        if(!Application.isPlaying){
            Debug.Log("Activate");
            return;
        }


            //onActivate = true;
            //particle.Play();
            //_animator.SetBool(IsActive,onActivate);
            PrograssButtonActivatedObject(true);
            Debug.Log("Activation");
        
    }

    protected override void Deactivated()
    {
        if(!Application.isPlaying){
            Debug.Log("Deactivate");
            return;
        }

   
            //onActivate = false;
            //particle.Stop();
            //_animator.SetBool(IsActive,onActivate);
            PrograssButtonActivatedObject(false);
            Debug.Log("Deactivation");
        
        
    }
    #region Network
    private LaserTriggerButton_Net net;
    private LaserTriggerButton_Net Net { get { if (net == null) net = GetComponent<LaserTriggerButton_Net>(); return net; } }

    #endregion

    public void Charging()
    {
        if(!Application.isPlaying) return;

        Net.Server_SetChargingCount();
        //curChargingRate = defChargingRate;

        //if(chargingCoroutine == null){
        //    chargingCoroutine = StartCoroutine(ChargingTimerCoroutine());
        //}

        //if(chargingCount < maxChargingCount){
        //    chargingCount++;

        //    //todo Shader 0914
        //    ChargingEffectIntensity();
        //    //todo Shader 0914
        //}
        //Debug.Log("Charging");

    }

    //private void ChargingEffectIntensity(){
    //    float percent = chargingCount / 200f;

    //    if(percent < 0.01f){
    //        percent = 0;
    //    }

    //    chargingSprite.transform.localScale = new Vector3(percent,percent);
    //}


    //IEnumerator ChargingTimerCoroutine(){
    //    onCharging = true;

    //    while(curChargingRate > 0){
    //        curChargingRate -= Time.deltaTime;
    //        yield return null;

    //    }

    //    curChargingRate = 0;
    //    onCharging = false;
    //    chargingCoroutine = null;
    //}

}
