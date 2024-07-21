using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using UnityEngine.InputSystem;

enum PrograssLevel
{
    One,
    Two,
    Three,
    End
}

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

    string openningSentence = @"Mob Spawn Range: 4
,Hopper Transfer: 8 Hopper Check: 8 Hopper Amount: 1
,Random Lighting Updates: false
,Structure Info Saving: true
,Cactus Growth Modifier: 100%
,Cane Growth Modifier: 100%
,Melon Growth Modifier: 100%
,Mushroom Growth Modifier: 100%
,Pumpkin Growth Modifier: 100%
,Sapling Growth Modifier: 100%
,Wheat Growth Modifier: 100%
,NetherWart Growth Modifier: 100%
,Vine Growth Modifier: 100%
,Cocoa Growth Modifier: 100%
,Tile Max Tick Time: 50ms Entity max Tick Time: 50ms
,Preparing start region for level 0 (Seed: 2127560419623009344)
,Preparing spawn area: 89%
,Preparing start region for level 1 (Seed: 2127560419623009344)
,Preparing start region for level 2 (Seed: 2127560419623009344)
,Thanks for downloading SetSpawn!
,http://dev.bukkit.org/bukkit-plugins/setspawn
,Enabling SetSpawn v2.1
,Enabling RaspberryJuice v1.7
,ThreadListener Started
,Server permissions file permissions.";

    string titleSentence = @"-------------------------------------------------------------------------
