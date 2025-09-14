using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSupply : ButtonEntity,IInteractable
{
    [CustomHeader("Power Supply")]
    [SerializeField] Transform socketPosition;
  
    [Space(20)]
    [Header("Interacte")]
    public ObjectTypeEnum _objectType = ObjectTypeEnum.Mount;
    [SerializeField] float _BtnOffset;

    private Vector2 _topOfObj = new Vector2(0, 1.2f);
    private UI_Base _E_Btn;

    #region  Network
    private PowerSupply_Net P_Net => GetComponent<PowerSupply_Net>();

    #endregion

    #region  Get,Set


    protected override List<Vector2> GetTargetPositions()
    {
        List<Vector2> list = new();

        foreach (GameObject obj in targetObjects)
        {
            if (obj == null) continue;
            if (obj.TryGetComponent(out ActivatableObjectEntity _) || obj.TryGetComponent(out ButtonEntity _))
            {
                list.Add(ConvertPosition(obj.transform.position));
            }

        }

        return list;
    }

    public override void SetData<T>(T data)
    {
        base.SetData(data);

        if (Application.isPlaying)
        {
            P_Net.Server_SetInit();
            StartCoroutine(SyncTargetObjectNetIdCo()); 
        }
        
    }
    #region Sync TargetObejct

    private IEnumerator SyncTargetObjectNetIdCo() //Server
    {
        yield return new WaitForSeconds(1.5f);
        if (targetObjects.Count == 0) yield break;

        List<uint> uints = new();

        for (int i = 0; i < targetObjects.Count; i++)
        {
            var item = targetObjects[i].TryGetComponent(out BuildObj obj) ? obj : null;
            if (item == null) continue;
            uint id = obj.GetNetworkId();
            if (id == 9999) continue;

            uints.Add(id);
        }
        //Consum
        int consum = targetObjects.Count + lightObjects.Count;
        P_Net.consumption = consum;
        //Consum

        P_Net.Rpc_SetTargetObject(uints);

    }

    IEnumerator DelayFindTargetCo()
    {
        yield return new WaitForSeconds(1);

        List<GameObject> objList = new();

        foreach (Vector2 vec in targetPosition)
        {
            foreach (Transform tr in MapEditor.Instance.buttonActivatableObjectTransform)
            {
                if (tr.TryGetComponent(out ActivatableObjectEntity component))
                {
                    if (CompareVec(component.ButtonActivatedObjectStruct.position, vec))
                    {

                        objList.Add(tr.gameObject);
                        break;
                    }
                }
            }

            foreach (Transform tr in MapEditor.Instance.buttonObjectTransform)
            {
                if (tr.TryGetComponent(out ButtonEntity component))
                {
                    if (CompareVec(component.ButtonObjectData.position, vec))
                    {
                        objList.Add(tr.gameObject);
                        break;
                    }
                }
            }

        }

        targetObjects = objList;
    }
    public override void FindTargetObject()
    {
        if (!Application.isPlaying) return;
        StartCoroutine(DelayFindTargetCo());
       
    }
    #endregion

    public Vector2 GetSocketPosition()
    {
        return socketPosition.position;
    }

    #endregion

    #region Editor
    
    #endregion
    #region Active,Deactive
    protected override void Activation()
    {
        PrograssButtonActivatedObject(true);
        // LineOn(true);
    }
    protected override void Deactivated()
    {
        PrograssButtonActivatedObject(false);
        // LineOn(false);
    }
    public void Net_Activation()
    {
        Activation();
    }
    public void Net_Deactivated()
    {
        Deactivated();
    }

     protected override void PrograssButtonActivatedObject(bool onActivate)
    {
        TogglePowerSupply(onActivate);
    }
    
    private void TogglePowerSupply(bool toggle)
    {
        if(targetObjects.Count == 0) return; // This field is for server settings only
        
        foreach (var item in targetObjects)
        {
            if (item.TryGetComponent(out IPowerConsumer component))
            {
                if (toggle) component.PowerOn();
                else component.PowerOff();
            }
            

        }
        
        foreach (var item in lightObjects)
        {
            if (item.TryGetComponent(out LightObjectEntity component))
            {
                Debug.Log(item.name);
                // component.hasPower = toggle;
                if (toggle) component.PowerOn();
                else component.PowerOff();
            }
        }
    }
    #endregion

    #region Find Target

    public async override void Editor_Setting(MapEditor mapEditor)
    {
        //if(targetPosition.Count == 0) return;

            await util.Delay(()=>{
                List<GameObject> objList = new();
                foreach (Vector2 vec in targetPosition)
                {
                    GameObject matchedObj = null;
                    foreach (Transform tr in mapEditor.buttonActivatableObjectTransform)
                    {
                        if (tr.TryGetComponent(out ActivatableObjectEntity component))
                        {
                            if (CompareVec(component.ButtonActivatedObjectStruct.position, vec))
                            {
                                matchedObj = tr.gameObject;
                                objList.Add(matchedObj);
                                break;
                            }
                        }
                    }

                    if (matchedObj != null) continue;

                    foreach (Transform tr in mapEditor.buttonObjectTransform)
                    {
                        if (tr.TryGetComponent(out ButtonEntity component))
                        {
                            if (CompareVec(component.ButtonObjectData.position, vec))
                            {
                                objList.Add(tr.gameObject);
                                break;
                            }
                        }
                    }
                }

                targetObjects = objList;

                List<GameObject> list = new();
                OtherContainer otherContainer = mapEditor.otherContainer.GetComponent<OtherContainer>();

                foreach(Vector2 vec in ButtonObjectData.lightPositions)
                {
                    Debug.Log(vec);
                    otherContainer.GetCompareVec(vec,ref list);
                    //otherObject vec 전달 -> group transform 순회 같은거 있는지 확인 -> 있으면 해당 IPowerConsumer 반환
                }
                lightObjects = list;
                //Debug.Log($"Light Object Count : {lightObjects.Count}");

            });
    }



    #endregion


 #region  Main
    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     // if (collision.TryGetComponent(out HookSM hook))
    //     // {
    //     //     Transform grabItem = hook.GetGrabbedItem();
    //     //     if (grabItem == null)
    //     //     {
    //     //         if (P_Net.battery) P_Net.Cmd_ShowE(collision.gameObject, true);
    //     //     }
    
    //     // }
    //     // if(collision.TryGetComponent(out AirSM air))
    //     // {
    //     //     if(P_Net.battery) P_Net.Cmd_ShowE(collision.gameObject, true);
    //     // }


    //     if (collision.TryGetComponent(out Battery battery))
    //     {
    //         var interactable = battery.TryGetComponent(out InteractableObject component) ? component : null;
    //         if (interactable != null && interactable._isGrab)
    //         {
    //             // P_Net.Cmd_ShowE(collision.gameObject, true);
    //             battery.Net_SetPowerSupply(gameObject);
    //             return;
    //         }
    //         //Air Inhale object insert 0804
    //         if (ChackBatteryVelocity(battery) && NetworkServer.active)
    //         {
    //             //Insert Battery
    //             SetBattery(battery.gameObject);
    //             //Insert Battery
    //             return;
    //         }
    //         //Air Inhale object insert 0804
    //     }
    // }
    private float condition_InsertBatteryChargerValue = 5;
    private bool ChackBatteryVelocity(Battery battery)
    {
        Debug.Log(battery._rb.velocity.magnitude);
        return battery._rb.velocity.magnitude >= condition_InsertBatteryChargerValue;
    }

    // private void OnTriggerExit2D(Collider2D collider)
    // {
    //     if (collider.TryGetComponent(out Battery battery))
    //     {
    //         P_Net.Cmd_ShowE(collider.gameObject, false);
    //         battery.Net_SetPowerSupply(null);
    //     }
    // }


    #endregion
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
                    if (!P_Net._onSocket)
                    {
                        Interaction(item.transform);
                    }
                }
            }
        }
    }
    #region  Interacable
    public void Interaction(Transform accessor = null)
    {
        if (accessor != null && !accessor.TryGetComponent(out AirSM air))
        {
            if (accessor.TryGetComponent<BatteryInteractable>(out var newbattery))
            {
                P_Net.Cmd_SetBattery(newbattery.TryGetComponent<NetworkIdentity>(out var identity) ? identity.netId : 9999);
                HideEButton();
                return;
            }
        }
        else 
        {
            if (P_Net._onSocket)
            {
                P_Net.Cmd_SetBattery(9999);
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
            if (!P_Net._onSocket) return false;
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
        if (air == null || !P_Net._onSocket) return false;

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
