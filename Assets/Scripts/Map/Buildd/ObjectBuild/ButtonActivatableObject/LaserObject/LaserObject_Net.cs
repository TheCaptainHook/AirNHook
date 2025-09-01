using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEditor;
using Unity.VisualScripting;


public class LaserObject_Net : ActivatableObject_Net_Entity
{
    [SerializeField] public LineRenderer _lineRenderer;
    [SerializeField] public GameObject _endVFX;

    protected override void Active()
    {
        _endVFX.SetActive(true);
        _lineRenderer.enabled = true;
        ActiveSound();
    }
    protected override void Deactive()
    {
        _endVFX.SetActive(false);
        _lineRenderer.enabled = false;
        DeactiveSound();
    }

    protected override void SetData(ButtonActivatableObjectStruct data)
    {
        Init();
        base.SetData(data);

    }


    private void FixedUpdate()
    {
        if (!MapEditor.Instance.stageClear && onActive)
        {
            // UpdateLaser();
            UpdateLaser_();
        }
        else if (MapEditor.Instance.stageClear && onActive)
        {
            if (NetworkServer.active)
            {
                Server_ChangeOnActive(false);
            }
        }

    }

    [Header("Effect")]
    [SerializeField] ParticleSystem hitEffectParticle;

    [SerializeField] private Transform _firePoint;

    private int _maxBounces = 7;
    private float _maxDistance = 200f;
    private int _mirrorLayer;
    [SerializeField] LayerMask _layerMask;
    private ContactFilter2D _defaultFilter;
    private RaycastHit2D[] _raycastHitBuffer = new RaycastHit2D[1];

