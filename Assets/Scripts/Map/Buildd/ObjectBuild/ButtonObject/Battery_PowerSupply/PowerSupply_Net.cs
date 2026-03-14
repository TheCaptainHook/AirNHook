using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Unity.VisualScripting;
using UnityEngine.Animations;

public class PowerSupply_Net : ButtonEntity_Net, IInteractable
{
    // private PowerSupply powerSupply;
    // private PowerSupply PowerSupply 
    // {
    //     get
    //     {
    //         if(powerSupply == null) powerSupply = GetComponent<PowerSupply>();
    //         return powerSupply;
    //     }
    // }

    #region Init
    // public bool onSync;

    // [Server]
    // public void Server_SetInit()
    // {
    //     StartCoroutine(AllClientCheckCo(() =>
    //     {
    //         Rpc_SetInit(PowerSupply.ButtonObjectData);

    //     }));

    // // }
    // private IEnumerator AllClientCheckCo(Action action)
    // {
    //     int connectClients = NetworkServer.connections.Count;
    //     bool onReady = false;
    //     while (!onReady)
    //     {
    //         int num = 0;
    //         foreach (var conn in NetworkServer.connections.Values)
    //         {
    //             if (conn.isReady) num++;
    //         }

    //         if (connectClients == num) onReady = true;
    //         yield return null;
    //     }

    //     action?.Invoke();

    // }
    // // private ButtonObjectStruct data;
    // private List<uint> targetNetIdList;
    // [Server]
    // public void Server_SetTargetNetId(List<uint> list)
    // {
    //     Rpc_SetTargetNetId(list);
    // }
    // [ClientRpc]
    // private void Rpc_SetTargetNetId(List<uint> list)
    // {
    //     targetNetIdList = list;
    //     //PathFind,
    // }

    // [ClientRpc]
    // private void Rpc_SetInit(ButtonObjectStruct data)
    // {
    //     if (onSync) return;
    //     this.data = data;
    //     transform.position = data.position;
    //     transform.rotation = data.quaternion;
    //     onSync = true;
    // }


    // [Command(requiresAuthority = false)]
    // private void Cmd_SetInit()
    // {
    //     Server_SetInit();
    // }

    // public override void OnStartClient()
    // {
    //     base.OnStartClient();
    //     StartCoroutine(Delay(() => { Cmd_SetInit(); }));
    // }
    [SyncVar] public float _consumption;
    protected override void Server_Sync_OtherValue()
    {
        // Rpc_SetTargetObject
        StartCoroutine(Delay_Sync_OtehrValueCo());

    }

    private IEnumerator Delay_Sync_OtehrValueCo()
    {
        yield return new WaitUntil(() => Main._complete_FindAllObj);

        List<uint> uints = new();
        for (int i = 0; i < Main.targetObjects.Count; i++)
        {
            var item = Main.targetObjects[i].TryGetComponent(out BuildObj obj) ? obj : null;
            if (item == null) continue;
            uint id = obj.GetNetworkId();
            if (id == 9999) continue;
            // Debug.Log($"{item.name}, {i}");
            uints.Add(id);
        }
        _consumption = Main.targetObjects.Count + Main.lightObjects.Count;

        Rpc_SetTargetObject(uints);

    }

    #endregion

    #region Insert Battery

    // [SyncVar] public float consumption;
    private GameObject battery;
    public bool _onSocket = false;

    Coroutine supplyCoroutine;

    [Command(requiresAuthority = false)]
    public void Cmd_SetBattery(uint battery_id)
    {
        var item = NetworkServer.spawned.TryGetValue(battery_id, out NetworkIdentity identity) ? identity.gameObject : null;
        Server_SetBatter(item);
    }

    [Server]    // insert battery and Use.
    public void Server_SetBatter(GameObject newBattery)
    {
        if (!Compare(battery, newBattery))
        {
            Server_Connect(newBattery);
        }
    }


    private bool Check_BatteryCapacity(GameObject battery)
    {
        float capacity = battery.GetComponent<BatteryInteractable>().batteryCapacity;
        return capacity > _consumption;
    }

