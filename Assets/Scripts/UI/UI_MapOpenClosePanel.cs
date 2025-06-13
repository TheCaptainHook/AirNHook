using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MapOpenClosePanel : UI_Base
{
    #region Components
    [SerializeField] Animator animator;
    [SerializeField] TypingEffect typingEffect;
    #endregion


    #region Animation

    #endregion

    #region Loading Anim
    #endregion


    #region TopLayer
    [SerializeField] Image topLayerPanelImg;
    private Color orgTopLayerColor = Color.black;
    private Color transparentTopLayerColor = new Color(0, 0, 0, 0);
    /// <summary>
    /// true : Fade In
    /// false : Fade Out
    /// </summary>
    /// <param name="onOff"></param>
    /// <returns></returns>
    public IEnumerator TopLayer_FadeInOut(bool onOff)
    {
        Color targetColr;
        Color curColr;
        if (onOff)
        {
            targetColr = orgTopLayerColor;
            curColr = transparentTopLayerColor;
        }
        else
        {
            targetColr = transparentTopLayerColor;
            curColr = orgTopLayerColor;
        }

        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime;
            topLayerPanelImg.color = Color.Lerp(curColr, targetColr, percent);
            yield return null;
        }

        topLayerPanelImg.color = targetColr;
    }
    #endregion

    #region Map Info
    private Map CurMap
    {
        get
        {
            var map = MapEditor.Instance.CurMap;
            return map == null ? null : map;

        }
    }

    #endregion

    #region  UI_Base
    public override void OnEnable()
    {

    }

    #endregion


    #region  1
    
    #endregion

}


///
/// 1. Close Top Layer Door(RL) -> Top Layer Panel FadeIn
/// 2. Loading Animation Start
/// 3. yield return new WaitUntil(()=>playerCameraView.isCameraCenter);
/// 4. Loading Animation End
/// 5. Open Top Layer Door(RL) -> Top Layer Panel FadeOut
/// 6. InnerPanel_1(Black)(RL) half Open,
///    InnerPanel_2(White)(RL) half Open,
///    Typing Stage Info Text
/// 7. fadeOut Info Text
/// 8. InnerPanel_12, final Open
/// 