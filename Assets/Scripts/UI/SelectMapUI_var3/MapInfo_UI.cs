using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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


    private WaitForSeconds wait;

    private void Awake()
    {
        wait = new WaitForSeconds(0.2f);
    }
    //Stage Diifficulty gauge item
    [SerializeField] List<GameObject> stageDifficultyGaugeItemList;
    IEnumerator StageDifficultyGaugeCoroutine(int stageDifficulty)
    {
        StageDifficultyGaugeReset();
        if(stageDifficulty == 0) yield break;

        for(int i = 0; i< stageDifficulty;i++)
        {
            var item = stageDifficultyGaugeItemList[i];
            item.SetActive(true);
            yield return wait;
        }
    }
    private void StageDifficultyGaugeReset()
    {
        foreach(var item in stageDifficultyGaugeItemList)
        {
            if(item.activeSelf)
            item.SetActive(false);
        }
    }

    Color localColor = new Color(48f / 255f, 172f / 255f, 52f / 255f);

    public void Reset()
    {
        if(isOpen)
        {
            
            StopAllCoroutines();
            // animator.SetBool(Open, false);
            TypingEffect_Eraser();
            animator.SetTrigger(Close);
            // mapImage.sprite = null;
            // text.text = string.Empty;
            isOpen = false;
        }
        
    }

    bool isOpen;
    Coroutine typingCoroutine;
    public void ShowMapInfo(string mapName,int curStageLevel)
    {
        if(eraser_Co != null) StopCoroutine(eraser_Co);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        text.text = string.Empty;

        string id = mapName.Replace(">", "").Trim();
        Map map = GetMap(id,curStageLevel);
        if(map == null) return;

        animator.SetTrigger(Open);
        
        //Stage Image, Stage Difficulty
        StartCoroutine(SetImageAndDifficultyDelay(map.mapID,map.stageDifficulty));
        //Stage Image, Stage Difficulty

        isOpen = true;

        MapSaveData data = GetMapSaveData(map.mapID);
        

        string name = map.subMapName == string.Empty ? map.mapID : map.subMapName;

        string sentence = $"{name}\n\n Collect : {GetCollectableCount(data)}\n\nRecently : {data.recentlyClearTime}\nShort : {data.shortestClearTime}\nDeath : {data.deathCount}";
 
        //open anim
        // animator.SetBool(Open, true);
        

        typingCoroutine = StartCoroutine(TypingEffect.NormalTyping(text,sentence,localColor,1));

    }
    IEnumerator SetImageAndDifficultyDelay(string mapId,int stageDifficulty)
    {
        StartCoroutine(StageDifficultyGaugeCoroutine(stageDifficulty));
        yield return new WaitForSeconds(0.2f);
        SetMapImage(mapId);


    }

    Coroutine eraser_Co;
    //Animation trigger
    private void TypingEffect_Eraser()
    {
        if(eraser_Co != null) StopCoroutine(eraser_Co);
        eraser_Co = StartCoroutine(typingEffect.NormalEraser(text,5));
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
