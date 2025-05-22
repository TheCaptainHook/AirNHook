using Mirror;


public class Barrel_Net : NetworkBehaviour
{
    private Barrel barrel;
    private Barrel Main { get { barrel ??= GetComponent<Barrel>(); return barrel; } }


    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(Main.ObjectData);
    }
    [Command(requiresAuthority = false)]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }
    [ClientRpc]
    private void Rpc_InitSync(ObjectData data)
    {
        if (onSync) return;
        transform.position = data.position;
        transform.localScale = data.scale;
        transform.rotation = data.quaternion;

        onSync = true;
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        if (!onSync) Cmd_InitSync();
    }
}
