using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkCommand : NetworkBehaviour
{
    #region SetUp
    private void Awake()
    {
        Managers.Command = this;
    }
    #endregion

    // NOTE Command 이거 왜 static으론 안됨...? 화나네...
    [Command(requiresAuthority = false)]
    public void CmdCheck()
    {
        
    }
}
