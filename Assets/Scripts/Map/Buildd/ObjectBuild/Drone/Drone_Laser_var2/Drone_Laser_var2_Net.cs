using Mirror;
using UnityEngine;

public class Drone_Laser_var2_Net : DroneEntity_Net
{
  
    [Header("Drone_Laser Field")]
    [SerializeField] Drone_Laser_var2 droneLaser;
    [SerializeField] PathFinder pathFinder;
    [SerializeField] Drone_LaserParts parts;
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
            if(index == 0 && targetId != 9999) targetId = 9999;
            Rpc_DorneLaserState(index);
        }
    }

    /*
   0 GUARD,
   1 TRACKING,
   2 RETURN,
   3 ATTACK
    * */

    //----------------------TRACKING
    // [ReadOnly]
    // public Vector2 trackingBeforePosition;
    // private Vector2 defaultVec = new Vector2(9999, 9999);
    // public List<Vector2> returnPath; //TRACKING
    //----------------------TRACKING
    public GameObject target;
    public bool onAtack;
    [ClientRpc]
    private void Rpc_DorneLaserState(int index)
    {
        if (index == 0)
        {
            parts.target = null;
            droneLaser.PreStateSetUp(false, false);

        }
        //else if(index == 1) //TRACKING
        //{
        //    trackingBeforePosition = RB.position;
        //    droneLaser.PreStateSetUp(true, true);
        //}
        //else if (index == 2) //RETURN
        //{
        //    returnPath = pathFinder.FindPath(RB.position, trackingBeforePosition);
        //    droneLaser.returnIndex = 0;
        //    droneLaser.PreStateSetUp(true, false);
        //}
        else if (index == 3)
        {
            onAtack = true;
            droneLaser.PreStateSetUp(true, true);
            Managers.Sound.PlaySound3D(GlobalText.ALERT_SOUND, transform, 0.3f);
        }

        droneLaserState = (DRONE_LASER_STATE)index;
    }

    // [Server]
    // public void Server_AfterReturn()
    // {
    //     Rpc_AfterReturn();
    // }
    // private void Rpc_AfterReturn()
    // {
    //     RB.position = trackingBeforePosition;
    //     trackingBeforePosition = defaultVec;
    //     returnPath.Clear();
    //     droneLaserState = DRONE_LASER_STATE.GUARD;
    // }


    private uint targetId;  //Server
    [Server]
    public void Server_SetTarget(uint id)
    {
        if(targetId != id){
            Rpc_SetTarget(id);
            targetId = id;
        }
        
    }
    [ClientRpc]
    private void Rpc_SetTarget(uint id)
    {
        var target = NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity) ? identity : null;
        if(target == null) 
        {
            parts.target = null;
            return;
        }
        parts.target = identity.gameObject;
    }
}
