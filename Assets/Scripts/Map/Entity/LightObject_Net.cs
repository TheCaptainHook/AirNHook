using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class LightObject_Net : NetworkBehaviour
{
    private LightObjectEntity entity;
    private LightObjectEntity Entity
    {
        get
        {
            if(entity == null) entity = GetComponent<LightObjectEntity>();
            return entity;
        }
    }
    
    
    [SyncVar(hook = nameof(OnChangeHasPower))] 
    public bool hasPower;


    [Server]
    private void Server_SetHasPower(bool hasPower)
    {
        this.hasPower = hasPower;
    }
    [Command(requiresAuthority = false)]
    public void Cmd_SetHasPower(bool hasPower)
    {
        Server_SetHasPower(hasPower);
    }


    private void OnChangeHasPower(bool old,bool newVal)
    {
        if(newVal)
        {
            Entity.PowerOn();
        }
        else
        {
            Entity.PowerOff();
        }
    }


    
}
