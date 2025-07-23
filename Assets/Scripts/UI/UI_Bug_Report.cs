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
        inputField.text = "";
        base.CloseUI();
    }

    #region Mac

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
