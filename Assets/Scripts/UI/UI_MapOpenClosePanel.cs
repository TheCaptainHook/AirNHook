using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MapOpenClosePanel : UI_Base
{
    #region Components
    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] TypingEffect typingEffect;
    #endregion


    #region Animation
    private readonly int PROGRASS_1 = Animator.StringToHash("Prograss_1");
    private readonly int PROGRASS_2 = Animator.StringToHash("Prograss_2");
    private readonly int PROGRASS_3 = Animator.StringToHash("Prograss_3");
    #endregion

    #region Loading Anim
    [SerializeField] GameObject loadingObj;
    [SerializeField] Image loadingImage;
    private Color loadingOrgColor = new Color(255,255,255,1);
    private Color loadingFadeOutColor = new Color(255,255,255,0);
    #endregion


    #region TopLayer
    [Header("Top Layer")]
    [SerializeField] Image topLayerPanelImg;
    private Color orgTopLayerColor = Color.black;
    private Color transparentTopLayerColor = new Color(0, 0, 0, 0);
    /// <summary>
    /// true : Fade In
    /// false : Fade Out
    /// </summary>
    /// <param name="onOff"></param>
    /// <returns></returns>
    public IEnumerator TopLayer_FadeInOut(bool onOff, float duration = 0.5f)
    {
        Color startColor = onOff ? transparentTopLayerColor : orgTopLayerColor;
        Color endColor = onOff ? orgTopLayerColor : transparentTopLayerColor;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float easedT = Mathf.SmoothStep(0f, 1f, t);
            topLayerPanelImg.color = Color.Lerp(startColor, endColor, easedT);
            yield return null;
        }

        topLayerPanelImg.color = endColor;
    }
    #endregion

    #region Map Info
    private Color typingDefaultColor = Color.white;
    private Map CurMap
    {
        get
        {
            var map = MapEditor.Instance.CurMap;
            return map == null ? null : map;

        }
    }
    [Header("Map Info")]
    [SerializeField] TextMeshProUGUI mapNameText;
    [SerializeField] TextMeshProUGUI mapAudioNameText;
    [SerializeField] RectTransform noteImg;
    #endregion

    #region  UI_Base
    public override void OnEnable()
    {
        mapNameText.text = "";
        mapAudioNameText.text = "";
    }

    #endregion


    //Test
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.I))
    //    {
    //        StartCoroutine(Prograss_1());
    //    }
    //    if (Input.GetKeyDown(KeyCode.O))
    //    {
    //        StartCoroutine(Prograss_2());
    //    }
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        StartCoroutine(Prograss_3());
    //    }
    //}

    #region  1
    public IEnumerator Prograss_1()
    {
        //Close Top Layer Animation
        animator.SetTrigger(PROGRASS_1);
        //Close Top Layer Animation
        yield return new WaitForSeconds(1f);

        //Loading Animation Start
        StartCoroutine(LoadingCo());
        //Loading Animation Start

        //Top layer Fade In
        yield return StartCoroutine(TopLayer_FadeInOut(true));
        //Top layer Fade In


    }
    #endregion

    #region 2
    public IEnumerator Prograss_2()
    {

        Map curMap = CurMap;
        string mapName = curMap.subMapName != string.Empty ? curMap.subMapName : curMap.mapID;
        string mapAudioName = curMap.audioName != string.Empty ? curMap.audioName : "";

        //Loading Animation End
        // StopCoroutine(loadingCoroutine);
        // loadingCoroutine = null;
        // loadingObj.SetActive(false);
        onCompleteLoading = true;
        //Loading Animation End

        //Change door Image
        //Change door Image

        //Top layer Fade Out

        yield return StartCoroutine(TopLayer_FadeInOut(false));
        //Top layer Fade Out

        //Open Top Layer Animation, InnerPanel Half Open ANimation
        animator.SetTrigger(PROGRASS_2);
        //Open Top Layer Animation

        yield return new WaitForSeconds(1.5f);

        ////Map Name Typing
        yield return StartCoroutine(typingEffect.NormalTyping(mapNameText, mapName, typingDefaultColor, 1, 60));
        //yield return StartCoroutine(typingEffect.NormalTyping(mapNameText, "ABCDEFGAAAAAAAAAAAAAA", typingDefaultColor, 1, 40));
        //Map Audio Typing
        if (mapAudioName != string.Empty)
        {
            yield return StartCoroutine(typingEffect.NormalTyping(mapAudioNameText, $"{mapAudioName}", typingDefaultColor, 1, 50));

            //yield return StartCoroutine(typingEffect.NormalTyping(mapAudioNameText, "mapAudioNamemapAudioName", typingDefaultColor, 1, 30));
        }




    }
    #endregion

    #region 3
    public IEnumerator Prograss_3()
    {
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(typingEffect.TextDissolveFromLeft(mapNameText));

        StartCoroutine(typingEffect.TextDissolveFromLeft(mapAudioNameText));

        yield return new WaitForSeconds(.5f);

        //InnerPanel Open
        animator.SetTrigger(PROGRASS_3);
        //InnerPanel Open
        yield return new WaitForSeconds(1.5f);
    }
    #endregion






    #region Loading
    private float loadingSpeed = 2.5f;
    private bool onCompleteLoading = false;
    private IEnumerator LoadingCo()
    {
        StartCoroutine(Loading_FadeOut(true));

        int increase = 1;
        float t = 0;
        while (true)
        {
            t += Time.deltaTime * increase * loadingSpeed;
            loadingImage.fillAmount = t;

            if (t >= 1 || t <= 0)
            {
                if (onCompleteLoading) break;

                increase *= -1;
                t = Mathf.Clamp01(t);
                loadingImage.fillClockwise = !loadingImage.fillClockwise;
            }
            yield return null;
        }

        // yield return StartCoroutine(Loading_EndingCo());
        yield return StartCoroutine(Loading_FadeOut(false));

        loadingImage.fillClockwise = true;
        loadingImage.fillAmount = 0;

        onCompleteLoading = false;
    }

    /// <summary>
    /// Fade In : True, Fade Out : False
    /// </summary>
    /// <param name="inOut"></param>
    /// <returns></returns>
    private IEnumerator Loading_FadeOut(bool inOut)
    {
        float percent = 0;
        Color cur = inOut ? loadingFadeOutColor : loadingOrgColor;
        Color target = inOut ? loadingOrgColor : loadingFadeOutColor;

        while (percent < 1)
        {
            percent += Time.deltaTime;
            loadingImage.color = Color.Lerp(cur, target, percent);
            yield return null;
        }
        loadingImage.color = target;
        
    }
    #endregion
}

