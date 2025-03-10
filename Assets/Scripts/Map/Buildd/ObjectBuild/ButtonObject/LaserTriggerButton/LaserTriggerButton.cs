using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LaserTriggerButton : ButtonEntity
{
    [ReadOnly]
    [CustomHeader("Laser Trigger Button")]
    public int chargingCount;
    public int maxChargingCount = 200; //200


    #region StringCache
    private static readonly int IsActive = Animator.StringToHash("IsActive");
    #endregion
    
    #region State
    //private bool onCharging;
    #endregion



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

            PrograssButtonActivatedObject(true);
            Debug.Log("Activation");
        
    }

    protected override void Deactivated()
    {
        if(!Application.isPlaying){
            Debug.Log("Deactivate");
            return;
        }


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

        //Net.Server_SetChargingCount();
        Net.Cmd_SetChargingCount();


    }


}
