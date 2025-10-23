using Mirror;
using UnityEngine;



public class StageManager
{
    //public int stage;
    // TODO 로비 이름으로 변경
    public string stageName = GlobalText.LOBBY;
    
    public void LoadMap() 
    {
   
        MapEditor.Instance.LoadMap(GlobalText.LOBBY);
   
    }

    #region Editor


    // [Server]
    // public void CmdBatchObject<T>(string objName, T data, Transform parent)
    // {
    //     if (!NetworkServer.active) return;

    //     GameObject obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);

    //     obj.name = objName;
    //     obj.transform.SetParent(parent);
    //     NetworkServer.Spawn(obj, NetworkServer.localConnection);

    //     obj.GetComponent<BuildObj>().SetData(data);


    //     //
    //     if (obj.TryGetComponent(out NetworkIdentity identity))
    //     {
    //         Rpc_PoolingSetting(identity.netId);
    //     }
    // }
    //======================================= Refectoring 1018
    [Server]
    public void ServerBatchObject<T>(string objName, T data, TransformType trType)
    {
        if (!NetworkServer.active) return;

        //====Get Pooling
        GameObject obj = Managers.Pooling.N_GetItme(objName);
        if (obj.TryGetComponent(out BuildObj buildObj))
        {
            obj.SetActive(true);
            buildObj.SetData(data);
        }
        if (MapEditor.Instance.GetTransformByType(trType, out Transform parent))
        {
            obj.transform.SetParent(parent);
        }
        
        // MapEditor.Instance._n_activePoolingObject.Enqueue(identity.GetComponent<BuildObj>());
        // if(GetNetworkIdentity())

        if (GetNetworkIdentity(obj, out NetworkIdentity identity))
        {
            MapEditor.Instance._n_activePoolingObject.Enqueue(buildObj);
            Rpc_PoolingSetting(identity.netId, trType);
        }
        // GameObject obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[objName]);
        // GameObject obj = ResourceManager.Load<GameObject>();
        // obj.name = objName;
        // //====Get Pooling

        // NetworkServer.Spawn(obj, NetworkServer.localConnection);
        // obj.GetComponent<BuildObj>().SetData(data);


        //====Set Transform, Rpc Set n_activePoolingObject
        // if (obj.TryGetComponent(out NetworkIdentity identity))
        // {
        //     Rpc_PoolingSetting(identity.netId, trType);
        // }
        //====Set Transform
    }
    /**
    1. 서버에서 N_Dic 확인 후 없으면 생성. 서버만 풀링함
        -> Rpc로 net id 전달, 위치만 세팅.

    2. 서버전용 releaseToPool 함수 필요.
    **/

    
    //======================================= Refectoring 1018
    [ClientRpc]
    private void Rpc_PoolingSetting(uint id, TransformType trType)
    {
        if (NetworkServer.active) return;
        
        if (GetNetworkIdentity(id, out NetworkIdentity identity))
        {
            if (MapEditor.Instance.GetTransformByType(trType, out Transform parent))
            {
                identity.transform.SetParent(parent);
            } 
            
            identity.gameObject.SetActive(true);
            MapEditor.Instance._n_activePoolingObject.Enqueue(identity.GetComponent<BuildObj>());
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
    private bool GetNetworkIdentity(uint id,out NetworkIdentity identity)
    {
        identity =  NetworkClient.spawned.TryGetValue(id,out NetworkIdentity iden) ? iden : null;
        return identity != null;
    }
    private bool GetNetworkIdentity(GameObject obj, out NetworkIdentity identity)
    {
        identity = obj.TryGetComponent(out NetworkIdentity iden) ? iden : null;
        return identity != null;
    }
    
    

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
