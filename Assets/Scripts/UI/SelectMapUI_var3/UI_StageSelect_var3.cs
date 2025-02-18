using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Mirror;


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


public class UI_StageSelect_var3: UI_Base
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
    private bool inputProcessed;
    public float inputDelay; // 입력 딜레이 시간 설정 

    PrograssLevel _PrograssLevel;


    //List<IEnumerator> _IEnumeratorList;
    
    public GameObject _UI_ComputerScreen;
    public GameObject computer;

    [Header("Animation")]
    [SerializeField] Animator animator;
    private readonly int open = Animator.StringToHash("Open");
    private readonly int open_onPower = Animator.StringToHash("Open_onPower");
    private readonly int close = Animator.StringToHash("Close");

    [Header("Coroutine")]
    private Coroutine _PrograssCoroutine;
    private Coroutine _TransformCoroutine;


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
            textLineList.Add(newTextLine);

        }
    }


    // public override void OnEnable()
    // {
    //     if(computer == null)
    //     {
    //         computer = MapEditor.Instance.FindObj(MapEditor.Instance.objectTransform, 1000);
    //     }
    //     if(!computer.GetComponent<StageSelectObject>().onPower) animator.SetTrigger(open);
    //     else animator.SetTrigger(open_onPower);
        
        
    //     if(computer)
    //     computer.GetComponent<StageSelectorComputer>().Talking();

    //     // try
    //     // {
    //     //     if (_UI_ComputerScreen == null) { _UI_ComputerScreen = Managers.UI.GetUI<UI_ComputerScreen>().gameObject; }
    //     // }
    //     // catch(Exception ex)
    //     // {
    //     //     Debug.Log($"EX : {ex}");
    //     // }
        

    // }
        
    // private void Update()
    // {
    //     //Test Code
    //     if (Input.GetKeyDown(KeyCode.N))
    //     {
    //        HideUIOutsideCamera();
    //     }

    //     if (Input.GetKeyDown(KeyCode.M))
    //     {
    //        OpenUIOutsideCamera();
    //     }

    //     //Interaction

    //     // if (onInteractable && !onPrograss && !inputProcessed)
    //     // {
    //     //     GetKeyEvent();
            
    //     // }

    // }



    //------------------------------------------------------Network 250218
    
    public override void OnEnable()
    {
        // if(computer == null)
        // {
        //     computer = MapEditor.Instance.FindObj(MapEditor.Instance.objectTransform, 1000);
        // }
        // // if(!computer.GetComponent<StageSelectObject>().onPower) animator.SetTrigger(open);
        // // else animator.SetTrigger(open_onPower);

        //  if(!computer.GetComponent<StageSelectObject>().onPower) animator.SetTrigger(open);
        // else animator.SetTrigger(open_onPower);
        
        
        // if(computer)
        // computer.GetComponent<StageSelectorComputer>().Talking();


    }
    bool onReady;
    public void StartUi(uint computerId)
    {
        if (NetworkClient.spawned.TryGetValue(computerId, out NetworkIdentity foundObject))
        {
            computer = foundObject.gameObject;
        }
        if(computer == null) return;

        if(!computer.GetComponent<Computer_Net>().onPower)animator.SetTrigger(open);
        else animator.SetTrigger(open_onPower);

        computer.GetComponent<StageSelectorComputer>().Talking();
    }

    
    public void SetInputKey(int num)
    {
        if(onInteractable && !onPrograss && !inputProcessed && onReady)
        {
            StartCoroutine(ProcessInputWithDelay(num));
        }
    }
    public IEnumerator ProcessInputWithDelay(int num)
    {
        inputProcessed = true;

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

                switch (_PrograssLevel)
                {
                    case PrograssLevel.One:
                        Select_PrograssLevel_1();
                        break;
                    case PrograssLevel.Two:
                        Select_PrograssLevel_2();
                        break;
                    case PrograssLevel.Three:
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
        inputProcessed = false;
    }
    //------------------------------------------------------Network 250218




    // private void GetKeyEvent(){
    //      if (Input.GetKeyDown(KeyCode.DownArrow))
    //         {
    //             StartCoroutine(ProcessInputWithDelay(KeyCode.DownArrow));
    //         }

    //         if (Input.GetKeyDown(KeyCode.UpArrow))
    //         {
    //             StartCoroutine(ProcessInputWithDelay(KeyCode.UpArrow));
    //         }
            
    //         if (Input.GetKeyDown(KeyCode.Return))
    //         {
    //             StartCoroutine(ProcessInputWithDelay(KeyCode.Return));
    //         }


    //         if (Input.GetKeyDown(KeyCode.Backspace))
    //         {
    //             StartCoroutine(ProcessInputWithDelay(KeyCode.Backspace));

    //         }

    //         if (Input.GetKeyDown(KeyCode.Q))
    //         {
    //             StartCoroutine(ProcessInputWithDelay(KeyCode.Q));
    //         }
    // }

    private void SelectTextLine()
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
                Debug.Log(curSelectTextLine.text.text);
                mapInfo_UI.ShowMapInfo(curSelectTextLine.text.text,curStageLevel);
            }
        }
        else
        {
            curSelectTextLine = textLineList[curSelectTextLineIndex];
            curSelectTextLine.SelectSentence();

            if (_PrograssLevel == PrograssLevel.Three)
            {
                Debug.Log(curSelectTextLine.text.text);
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
        yield return _PrograssCoroutine = StartCoroutine(WriteTextLineCo_Title(titleSentence,false));
        
    }


    IEnumerator WriteTextLineCo_Title(string sentence,bool back = true)
    {
        if(back){
            yield return EraserTextLineCo(minSelectTextLineListIndex,maxSelectTextLineListIndex);
        }
        // TODO 1022
        //Eraser before TEXT
        //
        onPrograss = true;
        onInteractable = false;
        nextWriteTextLineIndex = 0;
        _PrograssLevel = PrograssLevel.One;
        List<string> sentenceList = util.SplitText(sentence, maxHorizontaText, new char[] { '\n' });
        for (int i = 0; i < sentenceList.Count; i++)
        {
            yield return WriteLine(sentenceList[i], localColor, true);
        }
        
        nextWriteTextLineIndex+=2;
        
        minSelectTextLineListIndex = nextWriteTextLineIndex;
        yield return WriteLine("Main", localColor, true);
        // yield return WriteLine("UserMap (준비중)", localColor, true, 25, 0.01f, false);

        maxSelectTextLineListIndex = nextWriteTextLineIndex-1;
        curSelectTextLineIndex = maxSelectTextLineListIndex;
        
        onInteractable = true;
        onPrograss = false;
        onReady = true;
        
    }

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

        if (GetSplitSentenceAndLaststring(textLineList[pathTextLineIndex].mainSentence) == "Main")
        {
            _PrograssLevel = PrograssLevel.Two;
            yield return EraserTextLineCo(minSelectTextLineListIndex, maxSelectTextLineListIndex);
            
            int index = Managers.Data.saveData._SaveFileData._PlayerSaveData.curStageLevel;
            for (int i = 0; i <=index; i++)
            {
               yield return WriteLine($"{i}", localColor, true);
            }
            maxSelectTextLineListIndex = nextWriteTextLineIndex-1;
        }
        else
        {
            //usermap Prograss
        }

        curSelectTextLineIndex = nextWriteTextLineIndex;

        onPrograss = false;
        onInteractable = true;
    }

    void Select_PrograssLevel_2()
    {
        int stageLevel = int.Parse(curSelectTextLine.mainSentence);
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
        yield return EraserTextLineCo(minSelectTextLineListIndex, maxSelectTextLineListIndex);
        Map[] maps = Managers.Data.mapData.mapMainStageDictionary[stageLevel];
        MapSaveData[] mapDatas = CheckPlayerData(maps);

        for (int i = 0; i < mapDatas.Length; i++)
        {
            if (mapDatas[i].clear)
            {
                //yield return WriteLine(maps[i].mapID, localColor, true);\
                yield return WriteLine(string.IsNullOrWhiteSpace(maps[i].subMapName) ? maps[i].mapID : maps[i].subMapName,localColor,true);
            }
            else if (mapDatas[i].openStage)
            {
                yield return WriteLine(string.IsNullOrWhiteSpace(maps[i].subMapName) ? maps[i].mapID : maps[i].subMapName, Color.yellow, true);
            }
           
        }

        maxSelectTextLineListIndex = nextWriteTextLineIndex-1;
        curSelectTextLineIndex = nextWriteTextLineIndex;

        onPrograss = false;
        onInteractable = true;
    }


    private Map GetMap(string mapName)
    {
        Map[] maps = Managers.Data.mapData.mapMainStageDictionary[curStageLevel];

        foreach(var map in maps)
        {
            if (map.mapID == mapName || map.subMapName == mapName) return map;

        }

        return null;

    }

    IEnumerator Select_PrograssLevel_3Co()
    {
        onPrograss = true;
        onInteractable = false;
        _PrograssLevel = PrograssLevel.End;

        textLineList[pathTextLineIndex].type = TypingType.Read;
        textLineList[pathTextLineIndex].text.color = Color.yellow;

        mapInfo_UI.Reset();

        try
        {
            ExitPointObj obj = MapEditor.Instance.FindObj(MapEditor.Instance.exitDoorObjectTransform, 301).GetComponent<ExitPointObj>();

            Map map = GetMap(curSelectTextLine.mainSentence);
            obj.nextMapId = map.mapID;
            selectMapId = map.mapID;
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
        }

        yield return EraserTextLineCo(minSelectTextLineListIndex,maxSelectTextLineListIndex);
        // StartCoroutine(textLineList[4].ChangeEncryption());
        yield return EraserTextLineCo(0, minSelectTextLineListIndex);
        // yield return textLineList[4].curTMTC.EraserAll();

        //Path Text Eraser Effect.
        textLineList[pathTextLineIndex].Clear();
        //Path Text Eraser Effect.

        textLineList[pathTextLineIndex].type = TypingType.Write;
        //250103
        animator.SetTrigger(close); 
        computer.GetComponent<Computer_Net>().Server_SetIsOpen(false);
        //250103
        _UI_KeyGenerator.gameObject.SetActive(true);
        _UI_KeyGenerator.KeyPrintingAni();
        yield return new WaitForSeconds(6f);
        _UI_KeyGenerator.gameObject.SetActive(false);
        // Screen On
        //SetScreenDataAndActive(selectMapId);

        //todo 0709 SpawnKey
        computer.GetComponent<StageSelectorComputer>().SpawnKey();

        //player Move control
        // PlayerMovement playerMovement = Managers.Game.Player.GetComponent<PlayerMovement>();
        // if(!playerMovement.canControl){
        //     playerMovement.canControl = true;
        // }
        //player Move control

        onPrograss = false;
        onInteractable = true;
        gameObject.SetActive(false);
    }
    private void SetScreenDataAndActive(string selectMapId)
    {
         try
        {
            if (_UI_ComputerScreen == null) { _UI_ComputerScreen = Managers.UI.GetUI<UI_ComputerScreen>().gameObject; }
        }
        catch(Exception ex)
        {
            Debug.Log($"EX : {ex}");
        }
        _UI_ComputerScreen.SetActive(true);
        _UI_ComputerScreen.GetComponent<UI_ComputerScreen>().SetData(selectMapId);
    }

    private void BackPrograss()
    {
        switch (_PrograssLevel)
        {
            case PrograssLevel.One:
                Shutdown();
                break;
            case PrograssLevel.Two:
                textLineList[pathTextLineIndex].WriteText("", localColor);
                StartCoroutine(WriteTextLineCo_Title(titleSentence));
                break;
            case PrograssLevel.Three:
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

        yield return EraserTextLineCo(0, maxSelectTextLineListIndex);
        animator.SetTrigger(close);
        yield return new WaitForSeconds(1f);
    //player Move control
        // PlayerMovement playerMovement = Managers.Game.Player.GetComponent<PlayerMovement>();
        // if(!playerMovement.canControl){
        //     playerMovement.canControl = true;
        // }
    //player Move control
        computer.GetComponent<Computer_Net>().Server_SetIsOpen(false);

        onPrograss = false;
        gameObject.SetActive(false);

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

    private MapSaveData[] CheckPlayerData(Map[] map) //TODO 0805
    {
        MapSaveData[] array = new MapSaveData[map.Length];

        for (int i = 0; i < map.Length; i++)
        {
            array[i] = Managers.Data.saveData.dic[map[i].mapID];
        }

        if (map[0].stageLevel == 0)
        {
            array[0].openStage = true;
        }
  
        return array;

    }


    // public IEnumerator ProcessInputWithDelay(KeyCode keyCode)
    // {
    //     inputProcessed = true;

    //     switch (keyCode)
    //     {
    //         case KeyCode.DownArrow:
    //             curSelectTextLineIndex++;
    //             SelectTextLine();
    //             break;
    //         case KeyCode.UpArrow:
    //             curSelectTextLineIndex--;
    //             SelectTextLine();
    //             break;
    //         case KeyCode.Return:
    //             if (curSelectTextLine == null || !curSelectTextLine.onSelectable)
    //                 break;

    //             switch (_PrograssLevel)
    //             {
    //                 case PrograssLevel.One:
    //                     Select_PrograssLevel_1();
    //                     break;
    //                 case PrograssLevel.Two:
    //                     Select_PrograssLevel_2();
    //                     break;
    //                 case PrograssLevel.Three:
    //                     textLineList[pathTextLineIndex].WriteText($"/{curSelectTextLine.mainSentence}");
    //                     _PrograssCoroutine = StartCoroutine(Select_PrograssLevel_3Co());
    //                     break;
    //             }
    //             break;
    //         case KeyCode.Backspace:
    //             BackPrograss();
    //             break;
    //         case KeyCode.Q:
    //             if(_PrograssLevel == PrograssLevel.Three)mapInfo_UI.Reset();
    //             Shutdown();
    //             break;
    //     }

    //     yield return new WaitForSeconds(inputDelay);
    //     inputProcessed = false;
    // }

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
