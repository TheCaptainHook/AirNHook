
using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/** ping criteria
0~50    : green
50~100  : Yellow
100~150 : Orange
150~250 : Red
250~    : Black
**/

public enum PingCriteria
{
    Green,
    Yellow,
    Orange,
    Red,
    Black
}
// public enum PingType
// {
//     Server,
//     Client
// }


public class UI_Ping : MonoBehaviour
{
    #region Client Ping

    #endregion
    #region Network Level
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI netPingText;
    #endregion

    private PingCriteria curPingCriteria;

    private WaitForSeconds wait;

    protected void Start()
    {
        wait = new WaitForSeconds(3);
        curPingCriteria = PingCriteria.Black;
    }

    // private void OnDisable()
    // {
    //     StopAllCoroutines();

    // }


    public float ping;
    public void StartPingCheck(bool isServer)
    {
        StopAllCoroutines();
        if(isServer)
        {
            ServerPingCheck();
            Debug.Log("Server Ping Check");
        }
        else
        {
            ClientPingCheck();
            Debug.Log("Client Ping Check");
        }
    }
    #region Server

    private Coroutine serverPingCheckCoroutine;
    public void ServerPingCheck()
    {
        if (serverPingCheckCoroutine != null) StopCoroutine(serverPingCheckCoroutine);
        serverPingCheckCoroutine = StartCoroutine(CheckNetworkLatency());
    }

    private float lastLatency = -1f;
    int sampleCount = 1;
    PingCriteria previousPingCriteria;
    IEnumerator CheckNetworkLatency()
    {
        while (true)
        {
            float totalLatency = 0f;
            int validSampleCount = 0; 

            for (int i = 0; i < sampleCount; i++)
            {
                Ping ping = new Ping("8.8.8.8");

                //Time Out
                float timeout = 2f;
                float startTime = Time.time;
                //Time Out

                while (!ping.isDone && (Time.time - startTime < timeout))
                {
                    yield return null;
                }

                if (!ping.isDone)
                {
                    Debug.LogWarning("Ping Timeout!");
                }
                else
                {
                    totalLatency += ping.time;
                    validSampleCount++;
                }
            }

            // 평균값 계산
            if (validSampleCount > 0)
            {
                float avgLatency = totalLatency / validSampleCount;

                if (Mathf.Abs(avgLatency - lastLatency) > 1f)
                {
                    //------UI
                    previousPingCriteria = GetPingCriteriaSwich(avgLatency);
                    if(previousPingCriteria != curPingCriteria)
                    {
                        curPingCriteria = previousPingCriteria;
                        ChangeImage(curPingCriteria);
                    }
                    //if PingCriteria.Orange , Show UI_Warning_Image

                    //if PingCriteria.Orange , Show UI_Warning_Image
                    //------UI
                    netPingText.text = $"[S]{Mathf.Floor(avgLatency)} ms";
                    lastLatency = avgLatency;
                    this.ping = avgLatency;
                }
            }
            else
            {
                curPingCriteria = PingCriteria.Black;
                ChangeImage(curPingCriteria);
                netPingText.text = "No Connection";
                //Debug.LogWarning("All Ping Timeout.");
            }

            // 3초마다 측정
            yield return wait;
        }
    }

    #endregion

    #region Client

    private Coroutine clientPingCheckCoroutine;
    public void ClientPingCheck()
    {
        if(clientPingCheckCoroutine != null) StopCoroutine(clientPingCheckCoroutine);
        clientPingCheckCoroutine = StartCoroutine(ClientPingCheckCo());
    }

    IEnumerator ClientPingCheckCo()
    {
        while(true)
        {
            //PingCriteriaSwich(NetworkTime.rtt);
            //------UI
            var ping =(float)NetworkTime.rtt*1000;
            previousPingCriteria = GetPingCriteriaSwich(ping);
            if (previousPingCriteria != curPingCriteria)
            {
                curPingCriteria = previousPingCriteria;
                ChangeImage(curPingCriteria);
                netPingText.text = $"[C]{Mathf.Floor(ping)} ms";
                this.ping = ping/1000f;
            }

            //if PingCriteria.Orange , Show UI_Warning_Image
                     
            //if PingCriteria.Orange , Show UI_Warning_Image
            //------UI
            yield return wait;
        }
    }


    #endregion





    #region Util

    private PingCriteria GetPingCriteriaSwich(float ping)
    {
        return ping switch
        {
            <= 50 => PingCriteria.Green,
            <= 100 => PingCriteria.Yellow,
            <= 150 => PingCriteria.Orange,
            <= 250 => PingCriteria.Red,
            _ => PingCriteria.Black
        };
        //if (pingText.color != pingColor) pingText.color = pingColor;
        //pingText.text = $"{ping} ms";
    }
    private void ChangeImage(PingCriteria pingCriteria)
    {
        switch (pingCriteria)
        {
            case PingCriteria.Green:
                image.color = Color.green;
                break;
            case PingCriteria.Yellow:
                image.color = Color.yellow;
                break;
            case PingCriteria.Orange:
                image.color = new Color(1, 100/255f, 0, 1);
                break;
            case PingCriteria.Red:
                image.color = Color.red;
                break;
            case PingCriteria.Black:
                image.color = Color.black;
                break;


        }

    }
    #endregion
}

