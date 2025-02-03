using Mirror;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.VisualScripting;

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

    //[Command]
    //public void CmdBatchObject<T>(string objName,T data,Transform tr)
    //{
    //    if (!NetworkServer.active || !NetworkClient.isConnected) return;

    //    GameObject obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);

    //    obj.GetComponent<BuildObj>().SetData(data);
    //    obj.transform.SetParent(tr);

    //    NetworkServer.Spawn(obj, NetworkServer.localConnection);
    //}

    Dictionary<string, List<uint>> dic;

    [Server]
    public void SetDic(GameObject obj,string trName)
    {
        if (dic == null) dic = new();
        if(!dic.ContainsKey(trName)) dic[trName] = new List<uint>();

        if(obj.TryGetComponent(out NetworkIdentity component))
        {
            uint netId = component.netId;
            dic[trName].Add(netId);
        }

    }

    [Server]
    public void Clear_Dic()
    {
        dic.Clear();
    }

    [Command]
    public void CmdBatchObject<T>(string objName, T data, string trName)
    {
        if (!NetworkServer.active || !NetworkClient.isConnected) return;

        GameObject obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);

        obj.name = objName;

        NetworkServer.Spawn(obj, NetworkServer.localConnection);

        SetDic(obj, trName);

        obj.GetComponent<BuildObj>().SetData(data);


        Transform parent = null;
        foreach (Transform tr in MapEditor.Instance.mapObjBoxTransform)
        {
            if (tr.name == trName)
            {
                parent = tr;
                break;
            }
        }
        if (parent != null)
            obj.transform.SetParent(parent);

    }


    public void NetworkObject_SetParent()
    {

        Debug.Log("Set parent");
        foreach (var item in dic )
        {         
            Transform  parent = GetMapEditorTransform(item.Key);
            foreach(uint id in item.Value)
            {
                Transform tr = GetNetworkIdentity(id).gameObject.transform;
                tr.SetParent(parent);
            }
        }

    }
    
    private Transform GetMapEditorTransform(string trName)
    {
        foreach (Transform tr in MapEditor.Instance.mapObjBoxTransform)
        {
            if (tr.name == trName)
            {
                return tr;
            }
        }
        return null;
    }
    private NetworkIdentity GetNetworkIdentity(uint id)
    {
        return NetworkClient.spawned.TryGetValue(id,out NetworkIdentity identity) ? identity : null;
    }

    // [ClientRpc]
    // private void Rpc_SetTransformParents(uint netId,string trName)
    // {
    //     NetworkClient.spawned.TryGetValue(netId,out NetworkIdentity identity);
    //     if(identity == null) return;

    //     foreach(Transform tr in MapEditor.Instance.mapObjBoxTransform)
    //     {
    //         if(tr.name == trName)
    //         {
    //             identity.gameObject.transform.SetParent(tr);
    //             return;
    //         }
    //     }
    // }


    // [ClientRpc]
    // private void Create<T>(string objName,T data,uint trId)
    // {
    //     GameObject obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);
    //     obj.GetComponent<BuildObj>().SetData(data);

    //     Transform parent = null;
    //     foreach (Transform tr in MapEditor.Instance.mapObjBoxTransform)
    //     {
    //         if (tr.GetComponent<NetworkIdentity>().netId == trId)
    //         {
    //             parent = tr;
    //             break;
    //         }
    //     }
    //     if (parent != null)
    //         obj.transform.SetParent(parent);

    //     NetworkServer.Spawn(obj, NetworkServer.localConnection);
    // }

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
