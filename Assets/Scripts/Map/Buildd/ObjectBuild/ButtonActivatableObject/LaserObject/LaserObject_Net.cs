using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class LaserObject_Net : ActivatableObject_Net_Entity
{
    [SerializeField] public LineRenderer _lineRenderer;
    [SerializeField] public GameObject _endVFX;

    protected override void Active()
    {
        _endVFX.SetActive(true);
        _lineRenderer.enabled = true;
    }
    protected override void Deactive()
    {
        _endVFX.SetActive(false);
        _lineRenderer.enabled = false;
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

    private int _maxBounces = 5;
    private float _maxDistance = 200f;
    private int _mirrorLayer;
    [SerializeField] LayerMask _layerMask;
    private ContactFilter2D _defaultFilter;
    private RaycastHit2D[] _raycastHitBuffer = new RaycastHit2D[1];
    // public Vector3[] _linePoints = new Vector3[6];

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


    public void UpdateLaser_()
    {
        Vector2 start = _firePoint.position;
        Vector2 dir = transform.right;
        int segmentCount = 0;

        for (int i = 0; i < _maxBounces; i++)
        {
            int hits = Physics2D.Raycast(start, dir, _defaultFilter, _raycastHitBuffer, _maxDistance);
            //Cant find Target
            if (hits == 0)
            {
                _lineRenderer.positionCount = 0;
                break;
            }

            var rh = _raycastHitBuffer[0];
            Debug.Log(rh.collider.gameObject.name);
            Vector2 hitPoint = rh.point;
            DrawLaser(i, start, hitPoint);

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

                start = rh.point + rh.normal * 0.01f; 
                Vector2 reflected = Vector2.Reflect(dir, rh.normal).normalized;

                if (reflected == Vector2.zero || float.IsNaN(reflected.x) || float.IsNaN(reflected.y))
                {
                    break;
                }
                segmentCount++;

                dir = reflected;
            }
            else if (rh.collider.TryGetComponent(out LaserTriggerButton lt) && Application.isPlaying)
            {
                SetHitParticleRotate(start, hitPoint);
                lt.Charging();
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
