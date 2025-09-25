using Mirror;

public class PairAuthDoor_Net : NetworkBehaviour
{
    private PairAuthDoor _main;
    private PairAuthDoor Main { get { _main ??= GetComponent<PairAuthDoor>(); return _main; } }


    [SyncVar]
    public bool _authSuccess;
    public bool _onProgress; // Only Change Server


    [Command(requiresAuthority = false)]
    public void Cmd_Auth()
    {
        if (_onProgress || _authSuccess) return;
        _onProgress = true;
        Rpc_Auth();
    }
    [ClientRpc]
    public void Rpc_Auth()
    {
        StartCoroutine(Main.AuthCoroutine());
    }


    [Server]
    public void Server_Open()
    {
        _authSuccess = true;
        Rpc_Open();
    }
    [ClientRpc]
    private void Rpc_Open()
    {
        Main.Open();
    }
}
