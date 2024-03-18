using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UGS;
using UnityEngine;

public class MapData
{
    public Dictionary<string, Map> mapDictionary = new Dictionary<string, Map>();

    public void SetUp()
    {
        foreach (TextAsset json in Resources.LoadAll<TextAsset>("MapDat"))
        {
            Map map = JsonUtility.FromJson<Map>(json.text);
            mapDictionary.Add(map.mapID, map);
            Debug.Log(map.mapID);
        }
    }
}
