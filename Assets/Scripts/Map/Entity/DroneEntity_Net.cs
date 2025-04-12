using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DroneEntity_Net : NetworkBehaviour
{
    private DroneEntity main;
    private DroneEntity Main { get { main ??= GetComponent<DroneEntity>(); return main; } }

    private Rigidbody2D rb;
    private Rigidbody2D RB { get { rb ??= GetComponent<Rigidbody2D>(); return rb; } }

    private Vector2[] paths => Main.paths;

    #region Init Sync
    public bool onSync;

    [Server]
    public void Server_InitSync()
    {
        if (!Application.isPlaying) return;
        Server_Init();

        onSync = true;
        Rpc_InitSync(Main.DroneStruct);
    }
    [Command(requiresAuthority = false)]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }
    [ClientRpc]
    private void Rpc_InitSync(DroneStruct data)
    {
        if (onSync) return;
        Main.DroneStruct = data;
        onSync = true;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!onSync) Cmd_InitSync();
    }
    #endregion


    private void Server_Init()
    {
        if (paths == null || paths.Length == 0) return;
        maxIndex = paths.Length;
        index = 0;
        increment = 1;
        targetPosition = paths[0];
        onReady = true;
        
    }
    private bool onReady;
    private int maxIndex;
    private int index;
    private int increment;
    [ReadOnly]
    public Vector2 targetPosition;
    [ReadOnly]
    public Vector2 dir;
    private void FixedUpdate()
    {
        if (isServer && onSync && onReady)
        {
            if (CheckDistance(RB.position, targetPosition))
            {
                index += increment;
                if (index >= maxIndex || index < 0)
                {
                    if (index >= maxIndex && paths[maxIndex - 1] == paths[0])
                    {
                        index = 0;
                    }
                    else
                    {
                        increment *= -1;
                        index += increment;
                    }

                }
                //targetPosition = paths[index];
                //dir = (targetPosition - RB.position).normalized * Time.fixedDeltaTime;
                //Send
                Rpc_Send_CurAndTargetPosition(RB.position, index);
            }
        }


    }

 
    [ClientRpc]
    private void Rpc_Send_CurAndTargetPosition(Vector2 curPosition,int index)
    {
        RB.position = curPosition;
        targetPosition = paths[index];
        dir = (targetPosition - curPosition).normalized;
        Main.DroneMovingAnimation(dir);
    }

  




    #region Utile
    private bool CheckDistance(Vector2 curPos, Vector2 targetPos)
    {
        if (Vector3.Distance(curPos, targetPos) < 0.1f)
        {
            return true;
        }
        return false;
    }
    #endregion
}
