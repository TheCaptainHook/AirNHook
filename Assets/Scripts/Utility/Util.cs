
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class Util
{

#region  Text

    public List<string> SplitText(string text, int length, char[] delimiters)
    {
        List<string> result = new List<string>();

        string[] parts = text.Split(delimiters, StringSplitOptions.None);

        foreach (string part in parts)
        {
            for (int i = 0; i < part.Length; i += length)
            {
                if (i + length <= part.Length)
                {
                    result.Add(part.Substring(i, length));
                }
                else
                {
                    result.Add(part.Substring(i));
                }
            }
        }

        return result;
    }

#endregion

#region  Mouse

    public Vector3 GetMouseWorldPosition(Vector3 screenPosition, Camera camera)
    {
        Vector3 worldPosition = camera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0;
        return worldPosition;
    }

#endregion


#region Transform
    public Transform CreateChildTransform(Transform parent, string name)
    {
        if (parent.Find(name) != null)
        {
           UnityEngine.Object.Destroy(parent.Find(name).gameObject);
        }

        GameObject childObject = new GameObject(name);
        Transform childTransform = childObject.transform;
        childTransform.SetParent(parent);

        return childTransform;

    }
    public Transform CreateChildTransform( string name)
    {
        GameObject childObject = new GameObject(name);
        Transform childTransform = childObject.transform;
        return childTransform;
    }


    #endregion


#region Date

    #endregion

    public async Task Delay(Action action, int delayTime = 1500)
    {
        await Task.Delay(delayTime);
        action?.Invoke();

    }


}
