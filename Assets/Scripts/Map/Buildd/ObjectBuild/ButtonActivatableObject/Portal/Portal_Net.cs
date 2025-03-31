using System.Collections;
using UnityEngine;
using Mirror;
using System;

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
    public void Server_SetTargetPortal(GameObject obj)
    {
        targetPortal = obj;
        Rpc_SetTargetPortal(obj.GetComponent<NetworkIdentity>().netId);
        onSync = true;
    }

    [ClientRpc]
    private void Rpc_SetTargetPortal(uint id)
    {
        if(NetworkClient.spawned.TryGetValue(id,out NetworkIdentity networkIdentity))
        {
            targetPortal = networkIdentity.gameObject;
        }       
    }

    [Server]
    public void SetTargetPortal(Vector2 targetPortalPosition)
    {
        this.targetPortalPosition = targetPortalPosition;  
    }



#region  Sync Init
  
    [Command(requiresAuthority = false)]
    private void Cmd_SyncData()
    {
        SyncData();
    }

    [Server]
    public void SyncData()
    {
        Rpc_SyncData(targetPortalPosition, orgPosition,targetPortal);
    }
    [ClientRpc]
    private void Rpc_SyncData(Vector2 targetPosition,Vector2 orgPosition,GameObject targetPortal)
    {
        transform.position = orgPosition;
        this.targetPortalPosition = targetPosition;

        if (targetPortal != null) this.targetPortal = targetPortal;

        onSync = true;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        //if (!onSync) StartCoroutine(Delay(()=> { }));
        Cmd_SyncData();

    }

    IEnumerator Delay(Action action)
    {
        while (!NetworkClient.ready) { Debug.Log("Ready Network Portal"); yield return null; }
       
        action?.Invoke();
    }


    #endregion

    //------------------------------------------------------------------Refactoring
    #region StringCache
    private static readonly int IsActive = Animator.StringToHash("IsActive");
    #endregion

    private Portal Portal => GetComponent<Portal>();
    private Animator Animator => GetComponent<Animator>();

    

    [Command(requiresAuthority = false)]
    public void Cmd_UsePortal(GameObject obj)
    {
        // var netIdentity = obj.GetComponent<NetworkIdentity>();
        // var playerConn = netIdentity.connectionToClient;
        // Target_CameraEffect(playerConn); 

        //UsePortal(obj);
        var netIdentity = obj.GetComponent<NetworkIdentity>();
        var playerConn = netIdentity.connectionToClient;
        TRpc_Portal(playerConn, obj);
    }

    //[Server]
    //public void UsePortal(GameObject obj)
    //{
    //    StartCoroutine(UsePortal_Co(obj));
    //}

    IEnumerator UsePortal_Co(GameObject obj)
    {
        var netIdentity = obj.GetComponent<NetworkIdentity>();
        var playerConn = netIdentity.connectionToClient;

        OnPrograss(true);

        //Rpc_HoldPlayer(obj);
        if (obj.TryGetComponent(out Rigidbody2D component))
        {
            component.simulated = false;
        }
       

        //Target_CameraEffect(playerConn);
        if (Camera.main != null)
        {
            Camera.main.GetComponent<PlayerCameraView>()?._CameraGlobalVolumeController?
                .PortalSpace_TimeTransitionEffect();
        }
        Debug.Log("Target!");

        obj.transform.position = targetPortalPosition + Vector2.up;
        //Rpc_TransmitPosition(obj);

        yield return new WaitForSeconds(1);

        //Rpc_RecoverPlayer(obj);
        component.simulated = true;

        yield return new WaitForSeconds(2);

        OnPrograss(false);

    }

       
    public void Animation_Active(bool active)
    {
        Animator.SetBool(IsActive, active);
        _TpEffect.SetActive(active);
    }

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
    private void OnPrograss(bool onOff)
    {
        onPrograss = onOff;

        var net = targetPortal.GetComponent<Portal_Net>();
        net.onPrograss = onOff;

        Animation_Active(!onOff);
        net.Animation_Active(!onOff);
    }
    

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




    [TargetRpc]
    public void TRpc_Portal(NetworkConnection conn,GameObject player)
    {
        if(!onPrograss)
        {
            StartCoroutine(UsePortal_Co(player));
        }

    }
#endregion
}
