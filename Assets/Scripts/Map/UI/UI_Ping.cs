
using Mirror;
using System.Collections;
using System.Text;
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
    [SerializeField] Sprite[] pingLevelSprites;

    private PingCriteria curPingCriteria;

    private WaitForSeconds wait;

    protected void Start()
    {
        wait = new WaitForSeconds(.5f);
        curPingCriteria = PingCriteria.Black;
    }

    public float ping;
    public void StartPingCheck(bool isServer)
    {
        StopAllCoroutines();
        if(isServer)
        {
            ServerPingCheck();
        }
        else
        {
            ClientPingCheck();
        }
    }
    #region Server

    private Coroutine serverPingCheckCoroutine;
    public void ServerPingCheck()
    {
        if (serverPingCheckCoroutine != null) StopCoroutine(serverPingCheckCoroutine);
        serverPingCheckCoroutine = StartCoroutine(CheckNetworkLatency());
    }
    PingCriteria previousPingCriteria;
    

    IEnumerator CheckNetworkLatency()
    {
        while (true)
        {
            Ping ping = new Ping("8.8.8.8");
            float timeout = 1f;
            float startTime = Time.time;

            while (!ping.isDone && (Time.time - startTime < timeout))
            {
                yield return null;
            }

            if (!ping.isDone)
            {
                curPingCriteria = PingCriteria.Black;
                ChangeImage(curPingCriteria);
                netPingText.text = $"No Connection";
                this.ping = 99999;
            }
            else
            {
                float latency = ping.time;
                // UI 표시 & 처리
                PingCriteria criteria = GetPingCriteriaSwich(latency);
                if (criteria != curPingCriteria)
                {
                    curPingCriteria = criteria;
                    ChangeImage(criteria);
                }
                if(criteria == PingCriteria.Black)
                {
                    netPingText.text = $"No Connection";
                    this.ping = 99999;
                }else{
                    netPingText.text = $"{Mathf.Floor(latency)} ms";
                    this.ping = latency;
                }
                
            }

            yield return null;
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
            //------UI
            var ping =(float)NetworkTime.rtt*1000;
            previousPingCriteria = GetPingCriteriaSwich(ping);
            if (previousPingCriteria != curPingCriteria)
            {
                curPingCriteria = previousPingCriteria;
                ChangeImage(curPingCriteria);
                netPingText.text = $"{Mathf.Floor(ping)} ms";
                this.ping = ping;
            }
            //------UI
            yield return null;
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
    }
    private void ChangeImage(PingCriteria pingCriteria)
    {
        switch (pingCriteria)
        {
            case PingCriteria.Green:
                image.sprite = pingLevelSprites[0];
                break;
            case PingCriteria.Yellow:
                image.sprite = pingLevelSprites[1];
                break;
            case PingCriteria.Orange:
                image.sprite = pingLevelSprites[2];
                break;
            case PingCriteria.Red:
                image.sprite = pingLevelSprites[3];
                break;
            case PingCriteria.Black:
                image.sprite = pingLevelSprites[4];
                break;
        }

    }
    #endregion
}

