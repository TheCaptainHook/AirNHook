using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingPad_Net : NetworkBehaviour
{
    [SyncVar] public int jumpingPower;
    [SyncVar] public bool onActive;

    [Server]
    public void Server_SetJumpingPower(int jumpingPower)
    {
        this.jumpingPower = jumpingPower;
    }
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
        rb.AddForce(Vector2.up * jumpingPower, ForceMode2D.Impulse);
    }
}