    //----------------------------------------Refectoring 0714

    #region  Connect
    [Server]
    private void Server_Connect(GameObject newBattery)
    {
        if (battery != null)
        {
            Server_DisConnect();
        }

        if (newBattery == null)
        {
            Debug.Log("Null Battery");
        }
        else
        {
            Rpc_Connect(newBattery);
            if (Check_BatteryCapacity(newBattery))
            {
                //Use Battery
                Rpc_OnSupplyEffect(true);
                Supply(newBattery);
                // PowerSupply.Net_Activation();
                Main.Activation();

            }
            else
            {
                //Cant use battery.
                Debug.Log("The battery doesn’t have much energy left");
            }

        }
    }
    [ClientRpc]
    private void Rpc_Connect(GameObject item)
    {
        if (item == null) return;

        var col = item.TryGetComponent(out Collider2D collider) ? collider : null;
        if (col != null) col.enabled = false;
        var rb = item.TryGetComponent(out Rigidbody2D rigidbody) ? rigidbody : null;
        if (rb != null)
        {
            rb.simulated = false;
            rb.velocity = Vector3.zero;
        }

        item.transform.rotation = Quaternion.identity;

        if (item.TryGetComponent(out ParentConstraint parentConstraint))
        {
            if (parentConstraint.sourceCount > 0)
            {
                parentConstraint.RemoveSource(0);
            }
            SetParentConstraint(parentConstraint, transform);
        }
        _onSocket = true;
        battery = item;
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

    #endregion
    #region  Disconnect
    [Server]
    private void Server_DisConnect()
    {
        if (supplyCoroutine != null)
        {
            StopCoroutine(supplyCoroutine);
            supplyCoroutine = null;
        }
        // PowerSupply.Net_Deactivated();
        Main.Deactivated();

        Rpc_OnSupplyEffect(false);
        Rpc_DisConnect();
    }

    [ClientRpc]
    private void Rpc_DisConnect()
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

        if (battery.TryGetComponent(out BatteryInteractable net))
        {
            net.Recover();
            net.RemoveSocketEffect();

            _onSocket = false;
            battery = null;
        }

    }
    
    #endregion

    #region  Clean
    public void ConnectClean()
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

