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


    [SyncVar] 
    private GameObject item;
    [SyncVar] 
    private bool isCorrectAnswer;





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

    // [Command(requiresAuthority = false)]
    //public void CmdUnLock()
    //{
    //     RpcUnLock();
    //}
    //[ClientRpc]
    //private void RpcUnLock()
    //{
    //     parts.Net_UnLock();
    //}




    [Command(requiresAuthority =false)]
    public void CmdRemoveSocket(bool onEffect)
    {
        RpcRemoveSocket(onEffect);
    }
    [ClientRpc]
    public void RpcRemoveSocket(bool onEffect)
    {
        parts.RemoveSocket(onEffect);
    }


    [Command(requiresAuthority =false)]
    public void CmdCorrectAnswer()
    {
        RpcCorrectAnswer();
    }
    [ClientRpc]
    public void RpcCorrectAnswer()
    {
        parts.Net_InCorrectAnswer();
    }
}
