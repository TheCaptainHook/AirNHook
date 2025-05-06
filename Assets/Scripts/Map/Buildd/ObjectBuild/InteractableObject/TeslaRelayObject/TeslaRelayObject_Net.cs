using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TeslaRelayObject_Net : TransportItemEntity
{
    [Server]
   public void Server_Effect()
   {

   }
   [ClientRpc]
   private void Rpc_Effect()
   {
    
   }
}
