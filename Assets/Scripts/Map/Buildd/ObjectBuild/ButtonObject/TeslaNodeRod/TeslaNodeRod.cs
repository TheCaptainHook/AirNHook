using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaNodeRod : ButtonEntity,IPowerConsumer
{
    [CustomHeader("Tesla Node Rod")]
    public bool isPowerSupplied;
    private TeslaNodeRod_Net net;

    private void Awake()
    {
        net = GetComponent<TeslaNodeRod_Net>();
    }

  

    #region Get,Set
    public override async void SetData<T>(T data)
    {
        try
        {
            if (typeof(T) == typeof(ButtonObjectStruct))
            {
                ButtonObjectStruct buttonData = (ButtonObjectStruct)(object)data;
                ButtonObjectData = buttonData;

                await util.Delay(() =>
                {
                    FindTargetObject();
                    if (buttonData.lightPositions.Count > 0) FindLightObject();
                    if (buttonData.encapsulationItems.Count > 0) FindEncapsulationItem();

                    if (Application.isPlaying)
                    {
                        net.Server_InitSync();
                    }
                });
               
               

            }
        }
        catch (Exception ex)
        {
            Debug.Log($"name : {gameObject.name},{ex}");
        }
    }
    public override void FindTargetObject()
    {
        List<GameObject> objList = new();
        foreach (Vector2 vec in targetPosition)
        {
            GameObject matchedObj = null;
            foreach (Transform tr in MapEditor.Instance.buttonActivatableObjectTransform)
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

    protected override List<Vector2> GetTargetPositions()
    {
        List<Vector2> list = new();

        foreach (GameObject obj in targetObjects)
        {
            if (obj == null) continue;
            Debug.Log(obj.name);
            if (obj.TryGetComponent(out ActivatableObjectEntity _) || obj.TryGetComponent(out ButtonEntity _))
            {
                list.Add(ConvertPosition(obj.transform.position));
            }

        }

        return list;
    }
    #endregion

    #region IPowerConsumer
    public bool hasPower
    {
        get { return net.hasPower > 0 ? true : false; }
        //set { net.Cmd_SetHasPower(value); }
        set { net.Server_SetHasPower(value); }
    }
    public int GetConsumption()
    {
        return 1;
    }
    public void PowerOn() //only server
    {
        if(NetworkServer.active)
            hasPower = true;


    }
    public void PowerOff()
    {
        if (NetworkServer.active)
            hasPower = false;

    }
    public Vector2 GetPowerLineConnectionPoint()
    {
        return transform.position;
    }
    public Vector2 GetTransformPosition()
    {
        return transform.position;
    }
    #endregion

    #region Main

  

    public void Net_Active()
    {
        if(net.onSync)
        Activation();
    }
    public void Net_DeActive()
    {
        if (net.onSync)
            Deactivated();
    }


    public Transform head;


    public override void Activation()
    {
        //Effect Rpc
            //TEST
        // head.color = Color.blue;
            //TEST
        net.LineActive();
        //Effect Rpc

        //Main Logic -Server
        PrograssButtonActivatedObject(true);
        //Main Logic -Server
        
    }
    public override void Deactivated()
    {
        //Effect Rpc
            //TEST
        // head.color = Color.red;
            //TEST
        net.LineDeActive();
        //Effect Rpc

        //Main Logic -Server
        PrograssButtonActivatedObject(false);
        //Main Logic -Server
       
    }
    protected override void PrograssButtonActivatedObject(bool onActivate)
    {
        if (targetObjects == null) return;
        foreach (GameObject obj in targetObjects)
        {
            if (obj.TryGetComponent(out IPowerConsumer consumer))
            {
                if(onActivate)consumer.PowerOn();
                else consumer.PowerOff();
            }

            if (obj.TryGetComponent(out ActivatableObjectEntity component))
            {
                component.ApplyActive(onActivate ? 1 : -1);
            }
          
        }

        foreach (GameObject obj in lightObjects)
        {
            if (obj.TryGetComponent(out IPowerConsumer component))
            {
                if (onActivate) component.hasPower = true;
                else component.hasPower = false;
            }

        }
    }
    private bool isActive;
    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        if(damageType == DamageType.Electric)
        {
            if(NetworkServer.active)
            {
                if(!isActive)
                {
                    isActive = true;
                    //hasPower = true;
                    PowerOn();
                }
                curResetRate = 0;
            }
        }
    }
    public float maxResetRate = 2;
    public float curResetRate = 0;
    private void Update()
    {
        if(isActive)
        {
            curResetRate += Time.deltaTime;
            if(curResetRate > maxResetRate)
            {
                isActive = false;
                curResetRate = 0;
                PowerOff();
            }
        }
    }

#endregion

#if UNITY_EDITOR
    #region Editor
    public async override void Editor_Setting(MapEditor mapEditor)
    {
        //if(targetPosition.Count == 0) return;

        await util.Delay(() => {
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

            foreach (Vector2 vec in ButtonObjectData.lightPositions)
            {
                otherContainer.GetCompareVec(vec, ref list);
            }
            lightObjects = list;

        });
    }

    void OnDrawGizmos()
    {
        Gizmos.color  = Color.red;
        Gizmos.DrawWireSphere(transform.position,5);
    }
    #endregion
#endif

}
