using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class LaserTriggerButton_Net : ButtonEntity_Net
{
    //private int maxChargingCount = 200; //200
    private float defChargingRate = 3f;
    private int maxChargingCount = 100; //100

    // private LaserTriggerButton trigger;
    // private LaserTriggerButton Trigger { get { if (trigger == null) trigger = GetComponent<LaserTriggerButton>();return trigger; } }

    [SerializeField] GameObject chargingSprite;
    [SerializeField] ParticleSystem particle;
    [SerializeField] private Animator _animator;

    [Space(20)]
    [SyncVar] public float chargingCount;
    [SyncVar] public float curChargingRate;
    [SyncVar] public bool onCharging;
    [SyncVar] public bool onActivate;

    private static readonly int IsActive = Animator.StringToHash("IsActive");

    #region  Clean
    public override void Server_Clean()
    {
    
        chargingCount = 0;
        curChargingRate = 0;
        onCharging = false;
        onActivate = false;

        base.Server_Clean();
    }
    protected override void Rpc_Clean()
    {
        base.Rpc_Clean();
        chargingSprite.transform.localScale = Vector2.zero;

        StopAllCoroutines();
        chargingCoroutine = null;
        
    }
    #endregion


    [Server]
    public void Server_SetChargingCount()
    {
        curChargingRate = defChargingRate;

        if (chargingCoroutine == null)
        {
            chargingCoroutine = StartCoroutine(ChargingTimerCoroutine());
        }

        if (chargingCount < maxChargingCount)
        {
            chargingCount++;

            Rpc_ChargingEffectIntensity();

        }
    }
    //[Command(requiresAuthority =false)]
    //public void Cmd_SetChargingCount()
    //{
    //    Server_SetChargingCount();      
    //}

    [ServerCallback]
    private void Update()
    {
        if (!isServer) return;

        if (!onCharging && chargingCount != 0)
        {
            chargingCount--;
            Rpc_ChargingEffectIntensity();
        }

        if (chargingCount >= maxChargingCount)
        {
            // Net_Act();
            if (!_isActive)
            {
                _isActive = true;
                Rpc_Effect(true);
                Main.Activation();
            }

        }
        else
        {
            if (_isActive)
            {
                _isActive = false;
                Rpc_Effect(false);
                Main.Deactivated();
            }
        }
    }


    // private void Net_Act()
    // {
    //     if (!onActivate)
    //     {
    //         onActivate = true;
    //         Rpc_Effect(true);
    //         Trigger.Net_Act();
    //     }
     
    // }
    // private void Net_Deact()
    // {
    //     if (onActivate)
    //     {
    //         onActivate = false;
    //         Rpc_Effect(false);
    //         Trigger.Net_Deact();
    //     }
       
    // }

    [ClientRpc]
    private void Rpc_Effect(bool onOff)
    {
        if(onOff)
        {
            particle.Play();
            _animator.SetBool(IsActive, onActivate);
            //파티클
            //애니메이션
        }
        else
        {
            particle.Stop();
            _animator.SetBool(IsActive, onActivate);
        }
        
    }


    private Coroutine chargingCoroutine;
    IEnumerator ChargingTimerCoroutine()
    {
        onCharging = true;

        while (curChargingRate > 0)
        {
            curChargingRate -= Time.deltaTime;
            yield return null;

        }

        curChargingRate = 0;
        onCharging = false;
        chargingCoroutine = null;
    }

    [ClientRpc]
    private void Rpc_ChargingEffectIntensity()
    {
        float percent = chargingCount / maxChargingCount;

        if (percent < 0.01f)
        {
            percent = 0;
        }

        chargingSprite.transform.localScale = new Vector3(percent, percent);
    }


    
}
