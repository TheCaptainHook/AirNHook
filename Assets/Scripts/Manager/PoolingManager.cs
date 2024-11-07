
using System.Collections.Generic;
using UnityEngine;
using System;

public class PoolingManager
{
    public Dictionary<string, object> poolingDic;
   

    public GameObject GetItme<T>() where T : class
    {
        if (!poolingDic.ContainsKey(typeof(T).Name))
        {
            poolingDic[typeof(T).Name] = new Pool<T>(CreateTransform<T>());
        }
        try
        {
           Pool<T> pool = poolingDic[typeof(T).Name] as Pool<T>;
           return pool.GetItem();
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
            return null;
        }
    }
    public T GetItme_T<T>() where T : class
    {
        if (!poolingDic.ContainsKey(typeof(T).Name))
        {
            poolingDic[typeof(T).Name] = new Pool<T>(CreateTransform<T>());
        }
        try
        {
            Pool<T> pool = poolingDic[typeof(T).Name] as Pool<T>;
            return pool.GetItem().GetComponent<T>();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            return default;
        }
    }

    public void ReleaseToPool<T>(GameObject obj) where T : class
    {
        try
        {
            Pool<T> pool = poolingDic[typeof(T).Name] as Pool<T>;
            pool.Enqueue(obj);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public void Setup()
    {
        poolingDic = new();
    }


    private Transform CreateTransform<T>()
    {
        GameObject obj = new GameObject(typeof(T).Name);
        obj.transform.SetParent(Managers.Instance.gameObject.transform);
        return obj.transform;

    }
}

public class Pool<T> where T : class
{
    public Queue<GameObject> queue;
    public Transform parents;

    public Pool(Transform parents)
    {
        queue = new();
        this.parents = parents;
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

    private void Create(int amount = 5)
    {
        for(int i =0; i< amount; i++)
        {
            GameObject obj = Managers.Stage.CmdBatchObject(typeof(T).Name);
            obj.SetActive(false);
            obj.transform.SetParent(parents);
            queue.Enqueue(obj);
        }
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
