using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CapsulObject : MonoBehaviour
{
    private Animator animator;
    private readonly int ACTIVE = Animator.StringToHash("Active");
    private Animator Animator { get { animator ??= GetComponent<Animator>(); return animator; } }
    private Collider2D col;
    private Collider2D Col { get { col ??= GetComponent<Collider2D>(); return col; } }
    [SerializeField] SortingGroup group;

    public Transform insertTr;
    public bool isActive;
    public void Active(bool onOff)
    {
        if (onOff) group.sortingOrder = 50;
        else group.sortingOrder = 5;

        isActive = onOff;

        Animator.SetBool(ACTIVE, onOff);

    }

    float resizeRatio = 0.7f;
    private Coroutine resizeCoroutine;
    private Vector3 itemOrgScale;
    public void Resize(Transform item, Bounds bounds)
    {
        if (recoverCoroutine != null)
        {
            StopCoroutine(recoverCoroutine);
            recoverCoroutine = null;
        }

        itemOrgScale = item.localScale;

        float capsuleHeight = Col.bounds.size.y * resizeRatio;

        float itemHeight = bounds.size.y;

        if (itemHeight > capsuleHeight)
        {
            float scaleRatio = capsuleHeight / itemHeight;
            Vector3 targetScale = item.localScale * scaleRatio;
            resizeCoroutine = StartCoroutine(ResizeCo(item, targetScale));
        }


    }
    float t = 0.5f;
    private IEnumerator ResizeCo(Transform item, Vector3 targetScale)
    {
        float percent = 0;
        while (percent < t)
        {
            percent += Time.deltaTime;
            item.localScale = Vector3.Lerp(itemOrgScale, targetScale, Mathf.Clamp01(percent));

            yield return null;
        }

        item.localScale = targetScale;
        Active(true);
        resizeCoroutine = null;
    }

    Coroutine recoverCoroutine;
    public void Recover(Transform item)
    {
        if (resizeCoroutine != null)
        {
            StopCoroutine(resizeCoroutine);
            resizeCoroutine = null;
        }
        recoverCoroutine = StartCoroutine(RecoverCo(item));
    }
    private IEnumerator RecoverCo(Transform item)
    {
        float percent = 0;
        while (percent < t)
        {
            percent += Time.deltaTime;
            item.localScale = Vector3.Lerp(item.localScale, itemOrgScale, Mathf.Clamp01(t));

            yield return null;
        }
        item.localScale = itemOrgScale;
        recoverCoroutine = null;

    }

}
