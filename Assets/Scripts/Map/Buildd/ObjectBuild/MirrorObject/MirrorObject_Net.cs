
using UnityEngine;
using Mirror;
using UnityEngine.Animations;



public class MirrorObject_Net : NetworkBehaviour
{
   [SerializeField] GameObject _Mirror;

    [Space(20)]
    [Header("Sync Data")]
    //[SyncVar(hook =nameof(OnChange_ThisObjectAuthority))]
  

    [SyncVar(hook =nameof(OnChageRotate_Z))] 
    public float rotate_Z;

    
    public bool onActive;

    private Collider2D col;
    private Collider2D Col { get { col ??= GetComponent<Collider2D>();return col; } }

    private MirrorObject mirrorObject;
    private MirrorObject Main { get { mirrorObject ??= GetComponent<MirrorObject>();return mirrorObject; } }

    [SerializeField] Transform hold_Pivot;
    #region  Init Sync
    public bool onSync;

    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(Main.ObjectData);
    }
    [ClientRpc]
    private void Rpc_InitSync(ObjectData data)
    {
        if(onSync) return;
        transform.position = data.position;
        transform.rotation = data.quaternion;

        onSync = true;
    }
    [Command(requiresAuthority = false)]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!onSync)Cmd_InitSync();
    }
    #endregion

    #region InnerPlayer Sync
    public GameObject InnerPlayer;

    [Command(requiresAuthority = false)]
    public void Cmd_InnerPlayer(uint netID)
    {
        Rpc_InnerPlayer(netID);
    }
    [ClientRpc]
    private void Rpc_InnerPlayer(uint netID)
    {
        if(netID == 9999)
        {
            InnerPlayer = null;
            return;
        }
        InnerPlayer = NetworkClient.spawned.TryGetValue(netID, out NetworkIdentity identity) ? identity.gameObject : null;

    }
    #endregion

    /**
     * 1. Enter Trigger -> InnerPlayer sync -> 서버
     * 2. innerplayer가 null 이 아니면 걍 개무시,
     * 3. innerplayer가 로컬인경우에만 e 작동하게 ,
     * **/



    #region  Server
    //[Server]
    //private void Server_SetInnerPlayer(GameObject player)
    //{
    //    InnerPlayer = player;
    //}

    //private void OnChange_ThisObjectAuthority(GameObject old,GameObject newVal)
    //{
    //    if(newVal == null)
    //    {
    //        //old Revoke Authority
    //        GrantOrRevokeAuthority(old,false);
    //    }else
    //    {
    //        //newVal Grant Authority
    //        GrantOrRevokeAuthority(newVal,true);
    //    }
    //}

    //[Command(requiresAuthority = false)]
    //private void GrantOrRevokeAuthority(GameObject obj,bool isAuthorized)
    //{
    //    if(obj.TryGetComponent(out NetworkIdentity identity))
    //    {
    //       TRpc_CheckIdentity(identity.connectionToClient,isAuthorized,obj);
    //    }
    //}

    //[TargetRpc]
    //private void TRpc_CheckIdentity(NetworkConnection conn,bool isAuthorized,GameObject player)
    //{
    //    if(isAuthorized)Holding(player);
    //    else Recover(player);

    //}

    [Server]
    public void Server_SetRot_z(float z)
    {
        rotate_Z = z;
    }
    [Command(requiresAuthority = false)]
    public void Cmd_SetRot_z(float z)
    {
        Server_SetRot_z(z);
    }



    #region UI
    [Command(requiresAuthority = false)]
    public void Cmd_ShowE(GameObject player,bool onOff)
    {
        if(player.TryGetComponent(out NetworkIdentity identity))
        {
            TRpc_ShowE(identity.connectionToClient, onOff);
        }
       
    }
    [TargetRpc]
    private void TRpc_ShowE(NetworkConnection conn,bool onOff)
    {
        if(onOff) Main.ShowE();
        else Main.HideE();

    }
    #endregion


    //hook
    private void OnChageRotate_Z(float old,float newVal)
    {

        MirrorRotate(newVal);

    }
    private void MirrorRotate(float val)
    {
        Quaternion curRot = _Mirror.transform.rotation;
        curRot.z = val;
        _Mirror.transform.rotation= curRot;
        // _Mirror.transform.rotation =
    }
#endregion

   
    //[Command(requiresAuthority = false)]
    //public void Cmd_SetInnerPlayer(GameObject player)
    //{
    //    //Server_SetInnerPlayer(player);
    //}

    //Hook





    public void Holding(GameObject player)
    {
        Managers.UI.HideUI<UI_ShowEButton>();
        onActive = true;
        var pm = player.GetComponent<PlayerSM>();

        //Show A,D button

        //Show A,D button

        Connection(player);
        pm.canMovable = false;

        player.GetComponent<PlayerSM>().deathEvent += Event_Recover;

    }
    public void Recover(GameObject player)
    {
        onActive = false;
        var pm = player.GetComponent<PlayerSM>();
       
        //Hide A,D button

        //Hide A,D button

        Disconnection(player);
        pm.canMovable = true;

        player.GetComponent<PlayerSM>().deathEvent -= Event_Recover;
    }


    private void Event_Recover()
    {
        Cmd_InnerPlayer(9999);
    }

    #region  Util
    private void Connection(GameObject player)
    {
        player.TryGetComponent(out Animator animator);
        animator.SetBool(GlobalText.MOVE_ANIMATION_STRING, false);

        if(player.TryGetComponent(out ParentConstraint parentConstraint))
        {
           if(parentConstraint.sourceCount >0)
            {
                parentConstraint.RemoveSource(0);
            }
            SetParentConstraint(parentConstraint, hold_Pivot);
        }
        //if (player.GetComponent<ParentConstraint>()) return;

        //ParentConstraint constraint = player.AddComponent<ParentConstraint>();
        //SetParentConstraint(constraint, hold_Pivot);
    }
    private void Disconnection(GameObject player)
    {
        if (player.TryGetComponent(out ParentConstraint component))
        {
            if (component.sourceCount > 0)
            {
                component.RemoveSource(0);
            }
            Col.enabled = false;
            Col.enabled = true;
        }


    }
    private void SetParentConstraint(ParentConstraint constraint, Transform parent)
    {
        ConstraintSource source = new ConstraintSource
        {
            sourceTransform = parent,
            weight = 1
        };
        constraint.AddSource(source);

        constraint.translationAtRest = transform.localPosition;
        constraint.translationOffsets = new Vector3[constraint.sourceCount];
        constraint.constraintActive = true;

        constraint.locked = true;
    }
    #endregion

}
