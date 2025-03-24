using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BeamDoor_Net : NetworkBehaviour
{   

  BeamDoor Main => GetComponent<BeamDoor>();


   
#region Init Sync
public bool onSync;
[Server]
public void Server_InitSync()
{
    Rpc_InitSync(Main.ButtonActivatedObjectStruct);
}
[ClientRpc]
private void Rpc_InitSync(ButtonActivatableObjectStruct data)
{
    if(onSync) return;

    transform.position = data.position;
    transform.rotation = data.quaternion;
    transform.localScale = data.scale;

    onSync = true;

}
[Command]
public void Cmd_InitSync()
{
    Server_InitSync();
}

public override void OnStartClient()
{
    base.OnStartClient();
    if(!onSync)Cmd_InitSync();
}
#endregion






}
