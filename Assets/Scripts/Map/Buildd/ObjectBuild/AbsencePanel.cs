using System.Collections;
using UnityEngine;

public class AbsencePanel : MonoBehaviour
{

    [SerializeField] GameObject _Air;
    [SerializeField] GameObject _Hook;

    private Vector3 targetScaleUp = new Vector3(1.5f, 1.5f);
    //private float scaleSpeed;

    private bool _OnAir, _OnHook;


    Coroutine _CharacterFadeEffectCoroutineAir;
    Coroutine _CharacterFadeEffectCoroutineHook;

    public void OnAbsencePanel()
    {
        gameObject.SetActive(true);
        StartCoroutine(ScaleCoroutine(transform,targetScaleUp,Vector3.one));
    }

    public void Enter(GameObject obj)
    {
        if (obj.TryGetComponent(out Hook hook))
        {
            if (_CharacterFadeEffectCoroutineHook != null)
            {
                StopCoroutine(_CharacterFadeEffectCoroutineHook);
                _CharacterFadeEffectCoroutineHook = null;
            }
            _CharacterFadeEffectCoroutineHook = StartCoroutine(ScaleCoroutine(_Hook.transform, new Vector3(.8f, .8f), new Vector3(.4f, .4f)));

            _OnHook = true;
            _Hook.SetActive(_OnHook);
        }
        else if (obj.TryGetComponent(out Air air))
        {
            if (_CharacterFadeEffectCoroutineAir != null)
            {
                StopCoroutine(_CharacterFadeEffectCoroutineAir);
                _CharacterFadeEffectCoroutineAir = null;
            }
            _CharacterFadeEffectCoroutineAir = StartCoroutine(ScaleCoroutine(_Air.transform, new Vector3(.8f, .8f), new Vector3(.4f, .4f)));

            _OnAir = true;
            _Air.SetActive(_OnAir);
        }
    }

    public void Exit(GameObject obj)
    {
        if (obj.TryGetComponent(out Hook hook))
        {
            _OnHook = false;
            _Hook.SetActive(false);
        }
        else if (obj.TryGetComponent(out Air air))
        {
            _OnAir = false;
            _Air.SetActive(false);
        }
    }


    public void NextMoveAnimation()
    {
        Debug.Log("Animation");
    }


    IEnumerator ScaleCoroutine(Transform transform,Vector3 targetScaleUp, Vector3 targetScale)
    {

        yield return StartCoroutine(ScaleTo(transform,targetScaleUp));
        yield return StartCoroutine(ScaleTo(transform,targetScale));
    }

    IEnumerator ScaleTo(Transform transform,Vector3 targetScale)
    {
        float percent = 0;
        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
        {
            percent += Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, percent);
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
