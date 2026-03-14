using Mirror;
using UnityEngine;

public class LeverHead : InteractableObjectEntity
{
    private LeverHead_Net l_net;
    private LeverHead_Net L_Net { get { l_net ??= GetComponent<LeverHead_Net>(); return l_net; } }
    public void AttachToLevelBody() //Server
    {
        L_Net.Server_Attach();
    }


    #region Effect
    #endregion


    public override void Clean()
    {
        L_Net.Clean();
    }
}
