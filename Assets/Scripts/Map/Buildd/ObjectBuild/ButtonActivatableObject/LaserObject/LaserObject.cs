
using UnityEngine;

public class LaserObject : ActivatableObjectEntity
{
    [CustomHeader("LaserObject")]
    public float _curDistanceRay;


    private Ray ray;

    #region Editor Property
    public Coroutine editor_showLaserCoroutine;
    #endregion




    public override void Activation()
    {
        Net.Server_ChangeOnActive(true);
    }

    public override void Deactivated()
    {
        Net.Server_ChangeOnActive(false);
    }
  


    #region  Editor

    [Space(20)]
    [Header("Editor")]
    [SerializeField] GameObject _endVFX;
    [SerializeField] public LineRenderer _lineRenderer;
    [SerializeField] ParticleSystem hitEffectParticle;
    [SerializeField] private Transform _firePoint;
    [SerializeField] LayerMask _mask;
    private bool isEnabled;

#if UNITY_EDITOR
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

        for (int i = 0; i < 10; i++)
        {
            ray = new Ray(start, dir);
            RaycastHit2D rh = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, _mask);
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

