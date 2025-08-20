
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserObject : ActivatableObjectEntity
{
    [CustomHeader("LaserObject")]
    //[SerializeField] private float _defDistanceRay = 50f;
    public float _curDistanceRay;

    [SerializeField] private Transform _firePoint;

    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private GameObject _endVFX;

    [SerializeField] LayerMask _layerMask;

    //[SerializeField] private bool _isEnabled;
    //private bool onActive;

    [Header("Effect")]
    [SerializeField] ParticleSystem hitEffectParticle;

    private Ray ray;

    #region Editor Property
    public Coroutine editor_showLaserCoroutine;
    #endregion


    private void FixedUpdate()
    {
        if (!MapEditor.Instance.stageClear && !turnOff && Net.onActive)
        {
            UpdateLaser();
        }
        else if (MapEditor.Instance.stageClear && Net.onActive)
        {
            if(NetworkServer.active)
            {
                Net.Server_ChangeOnActive(false);
            }
        }

    }
    public override void Activation()
    {
        Net.Server_ChangeOnActive(true);
    }

    public override void Deactivated()
    {
        Net.Server_ChangeOnActive(false);
    }

    private void UpdateLaser()
    {
        Vector2 start;
        Vector2 dir;
        start = _firePoint.position;
        dir = transform.right;

        int hitCount = 0;

        for (int i = 0; i < 5; i++)
        {
            ray = new Ray(start, dir);
            RaycastHit2D rh = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, _layerMask);
            if (rh.collider != null)
            {
                Vector2 colDir = rh.normal;
                DrawLaser(i, start, rh.point);
                hitCount++;
                
                if (rh.collider.TryGetComponent(out PlayerSM component) && Application.isPlaying)
                {
                    SetHitParticleRotate(start, rh.point); // todo 0914
                    component.TakeDamage(DamageType.Fire);
                    break;

                }
                else if (rh.collider.gameObject.layer == LayerMask.NameToLayer("Mirror"))
                {
                    if (rh.distance < 0.001f || Vector2.Distance(start, rh.point) < 0.01f)
                    {
                        break;
                    }

                    start = rh.point + rh.normal * 0.01f; // ← 방향 벡터 대신 실제 normal 기반 밀어내기
                    Vector2 reflected = Vector2.Reflect(ray.direction, rh.normal).normalized;

                    if (reflected == Vector2.zero || float.IsNaN(reflected.x) || float.IsNaN(reflected.y))
                    {
                        break;
                    }

                    dir = reflected;
                }
                else if (rh.collider.TryGetComponent(out LaserTriggerButton component2))
                {
                    if (Application.isPlaying)
                    {
                        SetHitParticleRotate(start, rh.point); // todo 0914
                        component2.Charging();
                    }
                
                    break;
                }
                else if(rh.collider.TryGetComponent(out BuildObj obj))
                {
                    SetHitParticleRotate(start, rh.point);
                    if (Application.isPlaying)
                    {
                        obj.TakeDamage(DamageType.Fire);
                    }

                    break;

                }

                SetHitParticleRotate(start, rh.point);

            }
            else
            {
                if (hitCount == 0)
                {
                    _lineRenderer.positionCount = 0;
                }
                break;
            }

        }

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

    #region  Editor
#if UNITY_EDITOR
    private bool isEnabled;
    public void Editor_Awake()
    {
        isEnabled = true;
        _endVFX.SetActive(isEnabled);
        _lineRenderer.enabled = isEnabled;
    }

    public void Editor_UpdateLaser()
    {
        if (isEnabled) UpdateLaser();
    }
    public void ResetLaser()
    {
        if (_lineRenderer == null) return;
        _lineRenderer.positionCount = 0;
        isEnabled = false;
        _endVFX.SetActive(isEnabled);
        _lineRenderer.enabled = isEnabled;
    }
#endif

    #endregion
    private void DrawLaser(int num, Vector2 start, Vector2 endPos)
    {
        _lineRenderer.positionCount = num + 2;
        _lineRenderer.SetPosition(num, start);
        _lineRenderer.SetPosition(num + 1, endPos);
    }

}

