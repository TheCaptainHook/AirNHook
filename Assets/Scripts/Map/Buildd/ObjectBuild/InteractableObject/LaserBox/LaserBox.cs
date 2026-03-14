
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class LaserBox : InteractableObjectEntity
{
    [SerializeField] LineRenderer _lineRenderer;


    private LaserBox_Net Net => GetComponent<LaserBox_Net>();

 
    private void FixedUpdate()
    {
        if (onLaser)
        {
            curRecvoerRate += Time.fixedDeltaTime;
            if (curRecvoerRate > maxRecoverRate)
            {
                LaserReset();
            }
        }
    }

    public bool onLaser;
    float maxRecoverRate = 0.05f;
    float curRecvoerRate = 0;
  
    public void Laser(int segmentCount)
    {
        if(!_defaultFilter.useLayerMask)
        {
            _defaultFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = targetLayerMask,
                useTriggers = true
            };
            _mirrorLayer = LayerMask.NameToLayer("Mirror");
        }

        onLaser = true;
        curRecvoerRate = 0;

        UpdateLaser(segmentCount);
    }


    #region Audio
    public List<LaserEffectAudio> _laserEffectAudios;
    private void LaserAudio(int hits, Vector2 hitPoint)
    {
        if (_laserEffectAudios == null) _laserEffectAudios = new();
        //없는경우 사운드 할당
        if (_laserEffectAudios.Count < hits)
        {
            _laserEffectAudios.Add(new LaserEffectAudio(Managers.Sound.PlaySound3D(
                GlobalText.DRONE_LASER_SOUND, hitPoint, 1, true), hitPoint));
            return;
        }

        var source = _laserEffectAudios[hits - 1];
        if (!EqualsVector2(source.hitPoint, hitPoint))
        {
            source.source?.transform?.SetPositionAndRotation(new Vector3(hitPoint.x, hitPoint.y, 0f), Quaternion.identity);
            _laserEffectAudios[hits - 1] = new LaserEffectAudio(source.source, hitPoint);
        }

    }
    private bool EqualsVector2(Vector2 a, Vector2 b, float epsilon = 0.01f)
    {
        return Vector2.SqrMagnitude(a - b) < (epsilon * epsilon);
    }
    private void LaserAudioClean(int remain)
    {
        if (_laserEffectAudios.Count > remain)
        {
            for (int i = _laserEffectAudios.Count - 1; i >= remain; i--)
            {
                var source = _laserEffectAudios[i].source;
                source.GetAudioSource().Stop();
                Managers.Sound.Recycle(source);
                _laserEffectAudios.RemoveAt(i);
            }
        }
    }
    #endregion

    #region Laser

    private float _maxDistance = 200f;
    private int _mirrorLayer;
    [SerializeField] LayerMask targetLayerMask;
    private ContactFilter2D _defaultFilter;
    private RaycastHit2D[] _raycastHitBuffer = new RaycastHit2D[1];

    public void UpdateLaser(int remain)
    {
        Vector2 dir = Net.curLaserDir == Vector2.zero ? Vector2.right : Net.curLaserDir;
        Vector2 start = (Vector2)transform.position + (dir * 0.5f);

        int segmentCount = 0;
        Collider2D lastCol = null;

        for (int i = 0; i < remain; i++)
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
                //SetHitParticleRotate(start, hitPoint);
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
                //SetHitParticleRotate(start, hitPoint);
                lt.Charging();
                break;
            }
            else if (rh.collider.TryGetComponent(out LaserBox laserBox) && Application.isPlaying)
            {
                //SetHitParticleRotate(start, hitPoint);
                if (!laserBox.onLaser)
                {
                    int rm = remain - segmentCount;
                    laserBox.Laser(rm);
                }
                break;
            }
            else if (rh.collider.TryGetComponent(out BuildObj obj) && Application.isPlaying)
            {
                //SetHitParticleRotate(start, hitPoint);
                obj.TakeDamage(DamageType.Fire);
                break;
            }

            //SetHitParticleRotate(start, hitPoint);

        }

        LaserAudioClean(segmentCount);

    }
   
    private void LaserReset()
    {
        _lineRenderer.positionCount = 0;
        onLaser = false;
        curRecvoerRate = 0;

        for (int i = 0; i < _laserEffectAudios.Count; i++)
        {
            var source = _laserEffectAudios[i];
            source.source.GetAudioSource().Stop();
            Managers.Sound.Recycle(source.source);
        }
        _laserEffectAudios.Clear();
    }

    private void DrawLaser(int num, Vector2 start, Vector2 endPos)
    {
        _lineRenderer.positionCount = num + 2;
        _lineRenderer.SetPosition(num, start);
        _lineRenderer.SetPosition(num + 1, endPos);
    }


    #endregion

    public override void Reset()
    {
         LaserReset();
    }



}
