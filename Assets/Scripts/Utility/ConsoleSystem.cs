using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;


public class ConsoleSystem : MonoBehaviour
{
    public TMP_InputField inputField;
    
    private StringBuilder sb;
    [SerializeField] TextMeshProUGUI logText;
    [SerializeField] GameObject container;
    private bool onConsole;


    #region Map
    private List<string> mapIDList;
    #endregion

    private string helpSentence = @"    [Command]
    Map Load (mapId case sensitive)
        -> ex) Load {mapId}
    Reset Interactable Object (Case insensitive)
        -> ex) Reset Object 
    Show All Map ID (Case insensitive)
        -> ex) Show mapID
    Stage Select Open All Map (Case insensitive)
        -> ex) Open all map
    Log Clear (Case insensitive)
        -> ex) Clear
    Reset SaveData (Case insensitive)
        -> ex) Reset SaveData
";
    private List<string> GetMapIDList(){
        //string[] jsonFiles = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("MapDat");
        
        return jsonFiles.Select(file => file.name).ToList();
    }

    void Start()
    {
        sb = new();
        mapIDList = GetMapIDList();
        if (container.activeSelf)
        {
            container.SetActive(false);
            onConsole = false;
        }
       
    }

    private void Update()
    {
        if (container.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            CommandRead();
            inputField.ActivateInputField();
        }
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (onConsole)
            {
                container.SetActive(false);
                onConsole = false;
            }
            else
            {
                container.SetActive(true);
                onConsole = true;
                inputField.ActivateInputField();
            }
        }
    }
     #region Main
    private void CommandRead()
    {
        if (string.IsNullOrEmpty(inputField.text))
        {
            WriteLog($"{inputField.text} \n - text is null or empty\n");
            return;
        }

        if (inputField.text=="?")
        {
            WriteLog($"?\n{helpSentence}");
            inputField.text = "";
            return;
        }
        
        string command = inputField.text.ToLower();

        if (command == "reset object"){
            WriteLog($"\n   >{inputField.text}\n");
            MapEditor.Instance.ResetInteractableObjectPosition();
            inputField.text = "";
            return;
        }

        if(command == "show mapid"){
            WriteLog($"\n   >{inputField.text}");
            foreach(string id in mapIDList){
                WriteLog($"\t-{id}");
            }
            inputField.text = "";
            return;
        }
        if(command == "open all map"){
            WriteLog($"\n   >{inputField.text}\n\n");
            OpenAllMap();
            inputField.text = "";
            return;
        }
        if (command == "clear")
        {
            sb.Clear();
            logText.text = "";
            inputField.text = "";
            return;
        }
        if(command == "reset savedata")
        {
            //Del SaveData, achievmentData Reset
            Managers.Data.saveData.DeleteSaveFile();
            Managers.Data.saveData.SetUp();

            //Del SaveData, achievmentData Reset

            WriteLog($"\n   >{inputField.text}\npath : {Managers.Data.saveData.savePath}\n");
            inputField.text = "";
            return;
        }

        string[] strings = inputField.text.Split(" "); //ex Load mapId
        strings[0] = strings[0].ToLower();
        switch (strings[0])
        {
            case "load":
          
                if (mapIDList.Contains(strings[1]))
                {
                    WriteLog($"\n   >{inputField.text}\n");
                    inputField.text = "";
                    //MapEditor.Instance.MoveNextStage(strings[1]);
                    Managers.Command.ChangeStage(strings[1]);
                }
                else
                {
                    WriteLog($"{inputField.text} \n - Can't find map\n");
                    return;
                }
               
                break;
            default:
                  WriteLog($"{inputField.text} \n - Can't find command\n");
            return;
        }
    }
     #endregion



    private void WriteLog(string sentence)
    {
        sb.Append($"\n{sentence}");
        logText.text = sb.ToString();
        
    }



    private void OpenAllMap(){
        //1. modify PlayerSaveData.curStageLevel
        Managers.Data.saveData._SaveFileData._PlayerSaveData.curStageLevel = 1;

        foreach (var item in Managers.Data.saveData.dic)
        {
            item.Value.openStage = true;
        }
    }
  
}

