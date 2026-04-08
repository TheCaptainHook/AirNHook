using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.Animations;
using FunkyCode;

public class BatteryCharger_Net : NetworkBehaviour
{
    BatteryCharger main;
    BatteryCharger Main { get { main ??= GetComponent<BatteryCharger>();  return main; } }

    [ReadOnly]
    public GameObject battery;


    //====Light
    [SerializeField] LightSprite2D _mainSimborLight;
    [SerializeField] LightSprite2D _subSimborLight;
    [SerializeField] SpriteRenderer _subSimborSprite;

    //====Light


    #region Init
    public bool onSync;


    [Server]
    public void Server_SetInit()
    {
        Rpc_SetInit(Main.ObjectData);
    }
    [ClientRpc]
    private void Rpc_SetInit(ObjectData data)
    {
        if (onSync) return;
        transform.position = data.position;
        transform.rotation = data.quaternion;

        onSync = true;
    }
    [Command(requiresAuthority = false)]
    private void Cmd_SetInit()
    {
        Server_SetInit();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!onSync) Cmd_SetInit();
    }
    #endregion

    Coroutine charge;
    [Command(requiresAuthority = false)]
    public void Cmd_SetBattery(uint netId)
    {
        GameObject item = NetworkServer.spawned.TryGetValue(netId, out NetworkIdentity identity) ? identity.gameObject : null;
        Server_SetBattery(item);
    }
    [Server]
    public void Server_SetBattery(GameObject newBattery)
    {
        if (battery != null) return;

        if (newBattery != null && !Compare(battery, newBattery))
        {
            Rpc_Connect(newBattery); //Connect
            Charge(newBattery); //Only Server 251127 (1) Charging
        }

    }
    private bool Compare(GameObject a, GameObject b)
    {
        if (a == null || b == null) return false;

        NetworkIdentity aN = a.GetComponent<NetworkIdentity>();
        NetworkIdentity bN = b.GetComponent<NetworkIdentity>();

        return aN.netId == bN.netId;
    }

    public void Charge(GameObject item) //Server
    {
        charge = StartCoroutine(ChargeCo(item));
    }

    IEnumerator ChargeCo(GameObject item)
    {
        BatteryInteractable battery = item.GetComponent<BatteryInteractable>();
        
        //Charge Light
        _mainSimborLight.enabled = true;
        _subSimborLight.enabled = true;
        _subSimborSprite.enabled = true;
        //Charge Light
        
        while (battery.batteryCapacity < 100)
        {
            //battery.BatteryCapacity = 1;
            battery.Server_SetBatteryCapacity(3);
            yield return new WaitForSeconds(0.1f);
        }
        charge = null;
        // battery.Cmd_Recover();
        Rpc_Disconnect();
    }

    [ClientRpc]
    private void Rpc_Connect(GameObject newBattery) //Rpc
    {
        var col = newBattery.TryGetComponent(out Collider2D collider) ? collider : null;
        if (col != null) col.enabled = false;
        var rb = newBattery.TryGetComponent(out Rigidbody2D rigidbody) ? rigidbody : null;
        if (rb != null)
        {
            rb.simulated = false;
            rb.velocity = Vector3.zero;
        }
       newBattery.transform.rotation = Quaternion.identity;

        if (newBattery.TryGetComponent(out ParentConstraint parentConstraint))
        {
            if (parentConstraint.sourceCount > 0)
            {
                parentConstraint.RemoveSource(0);
            }
            SetParentConstraint(parentConstraint, transform);
        }

        battery = newBattery;

    }

    [ClientRpc]
    private void Rpc_Disconnect() //Rpc
    {
        //Charge Light off
         _mainSimborLight.enabled = false;
        _subSimborLight.enabled = false;
        _subSimborSprite.enabled = false;
        //Charge Light off
        
        if (battery == null) return;

        if (battery.TryGetComponent(out ParentConstraint component))
        {
            if (component.sourceCount > 0)
            {
                component.RemoveSource(0);
            }

        }

        var col = battery.TryGetComponent(out Collider2D collider) ? collider : null;
        if (col != null) col.enabled = true;
        var rb = battery.TryGetComponent(out Rigidbody2D rigidbody) ? rigidbody : null;
        if (rb != null)
        {
            rb.simulated = true;
            rb.velocity = Vector3.zero;
        }
        if (battery.TryGetComponent(out BatteryInteractable net))
        {
            net.Recover();
            net.RemoveSocketEffect();
        }

        battery = null;
    }    


     private void SetParentConstraint(ParentConstraint constraint, Transform parent)
    {
        ConstraintSource source = new ConstraintSource
        {
            sourceTransform = parent,
            weight = 1
        };
        constraint.AddSource(source);

        constraint.translationAtRest = transform.localPosition;
        constraint.translationOffsets = new Vector3[constraint.sourceCount];
        constraint.constraintActive = true;

        constraint.locked = true;
        
    }



    #region  Clean
    public void Clean()
    {
        if(isServer)
        {
            if(charge != null)
            {
                StopCoroutine(charge);
                charge = null;
            }

        }

        //써버 클린
        ConnectClean();
        //클라 클린
    

    }
    private void ConnectClean()
    {
         if (battery == null) return;

        if (battery.TryGetComponent(out ParentConstraint component))
        {
            if (component.sourceCount > 0)
            {
                component.RemoveSource(0);
            }
        }

        var col = battery.TryGetComponent(out Collider2D collider) ? collider : null;
        if (col != null) col.enabled = true;
        var rb = battery.TryGetComponent(out Rigidbody2D rigidbody) ? rigidbody : null;
        if (rb != null)
        {
            rb.simulated = true;
            rb.velocity = Vector3.zero;
        }

        // if (battery.TryGetComponent(out BatteryInteractable net))
        // {
        //     net.Recover();
        // }

        battery = null;
    }

    #endregion
}


