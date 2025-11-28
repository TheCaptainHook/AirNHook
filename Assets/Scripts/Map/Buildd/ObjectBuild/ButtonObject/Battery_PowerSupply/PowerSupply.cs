using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSupply : ButtonEntity
{
    [CustomHeader("Power Supply")]
    [SerializeField] Transform socketPosition;

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
            Net.Server_SetInit();
        }

    }
    #region  Clean
    public override void Clean()
    {
        GetComponent<PowerSupply_Net>().Clean();
    }
    #endregion
    #region Sync TargetObejct
   
    public override void FindTargetObject()
    {
        if (!Application.isPlaying) return;
        StartCoroutine(Delay_FindTargetCo());
        
    }
    private IEnumerator Delay_FindTargetCo()
    {
        _complete_FindAllObj = false;

        yield return new WaitUntil(() => MapEditor.Instance._l_complete_button_obj);
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
        _complete_FindAllObj = true;
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
    public override void Activation()
    {
        PrograssButtonActivatedObject(true);
    }
    public override void Deactivated()
    {
        PrograssButtonActivatedObject(false);
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
                    otherContainer.GetCompareVec(vec,ref list);
                }
                lightObjects = list;

            });
    }



    #endregion


 #region  Main
    //   private float condition_InsertBatteryChargerValue = 5;
    // private bool ChackBatteryVelocity(Battery battery)
    // {
    //     Debug.Log(battery._rb.velocity.magnitude);
    //     return battery._rb.velocity.magnitude >= condition_InsertBatteryChargerValue;
    // }


#endregion



}
