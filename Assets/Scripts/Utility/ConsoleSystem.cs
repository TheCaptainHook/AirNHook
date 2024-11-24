using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    Map Load 
        -> ex) Load {mapId}
    Reset Interactable Object
        -> ex) Reset Object
    Show All Map ID
        -> ex) Show MapID

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

        if(inputField.text == "Reset Object"){
            WriteLog($"\n   >Reset Object\n");
            MapEditor.Instance.ResetInteractableObjectPosition();
            return;
        }
        if(inputField.text == "Show MapID"){
            WriteLog($"\n   >Show MapId");
            foreach(string id in mapIDList){
                WriteLog($"\t-{id}");
            }
            inputField.text = "";
            return;
        }

        string[] strings = inputField.text.Split(" "); //ex Load mapId
        
        switch (strings[0])
        {
            case "Load":
          
                if (mapIDList.Contains(strings[1]))
                {
                    WriteLog($"\n   >{inputField.text}\n");
                    inputField.text = ""; 
                    MapEditor.Instance.MoveNextStage(strings[1]);
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


    private void UIAtLastCharacter(){
         TMP_TextInfo textInfo = inputField.textComponent.textInfo;

        // 텍스트가 비어 있을 경우 처리
        if (textInfo.characterCount == 0)
        {
            Debug.LogWarning("The text is empty.");
            return;
        }

        TMP_CharacterInfo lastCharInfo = textInfo.characterInfo[textInfo.characterCount - 1];
        Vector2 localPosition = (lastCharInfo.bottomLeft + lastCharInfo.topRight) / 2;

        // TextMeshPro의 로컬 좌표를 캔버스의 로컬 좌표로 변환
        Vector3 canvasLocalPosition = inputField.textComponent.rectTransform.localToWorldMatrix.MultiplyPoint3x4(localPosition);
        canvasLocalPosition = transform.worldToLocalMatrix.MultiplyPoint3x4(canvasLocalPosition);

        Debug.Log(canvasLocalPosition);

    }
  
}

