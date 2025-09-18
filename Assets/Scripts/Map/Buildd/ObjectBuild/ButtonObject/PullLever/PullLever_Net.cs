using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Animations;

public class PullLever_Net : NetworkBehaviour
{
    private PullLever Main => GetComponent<PullLever>();
    [SerializeField]  Lever lever;
    [SerializeField] Transform hold_Pivot;

    [SyncVar(hook =nameof(OnChangeOnActive))]public bool onActive;
    [ReadOnly]
    public bool onPrograssButtonActivatedObject;

    #region Server

    [Space(20)]
    public GameObject player;

    [Command(requiresAuthority =false)]
    public void Cmd_SetonPrograssButtonActivatedObject(bool onOff)
    {
        Rpc_Cmd_SetonPrograssButtonActivatedObject(onOff);
    }
    [ClientRpc]
    private void Rpc_Cmd_SetonPrograssButtonActivatedObject(bool onOff)
    {
        onPrograssButtonActivatedObject = onOff;
    }
    [Server]
    private void Server_SetActive(bool active)
    {
        onActive = active;
    }
    [Command(requiresAuthority = false)]
    public void Cmd_SeActive(bool active)
    {
        Server_SetActive(active);
    }
    private void OnChangeOnActive(bool old,bool newVal)
    {
        if(newVal)
        {
            Main.Pulling(newVal);
        }
        else
        {
            Main.Pulling(newVal);
        }
    }
    #endregion

  
    [ClientRpc]
    private void Rpc_SetPlayer(GameObject player)
    {
        this.player = player;
    }


    [Command(requiresAuthority = false)]
    public void Cmd_Interact(uint netId, bool onOff)
    {
        if (NetworkServer.spawned.TryGetValue(netId, out NetworkIdentity identity))
        {
            if(onOff) Rpc_SetPlayer(identity.gameObject);
            else Rpc_SetPlayer(null);

            Server_SetActive(onOff);

            TRpc_Interact(identity.connectionToClient, onOff, identity.gameObject);
   

        }
    }

    [TargetRpc]
    private void TRpc_Interact(NetworkConnection con,bool onOff,GameObject player)
    {
        if(onOff) Holding(player);
        else Recover(player);
    }

    private void Holding(GameObject player)
    {
        //Connection,Adjust the position using the holdPivot.
        Connection(player);

        //Player Animation

        //Player Animation

        //Add Event
        player.GetComponent<PlayerSM>().deathEvent += Event_Recover;
        //Add Event
    }
    private void Recover(GameObject player)
    {

        Disconnection(player);

        //Stop Animation

        //Stop Animation

        //Remove Event
        player.GetComponent<PlayerSM>().deathEvent -= Event_Recover;
        //Remove Event
    }
    #region  Util
    private void Connection(GameObject player)
    {
        ParentConstraint constraint = player.AddComponent<ParentConstraint>();
        SetParentConstraint(constraint,hold_Pivot);
    }
    private void Disconnection(GameObject player)
    {
        if(player.TryGetComponent(out ParentConstraint component))
        {
            Destroy(component);
        }
    }
    private void SetParentConstraint(ParentConstraint constraint,Transform parent)
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
    
    private void Event_Recover(DamageType damageType = DamageType.Default)
    {
        Recover(player);
        Cmd_SeActive(false);
    }




    #region  UI
    [Command(requiresAuthority = false)]
    public void Cmd_ShowE(GameObject player,bool onOff)
    {
         if(player.TryGetComponent(out NetworkIdentity identity))
        {
            TRpc_ShowE(identity.connectionToClient, onOff);
        }
    }
    [TargetRpc]
    private void TRpc_ShowE(NetworkConnection conn,bool onOff)
    {
        if(onOff) lever.ShowE();
        else lever.HideE();
    }
    #endregion
}
