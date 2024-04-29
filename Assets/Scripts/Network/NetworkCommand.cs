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

    #region Object
    [Server]
    public void BatchObject(string objName, ButtonActivatedDoorStruct data)
    {
        var obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);
        NetworkServer.Spawn(obj, NetworkServer.localConnection);

        CmdButtonDataSync(obj, data.id, data.linkId, data.activeRequirAmount, data.position,
            data.buttonActivatePositionList, data.leverPositionList, data.quaternion, data.scale);
    }
    
    /// <summary> Send Button Door Data </summary>
    [Command(requiresAuthority = false)]
    private void CmdButtonDataSync(GameObject obj, int id, int linkId, int activeRequireAmount, Vector2 position, List<Vector2> buttonActivatePositionList, List<Vector2> leverPositionList, Quaternion quaternion, Vector3 scale)
    {
        RpcButtonDataSync(obj, id, linkId, activeRequireAmount, position, buttonActivatePositionList, leverPositionList, quaternion, scale);
    }
    
    /// <summary> Receive and SetUp Button Door Data </summary>
    [ClientRpc]
    private void RpcButtonDataSync(GameObject obj, int id, int linkId, int activeRequireAmount, Vector2 position, List<Vector2> buttonActivatePositionList, List<Vector2> leverPositionList, Quaternion quaternion, Vector3 scale)
    {
        var data = new ButtonActivatedDoorStruct(id, linkId, activeRequireAmount, position, buttonActivatePositionList, leverPositionList, quaternion, scale);
        
        var door = obj.GetComponent<ButtonActivatedDoor>();
        door.ButtonActivatedDoorStruct = data;
        door.CheckActiveRequirAmount();
        obj.transform.SetParent(MapEditor.Instance.interactionObjectTransform);
    }
    #endregion
}
