using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleSystem : MonoBehaviour
{
    private List<string> lists;

    private string myText;
    private string oldText;

    public TMP_InputField inputField;

    private string path;

    private StringBuilder sb;
    [SerializeField] TextMeshProUGUI logText;
    [SerializeField] GameObject container;
    private bool onConsole;


    private string helpSentence = @"    - Help
    Map Load -> ex) Load Main {mapId}, Load Scene {mapId}
";

    void Start()
    {
        sb = new();
        path = Path.Combine(Application.dataPath, $"Resources/MapDat");
        lists = new();
        lists.Add("Load");

        if (container.activeSelf)
        {
            container.SetActive(false);
            onConsole = false;
        }
       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
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
    private void WriteLog(string sentence)
    {
        sb.Append($"\n{sentence}");
        logText.text = sb.ToString();
    }

    private void CommandRead()
    {
        if (string.IsNullOrEmpty(inputField.text))
        {
            WriteLog($"{inputField.text} \n -text is null or empty\n");
            return;
        }

        string[] strings = inputField.text.Split(" "); //ex Load Scene mapId


        if (inputField.text=="?")
        {
            WriteLog(helpSentence);
            inputField.text = "";
            return;
        }


        if (strings.Length != 3)
        {
            WriteLog($"{inputField.text} \n -Can't find command\n");
            return;
        }
        
        string newPath;
        switch (strings[0])
        {
            case "Load":
                newPath = Path.Combine(path, $"{strings[1]}");
                string[] jsonFiles = Directory.GetFiles(newPath, "*.json", SearchOption.AllDirectories);
                List<string> fileNames = jsonFiles.Select(file => Path.GetFileNameWithoutExtension(file)).ToList();
                if (fileNames.Contains(strings[2]))
                {
                    WriteLog($"\n   >{inputField.text}\n");
                    inputField.text = ""; 
                    MapEditor.Instance.MoveNextStage(strings[2]);
                }
                else
                {
                    WriteLog($"{inputField.text} \n -Can't find map\n");
                    return;
                }
               
                break;
        }
    }

}
