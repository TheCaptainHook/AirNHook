using UnityEditor;
using UnityEngine;

public class AudioSourceController : MonoBehaviour
{
    [field: SerializeField] private AudioSource _audioSource;
    private Transform _playerTransform;
    private Transform _soundTransform;
    private float _activeDistnace;
    private float _clipLength;
    private float _elapsedTime;
    private bool _init;
    private bool _isBGM;
    private bool _isRecycled;
    private bool _isLoop;
    private bool _is3DSound;
    private bool _destroyWhenTargetDestroyed;
    private SoundManager _soundManager;

    public bool IsRecycled => _isRecycled;
    public void MarkRecycled() => _isRecycled = true;

    private void Start()
    {
        _soundManager = Managers.Sound;
    }

    private void OnEnable()
    {
        _soundTransform = null;
        _playerTransform = null;
        _elapsedTime = 0f;
        _init = false;
        _isRecycled = false;
        _destroyWhenTargetDestroyed = false;
        _audioSource.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _audioSource.clip = null;
    }

    /// <summary>
    /// 일반 사운드
    /// </summary>
    /// <param name="isLoop"></param>
    /// <param name="isBGM"></param>
    /// <param name="is3DSound"></param>
    /// <param name="distance"></param>
    public void Initialize(bool isLoop, bool isBGM, bool is3DSound, float distance = 10f)
    {
        _init = true;

        _isLoop = isLoop;
        _isBGM = isBGM;
        _is3DSound = is3DSound;
        _activeDistnace = distance + 10f;
        _clipLength = _audioSource.clip.length;

        _audioSource.priority = 0;
    }
    /// <summary>
    /// Transform을 따라가는 사운드
    /// </summary>
    /// <param name="isLoop"></param>
    /// <param name="soundObj"></param>
    /// <param name="is3DSound"></param>
    /// <param name="distance"></param>
    public void Initialize(bool isLoop, Transform soundObj, bool destroyWhenTargetDestroyed, bool is3DSound, float distance)
    {
        _init = true;

        _isLoop = isLoop;
        _soundTransform = soundObj;
        _destroyWhenTargetDestroyed = destroyWhenTargetDestroyed;
        _is3DSound = is3DSound;
        _activeDistnace = distance + 10f;
        _clipLength = _audioSource.clip.length;

        transform.position = _soundTransform.position;
    }

    /// <summary>
    /// soundPos에서 재생되는 사운드
    /// </summary>
    /// <param name="isLoop"></param>
    /// <param name="soundPos"></param>
    /// <param name="is3DSound"></param>
    /// <param name="distance"></param>
    public void Initialize(bool isLoop, Vector3 soundPos, bool is3DSound, float distance)
    {
        _init = true;

        _isLoop = isLoop;
        _is3DSound = is3DSound;
        _activeDistnace = distance + 10f;
        _clipLength = _audioSource.clip.length;

        transform.position = soundPos;
    }

    private void Update()
    {
        if (!_init)
            return;

        if (_is3DSound && _playerTransform == null)
        {
            if (Managers.Game.Player == null)
                return;

            _playerTransform = Managers.Game.Player.transform;

            if (_playerTransform == null)
                return;
        }

        if (_isRecycled)
            return;

        if ((_isBGM || !_is3DSound) && !_audioSource.gameObject.activeSelf)
        {
            _audioSource.gameObject.SetActive(true);
            _audioSource.Play();
            return;
        }

        if (!_is3DSound)
        {
            if (!_isBGM && !_audioSource.isPlaying)
                Recylce();

            return;
        }

        float sqrDistance = (_playerTransform.position - transform.position).sqrMagnitude;
        bool inRange = sqrDistance <= _activeDistnace * _activeDistnace;

        if (_audioSource.gameObject.activeSelf != inRange)
        {
            _audioSource.gameObject.SetActive(inRange);

            if (inRange)
            {
                if (_elapsedTime >= _clipLength && !_isLoop)
                {
                    Recylce();
                    return;
                }

                _audioSource.time = (_elapsedTime >= _clipLength) ? 0 : _elapsedTime;
                _audioSource.Play();
            }
        }

        if (_soundTransform != null)
        {
            transform.position = _soundTransform.position;
        }
        else if (_destroyWhenTargetDestroyed)
        {
            Recylce();
            return;
        }

        if (_isLoop)
        {
            _elapsedTime = (_elapsedTime >= _clipLength) ? _elapsedTime - _clipLength : _elapsedTime;
            return;
        }

        if (_elapsedTime >= _clipLength)
        {
            Recylce();
        }

        _elapsedTime += Time.deltaTime;
    }

    private void Recylce()
    {
        _isRecycled = true;
        _audioSource.gameObject.SetActive(false);
        _soundManager.Recycle(this);
    }

    public AudioSource GetAudioSource()
    {
        return _audioSource;
    }


    public void ClipChange(AudioClip audioClip,bool isLoop)
    {
        _audioSource.clip = audioClip;
        _clipLength = audioClip.length;
        _elapsedTime = 0;
        _isLoop = isLoop;
        _audioSource.loop = _isLoop;
        
        _audioSource.priority = 0;
        _audioSource.Play();
    }
}
