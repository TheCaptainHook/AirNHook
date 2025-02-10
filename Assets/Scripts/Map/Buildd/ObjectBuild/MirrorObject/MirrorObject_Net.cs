
using UnityEngine;
using Mirror;


public class MirrorObject_Net : NetworkBehaviour
{
   [SerializeField] GameObject _Mirror;

    [Space(20)]
    [Header("Sync Data")]
    [SyncVar(hook =nameof(OnChange_ThisObjectAuthority))]
    public GameObject InnerPlayer;

    [SyncVar(hook =nameof(OnChageRotate_Z))] 
    public float rotate_Z;

    
    public bool onActive;

    private MirrorObject MirrorObject => GetComponent<MirrorObject>();

#region  Server
    [Server]
    private void Server_SetInnerPlayer(GameObject player)
    {
        InnerPlayer = player;
    }

    private void OnChange_ThisObjectAuthority(GameObject old,GameObject newVal)
    {
        if(newVal == null)
        {
            //old Revoke Authority
            GrantOrRevokeAuthority(old,false);
        }else
        {
            //newVal Grant Authority
            GrantOrRevokeAuthority(newVal,true);
        }
    }

     private void GrantOrRevokeAuthority(GameObject obj,bool isAuthorized)
    {
        if (!isServer) return;

        if(obj.TryGetComponent(out NetworkIdentity identity))
        {
           TRpc_CheckIdentity(identity.connectionToClient,isAuthorized,obj);
        }
    }

    [TargetRpc]
    private void TRpc_CheckIdentity(NetworkConnection conn,bool isAuthorized,GameObject player)
    {
        if(isAuthorized)Holding(player);
        else Recover(player);

    }

    [Server]
    public void Server_SetRot_z(float z)
    {
        rotate_Z = z;
    }
    [Command]
    public void Cmd_SetRot_z(float z)
    {
        Server_SetRot_z(z);
    }



    #region UI
    [Command]
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
        if(onOff) MirrorObject.ShowE();
        else MirrorObject.HideE();

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

   
    [Command]
    public void Cmd_SetInnerPlayer(GameObject player)
    {
        Server_SetInnerPlayer(player);
    }

    //Hook





    private void Holding(GameObject player)
    {
        onActive = true;
        //player holding
        
    }
    private void Recover(GameObject player)
    {
        onActive = false;
        //player recover
    }



   


}
