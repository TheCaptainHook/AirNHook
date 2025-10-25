using Mirror;
using System.Collections;
using UnityEngine;

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

    public override void SetData<T>(T data)
    {
        base.SetData(data);
        if (Application.isPlaying)
        {
            Net.onSync = true;
            Net.Server_InitSync();
        }
    }

    public void Net_Act()
    {
        Activation();
    }
    public void Net_Deact()
    {
        Deactivated();
    }
    public override void Activation()
    {
        if(!Application.isPlaying){
            return;
        }

            PrograssButtonActivatedObject(true);
        
    }

    public override void Deactivated()
    {
        if(!Application.isPlaying){
            return;
        }


            PrograssButtonActivatedObject(false);
            
    }
    #region Network
    private LaserTriggerButton_Net net;
    private LaserTriggerButton_Net Net { get { if (net == null) net = GetComponent<LaserTriggerButton_Net>(); return net; } }

    #endregion

    public void Charging()
    {
        if(!Application.isPlaying || !NetworkServer.active) return;

        Net.Server_SetChargingCount();
        //Net.Cmd_SetChargingCount();


    }


}
