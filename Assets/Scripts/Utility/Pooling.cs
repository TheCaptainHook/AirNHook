using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Pooling : MonoBehaviour
{
    [System.Serializable]
    public struct Pool
    {
        public string name;
        public GameObject prefeb;
        public int amount;
    }

    public List<Pool> poolList = new List<Pool>();
    Dictionary<string, Queue<GameObject>> poolDic;

    public void CreatePoolItem(Transform transform)
    {
        poolDic = new Dictionary<string, Queue<GameObject>>();
        foreach(var pool in poolList)
        {
            Queue<GameObject> objPool = new Queue<GameObject>();

            for (int i = 0; i < pool.amount; i++)
            {
                GameObject obj = Instantiate(pool.prefeb,transform.position,Quaternion.identity);
                obj.transform.SetParent(transform);
                obj.SetActive(false);
                objPool.Enqueue(obj);
            }
            poolDic.Add(pool.name, objPool);
        }
    }

    public GameObject GetPoolItem(string name)
    {
        if (poolDic.ContainsKey(name))
        {
            GameObject obj = poolDic[name].Dequeue();
            poolDic[name].Enqueue(obj);
            return obj;
        }

        return null;
    }

    public void Destroy(GameObject obj,float time)
    {
        StartCoroutine(DestroyCo(obj,time));
    }


    IEnumerator DestroyCo(GameObject obj,float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }


    public void AllDestroy(string name)
    {
        foreach(GameObject obj in poolDic[name])
        {
            obj.SetActive(false);
        }
    }
}





