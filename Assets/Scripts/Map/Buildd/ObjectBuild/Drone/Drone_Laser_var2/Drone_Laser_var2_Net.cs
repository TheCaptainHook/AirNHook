using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_Laser_var2_Net : DroneEntity_Net
{
    /*
     0 GUARD,
     1 TRACKING,
     2 RETURN,
     3 ATTACK
      * */
    [Header("Drone_Laser Field")]
    [SerializeField] Drone_Laser_var2 droneLaser;
    public DRONE_LASER_STATE droneLaserState;

    private void Update()
    {
        if(onSync)
        {
            DroneState(droneLaserState);
        }
    }

    private void DroneState(DRONE_LASER_STATE state)
    {
        droneLaser.StateChange(state);
    }


    [Server]
    public void Server_DroneLaserState(int index)
    {
        if (droneLaserState != (DRONE_LASER_STATE)index)
        {
            Rpc_DorneLaserState(index);
        }
    }
    [ClientRpc]
    private void Rpc_DorneLaserState(int index)
    {
        
        if(index == 0)
        {
            droneLaser.PreStateSetUp(false);
        }
        else
        {
            droneLaser.PreStateSetUp(true);
        }
        droneLaserState = (DRONE_LASER_STATE)index;
    }
}
