using Mirror;
// using Unity.VisualScripting;
using UnityEngine;

public class DroneEntity_Net : NetworkBehaviour
{
    private DroneEntity main;
    private DroneEntity Main { get { main ??= GetComponent<DroneEntity>(); return main; } }

    private Rigidbody2D rb;
    protected Rigidbody2D RB { get { rb ??= GetComponent<Rigidbody2D>(); return rb; } }

    public Vector2[] paths;

    #region Init Sync
    public bool onSync;

    [Server]
    public void Server_InitSync()
    {
        if (!Application.isPlaying) return;
        if(!onSync)
        {
            Server_Init();
            onSync = true;
        }

        Rpc_InitSync(Main.DroneStruct,RB.position,index);
    }
    [Command(requiresAuthority = false)]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }

    [ClientRpc]
    private void Rpc_InitSync(DroneStruct data,Vector2 curPosition,int index)
    {
        if (onSync) return;
        
        
        
        Main.DroneStruct = data;
        RB.position = curPosition;

        if(data.paths != null && data.paths.Length != 0)
        {
            paths = data.paths;
            targetPosition = paths[index];
            dir = (targetPosition - curPosition).normalized;
        }

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
        if (Main.paths == null || Main.paths.Length == 0) return;
        paths = Main.paths;
        maxIndex = paths.Length;
        index = 0;
        increment = 1;
        targetPosition = paths[0];
        onReady = true;
        
    }
    private bool onReady;
    public int maxIndex;
    public int index;
    public int nextIndex;
    private int increment;
    [ReadOnly]
    public Vector2 targetPosition;
    [ReadOnly]
    public Vector2 dir;

    [ReadOnly]
    public Vector2 startPot;

    private void FixedUpdate()
    {
        if (isServer && onSync && onReady)
        {
            if (CheckDistance(RB.position, targetPosition))
            {
                nextIndex = index + increment;
                
                if (nextIndex >= maxIndex || nextIndex < 0)
                {
                    if (nextIndex >= maxIndex && paths[maxIndex - 1] == paths[0])
                    {
                        index = 0;
                    }
                    else
                    {
                        increment *= -1;
                        index += increment;
                    }

                }
                else
                {
                    index = nextIndex;
                }

                startPot = targetPosition;
                targetPosition = paths[index];
                //Send
                Rpc_Send_CurAndTargetPosition(RB.position, index);
            }


        }


    }

 
    [ClientRpc]
    private void Rpc_Send_CurAndTargetPosition(Vector2 curPosition,int index)
    {
        if (!onSync) return;
        RB.position = curPosition;
        if (targetPosition == null || paths == null || paths.Length == 0) return;
        
        targetPosition = paths[index];
        dir = (targetPosition - curPosition).normalized;
        Main.DroneMovingAnimation(dir);

        //Limit area Setting
                
        //Limit area Setting
                
    }

    [Command(requiresAuthority = false)]
    public void Cmd_CallDropTransportItem()
    {
        if(isServer)
        {
            gameObject.GetComponent<Drone_MultiPurpose>().DroneDropTransportItem();
        }
    }

    #region Utile
    private bool CheckDistance(Vector2 curPos, Vector2 targetPos)
    {
        if (Vector2.Distance(curPos, targetPos) < 0.1f)
        {
            return true;
        }

        if (targetPos == startPot) return true;

        Vector2 toTarget = (targetPos - startPot).normalized;
        Vector2 toCurrent = (curPos - targetPos).normalized;

        float dot = Vector2.Dot(toTarget, toCurrent);
        return dot > 0.98f;

    }
    #endregion


}
