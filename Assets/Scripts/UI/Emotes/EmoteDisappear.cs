using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EmoteDisappear : MonoBehaviour
{
    [SerializeField] private GameObject _mainFrame;
    
    private void OnEnable()
    {
        Show();
        StartCoroutine(C0_OnTimeClear());
    }

    private IEnumerator C0_OnTimeClear()
    {
        yield return new WaitForSeconds(3f);
        Disapper();
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
    
    private void Show()                         
    {
        var seq = DOTween.Sequence();

        seq.Append(_mainFrame.transform.DOScale(0f, 0f));
        seq.Append(_mainFrame.transform.DOScale(1.3f, 0.15f));
        seq.Append(_mainFrame.transform.DOScale(1f, 0.05f));
    }
    private void Disapper()                         
    {
        var seq = DOTween.Sequence();
        
        seq.Append(_mainFrame.transform.DOScale(1.3f, 0.15f));
        seq.Append(_mainFrame.transform.DOScale(0f, 0.05f));
    }
    
}
