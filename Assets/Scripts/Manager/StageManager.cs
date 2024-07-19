using Mirror;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class StageManager
{
    //public int stage;
    // TODO 로비 이름으로 변경
    public string stageName = "Lobby";
    
    public void LoadMap()
    {
        if(!stageName.Equals("Lobby"))
            Managers.Game.CurrentState = GameState.Game;
        MapEditor.Instance.LoadMap(stageName);

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
     
        GameObject obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);


        //obj.transform.position = data.position;
        ////todo 0425
        //obj.GetComponent<BuildObj>().position = obj.transform.position;
        ////todo 0425

        //obj.GetComponent<BuildObj>().ObjectData = data;

        obj.GetComponent<BuildObj>().SetData(data);

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
    public void CmdBatchObject(string objName, ButtonActivatedObject data)
    {
        if (!NetworkServer.active || !NetworkClient.isConnected) return;

        var obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);

        if (data.id == 306)
        {
            ButtonActivated btn = obj.GetComponent<ButtonActivated>();
            btn.ButtonActivatedObject = data;

        }
        else if (data.id == 312)
        {
            LeverBody leverBody = obj.GetComponent<LeverBody>();
            leverBody.ButtonActivatedObject = data;
        }

        obj.transform.SetParent(MapEditor.Instance.interactionObjectTransform);

        NetworkServer.Spawn(obj, NetworkServer.localConnection);

    }

 
    [Command]
    public void CmdBatchObject(string objName,ButtonActivatedDoorStruct data)
    {
        if (!NetworkServer.active || !NetworkClient.isConnected) return;

        var obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);

        ButtonActivatedDoor door = obj.GetComponent<ButtonActivatedDoor>();
        door.ButtonActivatedDoorStruct = data;

        obj.transform.SetParent(MapEditor.Instance.interactionObjectTransform);

        NetworkServer.Spawn(obj, NetworkServer.localConnection);

        door.CheckActiveRequirAmount();

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



    public async Task Delay() //todo 0425
    {
        Task delayTask = Task.Delay(100);
        await delayTask;
    }

}
