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

    private Collider2D Collider => GetComponent<Collider2D>();
    public Puzzle_1_Item GetItem => item ? item.GetComponent<Puzzle_1_Item>() : null;

    [SyncVar(hook = nameof(OnChangeSocketItem))] 
    public GameObject item;
    [SyncVar(hook = nameof(OnChangeCorrect))] 
    public bool isCorrectAnswer;


    [Server]
    public void SetSocketItem(GameObject item)
    {
        this.item = item;
    }
    [Server]
    private void SetCorrect(bool isCorrectAnswer)
    {
        this.isCorrectAnswer = isCorrectAnswer;
    }


    [Command(requiresAuthority = false)]
    public void Cmd_SetSocketItem(GameObject item)
    {
        SetSocketItem(item);
    }
    [Command(requiresAuthority = false)]
    public void Cmd_SetCorrect(bool val)
    {
        this.isCorrectAnswer = val;
    }

    private void OnChangeSocketItem(GameObject old,GameObject newVal)
    {
        if (old) RemoveSocket(old);

        if (newVal)
        {
            InsertSocket(newVal);
        }

    }
    private void OnChangeCorrect(bool old, bool newVal)
    {
        if (newVal)
        {
            Collider.enabled = false;
            Parts.Net_SetCorrectEffect();
        }
    }


    private void RemoveSocket(GameObject item)
    {
       if(item.TryGetComponent(out Puzzle_1_Item component))
        {
            component.RemoveSocket();
            Parts.InsertAnimation(false);
        }
        
    }
    private void InsertSocket(GameObject item)
    {
        Collider.enabled = false;
        Collider.enabled = true;
        Parts.InsertAnimation(true);
    }



    [Command(requiresAuthority = false)]
    public void Cmd_RemoveEffect()
    {
        Rpc_RemoveEffect();
    }
    [ClientRpc]
    public void Rpc_RemoveEffect(){
        Parts.Net_RemovEffect();
    }


