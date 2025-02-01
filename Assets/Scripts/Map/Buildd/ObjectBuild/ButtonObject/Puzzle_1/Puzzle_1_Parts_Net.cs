using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Puzzle_1_Parts_Net : NetworkBehaviour
{
    Puzzle_1_Parts Parts {
        get{
            return GetComponent<Puzzle_1_Parts>();
        }
    }


    [SyncVar(hook = nameof(OnChangeSocketItem))] 
    public GameObject item;
    [SyncVar] 
    private bool isCorrectAnswer;


    [Server]
    public void SetSocketItem(GameObject item)
    {
        this.item = item;
    }
    [Command(requiresAuthority = false)]
    public void Cmd_SetSocketItem(GameObject item)
    {
        SetSocketItem(item);
    }

    private void OnChangeSocketItem(GameObject old,GameObject newVal)
    {     
        Parts.Net_InsertSocketItem(newVal);
    }




    // [Command(requiresAuthority = false)]
    // public void CmdLock()
    // {
    //     RpcLock();
    // }
    // [ClientRpc]
    // private void RpcLock()
    // {
    //     Parts.Net_Lock();
    // }

    [Command(requiresAuthority = false)]
    public void CmdUnLock()
    {
        RpcUnLock();
    }
    [ClientRpc]
    private void RpcUnLock()
    {
        Parts.Net_UnLock();
    }



    /// <summary>
    /// false : Not effect
    /// </summary>
    /// <param name="onEffect"></param>
    [Command(requiresAuthority =false)]
    public void CmdRemoveSocket(bool onEffect)
    {
        RpcRemoveSocket(onEffect);
    }
    [ClientRpc]
    public void RpcRemoveSocket(bool onEffect)
    {
        Parts.RemoveSocket(onEffect);
    }


    [Command(requiresAuthority =false)]
    public void CmdCorrectAnswer()
    {
        RpcCorrectAnswer();
    }
    [ClientRpc]
    public void RpcCorrectAnswer()
    {
        Parts.Net_InCorrectAnswer();
    }
}
