
using Mirror;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PingAlways : UI_Base
{
    public override void OnEnable()
    {
        if (NetworkServer.active) StartPingCheck(true);
        else StartPingCheck(false);
    }

    private PingCriteria curPingCriteria;

    private WaitForSeconds wait;
    //[SerializeField] Image image;
    [SerializeField] TextMeshProUGUI netPingText;
    public float ping;
    protected override void Start()
    {
        wait = new WaitForSeconds(1.5f);
        curPingCriteria = PingCriteria.Black;
    }

    public void StartPingCheck(bool isServer)
    {
        StopAllCoroutines();
        if (isServer)
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

    private Coroutine serverPingCheckCoroutine;
    PingCriteria previousPingCriteria;
    public void ServerPingCheck()
    {
        if (serverPingCheckCoroutine != null) StopCoroutine(serverPingCheckCoroutine);
        serverPingCheckCoroutine = StartCoroutine(CheckNetworkLatency());
    }
    IEnumerator CheckNetworkLatency()
    {
        while (true)
        {
            Ping ping = new Ping("8.8.8.8");
            float timeout = 2f;
            float startTime = Time.time;

            while (!ping.isDone && (Time.time - startTime < timeout))
            {
                yield return null;
            }

            if (!ping.isDone)
            {
                curPingCriteria = PingCriteria.Black;
                //ChangeImage(curPingCriteria);
            }
            else
            {
                float latency = ping.time;
                // UI 표시 & 처리
                PingCriteria criteria = GetPingCriteriaSwich(latency);
                if (criteria != curPingCriteria)
                {
                    curPingCriteria = criteria;
                    //ChangeImage(criteria);
                }

                netPingText.text = $"[S]{Mathf.Floor(latency)} ms";
                this.ping = latency;
            }

            yield return wait;
        }
    }
    private Coroutine clientPingCheckCoroutine;
    public void ClientPingCheck()
    {
        if (clientPingCheckCoroutine != null) StopCoroutine(clientPingCheckCoroutine);
        clientPingCheckCoroutine = StartCoroutine(ClientPingCheckCo());
    }

    IEnumerator ClientPingCheckCo()
    {
        while (true)
        {
            //PingCriteriaSwich(NetworkTime.rtt);
            //------UI
            var ping = (float)NetworkTime.rtt * 1000;
            previousPingCriteria = GetPingCriteriaSwich(ping);
            if (previousPingCriteria != curPingCriteria)
            {
                curPingCriteria = previousPingCriteria;
                //ChangeImage(curPingCriteria);
                netPingText.text = $"[C]{Mathf.Floor(ping)} ms";
                this.ping = ping;
            }

            //if PingCriteria.Orange , Show UI_Warning_Image

            //if PingCriteria.Orange , Show UI_Warning_Image
            //------UI
            yield return wait;
        }
    }

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
    //private void ChangeImage(PingCriteria pingCriteria)
    //{
    //    switch (pingCriteria)
    //    {
    //        case PingCriteria.Green:
    //            image.color = Color.green;
    //            break;
    //        case PingCriteria.Yellow:
    //            image.color = Color.yellow;
    //            break;
    //        case PingCriteria.Orange:
    //            image.color = new Color(1, 100 / 255f, 0, 1);
    //            break;
    //        case PingCriteria.Red:
    //            image.color = Color.red;
    //            break;
    //        case PingCriteria.Black:
    //            image.color = Color.black;
    //            break;


    //    }

    //}
}
