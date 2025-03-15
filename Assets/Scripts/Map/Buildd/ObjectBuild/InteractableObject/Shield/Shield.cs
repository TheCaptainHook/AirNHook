
using UnityEngine;

public class Shield : BuildObj
{
    Shield_Net Net => GetComponent<Shield_Net>();


    private void Awake(){
        DissolveInitSetting();
    }



    public override void SetData<T>(T data)
    {
        base.SetData(data);
        Net.onSync = true;
        Net.Server_InitSync();
        // if(Application.isPlaying)
        // {
        //     Net.Server_SetOrgPot(ObjectData.position);
        // }
    }


    public override void TurnOff()
    {
        base.TurnOff();
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
    }
    public override void TurnOn()
    {
        base.TurnOn();
        _rb.gravityScale = 1;
    }



 

}
