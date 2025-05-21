using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Puzzle_1_Parts_Net : NetworkBehaviour
{
    Puzzle_1_Parts parts;
    Puzzle_1_Parts Main { get { parts ??= GetComponent<Puzzle_1_Parts>(); return parts; } }

    private Collider2D Collider => GetComponent<Collider2D>();
    //public Puzzle_1_Item GetItem => item ? item.GetComponent<Puzzle_1_Item>() : null;

    //[SyncVar(hook = nameof(OnChangeSocketItem))]
    //public GameObject item;

    [SyncVar(hook = nameof(OnChangeCorrect))]
    public bool isCorrectAnswer;










    [Command(requiresAuthority = false)]
    public void Cmd_DisConnect()
    {
        Rpc_DisConnect();
    }
    [ClientRpc]
    private void Rpc_DisConnect()
    {
        if (Main.insert_Item == null) return;
        Main.DisConnect();
    }














    //[Server]
    //public void SetSocketItem(GameObject item)
    //{
    //    this.item = item;
    //}
    //[Server]
    //private void SetCorrect(bool isCorrectAnswer)
    //{
    //    this.isCorrectAnswer = isCorrectAnswer;
    //}


    //[Command(requiresAuthority = false)]
    //public void Cmd_SetSocketItem(GameObject item)
    //{
    //    SetSocketItem(item);
    //}
    [Command(requiresAuthority = false)]
    public void Cmd_SetCorrect(bool val)
    {
        this.isCorrectAnswer = val;
    }

    //private void OnChangeSocketItem(GameObject old, GameObject newVal)
    //{
    //    if (old) RemoveSocket(old);

    //    if (newVal)
    //    {
    //        InsertSocket(newVal);
    //    }

    //}
    private void OnChangeCorrect(bool old, bool newVal)
    {
        if (newVal)
        {
            Collider.enabled = false;
            Main.Net_SetCorrectEffect();
        }
    }


    //private void RemoveSocket(GameObject item)
    //{
    //    if (item.TryGetComponent(out Puzzle_1_Item component))
    //    {
    //        component.RemoveSocket();
    //        Main.InsertAnimation(false);
    //    }

    //}
    //private void InsertSocket(GameObject item)
    //{
    //    Collider.enabled = false;
    //    Collider.enabled = true;
    //    Main.InsertAnimation(true);
    //}



    [Command(requiresAuthority = false)]
    public void Cmd_RemoveEffect()
    {
        Rpc_RemoveEffect();
    }
    [ClientRpc]
    public void Rpc_RemoveEffect()
    {
        Main.Net_RemovEffect();
    }


    #region UI
    [Command(requiresAuthority = false)]
    public void Cmd_ShowE(GameObject player, bool onOff)
    {
        if (player.TryGetComponent(out NetworkIdentity component))
        {
            TRpc_ShowE(component.connectionToClient, onOff);
        }
    }
    [TargetRpc]
    private void TRpc_ShowE(NetworkConnection conn, bool onOff)
    {
        if (onOff) Main.ShowE();
        else Main.HideE();
    }
    #endregion
}


