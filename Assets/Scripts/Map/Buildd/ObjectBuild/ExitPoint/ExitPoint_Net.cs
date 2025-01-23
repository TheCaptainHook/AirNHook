using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint_Net : NetworkBehaviour
{
    AbsencePanel panel;
    AbsencePanel Panel
    {
        get
        {
            if(panel== null) panel = GetComponent<AbsencePanel>();
            return panel;
        }
       
    }

    [Command(requiresAuthority = false)]
    public void Enter(GameObject obj)
    {
        RpcEnter(obj);
    }
    [ClientRpc]
    public void RpcEnter(GameObject obj)
    {
        Panel.Enter(obj);
    }
    [Command(requiresAuthority = false)]
    public void Exit(GameObject obj)
    {
        RpcExit(obj);
    }
    [ClientRpc]
    public void RpcExit(GameObject obj)
    {
        Panel.Exit(obj);
    }
    

}
