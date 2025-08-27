
using UnityEngine;

public class LaserObject : ActivatableObjectEntity
{
    [CustomHeader("LaserObject")]
    //[SerializeField] private float _defDistanceRay = 50f;
    public float _curDistanceRay;







    //[SerializeField] private bool _isEnabled;
    //private bool onActive;

    // [Header("Effect")]


    private Ray ray;

    #region Editor Property
    public Coroutine editor_showLaserCoroutine;
    #endregion


    // private void FixedUpdate()
    // {
    //     if (!MapEditor.Instance.stageClear && !turnOff && Net.onActive)
    //     {
    //         // UpdateLaser();
    //         UpdateLaser_();
    //     }
    //     else if (MapEditor.Instance.stageClear && Net.onActive)
    //     {
    //         if (NetworkServer.active)
    //         {
    //             Net.Server_ChangeOnActive(false);
    //         }
    //     }

    // }

    public override void Activation()
    {
        Net.Server_ChangeOnActive(true);
    }

    public override void Deactivated()
    {
        Net.Server_ChangeOnActive(false);
    }
    //---------------------------------------------------------------------------Refactoring 0825
    // private int _maxBounces = 5;
    // private float _maxDistance = 200f;
    // private int _mirrorLayer;
    // [SerializeField] LayerMask _layerMask;
    // private ContactFilter2D _mirrorFilter;
    // private ContactFilter2D _defaultFilter;
    // private RaycastHit2D[] _raycastHitBuffer = new RaycastHit2D[1];
    // public Vector3[] _linePoints = new Vector3[6];

    // public void Init()
    // {
    //     _mirrorLayer = LayerMask.NameToLayer("Mirror");
    //     _mirrorFilter = new ContactFilter2D
    //     {
    //         useLayerMask = true,
    //         layerMask = _layerMask,
    //         useTriggers = true
    //     };
    //     _defaultFilter = new ContactFilter2D
    //     {
    //         useLayerMask = true,
    //         layerMask = _layerMask,
    //         useTriggers = true
    //     };

    //     int needed = _maxBounces + 2;
    //     if (_linePoints == null || _linePoints.Length < needed)
    //         _linePoints = new Vector3[needed];
    // }

    // private void UpdateLaser_()
    // {
    //     Vector2 start = _firePoint.position;
    //     Vector2 dir = transform.right;
    //     int segmentCount = 0;
    //     _linePoints[segmentCount] = start;

    //     for (int i = 0; i < _maxBounces; i++)
    //     {
    //         int hits = Physics2D.Raycast(start, dir, _defaultFilter, _raycastHitBuffer, _maxDistance);
    //         //Cant find Target
    //         if (hits == 0)
    //         {
    //             _lineRenderer.positionCount = 0;
    //             break;
    //         }

    //         var rh = _raycastHitBuffer[0];
    //         Vector2 hitPoint = rh.point;
    //         // _linePoints[segmentCount] = start;
    //         // _linePoints[segmentCount + 1] = hitPoint;
    //         // segmentCount++;


    //         if (rh.collider.TryGetComponent(out PlayerSM player) && Application.isPlaying)
    //         {
    //             _linePoints[segmentCount + 1] = hitPoint;
    //             SetHitParticleRotate(start, hitPoint);
    //             player.TakeDamage(DamageType.Fire);
    //             break;
    //         }
    //         else if (rh.collider.gameObject.layer == _mirrorLayer)
    //         {
    //             float sqrDist = (hitPoint - start).sqrMagnitude;
    //             if (rh.distance < 0.01f || sqrDist < 0.0001f) break;

    //             start = rh.point + rh.normal * 0.01f; // ← 방향 벡터 대신 실제 normal 기반 밀어내기
    //             Vector2 reflected = Vector2.Reflect(ray.direction, rh.normal).normalized;

    //             if (reflected == Vector2.zero || float.IsNaN(reflected.x) || float.IsNaN(reflected.y))
    //             {
    //                 break;
    //             }
    //             _linePoints[segmentCount + 1] = hitPoint;
    //             segmentCount++;

    //             dir = reflected;
    //         }
    //         else if (rh.collider.TryGetComponent(out LaserTriggerButton lt) && Application.isPlaying)
    //         {
    //             SetHitParticleRotate(start, hitPoint);
    //             _linePoints[segmentCount + 1] = hitPoint;
    //             lt.Charging();
    //             break;
    //         }
    //         else if (rh.collider.TryGetComponent(out BuildObj obj) && Application.isPlaying)
    //         {
    //             SetHitParticleRotate(start, hitPoint);
    //             _linePoints[segmentCount + 1] = hitPoint;
    //             obj.TakeDamage(DamageType.Fire);
    //             break;
    //         }
    //         else SetHitParticleRotate(start, hitPoint);

    //     }

    //     if (segmentCount > 0)
    //     {
    //         _lineRenderer.positionCount = segmentCount;
    //         _lineRenderer.SetPositions(_linePoints);
    //     }


    // }
    //---------------------------------------------------------------------------Refactoring 0825



    #region  Editor
#if UNITY_EDITOR
    [Space(20)]
    [Header("Editor")]
    [SerializeField] GameObject _endVFX;
    [SerializeField] public LineRenderer _lineRenderer;
    [SerializeField] ParticleSystem hitEffectParticle;
    [SerializeField] private Transform _firePoint;
    [SerializeField] LayerMask _mask;

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
            RaycastHit2D rh = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, _mask);
            // int hits = Physics2D.Raycast(start, dir, _defaultFilter, _raycastHitBuffer, _maxDistance);
            // int hit = Physics2D.Raycast(start, dir, Mathf.Infinity, _layerMask);
            if (rh.collider != null)
            {
                Vector2 colDir = rh.normal;
                DrawLaser(i, start, rh.point);
                hitCount++;

                if (rh.collider.gameObject.layer == LayerMask.NameToLayer("Mirror"))
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
    private void DrawLaser(int num, Vector2 start, Vector2 endPos)
    {
        _lineRenderer.positionCount = num + 2;
        _lineRenderer.SetPosition(num, start);
        _lineRenderer.SetPosition(num + 1, endPos);
    }
    
#endif

    #endregion


}

