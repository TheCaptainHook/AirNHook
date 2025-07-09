using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UI_Bug_Report : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button sendButton;

    #region Wait Popup
    [SerializeField] GameObject waitPopup;
    [SerializeField] TextMeshProUGUI waitPopupText;
    #endregion


    private void Awake()
    {
        sendButton.onClick.AddListener(() => SendReport());
        
    }

    private string url = "https://script.google.com/macros/s/AKfycbytEO_r0M_tUjki68bZzwdKpabmoI9yytIbAguka7_qLOaAd9Xb3c0qc32L6PPy_g/exec";


    public void SendReport()
    {
        if (!SteamManager.Initialized) return;
        if (string.IsNullOrWhiteSpace(inputField.text)) return;

        var id = SteamUser.GetSteamID();
 
        ReportData report = new ReportData
        {
            id = id.m_SteamID.ToString(),
            nickName = SteamFriends.GetPersonaName(),
            content = inputField.text
        };

        StartCoroutine(PostToGoogleSheet(report));
    }
    IEnumerator PostToGoogleSheet(ReportData report)
    {
        sendButton.interactable = false;
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
    

        sendButton.interactable = true;
    }




}

[System.Serializable]
public class ReportData
{
    public string id;
    public string nickName;
    public string content;
}
