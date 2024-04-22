using Mirror;
using UnityEngine;

public class StageManager
{
    //public int stage;
    // TODO 로비 이름으로 변경
    public string stageName = "Lobby";
    
    public void LoadMap()
    {
        if(!stageName.Equals("Lobby"))
            Managers.Game.CurrentState = GameState.Game;
        MapEditor.Instance.LoadMap(stageName, MapType.Scene);

        //if (NetworkServer.active && NetworkClient.isConnected)
        //{
        //    var list = MapEditor.Instance.curMap.FindObject_Vector2(307);

        //    foreach (var key in list)
        //    {
        //        var obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict["Key"]);
        //        obj.transform.position = key;
        //        NetworkServer.Spawn(obj);
        //    }
        //}
    }
    
    #region Editor

    [Command]
    public void CmdBatchObject(string objName,ObjectData data)
    {
        if (!NetworkServer.active || !NetworkClient.isConnected) return;

        var obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);
        obj.transform.position = data.position;
        obj.GetComponent<BuildObj>().ObjectData = data;
        obj.transform.SetParent(MapEditor.Instance.networkingObjectTransform);
        NetworkServer.Spawn(obj, NetworkServer.localConnection);
    }

    [Command]
    public void CmdBatchObject(string objName, ExitObjStruct data)
    {
        if (!NetworkServer.active || !NetworkClient.isConnected) return;

        var obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);
        obj.transform.position = data.position;
        obj.GetComponent<ExitPointObj>().SetData(data);
        obj.transform.SetParent(MapEditor.Instance.exitDoorObjectTransform);
        NetworkServer.Spawn(obj, NetworkServer.localConnection);

    }

    [Command]
    public GameObject CmdBatchObject(string objName, Transform transform, Vector2 pot)
    {
        if (!NetworkServer.active || !NetworkClient.isConnected) return null;

        var obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName], pot);
        obj.transform.SetParent(transform);
        
        NetworkServer.Spawn(obj, NetworkServer.localConnection);
        return obj;

    }

    [Command]
    public GameObject CmdBatchObject(string objName)
    {
        if (!NetworkServer.active || !NetworkClient.isConnected) return null;

        var obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);
        NetworkServer.Spawn(obj, NetworkServer.localConnection);

        return obj;
    }

    [Command(requiresAuthority = false)]
    public void CmdDestroyObject(GameObject gameObject)
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion


}
