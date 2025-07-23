using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UI_Bug_Report : UI_Base
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] Button sendButton;

    #region Wait Popup
    [SerializeField] GameObject waitPopup;
    [SerializeField] TextMeshProUGUI waitPopupText;
    [SerializeField] Button exitBtn;
    #endregion

    public override void OnEnable()
    {

    }


    private void Awake()
    {
        sendButton.onClick.AddListener(()=> StartCoroutine(DelaySendCo()));
        exitBtn.onClick.AddListener(CloseUI);       
    }
    protected override void CloseUI()
    {
        SafeClear();
    }
    public void SafeClear()
    {
        StartCoroutine(SafeClearCo(() => base.CloseUI()));
    }
    IEnumerator SafeClearCo(Action action)
    {
#if UNITY_STANDALONE_OSX
        Debug.Log("Mac_2);
        inputField.DeactivateInputField();
        yield return null;
        inputField.text = "";
        inputField.ForceLabelUpdate();
        yield return null;
        inputField.ActivateInputField();
#else
        yield return null;
        inputField.text = "";
        inputField.ForceLabelUpdate();
#endif
        action?.Invoke();
    }

    #region Mac
    private string beforeString;
    public void OnDeselect()
    {
//#if UNITY_STANDALONE_OSX
        string rawText = inputField.textComponent.text;
        Debug.Log("Mac_1");
        if (rawText.Contains("<u>") && rawText.Contains("</u>"))
        {
            string cleaned = Regex.Replace(rawText, "<.*?>", "");
            beforeString= cleaned;
            
            Debug.Log($"[macOS] 조합 문자열 강제 확정 후 복구됨]\n {cleaned}");
        }
//#endif
    }
    public void OnSelect()
    {
#if UNITY_STANDALONE_OSX
Debug.Log("Mac_Select");
        if(!string.IsNullOrEmpty(beforeString))
        inputField.text = beforeString;
#endif
    }
    private bool IsComposingText(string text)
    {
        try
        {
            Debug.Log("Mac_isComposingText");
            return text != null && text.Contains("<u>") && text.Contains("</u>");
        }
        catch
        {
            return true;
        }
    }
#endregion

    private string url = "https://script.google.com/macros/s/AKfycbyO1ZHhpfdUWEJmC5tz9VKEyOJPY4QlFQCckIqvp7UKQn4jIuPR41jy9t0kM-j9iDJY/exec";


    private void All_IsInteractable(bool value)
    {
        inputField.interactable = value;
        dropdown.interactable = value;
        sendButton.interactable = value;
        exitBtn.interactable = value;
    }
    private IEnumerator DelaySendCo()
    {
        yield return null;
        SendReport();
    }
    private void SendReport()
    {
        if (!SteamManager.Initialized) return;
        if (string.IsNullOrWhiteSpace(inputField.text)) return;

        StartCoroutine(PostToGoogleSheet());
        //Debug.Log(inputField.textComponent.text);
    }
    #region Util

    #endregion
    IEnumerator PostToGoogleSheet()
    {
        var id = SteamUser.GetSteamID();

        ReportData report = new ReportData
        {
            id = id.m_SteamID.ToString(),
            tag = dropdown.options[dropdown.value].text,
            nickName = SteamFriends.GetPersonaName(),
            content = inputField.textComponent.text
        };

        All_IsInteractable(false);

        waitPopup.SetActive(true);
        waitPopupText.text = "전송 중";

        string json = JsonUtility.ToJson(report);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 60;

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("업로드 성공: " + request.downloadHandler.text);
            waitPopupText.text = "전송 완료";
        }
        else
        {
            Debug.LogError("에러: " + request.error);
            waitPopupText.text = "전송 실패";
        }

        yield return new WaitForSeconds(0.5f);
        waitPopup.SetActive(false);

        All_IsInteractable(true);
    }




}

[System.Serializable]
public class ReportData
{
    public string id;
    public string tag;
    public string nickName;
    public string content;
}
