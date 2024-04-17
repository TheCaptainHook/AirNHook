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





}
