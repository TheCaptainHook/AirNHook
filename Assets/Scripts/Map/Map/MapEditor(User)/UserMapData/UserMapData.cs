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


    public Sprite LoadImage(int width,int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        texture.LoadImage(mapImage);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
        return sprite;
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

    // 만들어진 맵 먼저 데이터화
    //스크린샷 찍어서 이미지 바이트화
    //
    // string userMapDataJson = JsonUtility.ToJson(new UserMapData(맵데이, 맵이미지 바이트,new DateTimeDate(DateTime.Now)),true);
}
