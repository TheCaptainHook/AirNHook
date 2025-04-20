using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_Laser_var2_Net : DroneEntity_Net
{
  
    [Header("Drone_Laser Field")]
    [SerializeField] Drone_Laser_var2 droneLaser;
    [SerializeField] PathFinder pathFinder;
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
            if(trackingBeforePosition != defaultVec)
            {
                Rpc_DorneLaserState(2);
                return;

            }

            Rpc_DorneLaserState(index);
        }
    }

    /*
   0 GUARD,
   1 TRACKING,
   2 RETURN,
   3 ATTACK
    * */
    public Vector2 trackingBeforePosition;
    private Vector2 defaultVec = new Vector2(9999, 9999);
    public List<Vector2> returnPath;

    
    [ClientRpc]
    private void Rpc_DorneLaserState(int index)
    {
        if(index == 0)
        {
            droneLaser.PreStateSetUp(false,false);
        }
        else if(index == 1)
        {
            trackingBeforePosition = RB.position;
            droneLaser.PreStateSetUp(true, true);
        }
        else if (index == 2)
        {
            returnPath = pathFinder.FindPath(RB.position, trackingBeforePosition);
            droneLaser.returnIndex = 0;
            droneLaser.PreStateSetUp(true, false);
        }
        else if(index == 3)
        {
            droneLaser.PreStateSetUp(true,true);
        }

        droneLaserState = (DRONE_LASER_STATE)index;
    }

    [Server]
    public void Server_AfterReturn()
    {
        Rpc_AfterReturn();
    }
    private void Rpc_AfterReturn()
    {
        RB.position = trackingBeforePosition;
        trackingBeforePosition = defaultVec;
        returnPath.Clear();
        droneLaserState = DRONE_LASER_STATE.GUARD;
    }
}
