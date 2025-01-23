using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint_Net : NetworkBehaviour
{
    [SerializeField] ExitPointObj exit;

    [Command(requiresAuthority = false)]
    public void OnAbsencePanel()
    {
        RpcOnAbsencePanel();
    }
    [ClientRpc]
    public void RpcOnAbsencePanel()
    {
        exit.OnAbsence();
    }
    [Command(requiresAuthority = false)]
    public void Enter(GameObject obj)
    {
        RpcEnter(obj);
    }
    [ClientRpc]
    public void RpcEnter(GameObject obj)
    {
        exit.Enter(obj);
    }
    [Command(requiresAuthority = false)]
    public void Exit(GameObject obj)
    {
        RpcExit(obj);
    }
    [ClientRpc]
    public void RpcExit(GameObject obj)
    {
        exit.Exit(obj);
    }
    

}
