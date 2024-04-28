using Mirror;
using UnityEngine;
using System.Threading.Tasks;

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

        var obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);
        obj.transform.position = data.position;
        //todo 0425
        obj.GetComponent<BuildObj>().position = obj.transform.position;
        //todo 0425

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
        //obj.GetComponent<ExitPointObj>().SetData(data);
        obj.transform.SetParent(MapEditor.Instance.exitDoorObjectTransform);
        NetworkServer.Spawn(obj, NetworkServer.localConnection);

    }

    //[Command]
    //public void CmdBatchObject(string objName, ButtonActivatedDoorStruct data)
    //{
    //    if (!NetworkServer.active || !NetworkClient.isConnected) return;

    //    var obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);

    //    ButtonActivatedDoor door = obj.GetComponent<ButtonActivatedDoor>();
    //    door.ButtonActivatedDoorStruct = data;
    //    obj.transform.SetParent(MapEditor.Instance.interactionObjectTransform);

    //    MapDataStruct btn = Managers.Data.mapData.mapObjectDataDictionary[306];

    //    foreach (Vector2 pot in data.buttonActivatePositionList)
    //    {
    //        //GameObject btnActivated = Object.Instantiate(Resources.Load<GameObject>(btn.path));
    //        Debug.Log(btn.path);
    //        GameObject btnActivated = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[btn.name]);
    //        btnActivated.GetComponent<ButtonActivated>().SetLinkDoor(pot, door);
    //        btnActivated.transform.SetParent(MapEditor.Instance.dontSaveObjectTransform);
    //    }
    //    foreach (Vector2 pot in data.leverPositionList)
    //    {
    //        GameObject leverBody = CmdBatchObject("LeverBody", MapEditor.Instance.dontSaveObjectTransform, pot);

    //        if (leverBody is not null)
    //            leverBody.GetComponent<LeverBodyNet>().CmdSetLinkDoor(pot, data.linkId);

    //    }

    //    NetworkServer.Spawn(obj, NetworkServer.localConnection);

    //}
    [Command]
    public void CmdBatchObject(string objName,ButtonActivatedDoorStruct data)
    {
        if (!NetworkServer.active || !NetworkClient.isConnected) return;

        Debug.Log("Create Interaction door");

        var obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);
        Debug.Log("Create!!");



        ButtonActivatedDoor door = obj.GetComponent<ButtonActivatedDoor>();
        door.ButtonActivatedDoorStruct = data;
        door.CheckActiveRequirAmount();
        obj.transform.SetParent(MapEditor.Instance.interactionObjectTransform);
        //todo 0425
        //MapDataStruct btn = Managers.Data.mapData.mapObjectDataDictionary[306];

        //foreach (Vector2 pot in data.buttonActivatePositionList)
        //{
        //    //GameObject btnActivated = Object.Instantiate(Resources.Load<GameObject>(btn.path));
        //    GameObject btnActivated = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[btn.name]);
        //    btnActivated.GetComponent<ButtonActivated>().SetLinkDoor(pot, door);
        //    btnActivated.transform.SetParent(MapEditor.Instance.dontSaveObjectTransform);
        //}
        //foreach (Vector2 pot in data.leverPositionList)
        //{
        //    GameObject leverBody = CmdBatchObject("LeverBody", MapEditor.Instance.dontSaveObjectTransform, pot);

        //    if (leverBody is not null)
        //        leverBody.GetComponent<LeverBodyNet>().CmdSetLinkDoor(pot, data.linkId);

        //}
        //todo 0425
        NetworkServer.Spawn(obj, NetworkServer.localConnection);

    }

    //todo 0425
    //private async Task<GameObject> GetTaskObj(string objName)
    //{
    //    return ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);
    //}


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
