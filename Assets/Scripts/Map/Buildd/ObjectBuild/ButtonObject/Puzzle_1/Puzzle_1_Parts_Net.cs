using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class Puzzle_1_Parts_Net : NetworkBehaviour
{
    Puzzle_1_Parts parts;
    Puzzle_1_Parts Main { get { parts ??= GetComponent<Puzzle_1_Parts>(); return parts; } }

    private Collider2D Collider => GetComponent<Collider2D>();

    [SyncVar(hook = nameof(OnChangeCorrect))]
    public bool isCorrectAnswer;


    // [SyncVar] public GameObject _insertItem;

    
    [Command(requiresAuthority =false)]
    public void Cmd_Connection(uint netId)
    {
        Rpc_Connection(netId);
    }

    [ClientRpc]
    public void Rpc_Connection(uint netId)
    {
        var item = NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity) ? identity.gameObject : null;
        if (item == null) return;

        Connect(item.GetComponent<Puzzle_1_Item>());
    }


    [Command(requiresAuthority = false)]
    public void Cmd_DisConnect(bool wrongAnswer)
    {
        Rpc_DisConnect(wrongAnswer);
    }
    [ClientRpc]
    private void Rpc_DisConnect(bool wrongAnswer)
   
    {
        if (Main.insert_Item == null) return;
        DisConnect(wrongAnswer);
    }

    public Puzzle_1_Item insert_Item;
    public bool onSocket = false;

    #region  Conneect
    public void Connect(Puzzle_1_Item item) //Rpc
    {
        if (onSocket)
        {
            DisConnect();
        }

        Debug.Log("Connect Item[Parts]");
        
        //Sound
        Managers.Sound.PlaySound3D(GlobalText.PUZZLE_PARTS_INSTER, transform.position);
        //Sound

        var col = item.TryGetComponent(out Collider2D collider) ? collider : null;
        if (col != null) col.enabled = false;
        var rb = item.TryGetComponent(out Rigidbody2D rigidbody) ? rigidbody : null;
        if (rb != null)
        {
            rb.simulated = false;
            rb.velocity = Vector3.zero;
        }
        Main.insert_Item = item;
        insert_Item = item;

        if (item.TryGetComponent(out ParentConstraint parentConstraint))
        {
            if (parentConstraint.sourceCount > 0)
            {
                parentConstraint.RemoveSource(0);
            }
            SetParentConstraint(parentConstraint, transform);
        }
        Main.InsertAnimation(true);
        onSocket = true;

    }

    private void SetParentConstraint(ParentConstraint constraint, Transform parent)
    {
        ConstraintSource source = new ConstraintSource
        {
            sourceTransform = parent,
            weight = 1
        };
        constraint.AddSource(source);

        constraint.translationAtRest = transform.localPosition;
        constraint.translationOffsets = new Vector3[constraint.sourceCount];
        constraint.constraintActive = true;

        constraint.locked = true;
        
    }

    #endregion
    #region  DisConnect
    public void DisConnect(bool wrongAnswer = false) //Rpc
    {
        //Sound
        Managers.Sound.PlaySound3D(GlobalText.PUZZLE_PARTS_INSTER, transform.position);
        //Sound
        
        if (insert_Item.TryGetComponent(out ParentConstraint component))
        {
            if (component.sourceCount > 0)
            {
                component.RemoveSource(0);
            }

        }
        Main.InsertAnimation(false);

        var col = insert_Item.TryGetComponent(out Collider2D collider) ? collider : null;
        if (col != null) col.enabled = true;
        var rb = insert_Item.TryGetComponent(out Rigidbody2D rigidbody) ? rigidbody : null;
        if (rb != null)
        {
            rb.simulated = true;
            rb.velocity = Vector3.zero;
        }

        insert_Item.RemoveSocketEffect(wrongAnswer);

        insert_Item.parts = null;
        insert_Item = null;
        Main.insert_Item = null;

        onSocket = false;
        
    }

    #endregion


    [Command(requiresAuthority = false)]
    public void Cmd_SetCorrect(bool val)
    {
        this.isCorrectAnswer = val;
    }


    private void OnChangeCorrect(bool old, bool newVal)
    {
        if (newVal)
        {
            Collider.enabled = false;
            Main.Net_SetCorrectEffect();
        }
    }


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


}


