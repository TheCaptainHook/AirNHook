using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Puzzle_1_Parts_Net : NetworkBehaviour
{
    Puzzle_1_Parts parts {
        get{
            return GetComponent<Puzzle_1_Parts>();
        }
    }
   [Command(requiresAuthority = false)]
   public void CmdLock()
   {
        RpcLock();
   }
   [ClientRpc]
   private void RpcLock()
   {
    parts.Net_Lock();
   }

    [Command(requiresAuthority = false)]
   public void CmdUnLock()
   {
        RpcUnLock();
   }
   [ClientRpc]
   private void RpcUnLock()
   {
        parts.Net_UnLock();
   }
}
