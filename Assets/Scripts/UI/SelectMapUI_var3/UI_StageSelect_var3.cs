using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;


enum PrograssLevel
{
    One,
    Two,
    Three,
    End
}


//DEVELOP CODE LINE, Method 0909 
//  :Select_PrograssLevel_3Co
//  : 


public class UI_StageSelect_var3: UI_Base,IPointerEnterHandler,IPointerExitHandler
{

    [Header("Info")]
    [SerializeField] UI_KeyGenerator _UI_KeyGenerator;
    [SerializeField] float _WriteAndEraserDelayRate;
    [SerializeField] int maxHorizontaText;// 얼마나 써야 다음 줄로 넘어가는가.
    [SerializeField] int contentMoveRect_TextLineIndex; //Change content RectTransform.localPosition.  + 30;
    [SerializeField] int pathTextLineIndex;
    private string selectMapId;

    [Header("Components")]
    Util util = new Util();

    [Header("Transform")]
    [SerializeField] RectTransform _BGContainerRectTransform;
    [SerializeField] Transform content;
    RectTransform contentRectTransform;

    [Header("Text")]
    [SerializeField] GameObject textLine;
    private List<TextLine> textLineList;

    #region Text
    string openningSentence = @",Preparing spawn area: 91%
,Preparing start region for level 7 (Seed: 5437123091234567890)
,Preparing start region for level 8 (Seed: 9342938470123456789)
,Preparing start region for level 9 (Seed: 7219238471293857102)
,Preparing start region for level 10 (Seed: 6789054321092345678)
,Preparing spawn area: 92%
,Hopper Transfer: 8 Hopper Check: 10 Hopper Amount: 1
,Mob Spawn Range: 4
,Random Lighting Updates: false
,Structure Info Saving: true
,Cactus Growth Modifier: 120%
,Cane Growth Modifier: 120%
,Melon Growth Modifier: 120%
,Mushroom Growth Modifier: 120%
,Pumpkin Growth Modifier: 120%
,Sapling Growth Modifier: 120%
,Wheat Growth Modifier: 120%
,NetherWart Growth Modifier: 120%
,Vine Growth Modifier: 120%
,Cocoa Growth Modifier: 120%
,Tile Max Tick Time: 60ms Entity Max Tick Time: 60ms
,Preparing spawn area: 95%
,Preparing start region for level 11 (Seed: 5698234567012345678)
,Preparing start region for level 12 (Seed: 9876123456789012345)
,Preparing start region for level 13 (Seed: 1023456789123456789)
,Thanks for downloading SetSpawn!
,Enabling SetSpawn v2.2
,Enabling RaspberryJuice v1.8
,ThreadListener Started
,Preparing spawn area: 97%
,Preparing start region for level 14 (Seed: 6789234567890123456)
,Preparing start region for level 15 (Seed: 1234567890987654321)
,Preparing start region for level 16 (Seed: 9081726345012345678)
,Preparing start region for level 17 (Seed: 8712345678901234567)
,Thanks for downloading SetSpawn!
,http://dev.bukkit.org/bukkit-plugins/setspawn
,Enabling SetSpawn v2.3
,Enabling RaspberryJuice v1.9
,Mob Spawn Range: 5
,Preparing start region for level 18 (Seed: 1239874567890123456)
,Preparing start region for level 19 (Seed: 6578123456789012345)
,Preparing start region for level 20 (Seed: 4098172634567890123)
,Preparing start region for level 21 (Seed: 9847123456789012345)
,Preparing start region for level 22 (Seed: 8123456789012345678)
,Preparing start region for level 23 (Seed: 5678901234567890123)
,Preparing start region for level 24 (Seed: 1234567890123456789)
,Preparing start region for level 25 (Seed: 9081276345123456789)
,Preparing start region for level 26 (Seed: 6738123456789012345)
,Preparing start region for level 27 (Seed: 4527890123456789012)
,Preparing start region for level 28 (Seed: 7812345678901234567)
,Preparing start region for level 29 (Seed: 9012345678901234567)
,Preparing spawn area: 100%";

