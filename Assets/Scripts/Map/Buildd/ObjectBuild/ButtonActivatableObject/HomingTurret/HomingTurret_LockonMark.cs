using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingTurret_LockonMark : MonoBehaviour
{
    [SerializeField] private Transform _mark;
    [SerializeField] private Transform _main;
    [SerializeField] private Transform _sub;
    private Transform _target;

    [SerializeField] Vector2 _offset;
    private float _timeOffset;
    void Awake()
    {
        _cur_mainScale = _main_minScale;
        _cur_subScale = _sub_minScale;
        _timeOffset = Random.Range(0f, 10f);
    }
    void Update()
    {
        if(_target != null && _mark.gameObject.activeSelf)
        {
            TargetAnimation();
        }
    }

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


    [SerializeField] private float _main_minScale = 0.8f;
    [SerializeField] private float _main_maxScale = 1.15f;
    [SerializeField] private float _sub_minScale = 0.8f;
    [SerializeField] private float _sub_maxScale = 1.6f;
    [SerializeField] private float _animation_speed = 2f;
    private float _cur_mainScale;
    private float _cur_subScale;
    private float _percent;

    private void TargetAnimation()
    {
        float t = (Time.time + _timeOffset) * _animation_speed;
        _percent = Mathf.PingPong(t, 1f);
        
        _cur_mainScale = Mathf.Lerp(_main_minScale, _main_maxScale, _percent);
        _cur_subScale = Mathf.Lerp(_sub_minScale, _sub_maxScale, _percent);

        _main.localScale = Vector3.one * _cur_mainScale;
        _sub.localScale = Vector3.one * _cur_subScale;

    }
}
