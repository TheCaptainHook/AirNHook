using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Portal_Net : NetworkBehaviour
{
    [SyncVar] public Vector2 targetPortalPosition;

    private Portal Portal => GetComponent<Portal>();

    [Server]
    public void SetTargetPortal(Vector2 targetPortalPosition)
    {
        this.targetPortalPosition = targetPortalPosition;
    }

    [Command]
    public void Cmd_CallUsePortal()
    {
        Managers.AcManager.CallUsePortal();
    }

    [Command]
    public void Cmd_CameraEffect()
    {
        Camera.main.GetComponent<PlayerCameraView>()._CameraGlobalVolumeController.PortalSpace_TimeTransitionEffect();
    }




}
