using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LaserObject_Net : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnChangeOnActive))]
    public bool onActive;


    LaserObject laser;
    LaserObject Laser
    {
        get
        {
            if (laser == null) laser = GetComponent<LaserObject>();
            return laser;
        }
    }

    [Server]
    public void Server_SetOnActive(bool active)
    {
        onActive = active;
    }
    private void OnChangeOnActive(bool old, bool newVal)
    {
        if (newVal)
        {
            Laser.Net_Active();
        }
        else
        {
            Laser.Net_Deactive();
        }
    }
}