    string titleSentence = @"-------------------------------------------------------------------------
[Up - Up Arrow]      [Down - Down Arrow]   [Select - Enter]
[Back - Backspace]   [Q - Exit]
-------------------------------------------------------------------------";
    #endregion

    #region Mouse Pointer

    [Header("Mouse Pointer")]
    public Texture2D customCursorTexture;
    private UnityEngine.CursorMode cursorMode = UnityEngine.CursorMode.Auto;
    public Vector2 hotspot = new Vector2(-0.5f, 0.5f); // 클릭 기준점
    public bool isPointerInside = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerInside = true;
        Cursor.SetCursor(customCursorTexture, hotspot, cursorMode);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
        Cursor.SetCursor(null, Vector2.zero, cursorMode); // 기본 커서로 복귀
    }
    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0)) // 또는 항상 체크
    //    {
    //        PointerEventData pointerData = new PointerEventData(EventSystem.current)
    //        {
    //            position = Input.mousePosition
    //        };

    //        List<RaycastResult> results = new List<RaycastResult>();
    //        EventSystem.current.RaycastAll(pointerData, results);

    //        foreach (var result in results)
    //        {
    //            Debug.Log("Hit UI: " + result.gameObject.name);
    //        }
    //    }
    //}

    #endregion

    Color localColor = new Color(48f / 255f, 172f / 255f, 52f / 255f); 

    [Header("Stats")]
    [SerializeField] int maxTextLine;
    private TextLine curSelectTextLine; //현재 선택된 텍스트라인
    public int curSelectTextLineIndex; //현재 선택된 텍스트라인 인덱스

    private int nextWriteTextLineIndex; //다음에 쓸 Line Index
    private int minSelectTextLineListIndex; // 선택 가능한 라인 인덱스
    private int maxSelectTextLineListIndex; // 선택 가능한 라인 인덱스

    public bool onInteractable;
    public bool onPrograss;
    // private bool inputProcessed;
    public float inputDelay; // 입력 딜레이 시간 설정 

    PrograssLevel _PrograssLevel;


    //List<IEnumerator> _IEnumeratorList;
    
    public GameObject _UI_ComputerScreen;
    public GameObject computer;
    public Computer_Net Net
    {
        get
        {
            if (computer)
                return computer.TryGetComponent(out Computer_Net component) ? component : null;
            else return null;
        }
    }

    [Header("Animation")]
    [SerializeField] Animator animator;
    private readonly int OPEN = Animator.StringToHash("Open");
    private readonly int OPEN_ONPOWER = Animator.StringToHash("Open_onPower");
    private readonly int CLOSE = Animator.StringToHash("Close");

    [Header("Coroutine")]
    private Coroutine _PrograssCoroutine;
    // private Coroutine _TransformCoroutine;


    [Header("Map Info UI")]
    [SerializeField] MapInfo_UI mapInfo_UI;



    private void Awake()
    {
        contentRectTransform = content.transform as RectTransform;

        //TextLine Pooling
        textLineList = new();
        for (int i = 0; i < maxTextLine; i++)
        {
            TextLine newTextLine= Instantiate(textLine, content).GetComponent<TextLine>();
            newTextLine.index = i;
            newTextLine._UI_StageSelect_var3 = this;
            textLineList.Add(newTextLine);

        }
    }


    //------------------------------------------------------Network 250218
    public void GetHost_MapData(int stageLevel)
    {
        if (Managers.Data.mapData.mapMainStageDictionary.ContainsKey(stageLevel))
        {
            Map[] maps = Managers.Data.mapData.mapMainStageDictionary[stageLevel];
            Host_MapData[] mapDatas = CheckPlayerData(maps);
            // computer.GetComponent<Computer_Net>().Server_SetMapDatas(mapDatas);
            Net.Server_SetMapDatas(mapDatas);
        }
      
    }

    public void Reset()
    {
        Net.Server_ReadyAllClientReset();
    }
    public override void OnEnable()
    {

    }

    bool onReady;
    public void StartUi(uint computerId) //Serve
    {
        if (NetworkClient.spawned.TryGetValue(computerId, out NetworkIdentity foundObject))
        {
            computer = foundObject.gameObject;
        }
        if(computer == null) return;

        // if(!computer.GetComponent<Computer_Net>().onPower)animator.SetTrigger(OPEN);
        if(!Net.onPower)animator.SetTrigger(OPEN);
        else animator.SetTrigger(OPEN_ONPOWER);

        computer.GetComponent<StageSelectorComputer>().Talking();
        
    }

    
    public void SetInputKey(int num)
    {
        if(onInteractable && !onPrograss  && onReady)
        {
            StartCoroutine(ProcessInputWithDelay(num));
        }
    }
    /// <summary>
    /// 1 : Up, 
    /// 2 : Down, 
    /// 3 : Enter, 
    /// 4 : Backspace, 
    /// 5 : Q 
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public IEnumerator ProcessInputWithDelay(int num)
    {
        // inputProcessed = true;

        switch (num)
        {
            case 1:
                curSelectTextLineIndex++;
                SelectTextLine();
                break;
            case 2:
                curSelectTextLineIndex--;
                SelectTextLine();
                break;
            case 3:
                if (curSelectTextLine == null || !curSelectTextLine.onSelectable)
                    break;
                var mainSentence = textLineList[curSelectTextLineIndex].mainSentence;
                Debug.Log(mainSentence);
                if (mainSentence == "BACK" || mainSentence == "EXIT") 
                {
                    BackPrograss();
                    break;
                }


                switch (_PrograssLevel)
                {
                    case PrograssLevel.One:
                        //Select Sound
                        Managers.Sound.PlaySound(GlobalText.COMPUTER_SELECTMENU_SOUND_2);
                        //Select Sound

                        Select_PrograssLevel_1();
                        break;
                    case PrograssLevel.Two:
                        //Select Sound
                        Managers.Sound.PlaySound(GlobalText.COMPUTER_SELECTMENU_SOUND_2);
                        //Select Sound

                        Select_PrograssLevel_2();
                        break;
                    case PrograssLevel.Three:
                        //Select Sound
                        Managers.Sound.PlaySound(GlobalText.COMPUTER_SELECTMENU_EndSelect_Sound);
                        //Select Sound
                        if (GetMap(curSelectTextLine.mainSentence) == null) yield break;
                        textLineList[pathTextLineIndex].WriteText($"/{curSelectTextLine.mainSentence}");
                        _PrograssCoroutine = StartCoroutine(Select_PrograssLevel_3Co());
                        break;
                }
                
                
                break;
            case 4:
                BackPrograss();
                break;
            case 5:
                if(_PrograssLevel == PrograssLevel.Three)mapInfo_UI.Reset();
                Shutdown();
                break;
        }

        yield return new WaitForSeconds(inputDelay);
        // inputProcessed = false;
    }
    //------------------------------------------------------Network 250218

    public void UnSelectLine()
    {
        curSelectTextLine.UnSelectSentence();
        curSelectTextLineIndex = maxSelectTextLineListIndex;
    }
    public void SelectTextLine()
    {
        if(curSelectTextLineIndex < minSelectTextLineListIndex) //마지막 요소로
        {
            curSelectTextLineIndex = maxSelectTextLineListIndex;
        }
        else if(curSelectTextLineIndex > maxSelectTextLineListIndex)//첫번째 요소로
        {
            curSelectTextLineIndex = minSelectTextLineListIndex;
        }

        if(curSelectTextLineIndex >= contentMoveRect_TextLineIndex) //스크롤 아래로
        {
            ScrollingWirteLine(curSelectTextLineIndex);
        }
   
        if(curSelectTextLine != null)
        {
            curSelectTextLine.UnSelectSentence();
            curSelectTextLine = textLineList[curSelectTextLineIndex];
            curSelectTextLine.SelectSentence();

            if(_PrograssLevel == PrograssLevel.Three)
            {
                if (textLineList[curSelectTextLineIndex].mainSentence == "BACK")
                {
                    mapInfo_UI.Reset();
                    return;
                }
                // Debug.Log(curSelectTextLine.text.text);
                mapInfo_UI.ShowMapInfo(curSelectTextLine.text.text,curStageLevel);
            }
        }
        else
        {
            curSelectTextLine = textLineList[curSelectTextLineIndex];
            curSelectTextLine.SelectSentence();

            if (_PrograssLevel == PrograssLevel.Three)
            {
                if (textLineList[curSelectTextLineIndex].mainSentence == "BACK")
                {
                    mapInfo_UI.Reset();
                    return;
                }
                // Debug.Log(curSelectTextLine.text.text);
                mapInfo_UI.ShowMapInfo(curSelectTextLine.text.text, curStageLevel);
            }
        }
    }

    #region Write

    //Title

    IEnumerator WriteLine(string sentence, Color color, bool readAntWrite, float fontSize = 25, float delayTime = 0.01f, bool onSelectable = true)
    {
        if (textLineList[nextWriteTextLineIndex].CheckCompareString(sentence))
        {
            nextWriteTextLineIndex++;
            yield break;
        }
        if (nextWriteTextLineIndex >= contentMoveRect_TextLineIndex)
        {
            ScrollingWirteLine(nextWriteTextLineIndex);
        }
        TextLine textLine = textLineList[nextWriteTextLineIndex];
        nextWriteTextLineIndex++;

        yield return textLine.Task_WriteTyping(sentence, color, readAntWrite, fontSize, delayTime, onSelectable);

    }
    /// <summary>
    /// condition : lineIdx >= contentMoveRect_TextLineIndex
    /// </summary>
    /// <param name="lineIdx"></param>
    private void ScrollingWirteLine(int lineIdx){
        Vector2 position = contentRectTransform.localPosition;
        position.y = 30 * (lineIdx - contentMoveRect_TextLineIndex);
        contentRectTransform.localPosition = position;
    }

    #endregion

    #region Eraser

    IEnumerator EraserTextLineCo(int min,int max)
    {

        for (int i = max; i >= min; i--)
        {
            if (textLineList[i].type == TypingType.Read)
            {
                continue;
            }

            nextWriteTextLineIndex = i;
            
            yield return textLineList[i].Task_EraserText();
        }
    }


    private void EraserAllClear()
    {
        foreach(TextLine textLine in textLineList)
        {
            textLine.Clear();
        }

        if (curSelectTextLine != null) { curSelectTextLine.Reset(); curSelectTextLine = null; }
        
        nextWriteTextLineIndex = 0;
        minSelectTextLineListIndex = 0;
        maxSelectTextLineListIndex = 0;


        contentRectTransform.localPosition = Vector2.zero;

    }
    #endregion


