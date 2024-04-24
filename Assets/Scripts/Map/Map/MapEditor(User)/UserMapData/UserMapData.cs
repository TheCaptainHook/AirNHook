using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class UserMapData
{
    public string mapDataJson;
    public string dateTimeDataJson;
    public byte[] mapImage;
    public int hashValue;

    public UserMapData(string mapDataJson,byte[] mapImage,string dateTimeDataJson,int hashValue)
    {
        this.mapDataJson = mapDataJson;
        this.mapImage = mapImage;
        this.dateTimeDataJson = dateTimeDataJson;
        this.hashValue = hashValue;
    }



    public Map LoadMap()
    {
        return JsonUtility.FromJson<Map>(mapDataJson);
    }

    public string GetMapId()
    {
        return JsonUtility.FromJson<Map>(mapDataJson).mapID;
    }

    public DateTimeData LoadDateTimeData()
    {
        return JsonUtility.FromJson<DateTimeData>(dateTimeDataJson);
    }

    
}
