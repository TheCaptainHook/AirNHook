using UnityEngine;
using Mirror;
using UnityEngine.Animations;

public class MirrorObject_Net : NetworkBehaviour
{
    [SerializeField] GameObject _Mirror;

    [Space(20)]
    [Header("Sync Data")]

    public bool onActive;

    private Collider2D col;
    private Collider2D Col { get { col ??= GetComponent<Collider2D>(); return col; } }

    private MirrorObject mirrorObject;
    private MirrorObject Main { get { mirrorObject ??= GetComponent<MirrorObject>(); return mirrorObject; } }

    [SerializeField] Transform hold_Pivot;
    //#region  Init Sync
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

    private GameObject innerPlayer;
    public void Holding(GameObject player)
    {
        innerPlayer = player;

        onActive = true;
        var pm = player.GetComponent<PlayerSM>();
        
        pm.canMovable = false;
        pm.canAction = false;
        Connection(player);

        pm.deathEvent += Event_Recover;

        Main.isActive = true;
    }

    public void Recover()
    {
        Main.ChangeEbutton(false);
        
        onActive = false;
        Main.isActive = false;

        if (innerPlayer == null) return;

        var pm = innerPlayer.GetComponent<PlayerSM>();
        
        pm.canAction = true;
        pm.canMovable = true;

        pm.isControlObj = false;
        pm.canInteract = true;

        Disconnection(innerPlayer);
        
        pm.deathEvent -= Event_Recover;

        Cmd_InnerPlayer(9999);
        Cmd_Reset();
       
    }
   

    [Command(requiresAuthority = false)]
    private void Cmd_Reset()
    {
        Rpc_Reset();
    }
    [ClientRpc]
    private void Rpc_Reset()
    {
        Col.enabled = false;
        Col.enabled = true;
        //targetZ = 0;
    }

    private void Event_Recover(DamageType damageType = DamageType.Default)
    {
        Main.HideADEButton();
        Main.HideEButton();
        Recover();
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

    }
    private void Disconnection(GameObject player)
    {
        if (player.TryGetComponent(out ParentConstraint component))
        {
            if (component.sourceCount > 0)
            {
                component.RemoveSource(0);
            }

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