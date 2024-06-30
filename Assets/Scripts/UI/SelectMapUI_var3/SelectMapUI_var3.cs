using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;

enum PrograssLevel
{
    One,
    Two,
    Three
}

public class SelectMapUI_var3 : MonoBehaviour
{
    

    [Header("Info")]
    private Dictionary<int, Map[]> mapMainStageDictionary;
    [SerializeField] float _WriteAndEraserDelayRate;
    [SerializeField] int maxHorizontaText;// 얼마나 써야 다음 줄로 넘어가는가.
    [SerializeField] int contentMoveRect_TextLineIndex; //Change content RectTransform.localPosition.  + 30;
    [SerializeField] int pathTextLineIndex;

    [Header("Components")]
    Util util = new Util();
    
    [Header("Transform")]
    [SerializeField] Transform content;
    RectTransform contentRectTransform;

    [Header("Text")]
    [SerializeField] GameObject textLine;
    private List<TextLine> textLineList;
    string openningSentence = @"
,Mob Spawn Range: 4
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

    string titleSentence = "-------------------------------------------------------------------------,,Up - Up Arrow        Down - Down Arrow       Select - Enter      Back - Backspace,,-------------------------------------------------------------------------,,";
    Color localColor = new Color(48f / 255f, 172f / 255f, 52f / 255f); 

    [Header("Stats")]
    [SerializeField] int maxTextLine;
    private TextLine curSelectTextLine; //현재 선택된 텍스트라인
    private int curSelectTextLineIndex; //현재 선택된 텍스트라인 인덱

    private int nextWriteTextLineIndex; //다음에 쓸 Line Index
    private int minSelectTextLineListIndex; // 선택 가능한 라인 인덱스
    private int maxSelectTextLineListIndex; // 선택 가능한 라인 인덱스

    private bool onInteractable;
    private bool onPrograss;

    PrograssLevel _PrograssLevel;


    List<IEnumerator> _IEnumeratorList;

