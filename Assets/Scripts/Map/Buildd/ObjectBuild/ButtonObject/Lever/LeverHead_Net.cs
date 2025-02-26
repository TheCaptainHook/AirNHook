using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverHead_Net : InteractableObject
{
    private Collider2D Col => GetComponent<Collider2D>();
    private Rigidbody2D Rd => GetComponent<Rigidbody2D>();



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
        Rd.simulated = false;
    }

}
