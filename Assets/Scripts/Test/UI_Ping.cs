
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

/** ping criteria
0~50    : green
50~100  : Yellow
100~150 : Orange
150~250 : Red
250~    : Black
**/

public class UI_Ping : MonoBehaviour
{
    #region Client Ping
    [SerializeField] TextMeshProUGUI pingText;
    #endregion
    #region Network Level
    [SerializeField] TextMeshProUGUI netPingText;
    
    #endregion
   

    private void Update()
    {
         if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P");
            StartCoroutine(CheckNetworkLatency());
        }
    }

    private float lastLatency = -1f;
    IEnumerator CheckNetworkLatency()
    {
         while (true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get("https://www.google.com"))
            {
                float startTime = Time.time;
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    float latency = (Time.time - startTime) * 1000; // ms 변환

                    if (Mathf.Abs(latency - lastLatency) > 1f) // 변화가 1ms 이상일 때만 업데이트
                    {
                        netPingText.text = $"{latency:F2} ms";
                        lastLatency = latency;
                    }
                }
                else
                {
                    netPingText.text = "No Connection";
                }
            }

            yield return new WaitForSeconds(3);
        }
    }

    Color pingColor;
    private void PingCriteriaSwich(double ping)
    {
       pingColor = ping switch
        {
            <= 50  => Color.green,
            <= 100 => Color.yellow,
            <= 150 => new Color(1, 150 / 255f, 0, 1),
            <= 250 => Color.red,
            _      => pingText.color
        };
        if(pingText.color != pingColor) pingText.color = pingColor;
        pingText.text = $"{ping} ms";
    }

}

