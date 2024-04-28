using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkCommand : NetworkBehaviour
{
    #region Messages
    public struct ButtonDoorDataMessage : NetworkMessage
    {
        public GameObject obj;
        public ButtonActivatedDoorStruct data;
    }
    #endregion

    #region SetUp
    private void Awake()
    {
        Managers.Command = this;
        NetworkClient.RegisterHandler<ButtonDoorDataMessage>(SetUpButtonDoorData, false);
    }
    #endregion

    // NOTE Command 이거 왜 static으론 안됨...? 화나네...
    [Command(requiresAuthority = false)]
    public void CmdCheck()
    {
        
    }

    #region Object
    /// <summary> Send ExitDoor Data </summary>
    [Server]
    public void SendButtonDoorData(ButtonActivatedDoorStruct data)
    {
        var obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict["ButtonActivatedDoor"]);
        NetworkServer.Spawn(obj);
        
        var msg = new ButtonDoorDataMessage()
        {
            obj = obj,
            data = data
        };
        
        NetworkServer.SendToAll(msg);
    }
    
    /// <summary> Receive and SetUp ExitDoor Data </summary>
    private void SetUpButtonDoorData(ButtonDoorDataMessage message)
    {
        var door = message.obj.GetComponent<ButtonActivatedDoor>();
        door.ButtonActivatedDoorStruct = message.data;
        door.CheckActiveRequirAmount();
        message.obj.transform.SetParent(MapEditor.Instance.interactionObjectTransform);
    }
    #endregion
}
