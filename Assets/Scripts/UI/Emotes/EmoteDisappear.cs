using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EmoteDisappear : MonoBehaviour
{
    [SerializeField] private GameObject _mainFrame;
    [SerializeField] private PingWheel_Item_Marker pingWheel_Item;
    private void OnEnable()
    {
        Show();
        StartCoroutine(C0_OnTimeClear());
        //Update Ping Position 0728
        //Update Ping Position 0728
    }

    private IEnumerator C0_OnTimeClear()
    {
        yield return new WaitForSeconds(5f);
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
