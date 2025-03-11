using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Lumin;

public class LaserTriggerButton_Net : NetworkBehaviour
{
    //private int maxChargingCount = 200; //200
    private float defChargingRate = 3f;
    private int maxChargingCount = 100; //100

    private LaserTriggerButton trigger;
    private LaserTriggerButton Trigger { get { if (trigger == null) trigger = GetComponent<LaserTriggerButton>();return trigger; } }

    [SerializeField] GameObject chargingSprite;
    [SerializeField] ParticleSystem particle;
    [SerializeField] private Animator _animator;

    [Space(20)]
    [SyncVar] public float chargingCount;
    [SyncVar] public float curChargingRate;
    [SyncVar] public bool onCharging;
    [SyncVar] public bool onActivate;

    private static readonly int IsActive = Animator.StringToHash("IsActive");

    #region Init
    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(Trigger.ButtonObjectData);
    }
    [ClientRpc]
    public void Rpc_InitSync(ButtonObjectStruct data)
    {
        if (onSync) return;

        transform.position = data.position;
        transform.rotation = data.quaternion;

        onSync = true;
    }
    [Command]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!onSync) Cmd_InitSync();
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

            ChargingEffectIntensity();

        }
    }
    //[Command(requiresAuthority =false)]
    //public void Cmd_SetChargingCount()
    //{
    //    Server_SetChargingCount();      
    //}

    private void Update()
    {
        if (!isServer) return;
        
        if (!onCharging && chargingCount != 0)
        {
            chargingCount--;
            ChargingEffectIntensity();
        }

        if (chargingCount >= maxChargingCount)
        {
            Net_Act();
        }
        else
        {
            Net_Deact();
        }
    }


    private void Net_Act()
    {
        if (!onActivate)
        {
            onActivate = true;
            Rpc_Effect(true);
            Trigger.Net_Act();
        }
     
    }
    private void Net_Deact()
    {
        if (onActivate)
        {
            onActivate = false;
            Rpc_Effect(false);
            Trigger.Net_Deact();
        }
       
    }

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
    private void ChargingEffectIntensity()
    {
        float percent = chargingCount / maxChargingCount;

        if (percent < 0.01f)
        {
            percent = 0;
        }

        chargingSprite.transform.localScale = new Vector3(percent, percent);
    }


    
}
