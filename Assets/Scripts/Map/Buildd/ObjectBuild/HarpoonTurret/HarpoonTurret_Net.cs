
using Mirror;


public class HarpoonTurret_Net : NetworkBehaviour
{
  public bool onSync;
private HarpoonTurret Main => GetComponent<HarpoonTurret>();
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
}