#region  Openning
    private void OpenningTitle_() //Animator.event : Open
    {
        _PrograssLevel = PrograssLevel.One;
       _PrograssCoroutine = StartCoroutine(OpenningTitle());
    }
    private void OpenningTitle_OnPower() //Animator.event : Open_onPower
    {
        _PrograssCoroutine = StartCoroutine(WriteTextLineCo_Title(titleSentence,false));
    }
    
    IEnumerator OpenningTitle()
    {
        onPrograss = true;
        onInteractable = false;
        nextWriteTextLineIndex = 0;
        
        Managers.Sound.PlaySound(GlobalText.COMPUTER_ON_SOUND, 0.5f);
        List<string> sentenceList = util.SplitText(openningSentence, maxHorizontaText, new char[] { ',' });
        
        for (int i = 0; i < sentenceList.Count; i++)
        {
            nextWriteTextLineIndex++;
            if (nextWriteTextLineIndex >= contentMoveRect_TextLineIndex)
            {
                Vector2 position = contentRectTransform.localPosition;
                position.y = 30 * (nextWriteTextLineIndex - contentMoveRect_TextLineIndex);
                contentRectTransform.localPosition = position;
            }
            string text = $"[{DateTime.Now.ToString("HH:mm:ss")}]: {sentenceList[i]}";

            textLineList[i].WriteText(text, localColor);
            yield return new WaitForSeconds(_WriteAndEraserDelayRate);

        }

        yield return new WaitForSeconds(1);
        EraserAllClear();
        //-----------------------------------------Net
        // computer.GetComponent<Computer_Net>().onPower = true;
        Net.onPower = true;
        //-----------------------------------------Net
        yield return _PrograssCoroutine = StartCoroutine(WriteTextLineCo_Title(titleSentence,false));
        
    }
