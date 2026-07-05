using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEntity : BuildObj,IPowerConsumer
{

    [CustomHeader("ButtonEntity, Target Object")]
    [Header(@"
    -------------ButtonEntity Field
     * ↓ can added this field.
        - ActivatableObjectEntity was inherited 
    ")]
    public List<GameObject> targetObjects;

    private ButtonObjectStruct buttonObjectData;
    public ButtonObjectStruct ButtonObjectData
    {
        get { return buttonObjectData; }
        set
        {
            buttonObjectData = value;
            ObjectData = new ObjectData(value.id, value.position, value.scale);
            transform.position = value.position;
            transform.rotation = value.quaternion;
            transform.localScale = value.scale;
            targetPosition = value.targetPositions;
            lightPosition = value.lightPositions;
            encapsulationItemPosition = value.encapsulationItems;
        }
    }

    protected List<Vector2> targetPosition; //TODO 0829
    protected List<Vector2> lightPosition;
    protected List<Vector2> encapsulationItemPosition;


    private ButtonEntity_Net _net;
    protected ButtonEntity_Net Net { get { _net ??= GetComponent<ButtonEntity_Net>();return _net; } }
    #region  Debug
    private Transform debugModeTransform;
    private List<LineRenderer> lineRendererList;
    #endregion

    #region IPowerConsumer
    // public bool hasPower
    // {
    //     get { return ToggleButton_Net.hasPower > 0 ? true : false; }
    //     set { ToggleButton_Net.Cmd_SetHasPower(value); }
    // }
     public bool hasPower
    {
        get { return Net._hasPower > 0 ? true : false; }
        set { Net.Cmd_SetHasPower(value); }
    }
    public int GetConsumption()
    {
        return 1;
    }
    public void PowerOn()
    {
        Debug.Log("Power");
        //hasPower = true;
        Net.Cmd_SetHasPower(true);
    }
    public void PowerOff()
    {
        //hasPower = false;
        Net.Cmd_SetHasPower(false);
        Debug.Log("Power Off");
        //Deactivated();
        //ToggleButton_Net.Cmd_CallDeactivated();
        Net.Cmd_SetState(false);
    }
    public Vector2 GetPowerLineConnectionPoint(){
        return transform.position;
    }
    public Vector2 GetTransformPosition()
    {
        return transform.position;
    }
    #endregion
    

    #region Main Logic
    protected bool onPrograss;
    protected bool onActive;
    protected virtual IEnumerator Co_Activation() { yield break; }
    protected virtual IEnumerator Co_Deactivated() { yield break; }
    public virtual void Activation() { }
    public virtual void Deactivated() { }
    protected virtual void PrograssButtonActivatedObject(bool onActivate)
    {

        foreach (GameObject obj in targetObjects)
        {
            if (obj.TryGetComponent(out ActivatableObjectEntity component))
            {
                if (TryGetComponent(out NetworkIdentity identity))
                {
                    component.ApplyActive(onActivate ? 1 : -1, identity.netId);
                }
                else
                {
                    component.ApplyActive(onActivate ? 1 : -1);
                }

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
        foreach (EncapsulationField field in interactableObjects)
        {
            //   field.ApplyActive(onActivate ? 1 : -1);
            if (TryGetComponent(out NetworkIdentity identity))
            {
                field.ApplyActive(onActivate ? 1 : -1, identity.netId);
            }
            else
            {
                field.ApplyActive(onActivate ? 1 : -1);
            }
        }


    }

    #endregion

    #region IPowerConsumer, Light Object
    [Header(@"
    * ↓ Can use this field.
       - only Light Object
    ")]
    public List<GameObject> lightObjects;

    #endregion

    #region  Encapuslation Item
    [Header(@"only Interactable Object, need EncapsulationField")]
    public List<EncapsulationField> interactableObjects;
    #endregion


    #region  GET,SET
    protected Util util = new();
    public bool _complete_FindAllObj;
    public override void SetData<T>(T data)
    {
        try
        {
            if (typeof(T) == typeof(ButtonObjectStruct))
            {
                ButtonObjectStruct buttonData = (ButtonObjectStruct)(object)data;
                ButtonObjectData = buttonData;

                FindTargetObject();
                if (buttonData.lightPositions.Count > 0) FindLightObject();
                if (buttonData.encapsulationItems.Count > 0) FindEncapsulationItem();
                SetOtherDataParm(ButtonObjectData);

                if (Application.isPlaying) Net.Server_SetInit();
            }
            
        }
        catch (Exception ex)
        {
            Debug.Log($"name : {gameObject.name},{ex}");
        }

    }
    public virtual void SetOtherDataParm(ButtonObjectStruct data) //Server
    {
        
    }
    // IEnumerator DelayFindCoroutine()
    // {
    //     if (!Application.isPlaying) yield break;

    //     yield return new WaitForSeconds(1f);
    //     FindTargetObject();

    //     if (ButtonObjectData.lightPositions.Count > 0) FindLightObject();
    //     if (ButtonObjectData.encapsulationItems.Count > 0) FindEncapsulationItem();

    // }

    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonObjectStruct))
        {
            return (T)(object)new ButtonObjectStruct(
                id,
                transform.position,
                transform.rotation,
                transform.localScale,

                GetTargetPositions(),
                GetLightPositions(),
                GetEncapsulationTiems(),
                chargeRequired
                );
        }

        return default(T);
    }
    #endregion

    #region Util

    protected virtual List<Vector2> GetTargetPositions()
    {
        List<Vector2> list = new();

        foreach (GameObject obj in targetObjects)
        {
            if (obj == null) continue;
            if (obj.TryGetComponent(out ActivatableObjectEntity _))
            {
                list.Add(ConvertPosition(obj.transform.position));
            }

        }

        return list;
    }
    protected virtual List<Vector2> GetLightPositions()
    {
        List<Vector2> list = new();
        foreach (GameObject obj in lightObjects)
        {
            if (obj == null) continue;
            if (obj.TryGetComponent(out IPowerConsumer component))
            {
                list.Add(component.GetTransformPosition());
            }

        }
        return list;
    }
    protected virtual List<Vector2> GetEncapsulationTiems()
    {
        List<Vector2> list = new();
        foreach (EncapsulationField field in interactableObjects)
        {
            if (field == null) continue;
            var position = ConvertPosition(field.gameObject.transform.position);
            if (!list.Contains(position))
            {
                list.Add(position);
            }

        }
        return list;
    }

    protected Vector3 ConvertPosition(Vector3 vec)
    {
        return new Vector3(
            Mathf.Round(vec.x * 100) / 100,
            Mathf.Round(vec.y * 100) / 100,
            Mathf.Round(vec.z * 100) / 100
        );

    }

    public virtual void FindTargetObject()
    {

        if (!Application.isPlaying) return;
        List<GameObject> objList = new();

        foreach (Vector2 vec in targetPosition)
        {
            foreach (Transform obj in MapEditor.Instance.buttonActivatableObjectTransform)
            {
                if (obj.TryGetComponent(out ActivatableObjectEntity component))
                {
                    if (CompareVec(component.ButtonActivatedObjectStruct.position, vec))
                    {
                        if (component.ButtonActivatedObjectStruct.indicatorStruct.indicator == INDICATOR.MARK)
                        {
                            Debug.Log("[1] Indicator_2, need path check");
                            component.PathChacking(gameObject);
                        }
                        objList.Add(obj.gameObject);
                        break;
                    }
                }
            }
        }
        targetObjects = objList;
    }
    public virtual void FindLightObject()
    {
        if (!Application.isPlaying) return;
        List<GameObject> list = new();

        OtherContainer otherContainer = MapEditor.Instance.otherContainer.GetComponent<OtherContainer>();

        foreach (Vector2 vec in ButtonObjectData.lightPositions)
        {
            otherContainer.GetCompareVec(vec, ref list);
            //otherObject vec 전달 -> group transform 순회 같은거 있는지 확인 -> 있으면 해당 IPowerConsumer 반환
        }
        lightObjects = list;
    }
    protected bool CompareVec(Vector3 p1, Vector3 p2)
    {
        bool x = Mathf.Approximately(p1.x, p2.x);
        bool y = Mathf.Approximately(p1.y, p2.y);

        return x && y;
    }

    public virtual void FindEncapsulationItem()
    {
        if (!Application.isPlaying) return;
        List<EncapsulationField> list = new();

        foreach (Vector2 vec in encapsulationItemPosition)
        {
            foreach (BuildObj obj in MapEditor.Instance._n_activePoolingObject)
            {
                if (CompareVec(obj.ObjectData.position, vec))
                {
                    if (obj.TryGetComponent(out EncapsulationField field))
                    {
                        if (obj.ObjectData.indicator == INDICATOR.MARK)
                        {
                            Debug.Log("[1] Encapsulation Field Indicator Path Chack");
                            field.Indicator_2PathChaking(gameObject);
                        }
                        list.Add(field);
                        break;
                    }

                }

            }
        }

        interactableObjects = list;

    }

#if UNITY_EDITOR
    public override void Editor_Setting(MapEditor mapEditor)
    {
        List<GameObject> objList = new();

        foreach (Vector2 vec in targetPosition)
        {
            foreach (Transform obj in mapEditor.buttonActivatableObjectTransform)
            {
                if (obj.TryGetComponent(out ActivatableObjectEntity component))
                {
                    if (CompareVec(component.ButtonActivatedObjectStruct.position, vec))
                    {
                        objList.Add(obj.gameObject);
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
            //otherObject vec 전달 -> group transform 순회 같은거 있는지 확인 -> 있으면 해당 IPowerConsumer 반환
        }
        lightObjects = list;

        List<EncapsulationField> enList = new();
        foreach (Vector2 vec in encapsulationItemPosition)
        {
            foreach (Transform obj in mapEditor.objectTransform)
            {
                if (obj.TryGetComponent(out BuildObj buildObj))
                {
                    if (CompareVec(buildObj.position, vec))
                    {
                        enList.Add(obj.GetComponent<EncapsulationField>());
                        continue;
                    }
                }
            }

        }
        interactableObjects = enList;
    }
#endif

    #endregion


    #region Clean
    
    public virtual void Animation_Clean()
    {
        
    }
    #endregion
}


