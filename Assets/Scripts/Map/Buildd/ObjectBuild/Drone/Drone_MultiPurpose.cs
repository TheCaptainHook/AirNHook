
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

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision != null)
    //    {
    //        Debug.Log("collision");
    //        if (collision.gameObject.TryGetComponent(out Rigidbody2D component))
    //        {
    //            float vel = component.velocity.magnitude;
    //            Debug.Log(vel);
    //            component.velocity = Vector2.zero;
    //        }
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            if (collider.TryGetComponent(out Rigidbody2D component))
            {
                float vel = component.velocity.magnitude;
                if(vel >= 10)
                {
                    DroneDropTransportItem();
                }
                component.velocity = Vector2.zero;
            }
        }

    }


    private void SetTransformItem()
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
                SettingTransportItem(item);
                break;

            default:
                SettingTransportItem(item);
                break;
            

        }
    }

    private void SettingTransportItem(GameObject item)
    {
        BuildObj obj = item.GetComponent<BuildObj>();
        obj.SettingTransportItem(itemPlacementPosition);
    }

    private void DroneDropTransportItem()
    {
        if(transportItem == null) return;

        transportItem.GetComponent<BuildObj>().DropTransportItem();
    }

}