#endregion

#region Title
    IEnumerator WriteTextLineCo_Title(string sentence,bool back = true)
    {
        if(back){
            yield return EraserTextLineCo(minSelectTextLineListIndex,maxSelectTextLineListIndex);
        }

        onPrograss = true;
        onInteractable = false;
        nextWriteTextLineIndex = 0;
        _PrograssLevel = PrograssLevel.One;
        List<string> sentenceList = util.SplitText(sentence, maxHorizontaText, new char[] { '\n' });
        for (int i = 0; i < sentenceList.Count; i++)
        {
            yield return WriteLine(sentenceList[i], localColor, true,25,0.01f,false);
        }
        
        nextWriteTextLineIndex+=2;
        
        minSelectTextLineListIndex = nextWriteTextLineIndex;
        yield return WriteLine("Main", localColor, true,25,0.01f,true);
        yield return WriteLine("EXIT", Color.red, true, 25, 0.01f, true);


        // yield return WriteLine("UserMap (준비중)", localColor, true, 25, 0.01f, false);

        maxSelectTextLineListIndex = nextWriteTextLineIndex-1;
        curSelectTextLineIndex = maxSelectTextLineListIndex;

        //-----------------------------------------Net Ready
        // computer.GetComponent<Computer_Net>().Cmd_ReadyClient();
        Net.Cmd_ReadyClient();
        //-----------------------------------------Net Ready

        yield return new WaitForSeconds(0.5f);
        onInteractable = true;
        onPrograss = false;
        onReady = true;
        
    }
