using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Portal_Net : NetworkBehaviour
{
    [SyncVar] public Vector2 targetPortalPosition;
    [SyncVar] public bool onPrograss;

    [SyncVar] public GameObject targetPortal;

    private Portal Portal => GetComponent<Portal>();

    [Server]
    public void SetTargetPortal(Vector2 targetPortalPosition)
    {
        this.targetPortalPosition = targetPortalPosition;
    }

    [Server]
    public void SetTargetPortal(GameObject obj)
    {
        targetPortal = obj;
    }

    [Server]
    public void UsePortal(GameObject obj)
    {
        StartCoroutine(UsePortal_Co(obj));
    }

    IEnumerator UsePortal_Co(GameObject obj)
    {
        //onPrograss = true;
        Server_ChangePrograss();

        Rpc_HoldPlayer(obj);
        targetPortal.GetComponent<Portal>().Net_ChangeOnPrograss();
        Rpc_TransmitPosition(obj);

        yield return new WaitForSeconds(1);

        Rpc_RecoverPlayer(obj);
        yield return new WaitForSeconds(1);
        targetPortal.GetComponent<Portal>().Net_ChangeOnPrograss();
        Server_ChangePrograss();
    }


    [Server]
    public void Server_ChangePrograss()
    {
        onPrograss = !onPrograss;
    }

 

    [ClientRpc]
    public void Rpc_TransmitPosition(GameObject obj)
    {
        obj.transform.position = targetPortalPosition + Vector2.up;
    }



    [TargetRpc]
    public void Target_CameraEffect(NetworkConnection target)
    {
        if (Camera.main != null)
        {
            Camera.main.GetComponent<PlayerCameraView>()?._CameraGlobalVolumeController?
                .PortalSpace_TimeTransitionEffect();
        }
    }

    [Command]
    public void Cmd_UsePortal(GameObject obj)
    {
        Target_CameraEffect(connectionToClient);
        UsePortal(obj);

    }


    //[Command]
    //public void Cmd_CallUsePortal()
    //{
    //    Managers.AcManager.CallUsePortal();
    //}





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
}
