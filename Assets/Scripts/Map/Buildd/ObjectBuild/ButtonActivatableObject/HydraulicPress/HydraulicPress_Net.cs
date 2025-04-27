using Mirror;


public class HydraulicPress_Net : NetworkBehaviour
{
    public bool onSync;
    private HydraulicPress Main => GetComponent<HydraulicPress>();
    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(Main.ObjectData);
    }
    [ClientRpc]
    public void Rpc_InitSync(ObjectData data)
    {
        if (onSync) return;
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
        if (!onSync) Cmd_InitSync();
    }
    
}