#endregion

#region Prograss 1
    void Select_PrograssLevel_1()
    {
        if (curSelectTextLine == null) return;
        if (!curSelectTextLine.onSelectable) return;

        curSelectTextLine.Reset(); 

        textLineList[pathTextLineIndex].WriteText($"/{curSelectTextLine.mainSentence}");
        curSelectTextLine = null;

      _PrograssCoroutine = StartCoroutine(Select_PrograssLevel_1Co());
    }

   

    void Select_PrograssLevel_1Back()
    {
        if(curSelectTextLine != null)
        {
            curSelectTextLine.Reset();
            curSelectTextLine = null;
        }
        _PrograssCoroutine = StartCoroutine(Select_PrograssLevel_1Co());
    }

    IEnumerator Select_PrograssLevel_1Co()
    {
        onPrograss = true;
        onInteractable = false;

        Net.onReady = false;

        if (GetSplitSentenceAndLaststring(textLineList[pathTextLineIndex].mainSentence) == "Main")
        {
            _PrograssLevel = PrograssLevel.Two;

            Net.Server_SetIndex(Managers.Data.saveData._SaveFileData._PlayerSaveData.curStageLevel);
            yield return EraserTextLineCo(minSelectTextLineListIndex, maxSelectTextLineListIndex);

            for (int i = 0; i <= Net.index; i++)
            {
                yield return WriteLine($"{i}", localColor, true);
            }

            
            Net.onReady = true;
        }
        else
        {
            //usermap Prograss
        }

        yield return WriteLine("BACK", Color.red, true, 25, 0.01f, true);

        maxSelectTextLineListIndex = nextWriteTextLineIndex - 1;
        curSelectTextLineIndex = nextWriteTextLineIndex;
        yield return new WaitForSeconds(0.5f);
        onPrograss = false;
        onInteractable = true;
    }