[Up - Up Arrow]      [Down - Down Arrow]   [Select - Enter]
[Back - Backspace]   [Q - Exit]
-------------------------------------------------------------------------";

    Color localColor = new Color(48f / 255f, 172f / 255f, 52f / 255f); 

    [Header("Stats")]
    [SerializeField] int maxTextLine;
    private TextLine curSelectTextLine; //현재 선택된 텍스트라인
    public int curSelectTextLineIndex; //현재 선택된 텍스트라인 인덱

    private int nextWriteTextLineIndex; //다음에 쓸 Line Index
    private int minSelectTextLineListIndex; // 선택 가능한 라인 인덱스
    private int maxSelectTextLineListIndex; // 선택 가능한 라인 인덱스

    [HideInInspector]public bool onInteractable;
    [HideInInspector]public bool onPrograss;
    private bool inputProcessed;
    public float inputDelay; // 입력 딜레이 시간 설정 

    PrograssLevel _PrograssLevel;


    //List<IEnumerator> _IEnumeratorList;
    
    public GameObject _UI_ComputerScreen;
    public GameObject computer;

    [Header("Animation")]
    [SerializeField] Animator animator;
    private readonly int open = Animator.StringToHash("Open");
    private readonly int close = Animator.StringToHash("Close");

    [Header("Coroutine")]
    private Coroutine _PrograssCoroutine;
    private Coroutine _TransformCoroutine;

    private void Awake()
    {
        //_IEnumeratorList = new()
        //{
        //    WriteTextLineCo_Title(titleSentence),
        //};

        contentRectTransform = content.transform as RectTransform;

        //TextLine Pooling
        textLineList = new();
        for (int i = 0; i < maxTextLine; i++)
        {
            TextLine newTextLine= Instantiate(textLine, content).GetComponent<TextLine>();
            newTextLine.index = i;
            textLineList.Add(newTextLine);

        }

        
        //TestCode



    }


    public override void OnEnable()
    {
        if(computer == null)
        {
            computer = MapEditor.Instance.FindObj(MapEditor.Instance.objectTransform, 1000);
        }
        
        animator.SetTrigger(open);

        computer.GetComponent<StageSelectorComputer>().Talking();

        try
        {
            if (_UI_ComputerScreen == null) { _UI_ComputerScreen = Managers.UI.GetUI<UI_ComputerScreen>().gameObject; }
        }
        catch(Exception ex)
        {
            Debug.Log($"EX : {ex}");
        }
        

    }


    private void Update()
    {
        //Test Code
        if (Input.GetKeyDown(KeyCode.N))
        {
            HideUIOutsideCamera();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            OpenUIOutsideCamera();
        }

        //Interaction

        if (onInteractable && !onPrograss && !inputProcessed)
        {

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                StartCoroutine(ProcessInputWithDelay(KeyCode.DownArrow));
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                StartCoroutine(ProcessInputWithDelay(KeyCode.UpArrow));
            }
            
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartCoroutine(ProcessInputWithDelay(KeyCode.Return));
            }


            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                StartCoroutine(ProcessInputWithDelay(KeyCode.Backspace));

            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(ProcessInputWithDelay(KeyCode.Q));
            }


        }

    }



    private void SelectTextLine()
    {
        if(curSelectTextLineIndex < minSelectTextLineListIndex)
        {
            curSelectTextLineIndex = maxSelectTextLineListIndex;
            
        }
        else if(curSelectTextLineIndex > maxSelectTextLineListIndex)
        {
            curSelectTextLineIndex = minSelectTextLineListIndex;
        }

        if(curSelectTextLineIndex >= contentMoveRect_TextLineIndex)
        {
            Vector2 position = contentRectTransform.localPosition;
            position.y = 30 * (curSelectTextLineIndex - contentMoveRect_TextLineIndex);
            contentRectTransform.localPosition = position;
        }
   

        if(curSelectTextLine != null)
        {
            curSelectTextLine.UnSelectSentence();
            curSelectTextLine = textLineList[curSelectTextLineIndex];
            curSelectTextLine.SelectSentence();
        }
        else
        {
            curSelectTextLine = textLineList[curSelectTextLineIndex];
            curSelectTextLine.SelectSentence();
        }
    
    }


    #region Write

    //Title



    IEnumerator WriteLine(string sentence, Color color, bool readAntWrite, float fontSize = 25, float delayTime = 0.007f, bool onSelectable = true)
    {
     
        if (textLineList[nextWriteTextLineIndex].CheckCompareString(sentence))
        {
            nextWriteTextLineIndex++;
            yield break;
        }


        if (nextWriteTextLineIndex >= contentMoveRect_TextLineIndex)
        {
            Vector2 position = contentRectTransform.localPosition;
            position.y = 30 * (nextWriteTextLineIndex - contentMoveRect_TextLineIndex);
            contentRectTransform.localPosition = position;
        }
        TextLine textLine = textLineList[nextWriteTextLineIndex];
        nextWriteTextLineIndex++;

        yield return textLine.Task_WriteTyping(sentence, color, readAntWrite, fontSize, delayTime, onSelectable);

    }

    //private void WriteLine(string sentence, Color color, bool readAntWrite, float fontSize = 25, float delayTime = 0.01f, bool onSelectable = true)
    //{
    //    if (textLineList[nextWriteTextLineIndex].CheckCompareString(sentence))
    //    {
    //        nextWriteTextLineIndex++;
    //        return;
    //    }


    //    if (nextWriteTextLineIndex >= contentMoveRect_TextLineIndex)
    //    {
    //        Vector2 position = contentRectTransform.localPosition;
    //        position.y = 30 * (nextWriteTextLineIndex - contentMoveRect_TextLineIndex);
    //        contentRectTransform.localPosition = position;
    //    }

    //    textLineList[nextWriteTextLineIndex].WriteText(sentence, color, readAntWrite, fontSize, delayTime, onSelectable);
    //    nextWriteTextLineIndex++;
    //}

    private void Write(string sentence, Color color, bool readAntWrite, float fontSize = 25, float delayTime = 0.01f, bool onSelectable = true)
    {
        textLineList[nextWriteTextLineIndex].WriteText(sentence, color, readAntWrite, fontSize, delayTime, onSelectable);
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

    IEnumerator EraserTextLineCo() //All Eraser
    {
        for (int i = maxTextLine-1; i >= 0; i--)
        {
            if (textLineList[i].type == TypingType.Read) continue;
            if (textLineList[i].CheckEmpty()) continue;

            textLineList[i].EraserText();
            yield return new WaitForSeconds(_WriteAndEraserDelayRate);
        }

        nextWriteTextLineIndex = 0;
        
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



    private void OpenningTitle_()
    {
        _PrograssLevel = PrograssLevel.One;
       _PrograssCoroutine = StartCoroutine(OpenningTitle());
    }

    IEnumerator OpenningTitle()
    {
        onPrograss = true;
        onInteractable = false;
        nextWriteTextLineIndex = 0;
        
        Managers.Sound.PlaySound(AudioType.Computer_On, AudioMixerGroupType.Effects, false, 0.50f, 0f);
        List<string> sentenceList = util.SplitText(openningSentence, maxHorizontaText, new char[] { ',' });
        
        Debug.Log(sentenceList.Count);
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

        //todo ... 0629

        yield return new WaitForSeconds(1);
        EraserAllClear();
        yield return _PrograssCoroutine = StartCoroutine(WriteTextLineCo_Title(titleSentence));
        
    }

    IEnumerator WriteTextLineCo_Title(string sentence)
    {
        onPrograss = true;
        onInteractable = false;
        nextWriteTextLineIndex = 0;
        _PrograssLevel = PrograssLevel.One;

        List<string> sentenceList = util.SplitText(sentence, maxHorizontaText, new char[] { '\n' });
        for (int i = 0; i < sentenceList.Count; i++)
        {
            yield return WriteLine(sentenceList[i], localColor, true);
        }

        nextWriteTextLineIndex = sentenceList.Count + 1;

        //Init Select Line
        

        yield return WriteLine("Main", localColor, true);
        yield return WriteLine("UserMap (준비중)", localColor, true, 25, 0.01f, false);

        
        maxSelectTextLineListIndex = nextWriteTextLineIndex - 1;
        minSelectTextLineListIndex = maxSelectTextLineListIndex -1;
        curSelectTextLineIndex = maxSelectTextLineListIndex;

        onInteractable = true;
        onPrograss = false;
        
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
            
            int index = CheckDirectory();
            for (int i = 0; i <= index; i++)
            {
                Debug.Log(i);
               yield return WriteLine($"{i}", localColor, true);
            }

            maxSelectTextLineListIndex = minSelectTextLineListIndex + index;
        }
        else
        {
            //usermap Prograss
        }

        curSelectTextLineIndex = maxSelectTextLineListIndex;
       

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


    IEnumerator Select_PrograssLevel_2Co(int stageLevel)
    {

        onPrograss = true;
        onInteractable = false;

        _PrograssLevel = PrograssLevel.Three;

        yield return EraserTextLineCo(minSelectTextLineListIndex, maxSelectTextLineListIndex);
        Map[] maps = Managers.Data.mapData.mapMainStageDictionary[stageLevel];
        bool[] clearMaps = CheckPlayerData(maps);

        for (int i = 0; i < clearMaps.Length; i++)
        {
            if (clearMaps[i])
            {
                yield return WriteLine(maps[i].mapID, localColor, true);
            }
            else
            {
                yield return WriteLine(maps[i].mapID, Color.red, true,25,0.01f,false);
            }
        }

        maxSelectTextLineListIndex = minSelectTextLineListIndex + maps.Length-1;
        curSelectTextLineIndex = maxSelectTextLineListIndex;


        onPrograss = false;
        onInteractable = true;
    }

 

    //이부분만 수정하면 됨.
    IEnumerator Select_PrograssLevel_3Co()
    {
        onPrograss = true;
        onInteractable = false;
        _PrograssLevel = PrograssLevel.End;

       
        try
        {
            ExitPointObj obj = MapEditor.Instance.FindObj(MapEditor.Instance.exitDoorObjectTransform, 301).GetComponent<ExitPointObj>();
            obj.nextMapId = curSelectTextLine.mainSentence;
            selectMapId = curSelectTextLine.mainSentence;
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
        }


        yield return EraserTextLineCo(0, maxSelectTextLineListIndex);


        animator.SetTrigger(close);
        _UI_KeyGenerator.gameObject.SetActive(true);
        _UI_KeyGenerator.KeyPrintingAni();
        yield return new WaitForSeconds(3f);
        _UI_KeyGenerator.gameObject.SetActive(false);
        // Screen On
        SetScreenDataAndActive(selectMapId);

        //todo 0709 SpawnKey
        computer.GetComponent<StageSelectorComputer>().SpawnKey();

        onPrograss = false;
        onInteractable = true;
    }

    private void SetScreenDataAndActive(string selectMapId)
    {
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

        yield return EraserTextLineCo(0, maxSelectTextLineListIndex);
        animator.SetTrigger(close);
        yield return new WaitForSeconds(.5f);

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

    private bool[] CheckPlayerData(Map[] map)
    {
        bool[] array = new bool[map.Length];

        if (map[0].stageLevel == 0)
        {
            for (int i = 0; i < map.Length; i++)
            {
                if (Managers.Data.loadData.playData[map[i].mapID].stageClear)
                {
                    array[i] = true;
                }
                else
                {
                    array[i] = false;
                }
            }
        }
        else
        {
            Map[] beforeStage = Managers.Data.mapData.mapMainStageDictionary[map[0].stageLevel - 1];
            for (int i = 0; i < map.Length; i++)
            {
                if (i == 0)
                {
                    if (Managers.Data.loadData.playData[beforeStage[beforeStage.Length - 1].mapID].stageClear)
                    {
                        array[i] = true;
                    }
                    else
                    {
                        array[i] = false;
                    }
                }
                else
                {
                    if (Managers.Data.loadData.playData[map[i].mapID].stageClear)
                    {
                        array[i] = true;
                    }
                    else
                    {
                        array[i] = false;
                    }
                }

            }

        }


        return array;

    }



    private IEnumerator ProcessInputWithDelay(KeyCode keyCode)
    {
        inputProcessed = true;

        switch (keyCode)
        {
            case KeyCode.DownArrow:
                curSelectTextLineIndex++;
                SelectTextLine();
                break;
            case KeyCode.UpArrow:
                curSelectTextLineIndex--;
                SelectTextLine();
                break;
            case KeyCode.Return:
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
            case KeyCode.Backspace:
                BackPrograss();
                break;
            case KeyCode.Q:
                Shutdown();
                break;
        }

        yield return new WaitForSeconds(inputDelay);
        inputProcessed = false;
    }

    #endregion
    /// <summary>
    /// 호스트가 상호작용중일때는 움직이지 못하게,
    /// 다른 클라이언트가 접근하면 유아이 켜지
    /// </summary>
    #region Hide And Open UI
    public void HideUIOutsideCamera()
    {
        float uiWidth = 1080f;
        float uiHeight = 720f;

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
