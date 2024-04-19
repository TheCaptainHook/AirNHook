using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserMapData
{
    public string mapDataJson;
    public string dateTimeDataJson;
    public byte[] mapImage;

    public UserMapData(string mapDataJson,byte[] mapImage,string dateTimeDataJson)
    {
        this.mapDataJson = mapDataJson;
        this.mapImage = mapImage;
        this.dateTimeDataJson = dateTimeDataJson;
    }


    public Texture2D LoadImage(int width,int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        texture.LoadImage(mapImage);
        return texture;
    }


    public Map LoadMap(string mapDataJson)
    {
        return JsonUtility.FromJson<Map>(mapDataJson);
    }


    public DateTimeData LoadDateTimeData()
    {
        return JsonUtility.FromJson<DateTimeData>(dateTimeDataJson);
    }

    // 만들어진 맵 먼저 데이터화
    //스크린샷 찍어서 이미지 바이트화
    //
    // string userMapDataJson = JsonUtility.ToJson(new UserMapData(맵데이, 맵이미지 바이트,new DateTimeDate(DateTime.Now)),true);
}
