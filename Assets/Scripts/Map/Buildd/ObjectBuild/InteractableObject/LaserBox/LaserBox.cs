
using Mirror;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class LaserBox : BuildObj
{
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] LayerMask targetLayerMask;


    private LaserBox_Net Net =>GetComponent<LaserBox_Net>();

    public override void SetData<T>(T data)
    {
        base.SetData(data);
        Net.onSync = true;
        Net.Server_InitSync();
    }

    private void Awake()
    {
        parentConstraint = GetComponent<ParentConstraint>();
        DissolveInitSetting();
    }

   
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
    float maxRecoverRate = 0.1f;
    float curRecvoerRate = 0;
    public bool onBoom;
    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        if(onBoom) return;

        onLaser = true;
        curRecvoerRate = 0;
        GetLaserDir();
        //Laser();
        Laser(curLaserDir);

        //Only Server
        //if (NetworkServer.active)
        //    Net.Server_DamageCount();
    }

    private ParentConstraint parentConstraint;
    private void GetLaserDir()
    {
        if(parentConstraint.sourceCount >0)
        {
            Transform source = parentConstraint.GetSource(0).sourceTransform;
            Transform parent = source.parent.parent;
            float y = parent.rotation.y;
            if (y == 0) curLaserDir = Vector2.right;
            else  curLaserDir = Vector2.left;
        }

      
    }

    #region Laser
    public Vector2 curLaserDir;
    RaycastHit2D rh;
    Ray ray;

    public void Laser(Vector2 laserDir)
    {
        Vector2 start;
        Vector2 dir;
        dir = laserDir == Vector2.zero ? Vector2.right : laserDir;
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

                //Check collider
                if (rh.collider.TryGetComponent(out PlayerSM component) && Application.isPlaying)
                {
                    component.TakeDamage(DamageType.Fire);
                    break;

                }
                else if (rh.collider.gameObject.layer == LayerMask.NameToLayer("Mirror"))
                {
                    start = rh.point;
                    dir = Vector2.Reflect(ray.direction, colDir);
                }
                else if (rh.collider.TryGetComponent(out LaserTriggerButton component2))
                {
                    if (Application.isPlaying)
                    {
                        component2.Charging();
                    }
                    else
                    {
                    }
                    break;
                }
                else if (rh.collider.TryGetComponent(out BuildObj obj))
                {
                    if (obj.distructionStatus == DistructionStatus.Indestructible)
                    {
                        continue;
                    }
                    else
                    {
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

    public void Reset()
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