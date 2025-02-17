using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapInfo_UI : MonoBehaviour
{
    private TypingEffect typingEffect;
    private TypingEffect TypingEffect
    {
        get
        {
            if(typingEffect == null) typingEffect = GetComponent<TypingEffect>();
            return typingEffect;
        }
    }

    private readonly int Open = Animator.StringToHash("Open");
    private readonly int Close = Animator.StringToHash("Close");


    [SerializeField] Animator animator;


    [SerializeField] Image mapImage;
    [SerializeField] TextMeshProUGUI text;


    Color localColor = new Color(48f / 255f, 172f / 255f, 52f / 255f);

    public void Reset()
    {
        mapImage.sprite = null;
        StopAllCoroutines();
        animator.SetBool(Open, false);
        text.text = string.Empty;
    }


    Coroutine typingCoroutine;
    public void ShowMapInfo(string mapName,int curStageLevel)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        text.text = string.Empty;

        string id = mapName.Replace(">", "").Trim();
        Map map = GetMap(id,curStageLevel);
        if(map == null) return;


        MapSaveData data = GetMapSaveData(map.mapID);
        SetMapImage(map.mapID);

        string name = map.subMapName == string.Empty ? map.mapID : map.subMapName;

        string sentence = $"{name}\n\n Collect : {GetCollectableCount(data)}\n\nRecently : {data.recentlyClearTime}\nShort : {data.shortestClearTime}\nDeath : {data.deathCount}";
 
        //open anim
        animator.SetBool(Open, true);

        typingCoroutine = StartCoroutine(TypingEffect.NormalTyping(text,sentence,localColor,1));

    }


    private Map GetMap(string mapName,int curStageLevel)
    {
        Map[] maps = Managers.Data.mapData.mapMainStageDictionary[curStageLevel];
        //Map map;
        foreach (var map in maps)
        {
            if (map.mapID == mapName || map.subMapName == mapName) return map;

        }

        return null;
    }

    
    private MapSaveData GetMapSaveData(string mapId)
    {
        return Managers.Data.saveData.dic[mapId];
    }

    private int GetCollectableCount(MapSaveData data)
    {
        var list = data._CollectableObjectStructList;
        int num = 0;
        foreach (var dat in list)
        {
            if (!dat.isFound) num++;
        }
        return num;
    }
    private void SetMapImage(string mapId)
    {
        string path = $"Prefabs/MapScreenShot/{mapId}";
        Sprite sprit = Resources.Load<Sprite>(path);
         mapImage.sprite = sprit;
    }
}
