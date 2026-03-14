
using System.Collections;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(LightObject_Net))]
public class LightObjectEntity : BuildObj,IPowerConsumer
{
    [Header("Default Light Object")]
    public GameObject _Light_Object;

    private LightObject_Net lightObject_Net;
    private LightObject_Net L_Net
    {
        get
        {
            if(lightObject_Net == null) lightObject_Net = GetComponent<LightObject_Net>();
            return lightObject_Net;
        }
    }

    #region IPowerConsumer
    public bool hasPower //Server
    {
        get
        {
            return L_Net.hasPower >0 ? true : false;
        }
        set
        {
            // L_Net.Cmd_SetHasPower(value);
            L_Net.Server_SetHasPower(value);
        }
    }
    public int GetConsumption()
    {
        return 1;
    }
    public virtual void PowerOn()
    {
        if(hasPower) return;
        Debug.Log("PowerOn");
        hasPower = true;
        // _Light_Object.SetActive(true);
    }
    public virtual void PowerOff()
    {
        Debug.Log("PowerOff");
        hasPower = false;
        // _Light_Object.SetActive(false);
    }
    public Vector2 GetPowerLineConnectionPoint(){
        return transform.position;
    }
    public Vector2 GetTransformPosition(){
        return ConvertPosition(transform.position);
    }
    #endregion

 
    #region  Get,Set
    public override T GetData<T>()
    {
         if(typeof(T)==typeof(ObjectData)){
            return (T)(object)new ObjectData(id,ConvertPosition(transform.position),transform.rotation,transform.localScale,chargeRequired);
        }

       return default(T);
    }
    
    public override void SetData<T>(T data)
    {
        if(typeof(T) == typeof(ObjectData)){
            ObjectData objData = (ObjectData)(object)data;
            SetData(objData);

            if (Application.isPlaying) L_Net.Server_Init();
        

        }

    }


    //IEnumerator NetworkReady_SetChargeRequired()
    //{
    //    while(!NetworkClient.ready)
    //    {
    //        yield return null;
    //    }
    //    L_Net.Cmd_SetChargeRequired(chargeRequired);
    //}

    public override void SetData(ObjectData data)
    {
        ObjectData = data;
        transform.position = data.position;
        transform.rotation = data.quaternion;
        transform.localScale = data.scale;
        chargeRequired = data.chargeRequired;

    }
    #endregion


    private Vector3 ConvertPosition(Vector3 vec)
    {

        return new Vector3(
            Mathf.Round(vec.x * 100) / 100, 
            Mathf.Round(vec.y * 100) / 100, 
            Mathf.Round(vec.z * 100) / 100
        );


    }




}
