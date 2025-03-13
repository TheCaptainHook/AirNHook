using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverHead_Net : TransportItemEntity
{


    [Server]
    public void Server_Attach()
    {
        Destroyed();
        Rpc_Attach();
    }

    [ClientRpc]
    private void Rpc_Attach()
    {
        Col.enabled = false;
        Rb.simulated = false;
    }
    [Server]
    public void Server_Att()
    {
        Rpc_Att();
    }
    [ClientRpc]
    private void Rpc_Att()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
