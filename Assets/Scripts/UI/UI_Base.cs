using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public abstract class UI_Base : MonoBehaviour
{
    public bool IsEnabled { get; private set; } = true;

    protected virtual void Start()
    {
        SetLanguage();
    }

    public abstract void OnEnable();
    protected virtual void OpenUI()
    {
        IsEnabled = true;
        gameObject.SetActive(true);
    }

    protected virtual void CloseUI()
    {
        gameObject.SetActive(false);
        IsEnabled = false;
    }

    public virtual void SetLanguage() { }
    
    public void SetSentence(TMP_Text target, int id)
    {
        var text = Managers.Data.language.GetSentence(id);
        if (text != " ")
            target.text = text;
    }
    
    
    //<summary> UI 애니메이션 관련 코드들 <summary/>
    protected IEnumerator Fade(bool isFadein, CanvasGroup _canvasGroup) 
    {
        float timer = 0f;
        while(timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            _canvasGroup.alpha = isFadein ? Mathf.Lerp(0f,1f,timer) : Mathf.Lerp(1f,0f,timer);
        }

        if(!isFadein)
        {
            CloseUI();
        }
    }
    protected void AppendAnim(GameObject mainFrame, float startScale, float startDuration, float endScale, float endDuration)
    {
        //등장 애니메이션
        var seq = DOTween.Sequence();
        seq.Append(mainFrame.transform.DOScale(startScale, startDuration));
        seq.Append(mainFrame.transform.DOScale(endScale, endDuration));
    }
    protected IEnumerator BounceRoutine(GameObject titleImg,Vector3 startSize, Vector3 endSize, AnimationCurve curve)
    {
        float current = 0;
        float percent = 0;
        
        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / 1;

            titleImg.transform.localScale = Vector3.Lerp(startSize, endSize, curve.Evaluate(percent));

            yield return null;
        }

        StartCoroutine(BounceRoutine(titleImg, endSize, startSize, curve));
    }
    
}
