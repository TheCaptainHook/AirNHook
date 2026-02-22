using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingTurret_LockonMark : MonoBehaviour
{
    [SerializeField] private HomingTurret_Net _net;
    [SerializeField] private Transform _mark;
    [SerializeField] private Transform _main;
    [SerializeField] private Transform _sub;
    private Transform _target;

    [SerializeField] Vector2 _offset;
    private float _timeOffset;
    void Awake()
    {
        // _cur_mainScale = _main_minScale;
        // _cur_subScale = _sub_minScale;
        _timeOffset = Random.Range(0f, 10f);
    }
    // void Update()
    // {
    //     if(_target != null && _mark.gameObject.activeSelf)
    //     {
    //         //TargetAnimation();
    //     }
    // }
    AudioSourceController _audioSourceController;


    private readonly int _lockOnHash = Animator.StringToHash("LockedOn");
    private Animator ani;
    private Animator Ani {get {ani ??= _mark.GetComponent<Animator>(); return ani; } }
    public void LockOn(Transform target)
    {
        if(_net.Get_Cur_fireCount != 0) return;
        
        if(_target == target) return;
        _target = target;

        if(!_mark.gameObject.activeSelf) _mark.gameObject.SetActive(true);

        if(_targetting_Coroutine != null)
        {
            StopCoroutine(_targetting_Coroutine);
            // _audioSourceController = _audioSourceController != null ? Managers.Sound.StopSound(_audioSourceController) : null;
            _targetting_Coroutine = null;
        }

        
        _targetting_Coroutine = StartCoroutine(Targetting());
        
        Ani.SetTrigger(_lockOnHash);
    }

    public void LockOff()
    {
        _target = null;
        if(_targetting_Coroutine != null)
        {
            StopCoroutine(_targetting_Coroutine);
            _targetting_Coroutine = null;
        }

        Managers.Sound.StopSound(_audioSourceController);
        _audioSourceController = null;


       if(_mark.gameObject.activeSelf)
        {
            foreach (var p in Ani.parameters)
            {
                if (p.type == AnimatorControllerParameterType.Trigger)
                Ani.ResetTrigger(p.nameHash);
            }

            Ani.Rebind();
            Ani.Update(0f); // 즉시 반영

            _mark.gameObject.SetActive(false);
        }
       
        
    }
    public void Reset()
    {
        StopAllCoroutines();
        _targetting_Coroutine = null;
        _target = null;

        if(_audioSourceController != null)
        {
            Managers.Sound.StopSound(_audioSourceController);
            _audioSourceController = null;
        }

        if(_mark.gameObject.activeSelf)
        {
            foreach (var p in Ani.parameters)
            {
                if (p.type == AnimatorControllerParameterType.Trigger)
                Ani.ResetTrigger(p.nameHash);
            }

            Ani.Rebind();
            Ani.Update(0f); // 즉시 반영

            _mark.gameObject.SetActive(false);
        }
      

    }

    private Coroutine _targetting_Coroutine;
    private IEnumerator Targetting()
    {
        _audioSourceController = Managers.Sound.PlaySound3D(GlobalText.MISSILE_LOCK_ALERT,_target.position);

        if(!_mark.gameObject.activeSelf) _mark.gameObject.SetActive(true);
        while(_target != null)
        {
            transform.position = _target.position + (Vector3)_offset;
            _audioSourceController.gameObject.transform.position = _target.position;

            yield return null;
        }
    }


    // [SerializeField] private float _main_minScale = 0.8f;
    // [SerializeField] private float _main_maxScale = 1.15f;
    // [SerializeField] private float _sub_minScale = 0.8f;
    // [SerializeField] private float _sub_maxScale = 1.6f;
    // [SerializeField] private float _animation_speed = 2f;
    // private float _cur_mainScale;
    // private float _cur_subScale;
    // private float _percent;

    // private void TargetAnimation()
    // {
    //     float t = (Time.time + _timeOffset) * _animation_speed;
    //     _percent = Mathf.PingPong(t, 1f);
        
    //     _cur_mainScale = Mathf.Lerp(_main_minScale, _main_maxScale, _percent);
    //     _cur_subScale = Mathf.Lerp(_sub_minScale, _sub_maxScale, _percent);

    //     _main.localScale = Vector3.one * _cur_mainScale;
    //     _sub.localScale = Vector3.one * _cur_subScale;

    // }
}
