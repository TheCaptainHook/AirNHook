
using Mirror;
using UnityEngine;


public enum Drone_TransportItemType{
    Box,
    InvincibleBox,
    StrongBox,
    Battery,
    Key,
    LeverHead,
    LightningRod,
    TeslaRelayObject,
    None

}


public class Drone_MultiPurpose : DroneEntity
{

    //public Drone_TransportItemType drone_TransportItemType => DroneStruct.drone_TransportItemType;
    [CustomHeader("Drone MultiPurpose")]
    public Drone_TransportItemType drone_TransportItemType;

    [ReadOnly]
    public Transform itemPlacementPosition;
    private GameObject transportItem;

    public override T GetData<T>()
    {
        if(typeof(T)==typeof(DroneStruct)){
            return (T)(object)new DroneStruct(id,transform.position,transform.localScale,ConvertPaths(paths),moveSpeed,drone_TransportItemType);
        }
        return default(T);
    }



    public override void SetData<T>(T data)
    {
        base.SetData(data);
        drone_TransportItemType = DroneStruct.drone_TransportItemType;

        if(Application.isPlaying)
        {
            SetTransformItem();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            if (collider.TryGetComponent(out Rigidbody2D component))
            {
                float vel = component.velocity.magnitude;
                if (vel >= 10)
                {
                    //DroneDropTransportItem();
                    Net.Cmd_CallDropTransportItem();


                }
                component.velocity = Vector2.zero;
            }
        }

    }


    private void SetTransformItem() //Only Server
    {
        if(DroneStruct.drone_TransportItemType == Drone_TransportItemType.None) return;

        GameObject item = Managers.Stage.CmdBatchObject(drone_TransportItemType.ToString());
        item.transform.SetParent(MapEditor.Instance.networkingObjectTransform); 
        transportItem = item;
        if(item == null) return;

        switch(DroneStruct.drone_TransportItemType){
            case Drone_TransportItemType.Key:
                MapEditor.Instance.exitDoorObjectTransform.GetChild(0).GetComponent<ExitPointObj>().AddKeyAmount();
                //SetPosition
                DroneSettingTransportItem(item);
                break;

            default:
                DroneSettingTransportItem(item);
                break;
            
        }
    }

    private void DroneSettingTransportItem(GameObject item) //Only Server
    {
        BuildObj obj = item.GetComponent<BuildObj>();
        obj.SettingTransportItem(gameObject);
        // obj.SettingTransportItem(GetComponent<NetworkIdentity>().netId);
    }

    public void DroneDropTransportItem()
    {
        if(transportItem == null) return;

        transportItem.GetComponent<BuildObj>().DropTransportItem();
    }


    public override void Clean()
    {
        if(NetworkServer.active)
        {
            DroneDropTransportItem();
        }

       if(transportItem.TryGetComponent(out BuildObj buildObj))
        {
          buildObj.isTransportItem = false; 
        };
    

        base.Clean();
    }

}
