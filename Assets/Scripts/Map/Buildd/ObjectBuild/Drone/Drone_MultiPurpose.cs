
using UnityEngine;


public enum Drone_TransportItemType{
    Box,
    InvincibleBox,
    StrongBox,
    Battery,
    Key,
    LeverHead,
    LightningRod,
    None


}


public class Drone_MultiPurpose : DroneEntity
{
    [CustomHeader("Drone MultiPurpose")]
    public Drone_TransportItemType drone_TransportItemType;


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
         if(typeof(T)==typeof(DroneStruct)){
          DroneStruct dronsSt = (DroneStruct)(object)data;
          DroneStruct = dronsSt;
          drone_TransportItemType = DroneStruct.drone_TransportItemType;
        }
        //Test
        if(Application.isPlaying){
            // Prograss();
            SetTransformItem();
            CallPrograssAction();

        }
    }


    private void SetTransformItem()
    {
        if(DroneStruct.drone_TransportItemType == Drone_TransportItemType.None) return;
        GameObject item = Managers.Stage.CmdBatchObject(drone_TransportItemType.ToString());
        transportItem = item;
        Debug.Log(item);
        if(item == null) return;
        switch(DroneStruct.drone_TransportItemType){
            case Drone_TransportItemType.Key:
                MapEditor.Instance.exitDoorObjectTransform.GetChild(0).GetComponent<ExitPointObj>().AddKeyAmount();
                //SetPosition
                SettingTransformItem(item);
            break;
            default:

            break;
            

        }
    }

    private void SettingTransformItem(GameObject item)
    {
        BuildObj obj = item.GetComponent<BuildObj>();
        obj.SettingTransportItem(itemPlacementPosition);
    }

    private void DropTransformItem()
    {
        if(transportItem == null) return;

        transportItem.GetComponent<BuildObj>().DropTransportItem();
    }

}
