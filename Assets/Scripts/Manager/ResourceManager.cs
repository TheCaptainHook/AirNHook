using UnityEngine;

public class ResourceManager
{
    private static T Load<T>(string path) where T : UnityEngine.Object
    {
        //TODO 오브젝트 풀 사용시 코드 추가.

        return Resources.Load<T>(path);
    }
    
    public static GameObject Instantiate(string path, Transform parent = null)
    {
        var origin = Load<GameObject>(path);

        if (origin == null)
        {
            return null;
        }

        //TODO 오브젝트 풀 사용시 코드 추가.

        var go = Object.Instantiate(origin, parent);
        go.name = origin.name;

        return go;
    }

    public static GameObject Instantiate(GameObject gameObject, Transform parent = null)
    {
        //TODO 오브젝트 풀 사용시 코드 추가.
        
        var go = Object.Instantiate(gameObject, parent);
        go.name = gameObject.name;

        return go;
    }
    
    public static void Destroy(GameObject go)
    {
        if (go == null)
            return;

        //TODO 오브젝트 풀 사용시 코드 추가.

        Object.Destroy(go);
    }
}