    public void Init()
    {
        _mirrorLayer = LayerMask.NameToLayer("Mirror");

        _defaultFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = _layerMask,
            useTriggers = true
        };
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if(audioSourceController != null) Managers.Sound.StopSound(audioSourceController);
        if(_laserEffectAudios.Count > 0)
        {
            LaserAudioClean(0);
        }
    }

    #region Audio
    private AudioSourceController audioSourceController;
    private Coroutine activeSoundCo;
    private Coroutine deactiveSoundCo;
    private void ActiveSound()
    {
        if (deactiveSoundCo != null) StopCoroutine(deactiveSoundCo);
        activeSoundCo = StartCoroutine(ActiveSoundCo());
    }
    IEnumerator ActiveSoundCo()
    {
        var startClip = Managers.Sound.GetAudioClip(GlobalText.LASER_BEAM_START);
        Managers.Sound.PlaySound3D(GlobalText.LASER_BEAM_START, transform.position);
  
        if (audioSourceController == null) audioSourceController = Managers.Sound.PlaySound3D(GlobalText.LASER_BEAM_LOOP, transform.position, 1, true);
       
        var source = audioSourceController.GetAudioSource();
        while(source.volume < 1)
        {
            source.volume = Mathf.MoveTowards(0, source.volume, Time.deltaTime);
            yield return null;
        }
        activeSoundCo = null;
    }
    private void DeactiveSound()
    {
        if (audioSourceController != null)
        {
            if (activeSoundCo != null) StopCoroutine(activeSoundCo);
            deactiveSoundCo = StartCoroutine(DeactiveSoundCo());
        }
        
        LaserAudioClean(0);
    }

    private IEnumerator DeactiveSoundCo()
    {
        var source = audioSourceController.GetAudioSource();
        while (source.volume > 0)
        {
            source.volume = Mathf.MoveTowards(source.volume, 0, Time.deltaTime);
            yield return null;
        }
        Managers.Sound.StopSound(audioSourceController);
        audioSourceController = null;
        deactiveSoundCo = null;
    }

    public List<LaserEffectAudio> _laserEffectAudios;
    private void LaserAudio(int hits,Vector2 hitPoint)
    {
        if (_laserEffectAudios == null) _laserEffectAudios = new();
        //없는경우 사운드 할당
        if (_laserEffectAudios.Count < hits)
        {
            _laserEffectAudios.Add(new LaserEffectAudio(Managers.Sound.PlaySound3D(
                GlobalText.LASER_BEAM_LOOP,hitPoint,0.7f,true),hitPoint));
            return;
        }

        var source = _laserEffectAudios[hits-1];
        if (!EqualsVector2(source.hitPoint,hitPoint))
        {
            source.source?.transform?.SetPositionAndRotation(new Vector3(hitPoint.x, hitPoint.y, 0f), Quaternion.identity);
            _laserEffectAudios[hits-1] = new LaserEffectAudio(source.source, hitPoint);
        }

    }
    private bool EqualsVector2(Vector2 a, Vector2 b,float epsilon = 0.01f)
    {
        return Vector2.SqrMagnitude(a - b) < (epsilon * epsilon);
    }
    private void LaserAudioClean(int segmentCount)
    {
        if (_laserEffectAudios.Count > segmentCount)
        {
            for(int i = _laserEffectAudios.Count-1;i>=segmentCount;i--)
            {
                var source = _laserEffectAudios[i].source;
                source.GetAudioSource().Stop();
                Managers.Sound.Recycle(source);
                _laserEffectAudios.RemoveAt(i);
            }
        }
    }
    #endregion
    public void UpdateLaser_()
    {
        Vector2 start = _firePoint.position;
        Vector2 dir = transform.right;
        int segmentCount = 0;
        Collider2D lastCol = null;

        for (int i = 0; i < _maxBounces; i++)
        {
            int hits = Physics2D.Raycast(start, dir, _defaultFilter, _raycastHitBuffer, _maxDistance);
            if (hits == 0)
            {
                _lineRenderer.positionCount = 0;
                break;
            }

            var rh = _raycastHitBuffer[0];
            if (lastCol == rh.collider) return;

            lastCol = rh.collider;
            Vector2 hitPoint = rh.point;

            float segSqr = (hitPoint - start).sqrMagnitude;
            if (segSqr < 0.0001f) break;

            DrawLaser(i, start, hitPoint);
           
            segmentCount++;
            LaserAudio(segmentCount, hitPoint);

            if (rh.collider.TryGetComponent(out PlayerSM player) && Application.isPlaying)
            {
                SetHitParticleRotate(start, hitPoint);
                player.TakeDamage(DamageType.Fire);
                break;
            }
            else if (rh.collider.gameObject.layer == _mirrorLayer)
            {
                float sqrDist = (hitPoint - start).sqrMagnitude;
                if (rh.distance < 0.01f || sqrDist < 0.0001f) break;

                start = rh.point + rh.normal * 0.03f;
                Vector2 reflected = Vector2.Reflect(dir, rh.normal).normalized;

                if (reflected == Vector2.zero || float.IsNaN(reflected.x) || float.IsNaN(reflected.y))
                {
                    break;
                }

                dir = reflected;
            }
            else if (rh.collider.TryGetComponent(out LaserTriggerButton lt) && Application.isPlaying)
            {
                SetHitParticleRotate(start, hitPoint);
                lt.Charging();
                break;
            }
            else if (rh.collider.TryGetComponent(out LaserBox laserBox) && Application.isPlaying)
            {
                SetHitParticleRotate(start, hitPoint);
                
                if(!laserBox.onLaser)
                {
                    int remain = _maxBounces - segmentCount;
                    laserBox.Laser(remain);
                }
                break;
            }
            else if (rh.collider.TryGetComponent(out BuildObj obj) && Application.isPlaying)
            {
                SetHitParticleRotate(start, hitPoint);
                obj.TakeDamage(DamageType.Fire);
                break;
            }

            SetHitParticleRotate(start, hitPoint);

        }

        LaserAudioClean(segmentCount);

    }

    private void SetHitParticleRotate(Vector3 start, Vector3 hitPoint)
    {
        if (hitEffectParticle.transform.position != hitPoint)
        {
            hitEffectParticle.transform.position = hitPoint;
            _endVFX.transform.position = hitPoint;
        }

        Vector2 direction = hitPoint - start;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(angle, -90, 0);

        if (hitEffectParticle.transform.rotation != rotation)
        {
            hitEffectParticle.transform.rotation = Quaternion.Euler(angle, -90, 0);
            _endVFX.transform.rotation = Quaternion.Euler(angle, -90, 0);
        }


        hitEffectParticle.Play();

    }
    


    private void DrawLaser(int num, Vector2 start, Vector2 endPos)
    {
        _lineRenderer.positionCount = num + 2;
        _lineRenderer.SetPosition(num, start);
        _lineRenderer.SetPosition(num + 1, endPos);
    }

}
