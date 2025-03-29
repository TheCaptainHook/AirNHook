using System.Collections;
using UnityEngine;
using Mirror;

public class Portal_Net : NetworkBehaviour
{
    [SerializeField] GameObject _TpEffect;
    [Space(20)]
    [Header("------------------------------------")]

    //------------------------------------------------------------------Effect sync
    [SyncVar(hook = nameof(ChangeOnActive))] public bool onActive;
    private void ChangeOnActive(bool old, bool newVal)
    {
        Animator.SetBool(IsActive, newVal);
        _TpEffect.SetActive(newVal);

    }
    [Server] //sync
    public void Server_SetOnActive(bool val)
    {
        onActive = val;
    }


    [Command]
    public void Cmd_CallSetOnActive(bool val)
    {
        Server_SetOnActive(val);
    }
    //------------------------------------------------------------------Effect sync

    //------------------------------------------------------------------Refactoring0303
    public Vector2 orgPosition => Portal.ButtonActivatedObjectStruct.position;
    public GameObject targetPortal;
    public bool onSync;
    public Vector2 targetPortalPosition;
    public bool onPrograss;

    [Server]
    public void SetTargetPortal(GameObject obj)
    {
        targetPortal = obj;
        onSync = true;
    }

    [Server]
    public void SetTargetPortal(Vector2 targetPortalPosition)
    {
        this.targetPortalPosition = targetPortalPosition;  
    }



#region  Sync Init
  
    [Command]
    private void Cmd_SyncData()
    {
        SyncData();
    }

    [Server]
    public void SyncData()
    {
        Rpc_SyncData(targetPortalPosition, orgPosition);
    }
    [ClientRpc]
    private void Rpc_SyncData(Vector2 targetPosition,Vector2 orgPosition)
    {
        if(!onSync)
        {
            transform.position = orgPosition;
            this.targetPortalPosition = targetPosition;
            onSync = true;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        while (!NetworkClient.ready) yield return null;
        Cmd_SyncData();
    }
    #endregion

    //------------------------------------------------------------------Refactoring
    #region StringCache
    private static readonly int IsActive = Animator.StringToHash("IsActive");
    #endregion

    private Portal Portal => GetComponent<Portal>();
    private Animator Animator => GetComponent<Animator>();

    

    [Command]
    public void Cmd_UsePortal(GameObject obj)
    {
        // var netIdentity = obj.GetComponent<NetworkIdentity>();
        // var playerConn = netIdentity.connectionToClient;
        // Target_CameraEffect(playerConn); 

        UsePortal(obj);

    }

    [Server]
    public void UsePortal(GameObject obj)
    {
        StartCoroutine(UsePortal_Co(obj));
    }

    IEnumerator UsePortal_Co(GameObject obj)
    {
        //onPrograss = true;
        onPrograss = true;

        Rpc_HoldPlayer(obj);
        targetPortal.GetComponent<Portal>().Net_ChangeOnPrograss(true);

        var netIdentity = obj.GetComponent<NetworkIdentity>();
        var playerConn = netIdentity.connectionToClient;

        Target_CameraEffect(playerConn);
        Rpc_TransmitPosition(obj);


        //Animation OFF
        //main , targetPortal OFF
        Animator.SetBool(IsActive, false);
        _TpEffect.SetActive(false);
        //Animation OFF

        yield return new WaitForSeconds(1);

        Rpc_RecoverPlayer(obj);
        yield return new WaitForSeconds(2);

        //Animation ON
        //main , targetPortal ON
        Animator.SetBool(IsActive, true);
        _TpEffect.SetActive(true);
        //Animation ON

        targetPortal.GetComponent<Portal>().Net_ChangeOnPrograss(false);

        onPrograss = false;
    }
    // [Server]
    // public void Server_ChangePrograss()
    // {
    //     onPrograss = !onPrograss;
    // }
       

    [ClientRpc]
    public void Rpc_TransmitPosition(GameObject obj)
    {
        obj.transform.position = targetPortalPosition + Vector2.up;
    }


#region  Camera
    [TargetRpc]
    public void Target_CameraEffect(NetworkConnection target)
    {
        if (Camera.main != null)
        {
            Camera.main.GetComponent<PlayerCameraView>()?._CameraGlobalVolumeController?
                .PortalSpace_TimeTransitionEffect();
        }
        Debug.Log("Target!");

    }
#endregion

    

#region  Hold,Recover

    [ClientRpc]
    public void Rpc_HoldPlayer(GameObject obj)
    {
        if(obj.TryGetComponent(out Rigidbody2D component))
        {
            component.simulated = false;
        }
    }

    [ClientRpc]
    public void Rpc_RecoverPlayer(GameObject obj)
    {
        if (obj.TryGetComponent(out Rigidbody2D component))
        {
            component.simulated = true;
        }
    }
#endregion
}
