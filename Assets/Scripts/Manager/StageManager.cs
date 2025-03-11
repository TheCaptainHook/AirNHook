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



    [Server]
    public void CmdBatchObject<T>(string objName, T data, Transform parent)
    {
        if (!NetworkServer.active ) return;

        GameObject obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);

        obj.name = objName;
        obj.transform.SetParent(parent);

        NetworkServer.Spawn(obj, NetworkServer.localConnection);
        obj.GetComponent<BuildObj>().SetData(data);

      
        // Transform parent = null;
        // foreach (Transform tr in MapEditor.Instance.mapObjBoxTransform)
        // {
        //     if (tr.name == trName)
        //     {
        //         parent = tr;
        //         break;
        //     }
        // }
        // if (parent != null)


    }


    //[Command]
    //public void Server_SetParent()
    //{
    //    foreach (var item in dic)
    //    {
    //        //Transform parent = GetMapEditorTransform(item.Key);
    //        Debug.Log($"parent {GetMapEditorTransform(item.Key)}");
    //        foreach (uint id in item.Value)
    //        {
    //            //Transform tr = GetNetworkIdentity(id).gameObject.transform;
    //            //tr.SetParent(parent);
    //            Debug.Log($"id : {id}");
    //        }
    //    }
    //}
    //[ClientRpc]
    //private void Rpc_SetParent(SyncDictionary<string, SyncList<uint>> dic)
    //{
    //    foreach (var item in dic)
    //    {
    //        //Transform parent = GetMapEditorTransform(item.Key);
    //        Debug.Log($"parent {GetMapEditorTransform(item.Key)}");
    //        foreach (uint id in item.Value)
    //        {
    //            //Transform tr = GetNetworkIdentity(id).gameObject.transform;
    //            //tr.SetParent(parent);
    //            Debug.Log($"id : {id}");
    //        }
    //    }
    //}

    //[Command]
    //public void NetworkObject_SetParent()
    //{
    //    Server_SetParent();
    //}
    
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

    [Server]
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