        if (battery.TryGetComponent(out BatteryInteractable net))
        {
            net.Recover();
            // net.RemoveSocketEffect();

            _onSocket = false;
            battery = null;
        }
    }

    public override void Clean()
    {
        if (isServer) _consumption = 0;
        ConnectClean();
             
        for (int i = 0; i < pdu_List.Count; i++)
        {
            var item = pdu_List[i];
            item.Clean();
            Destroy(item.gameObject);
        }
        if (lineContainer != null)
        {
            Destroy(lineContainer.gameObject);
            lineContainer = null;
        }
        _onSync = false;
    }
    #endregion

    #region Draw,Eraser
    private List<PowerSupply_DrawLineUtility> pdu_List;
    [ReadOnly]
    // public List<uint> targetObjectNetIdList;
    private Transform lineContainer;
    [SerializeField] Material lineMat;

    [ClientRpc]
    public void Rpc_SetTargetObject(List<uint> list)
    {
        // targetObjectNetIdList = list;
        lineContainer = new GameObject("Line_Container").transform;
        lineContainer.SetParent(transform);
        pdu_List = new();

        for (int i = 0; i < list.Count; i++)
        {
            var item = new GameObject("Line").transform;
            item.SetParent(lineContainer);
            var line = item.AddComponent<PowerSupply_DrawLineUtility>();
            item.AddComponent<PathFinder>();

            pdu_List.Add(line);
            line.Setting(Main.ButtonObjectData.position, list[i], lineMat);
        }
    }

    private void DrawLine()
    {
        for (int i = 0; i < pdu_List.Count; i++)
        {
            var item = pdu_List[i];
            item.DrawOn();
        }
    }
    private void EraseLine()
    {
        for (int i = 0; i < pdu_List.Count; i++)
        {
            pdu_List[i].EraseOn_TargetToMain();
        }
    }

    #endregion

    private void Supply(GameObject item)//only server
    {
        if (supplyCoroutine != null)
        {
            StopCoroutine(supplyCoroutine);
        }
        supplyCoroutine = StartCoroutine(SupplyCo(item));
    }


    IEnumerator SupplyCo(GameObject item) //only server
    {
        BatteryInteractable battery = item.GetComponent<BatteryInteractable>();

        while (battery.batteryCapacity > 0)
        {
            battery.Server_SetBatteryCapacity(-_consumption);

            yield return new WaitForSeconds(1);
        }

        supplyCoroutine = null;
        // PowerSupply.Net_Deactivated();
        Main.Deactivated();

        Rpc_OnSupplyEffect(false);

    }


    //----------------------------------------Refectoring 0714
    #endregion

    #region  Effect
    [ClientRpc]
    private void Rpc_OnSupplyEffect(bool onOff)
    {
        // PowerSupply.LineOn(onOff);
        if (onOff) DrawLine();
        else EraseLine();

    }

    #endregion


    private bool Compare(GameObject a, GameObject b)
    {
        if (a == null || b == null) return false;

        NetworkIdentity aN = a.GetComponent<NetworkIdentity>();
        NetworkIdentity bN = b.GetComponent<NetworkIdentity>();

        return aN.netId == bN.netId;
    }


    #region Trigger Enter
     private float condition_InsertVelocityValue = 15;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if (!NetworkServer.active) return;
        if (collision != null)
        {
            if (collision.TryGetComponent(out BatteryInteractable item))
            {
                var velocity = item.Rb.velocity.magnitude;
                if (velocity >= condition_InsertVelocityValue)
                {
                    if (!_onSocket)
                    {
                        Interaction(item.transform);
                    }
                }
            }
        }
    }

    #endregion

    #region Interactoable
    private UI_Base _E_Btn;
    public ObjectTypeEnum _objectType = ObjectTypeEnum.Mount;
    private Vector2 _topOfObj = new Vector2(0, 1.2f);
     public void Interaction(Transform accessor = null)
    {
        if (accessor != null && !accessor.TryGetComponent(out AirSM air))
        {
            if (accessor.TryGetComponent<BatteryInteractable>(out var newbattery))
            {
                Cmd_SetBattery(newbattery.TryGetComponent<NetworkIdentity>(out var identity) ? identity.netId : 9999);
                HideEButton();
                return;
            }
        }
        else 
        {
            if (_onSocket)
            {
                Cmd_SetBattery(9999);
                return;
            }
            
        }
      
    }

    public bool CanInteract()
    {
        if (Hook_IsInteractionValid()) return true;
        if (Air_IsInteractionValid()) return true;

        return false;
        // return true;
    }
    private bool Hook_IsInteractionValid() //
    {
        var hook = Managers.Game.Player.TryGetComponent(out HookSM component) ? component : null;
        if (hook == null) return false;

        if (hook.GetGrabbedItem() == null)
        {
            Debug.Log("Hook grabbed item is null");
            if (!_onSocket) return false;
            else return true;
        }
        else
        {
            if (hook.GetGrabbedItem().TryGetComponent<BatteryInteractable>(out var batteryInteractable))
            {
                return true;
            }
            else
            {
                Debug.Log($"Hook grabbed item is not BatteryInteractable, {hook.GetGrabbedItem().name}");
                return false;
            }
        }
    }
    private bool Air_IsInteractionValid() //
    {
        var air = Managers.Game.Player.TryGetComponent(out AirSM component) ? component : null;
        if (air == null || !_onSocket) return false;

        Debug.Log("AirSM is valid for interaction");
        return true;

    }
    public bool Interacting(bool value, GameObject player)
    {
        return true;
    }

    public ObjectTypeEnum GetObjectType()
    {
        return _objectType;
    }

    public void ShowEButton()
    {
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position = transform.position + (Vector3)_topOfObj;
    }

    public void HideEButton()
    {
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }

    #endregion

}