#endregion
#region  Prograss 2
    void Select_PrograssLevel_2()
    {
        int stageLevel = int.Parse(curSelectTextLine.mainSentence);
        GetHost_MapData(stageLevel);
        textLineList[pathTextLineIndex].WriteText($"/{curSelectTextLine.mainSentence}");

        curSelectTextLine.Reset();
        curSelectTextLine = null;

        _PrograssCoroutine = StartCoroutine(Select_PrograssLevel_2Co(stageLevel));
    }

    private int curStageLevel;
    IEnumerator Select_PrograssLevel_2Co(int stageLevel) //TODO 0805
    {
        onPrograss = true;
        onInteractable = false;
        curStageLevel = stageLevel;

        _PrograssLevel = PrograssLevel.Three;

        Net.onReady = false;

        yield return EraserTextLineCo(minSelectTextLineListIndex, maxSelectTextLineListIndex);

        Host_MapData[] mapDatas = Net.host_MapDatas;

        for (int i = 0; i < mapDatas.Length; i++)
        {
            if (mapDatas[i].clear)
            {
                yield return WriteLine(string.IsNullOrWhiteSpace(mapDatas[i].subMapName) ? mapDatas[i].mapId : mapDatas[i].subMapName, localColor, true);
            }
            else if (mapDatas[i].onOpenStage)
            {
                yield return WriteLine(string.IsNullOrWhiteSpace(mapDatas[i].subMapName) ? mapDatas[i].mapId : mapDatas[i].subMapName, Color.yellow, true);
            }
           
        }

        yield return WriteLine("BACK", Color.red, true, 25, 0.01f, true);

        maxSelectTextLineListIndex = nextWriteTextLineIndex-1;
        curSelectTextLineIndex = nextWriteTextLineIndex;
        yield return new WaitForSeconds(0.5f);

        Net.onReady = true;

        onPrograss = false;
        onInteractable = true;
    }


    private Map GetMap(string mapName)
    {
        if (!Managers.Data.mapData.mapMainStageDictionary.ContainsKey(curStageLevel)) return null;
        Map[] maps = Managers.Data.mapData.mapMainStageDictionary[curStageLevel];

        foreach(var map in maps)
        {
            if (map.mapID == mapName || map.subMapName == mapName) return map;

        }

        return null;

    }
    #endregion

    #region Prograss 3(End)

    IEnumerator Select_PrograssLevel_3Co()
    {
        onPrograss = true;
        onInteractable = false;
        _PrograssLevel = PrograssLevel.End;

        textLineList[pathTextLineIndex].type = TypingType.Read;
        textLineList[pathTextLineIndex].text.color = Color.yellow;

        mapInfo_UI.Reset();

        Net.onReady = false;

        try
        {
            ExitPointObj obj = MapEditor.Instance.FindObj(MapEditor.Instance.exitDoorObjectTransform, 301).GetComponent<ExitPointObj>();

            Map map = GetMap(curSelectTextLine.mainSentence);
            obj.nextMapId = map.mapID;
            obj.Net_SetNextMapId(map.mapID);

            selectMapId = map.mapID;
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
        }

        yield return EraserTextLineCo(minSelectTextLineListIndex,maxSelectTextLineListIndex);
        
        yield return EraserTextLineCo(0, minSelectTextLineListIndex);
        
        //Path Text Eraser Effect.
        textLineList[pathTextLineIndex].Clear();
        //Path Text Eraser Effect.

        textLineList[pathTextLineIndex].type = TypingType.Write;
     
        animator.SetTrigger(CLOSE);
       

        _UI_KeyGenerator.gameObject.SetActive(true);
        _UI_KeyGenerator.KeyPrintingAni();
        yield return new WaitForSeconds(6f);
        _UI_KeyGenerator.gameObject.SetActive(false);

        computer.GetComponent<StageSelectorComputer>().SpawnKey();


        //------------------------------------player Move control
        // var player = Managers.Game.Player.GetComponent<PlayerSM>();
        // player.canMovable = true;
        // player.canAction = true;
        // player.doNotTouch = false;
        FreezePlayerState(false);
        //------------------------------------player Move control
        //-----------------------------Reset
        Net.Server_SetIsOpen(false);
        Reset();
        //-----------------------------Reset

        onPrograss = false;

        onInteractable = true;
        gameObject.SetActive(false);
    }
    #endregion

    private void BackPrograss()
    {
        switch (_PrograssLevel)
        {
            case PrograssLevel.One:
                Shutdown();
                break;
            case PrograssLevel.Two:
               
                Managers.Sound.PlaySound(GlobalText.COMPUTER_SELECTMENU_SOUND_1);
                
                textLineList[pathTextLineIndex].WriteText("", localColor);
                StartCoroutine(WriteTextLineCo_Title(titleSentence));
                break;
            case PrograssLevel.Three:
                
                Managers.Sound.PlaySound(GlobalText.COMPUTER_SELECTMENU_SOUND_1);

                mapInfo_UI.Reset();
                string[] sentences = GetSplitSentenceAndLaststring();
                string newPath = $"/{sentences[1]}";
                textLineList[pathTextLineIndex].WriteText(newPath,localColor);
                Select_PrograssLevel_1Back();
                break;
        }
    }



    #region Shutdown

    public void Shutdown()
    {
        StartCoroutine(ShutdownCo());
    }

    IEnumerator ShutdownCo()
    {
        onPrograss = true;
        onInteractable = false;
        
        onReady = false;

        Reset();

        yield return EraserTextLineCo(0, maxSelectTextLineListIndex);
        animator.SetTrigger(CLOSE);
        yield return new WaitForSeconds(1f);
        //------------------------------------player Move control
        FreezePlayerState(false);
        //------------------------------------player Move control

        yield return new WaitForSeconds(1f);

        Net.Server_SetIsOpen(false);

        onPrograss = false;
        gameObject.SetActive(false);

    }

    private void FreezePlayerState(bool onOff)
    {
        var player = Managers.Game.Player.TryGetComponent(out PlayerSM sm) ? sm : null;
        if (player == null) return;

        sm.FreezePlayerState(onOff);
    }
    #endregion

    #region Util

    int CheckDirectory()
    {
        string path = Path.Combine(Application.dataPath, "Resources/MapDat/Main");
        int index = 0;
        while (true)
        {
            if (Directory.Exists(Path.Combine(path, index.ToString())))
            {
                index++;

            }
            else { break; }

        }
        return index - 1;
    }

    private string GetSplitSentenceAndLaststring(string sentence)
    {
       string[] sentences = sentence.Split("/", StringSplitOptions.None);
    
        return sentences[sentences.Length - 1];

    }

    private string[] GetSplitSentenceAndLaststring()
    {
        string[] sentences = textLineList[pathTextLineIndex].mainSentence.Split("/", StringSplitOptions.None);

        return sentences;

    }

    private Host_MapData[] CheckPlayerData(Map[] map) 
    {
        //MapSaveData[] array = new MapSaveData[map.Length];
        Host_MapData[] array = new Host_MapData[map.Length];

        for (int i = 0; i < map.Length; i++)
        {
            //array[i] = Managers.Data.saveData.dic[map[i].mapID];
            var info = Managers.Data.saveData.dic[map[i].mapID];      
            array[i] = new Host_MapData(info.mapName,info.mapSubName, info.clear, info.openStage);
        }

        if (map[0].stageLevel == 0)
        {
            array[0] = new Host_MapData(array[0].mapId, array[0].subMapName, array[0].clear, true);
            // Managers.Data.saveData._SaveFileData
        }
  
        return array;

    }


    #endregion
    /// <summary>
    /// 호스트가 상호작용중일때는 움직이지 못하게,
    /// 다른 클라이언트가 접근하면 유아이 켜지
    /// </summary>
    #region Hide And Open UI
    public void HideUIOutsideCamera()
    {
        float uiWidth = 1920f;
        float uiHeight = 1080f;

        Camera mainCamera = Camera.main;

        float screenWidth = mainCamera.pixelWidth;
        float screenHeight = mainCamera.pixelHeight;

        // Convert UI size to viewport size
        float uiViewportWidth = uiWidth / screenWidth;
        float uiViewportHeight = uiHeight / screenHeight;

        // Move the UI element to be outside the camera view
        _BGContainerRectTransform.anchorMin = new Vector2(1 + uiViewportWidth, 1 + uiViewportHeight);
        _BGContainerRectTransform.anchorMax = new Vector2(1 + uiViewportWidth, 1 + uiViewportHeight);
        
    }


    public void OpenUIOutsideCamera()
    {
        _BGContainerRectTransform.anchorMin = new Vector2(.5f,.5f);
        _BGContainerRectTransform.anchorMax = new Vector2(.5f, .5f);
    }
    #endregion


}
