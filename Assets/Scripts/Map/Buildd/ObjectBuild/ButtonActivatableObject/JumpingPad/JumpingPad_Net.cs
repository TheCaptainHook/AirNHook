using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingPad_Net : NetworkBehaviour
{
    [SyncVar] public int jumpingPower;

    [Server]
    public void Server_SetJumpingPower(int jumpingPower)
    {
        this.jumpingPower = jumpingPower;
    }
}
