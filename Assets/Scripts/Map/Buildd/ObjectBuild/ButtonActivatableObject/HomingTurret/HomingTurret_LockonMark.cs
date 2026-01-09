using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingTurret_LockonMark : MonoBehaviour
{
    [SerializeField] private Transform _mark;
    private Transform _target;

    [SerializeField] Vector2 _offset;

    public void LockOn(Transform target)
    {
        if(_target == target) return;
        _target = target;
        if(!_mark.gameObject.activeSelf) _mark.gameObject.SetActive(true);

        if(_targetting_Coroutine != null)
        {
            StopCoroutine(_targetting_Coroutine);
            _targetting_Coroutine = null;
        }

        _targetting_Coroutine = StartCoroutine(Targetting());
    }

    public void LockOff()
    {
        _target = null;
        if(_targetting_Coroutine != null)
        {
            StopCoroutine(_targetting_Coroutine);
            _targetting_Coroutine = null;
        }
        _mark.gameObject.SetActive(false);
    }
    public void Reset()
    {
        StopAllCoroutines();
        _targetting_Coroutine = null;
        _target = null;
    }

    private Coroutine _targetting_Coroutine;
    private IEnumerator Targetting()
    {
        if(!_mark.gameObject.activeSelf) _mark.gameObject.SetActive(true);
        while(_target != null)
        {
            transform.position = _target.position + (Vector3)_offset;
            yield return null;
        }
    }
}
