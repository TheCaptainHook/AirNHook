using System;
using System.Collections;
using UnityEngine;

public class OnGUITest : MonoBehaviour
{
    private void OnGUI()
    {
        var i = 0;
        foreach (var value in Managers.Data.mapData.mapMainDictionary.Values)
        {
            GUI.Box(new Rect (10,10 + i * 10,100,50), new GUIContent(value.mapID));
            i++;
        }
    }
}
