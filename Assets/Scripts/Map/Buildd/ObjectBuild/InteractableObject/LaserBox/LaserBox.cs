
using Mirror;
using UnityEngine;


public class LaserBox : InteractableObjectEntity
{
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] LayerMask targetLayerMask;


    private LaserBox_Net Net => GetComponent<LaserBox_Net>();

    private void Update()
    {
        if (onLaser)
        {
            curRecvoerRate += Time.deltaTime;
            if (curRecvoerRate > maxRecoverRate)
            {
                LaserReset();
            }
        }
    }

    bool onLaser;
    float maxRecoverRate = 0.08f;
    float curRecvoerRate = 0;
    public bool onBoom;
    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        if (onBoom) return;

        onLaser = true;
        curRecvoerRate = 0;
        //GetLaserDir(); //
        //Laser();
        Laser(Net.curLaserDir);

        //Only Server
        //if (NetworkServer.active)
        //    Net.Server_DamageCount();
    }

    //private ParentConstraint parentConstraint;
    //public void GetLaserDir()
    //{
    //    if (parentConstraint.sourceCount > 0)
    //    {
    //        Transform source = parentConstraint.GetSource(0).sourceTransform;
    //        Transform parent = source.parent.parent;
    //        float y = parent.rotation.y;

    //        if (y == 0) Net.Cmd_SetLaserDir(Vector2.right);
    //        else Net.Cmd_SetLaserDir(Vector2.left);
    //    }
    //}

    #region Laser
    //public Vector2 curLaserDir;
    RaycastHit2D rh;
    Ray ray;

    public void Laser(Vector2 laserDir)
    {
        Vector2 start;
        Vector2 dir;

        dir = Net.curLaserDir == Vector2.zero ? Vector2.right : Net.curLaserDir;
        start = (Vector2)transform.position + (dir * 0.5f);
        ray = new Ray(start, dir);

        int hitCount = 0;

        for (int i = 0; i < 5; i++)
        {
            ray = new Ray(start, dir);
            rh = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, targetLayerMask);
            if (rh.collider != null)
            {
                Vector2 colDir = rh.normal;
                DrawLaser(i, start, rh.point);
                hitCount++;

                if (rh.collider.gameObject == gameObject) break;

                //Check collider
                if (rh.collider.TryGetComponent(out PlayerSM component) && Application.isPlaying)
                {
                    //Impact Effect

                    //Impact Effect
                    component.TakeDamage(DamageType.Fire);
                    break;

                }
                else if (rh.collider.gameObject.layer == LayerMask.NameToLayer("Mirror"))
                {
                    //start = rh.point;
                    //dir = Vector2.Reflect(ray.direction, colDir);
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
                        //Impact Effect

                        //Impact Effect
                        component2.Charging();
                    }

                    break;
                }
                else if (rh.collider.TryGetComponent(out BuildObj obj))
                {
                    if (obj.distructionStatus == DistructionStatus.Indestructible && !obj.TryGetComponent(out LaserBox laserBox))
                    {
                        continue;
                    }
                    else
                    {
                        //Impact Effect

                        //Impact Effect
                        if (Application.isPlaying)
                        {
                            obj.TakeDamage(DamageType.Fire);
                        }

                        break;
                    }


                }
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
    private void LaserReset()
    {
        _lineRenderer.positionCount = 0;
        onLaser = false;
        curRecvoerRate = 0;
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
        onBoom = false;
    }



}

/**
 * 1. LaserDrain 
 *  -> Charging Laser Energy
 *      -> OverCharged -> Explode
 * **/