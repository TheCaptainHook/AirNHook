using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongBox_Net : InteractableObject
{
    [SyncVar] public Vector2 orgPosition;

    [Server]
    public void Server_SetOrgPosition(Vector2 orgPosition)
    {
        this.orgPosition = orgPosition;
    }
}
