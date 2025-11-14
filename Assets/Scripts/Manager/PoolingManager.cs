
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using Mirror;


public class PoolingManager
{
    public Dictionary<string, object> N_Dic;
    public Dictionary<string, object> D_Dic;


    public void Setup()
    {
        N_Dic = new();
        D_Dic = new();
    }


    #region  Default Pooling
    public GameObject D_GetItem(GameObject prefab) 
    {
        if(!D_Dic.ContainsKey(prefab.name))
        {
            D_Dic[prefab.name] = new D_Pooling(prefab, CreateParentTransform(prefab)); 
        }

        D_Pooling pooling = D_Dic[prefab.name] as D_Pooling;
        return pooling.GetItem();
    }
    public void D_ReleaseToPool(GameObject obj)
    {
        D_Pooling pooling = D_Dic[obj.name] as D_Pooling;
        pooling.Enqueue(obj);
    }
    #endregion 

    #region  NetWork Pooling [Server]
    
    // public GameObject N_GetItme<T>() where T : class
    // {
    //     if (!N_Dic.ContainsKey(typeof(T).Name))
    //     {
    //         N_Dic[typeof(T).Name] = new N_Pool<T>(CreateTransform<T>());
    //     }
    //     try
    //     {
    //        N_Pool<T> pool = N_Dic[typeof(T).Name] as N_Pool<T>;
    //        return pool.GetItem();
    //     }
    //     catch(Exception ex)
    //     {
    //         Debug.Log(ex);
    //         return null;
    //     }
    // }
    public GameObject N_GetItme(string name)
    {
        if (!N_Dic.ContainsKey(name))
        {
            N_Dic[name] = new N_Pool(name, CreateTransform(name));
        }
        
        try
        {
           N_Pool pool = N_Dic[name] as N_Pool;
           return pool.GetItem();
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
            return null;
        }
    }
    

    // public void N_ReleaseToPool<T>(GameObject obj) where T : class
    // {
    //     try
    //     {
    //         N_Pool<T> pool = N_Dic[typeof(T).Name] as N_Pool<T>;
    //         pool.Enqueue(obj);
    //     }
    //     catch (Exception ex)
    //     {
    //         Debug.LogError(ex);
    //     }
    // }
    public void N_ReleaseToPool(GameObject obj) 
    {
        try
        {
            N_Pool pool = N_Dic[obj.name] as N_Pool;
            pool.Enqueue(obj);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
    #endregion


    private Transform CreateParentTransform(GameObject obj)
    {
        GameObject Ob = new GameObject(obj.name);
        Ob.transform.SetParent(Managers.Instance.gameObject.transform);
        return Ob.transform;

    }
    private Transform CreateTransform<T>()
    {
        GameObject obj = new GameObject(typeof(T).Name);
        obj.transform.SetParent(Managers.Instance.gameObject.transform);
        return obj.transform;
    }
    private Transform CreateTransform(string name)
    {
         GameObject obj = new GameObject(name);
        obj.transform.SetParent(Managers.Instance.gameObject.transform);
        return obj.transform;
    }
}

#region Default
public class D_Pooling
{
    GameObject prefab;
    Transform parents;
    Queue<GameObject> queue;
    public D_Pooling(GameObject prefab, Transform parents)
    {
        this.prefab = prefab;
        this.parents = parents;
        queue = new();
    }

    public GameObject GetItem()
    {
        if (IsEmpty())
        {
            Create();
        }
        return queue.Dequeue();
    }
    private void Create()
    {
        GameObject obj = Object.Instantiate(this.prefab);
        obj.name = this.prefab.name;
        obj.SetActive(false);
        obj.transform.SetParent(parents);
        queue.Enqueue(obj);
    }

    public void Enqueue(GameObject obj)
    {
        obj.transform.SetParent(parents);
        obj.SetActive(false);
        queue.Enqueue(obj);
    }
    public bool IsEmpty()
    {
        return queue.Count == 0;
    }

}
#endregion

#region  NetWork

public class N_Pool
{
    // string name;
    public string name;
    public Queue<GameObject> queue;
    public Transform parents;

    public N_Pool(string name ,Transform parents)
    {
        this.name = name;
        this.parents = parents;
        queue = new();
    }

    public GameObject GetItem()
    {
        if (IsEmpty())
        {
            Create();
        }

        GameObject obj = queue.Dequeue();
        return obj;
    }

    private void Create()
    {
        GameObject obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[name]);
        NetworkServer.Spawn(obj, NetworkServer.localConnection);
        // var obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[name]);
        // // GameObject obj = Object.Instantiate(this.prefab);
        obj.name = name;
        obj.SetActive(false);
        obj.transform.SetParent(parents);
        queue.Enqueue(obj);
    }
    

    public void Enqueue(GameObject obj)
    {
        obj.transform.SetParent(parents);
        obj.SetActive(false);
        queue.Enqueue(obj);
    }

    

    private bool IsEmpty()
    {
        return queue.Count == 0;
    }

}
#endregion