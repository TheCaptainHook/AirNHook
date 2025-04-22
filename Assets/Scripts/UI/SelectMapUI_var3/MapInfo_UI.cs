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
    [SerializeField] TextMeshProUGUI text;

    // private WaitForSeconds wait;

    // private void Awake()
    // {
    //     wait = new WaitForSeconds(0.2f);
    // }
    

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
    #region  Difficult,Map Image Setting
    //Stage Diifficulty gauge item
    [Header("Difficulty")]
    [SerializeField] Sprite[] stageDifficultyItems;
    [SerializeField] Image difficultyImage;

    [Space(20)]
    [Header("Map Image")]
    [ReadOnly]
    [SerializeField] Image mapImage;    
    private void SetDifficultyImage(int stageDifficulty)
    {
        int level = Mathf.Clamp(stageDifficulty,0,3);
        difficultyImage.sprite = stageDifficultyItems[level];
    }
    IEnumerator SetImageAndDifficultyDelay(string mapId,int stageDifficulty)
    {
        // StartCoroutine(StageDifficultyGaugeCoroutine(stageDifficulty));
        SetDifficultyImage(stageDifficulty);
        yield return new WaitForSeconds(0.5f);
        SetMapImage(mapId);
    }

    private Map GetMap(string mapName,int curStageLevel)
    {
        if (!Managers.Data.mapData.mapMainStageDictionary.ContainsKey(curStageLevel)) return null ;

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
    private void SetMapImage(string mapId)
    {
        string path = $"Prefabs/MapScreenShot/{mapId}";
        Sprite sprit = Resources.Load<Sprite>(path);
         mapImage.sprite = sprit;
    }

    #endregion
   


    Coroutine eraser_Co;
    //Animation trigger
    private void TypingEffect_Eraser()
    {
        if(eraser_Co != null) StopCoroutine(eraser_Co);
        eraser_Co = StartCoroutine(typingEffect.NormalEraser(text,5));
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
   
}
