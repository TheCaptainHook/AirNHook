using Mirror;
using UnityEngine;

public class LeverBodyNet : NetworkBehaviour
{
    private LeverBody _leverBody;

    private void Awake()
    {
        Debug.Log("awake");
        _leverBody = GetComponent<LeverBody>();
    }

    //[Command(requiresAuthority = false)]
    //public void CmdSetLinkDoor(Vector2 pot, int linkId)
    //{
    //    RpcSetLinkDoor(pot, linkId);
    //}

    //[ClientRpc]
    //private void RpcSetLinkDoor(Vector2 pot, int linkId)
    //{
    //    _leverBody.SetLinkDoor(pot, linkId, MapEditor.Instance.interactionObjectTransform);
    //}

    [Command(requiresAuthority = false)]
    public void CmdLeverActivate()
    {
        RpcLeverActivate();
    }

    [ClientRpc]
    private void RpcLeverActivate()
    {
        _leverBody.Activation();
    }
}