    private void Awake()
    {
        _IEnumeratorList = new()
        {
            WriteTextLineCo_Title(titleSentence),
        };

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

    private void Start()
    {
        _PrograssLevel = PrograssLevel.One;
        StartCoroutine(OpenningTitle());
        //StartCoroutine(_IEnumeratorList[(int)_PrograssLevel]);

    }


    private void Update()
    {
        //Test Code
        if (Input.GetKeyDown(KeyCode.Space) && !onPrograss)
        {
            StartCoroutine(EraserTextLineCo());
        }

        if (Input.GetKeyDown(KeyCode.Z) && !onPrograss)
        {
            StartCoroutine(WriteTextLineCo_Title(titleSentence));
        }


    }


    private void LateUpdate()
    {
        if (onInteractable && !onPrograss)
        {
            
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                curSelectTextLineIndex++;
                SelectTextLine(1);

            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                curSelectTextLineIndex--;
                SelectTextLine(-1);

            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                switch (_PrograssLevel)
                {
                    case PrograssLevel.One:
                        Select_PrograssLevel_1();
                        break;
                    case PrograssLevel.Two:
                        Select_PrograssLevel_2();
                        break;
                    case PrograssLevel.Three:
                        //test Code
                        textLineList[pathTextLineIndex].WriteText($"/{curSelectTextLine.mainSentence}");
                        //Spawn Key
                        //SelectMapUI setAtive(false)
                        //screen 띄우기
                        break;
                }
                
            }


            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                BackPrograss();
            }


        }
    }


    


    private void SelectTextLine(int upAndDown)
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

    IEnumerator OpenningTitle()
    {
        onPrograss = true;
        onInteractable = false;
        nextWriteTextLineIndex = 0;

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

        WriteLine(". . .", localColor, true, 25, 0.5f);
        yield return new WaitForSeconds(0.5f);
        EraserAllClear();

        yield return StartCoroutine(_IEnumeratorList[(int)_PrograssLevel]);

        onInteractable = true;
        onPrograss = false;
    }


    //Title
    IEnumerator WriteTextLineCo_Title(string sentence)
    {
        onPrograss = true;
        onInteractable = false;
        nextWriteTextLineIndex = 0;
        _PrograssLevel = PrograssLevel.One;

        List<string> sentenceList = util.SplitText(sentence, maxHorizontaText, new char[] { ',', '.' });
        for (int i = 0; i < sentenceList.Count; i++)
        {
            //textLineList[i].WriteText(sentenceList[i],Color.white,true);
            WriteLine(sentenceList[i],localColor,true);
            yield return new WaitForSeconds(_WriteAndEraserDelayRate);
        }
        
        nextWriteTextLineIndex = sentenceList.Count+1;


        Debug.Log(nextWriteTextLineIndex);

        //todo 0626 TestCode
        //Init Select Line
        curSelectTextLineIndex = minSelectTextLineListIndex;
        minSelectTextLineListIndex = 8;
        maxSelectTextLineListIndex = 9;

        nextWriteTextLineIndex = minSelectTextLineListIndex;
        WriteLine("Main", localColor, true);
        WriteLine("UserMap (준비중)", localColor, true,25,0.01f,false);

        SelectTextLine(1);


        onInteractable = true;
        onPrograss = false;
    }

    private void WriteLine(string sentence,Color color,bool readAntWrite,float fontSize = 25,float delayTime = 0.01f,bool onSelectable = true)
    {
        if (textLineList[nextWriteTextLineIndex].CheckCompareString(sentence))
        {
            nextWriteTextLineIndex++;
            return;
        }

        
        if (nextWriteTextLineIndex >= contentMoveRect_TextLineIndex)
        {
            Vector2 position = contentRectTransform.localPosition;
            position.y = 30 * (nextWriteTextLineIndex - contentMoveRect_TextLineIndex);
            contentRectTransform.localPosition = position;
        }

        textLineList[nextWriteTextLineIndex].WriteText(sentence, color, readAntWrite, fontSize, delayTime, onSelectable);
        nextWriteTextLineIndex++;
    }

    private void Write(string sentence, Color color, bool readAntWrite, float fontSize = 25, float delayTime = 0.01f, bool onSelectable = true)
    {
        textLineList[nextWriteTextLineIndex].WriteText(sentence, color, readAntWrite, fontSize, delayTime, onSelectable);
    }


    #endregion

    #region Eraser

    IEnumerator EraserTextLineCo(int min,int max)
    {
        onPrograss = true;
        onInteractable = false;
        for (int i = max; i >= min; i--)
        {
            if (textLineList[i].type == TypingType.Read) continue;

            nextWriteTextLineIndex = i;
            textLineList[i].EraserText();
            yield return new WaitForSeconds(_WriteAndEraserDelayRate);
        }
        yield return new WaitForSeconds(0.5f);
        onPrograss = false;
        onInteractable = true;
    }

    IEnumerator EraserTextLineCo() //All Eraser
    {
        onPrograss = true;
        for (int i = maxTextLine-1; i >= 0; i--)
        {
            if (textLineList[i].type == TypingType.Read) continue;
            if (textLineList[i].CheckEmpty()) continue;

            textLineList[i].EraserText();
            yield return new WaitForSeconds(_WriteAndEraserDelayRate);
        }

        nextWriteTextLineIndex = 0;
        
        onPrograss = false;
    }

    private void EraserAllClear()
    {
        Debug.Log("asd");
        foreach(TextLine textLine in textLineList)
        {
            textLine.Clear();
        }

        nextWriteTextLineIndex = 0;

        contentRectTransform.localPosition = Vector2.zero;

    }
    #endregion


    void Select_PrograssLevel_1()
    {
        if (curSelectTextLine == null) return;
        if (!curSelectTextLine.onSelectable) return;

        curSelectTextLine.Reset();

        textLineList[pathTextLineIndex].WriteText($"/{curSelectTextLine.mainSentence}");

        StartCoroutine(Select_PrograssLevel_1Co());
    }
    void Select_PrograssLevel_1Back()
    {
        curSelectTextLine.Reset();

        StartCoroutine(Select_PrograssLevel_1Co());
    }

    IEnumerator Select_PrograssLevel_1Co()
    {
        onPrograss = true;
        onInteractable = false;
        
        if (GetSplitSentenceAndLaststring(textLineList[pathTextLineIndex].mainSentence) == "Main")
        {
            _PrograssLevel = PrograssLevel.Two;
            yield return StartCoroutine(EraserTextLineCo(minSelectTextLineListIndex, maxSelectTextLineListIndex));

            int index = CheckDirectory();
            for (int i = 0; i <= index; i++)
            {
                WriteLine(i.ToString(), localColor, true);
            }

            maxSelectTextLineListIndex = minSelectTextLineListIndex + index;
        }
        else
        {
            //usermap Prograss
        }

        onPrograss = false;
        onInteractable = true;
    }

    void Select_PrograssLevel_2()
    {
        int stageLevel = int.Parse(curSelectTextLine.mainSentence);
        textLineList[pathTextLineIndex].WriteText($"/{curSelectTextLine.mainSentence}");
        curSelectTextLine.Reset();

        StartCoroutine(Select_PrograssLevel_2Co(stageLevel));
    }


    IEnumerator Select_PrograssLevel_2Co(int stageLevel)
    {
        onPrograss = true;
        onInteractable = false;

        _PrograssLevel = PrograssLevel.Three;

        yield return StartCoroutine(EraserTextLineCo(minSelectTextLineListIndex, maxSelectTextLineListIndex));
        Map[] maps = Managers.Data.mapData.mapMainStageDictionary[stageLevel];
        bool[] clearMaps = CheckPlayerData(maps);

        for (int i = 0; i < clearMaps.Length; i++)
        {
            if (clearMaps[i])
            {
                WriteLine(maps[i].mapID, localColor, true);
            }
            else
            {
                WriteLine(maps[i].mapID, Color.red, true,25,0.01f,false);
            }
        }

        maxSelectTextLineListIndex = minSelectTextLineListIndex + maps.Length-1;


        onPrograss = false;
        onInteractable = true;
    }

    int CheckDirectory()
    {
        string path = Path.Combine(Application.dataPath, "Resources/MapDat/Main");
        int index = 0;
        while (true)
        {
            if (Directory.Exists(Path.Combine(path,index.ToString()))){
                index++;

            }
            else { break; }
            
        }
        return index-1;
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



    private void BackPrograss()
    {
        if (_PrograssLevel == PrograssLevel.One) return;

        switch (_PrograssLevel)
        {
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

    // /Main/0

}
