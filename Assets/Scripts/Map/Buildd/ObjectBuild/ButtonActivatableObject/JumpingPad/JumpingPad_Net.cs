using Mirror;
using System;
using System.Collections;

using UnityEngine;

public class JumpingPad_Net : NetworkBehaviour
{


    #region  Animation
    Animator Animator => GetComponent<Animator>();
    readonly int Activated = Animator.StringToHash("Activated");
    #endregion

    private JumpingPad Main => GetComponent<JumpingPad>();

    #region  Init Sync
    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        StartCoroutine(AllClientReadyChecker_Co(()=>{Rpc_InitSync(Main.ButtonActivatedObjectStruct);}));
    }

    [ClientRpc]
    private void Rpc_InitSync(ButtonActivatableObjectStruct data)
    {
        if(onSync) return;

        transform.position = data.position;
        transform.rotation = data.quaternion;
        transform.localScale = data.scale;

        jumpingPower = data.jumpingPower;

        onSync = true;
    }

    [Command]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!onSync) Cmd_InitSync();
    }

    IEnumerator AllClientReadyChecker_Co(Action action)
    {
        while(true)
        {
            int connectionClinetAmount = NetworkServer.connections.Count;
            int num = 0;
            foreach(var conn in NetworkServer.connections.Values)
            {
                if(conn.isReady) num++;
            }

            if(connectionClinetAmount == num) break;

            yield return null;
        }
        action?.Invoke();

    }
    #endregion


    public int jumpingPower;
    [SyncVar(hook = nameof(OnChangeOnActive))] public bool onActive;
    private void OnChangeOnActive(bool old,bool newVal)
    {
        Animator.SetBool(Activated, newVal);
    }



    // [Server]
    // public void Server_SetJumpingPower(int jumpingPower)
    // {
    //     this.jumpingPower = jumpingPower;
    // }

    [Server]
    public void Server_SetOnActive(bool onActive)
    {
        this.onActive = onActive;
    }


    [Command]
    public void Cmd_Jumping(GameObject obj)
    {
        if (obj.TryGetComponent(out NetworkIdentity component))
        {
            TRpc_Jumping(component.connectionToClient, obj);
            return;
        }
        else
        {
            Jumping(obj.GetComponent<Rigidbody2D>());
        }


    }

    [TargetRpc]
    private void TRpc_Jumping(NetworkConnection con, GameObject obj)
    {
      if(obj.TryGetComponent(out Rigidbody2D component))
        {
            Jumping(component);
        }
           

    }


    public void Jumping(Rigidbody2D rb)
    {
        if (!onActive) return;
        rb.velocity = Vector2.zero;
        //rb.AddForce(Vector2.up * jumpingPower, ForceMode2D.Impulse);
        rb.AddForce(transform.up * jumpingPower, ForceMode2D.Impulse);
    }
}
