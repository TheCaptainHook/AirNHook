using System.Collections;
using UnityEngine;

public class AbsencePanel : MonoBehaviour
{

    [SerializeField] GameObject _Air;
    [SerializeField] GameObject _Hook;

    private Vector3 targetScaleUp = new Vector3(1.5f, 1.5f);

    public Coroutine _CharacterFadeEffectCoroutineAir;
    public Coroutine _CharacterFadeEffectCoroutineHook;

    public void OnAbsencePanel()
    {
        gameObject.SetActive(true);
        StartCoroutine(ScaleCoroutine(transform,targetScaleUp,Vector3.one));
    }

    public void HookPanelOpen()
    {
        if (_CharacterFadeEffectCoroutineHook != null)
        {
            StopCoroutine(_CharacterFadeEffectCoroutineHook);
            _CharacterFadeEffectCoroutineHook = null;
        }
        _Hook.SetActive(true);
        if (gameObject.activeSelf)
            _CharacterFadeEffectCoroutineHook = StartCoroutine(ScaleCoroutine(_Hook.transform, new Vector3(.8f, .8f), new Vector3(.4f, .4f)));


    }

    public void AirPanelOpen()
    {
        if (_CharacterFadeEffectCoroutineAir != null)
        {
            StopCoroutine(_CharacterFadeEffectCoroutineAir);
            _CharacterFadeEffectCoroutineAir = null;
        }
        _Air.SetActive(true);
        if(gameObject.activeSelf)
            _CharacterFadeEffectCoroutineAir = StartCoroutine(ScaleCoroutine(_Air.transform, new Vector3(.8f, .8f), new Vector3(.4f, .4f)));


    }


    public void HookPanelClose()
    {
        _Hook.SetActive(false);
    }
    public void AirPanelClose()
    {
        _Air.SetActive(false);
    }


    // public void Exit(GameObject obj)
    // {
    //     if (obj.TryGetComponent(out HookSM hook))
    //     {
    //         _OnHook = false;
    //         _Hook.SetActive(false);
    //     }
    //     else if (obj.TryGetComponent(out AirSM air))
    //     {
    //         _OnAir = false;
    //         _Air.SetActive(false);
    //     }
    // }



    IEnumerator ScaleCoroutine(Transform transform,Vector3 targetScaleUp, Vector3 targetScale)
    {

        yield return StartCoroutine(ScaleTo(transform,targetScaleUp));
        yield return StartCoroutine(ScaleTo(transform,targetScale));
    }

    IEnumerator ScaleTo(Transform transform,Vector3 targetScale)
    {
        float percent = 0;
        //while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
        while (percent <1)
        {
            percent += Time.deltaTime * 10;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, percent);
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
