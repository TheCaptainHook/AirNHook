using DG.Tweening;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingDisappear : MonoBehaviour
{
    [SerializeField] private GameObject _mainFrame;

    private void OnEnable()
    {
        Show();
        Managers.Sound.PlaySound(GlobalText.UI_PING, 1f);
        StartCoroutine(C0_OnTimeClear());
    }

    private IEnumerator C0_OnTimeClear()
    {
        yield return new WaitForSeconds(3f);
        Disapper();
        Managers.Game.Player.GetComponent<PlayerSM>().PingRemoved();
        yield return new WaitForSeconds(0.2f);
        // Destroy(gameObject);
        if (NetworkServer.active)
        {
            Managers.Pooling.N_ReleaseToPool(gameObject);
        }else
        {
            gameObject.SetActive(false);
        }
        
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
