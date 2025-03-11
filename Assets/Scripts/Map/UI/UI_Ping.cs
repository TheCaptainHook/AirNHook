
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
public enum PingType
{
    Server,
    Client
}


public class UI_Ping : UI_Base
{
    #region Client Ping

    #endregion
    #region Network Level
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI netPingText;
    #endregion

    private PingCriteria curPingCriteria;

    private WaitForSeconds wait;

    protected override void Start()
    {
        base.Start();
        wait = new WaitForSeconds(3);
        curPingCriteria = PingCriteria.Black;
    }
    public override void OnEnable()
    {
     
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        
    }



    public void StartPingCheck(PingType type)
    {
        StopAllCoroutines();
        if(type == PingType.Server)
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
                float timeout = 5f;
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
                    //------UI
                    netPingText.text = $"[Server] {Mathf.Floor(avgLatency)} ms";
                    lastLatency = avgLatency;
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
                netPingText.text = $"[Client] {Mathf.Floor(ping)} ms";
            }
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
                image.color = new Color(100 / 255f, 1, 0, 1);
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

