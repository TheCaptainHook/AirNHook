using System.Collections;
using Mirror;
using UnityEngine;


public class LeverHead_Net : TransportItemEntity
{


    [Server]
    public void Server_Attach()
    {
        Respawned();
        Rpc_Attach();
        
    }

    [ClientRpc]
    private void Rpc_Attach()
    {
        Col.enabled = false;
        Rb.simulated = false;
        gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);
    }
    
   public void Clean()
     {
        Col.enabled = true;
        Rb.simulated = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
