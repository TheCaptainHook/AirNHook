using System.Collections;
using Mirror;
using UnityEngine;


public class LeverHead_Net : TransportItemEntity
{


    [Server]
    public void Server_Attach()
    {
        Destroyed();
        Rpc_Attach();
        // Rpc_Att();
        StartCoroutine(DelayDestroy());
    }

    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(1);
        NetworkServer.Destroy(gameObject);
    }
    [ClientRpc]
    private void Rpc_Attach()
    {
        Col.enabled = false;
        Rb.simulated = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }
    
    // [Server]
    // public void Server_Att()
    // {
    //     Rpc_Att();
    // }
    // [ClientRpc]
    // private void Rpc_Att()
    // {

    // }
}
