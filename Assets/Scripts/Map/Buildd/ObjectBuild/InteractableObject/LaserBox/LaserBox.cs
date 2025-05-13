using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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


    //private void Update()
    //{
    //    if(onLaser)
    //    {
    //        curRecvoerRate += Time.deltaTime;
    //        if(curRecvoerRate > maxRecoverRate)
    //        {
    //            LaserReset();
    //        }
    //    }
    //}

    //bool onLaser;
    //float maxRecoverRate = 0.1f;
    //float curRecvoerRate = 0;
    
    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        //onLaser = true;
        //curRecvoerRate = 0;
        //Laser();
    }

    #region Laser

    RaycastHit2D rh;
    Ray ray;

    public void Laser()
    {
        Vector2 start;
        Vector2 dir;
        start = (Vector2)transform.position + (Vector2.right * 0.5f);
        dir = Vector2.right;
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
    //private void LaserReset()
    //{
    //    _lineRenderer.positionCount = 0;
    //    onLaser = false;
    //    curRecvoerRate = 0;
    //}

    private void DrawLaser(int num, Vector2 start, Vector2 endPos)
    {
        _lineRenderer.positionCount = num + 2;
        _lineRenderer.SetPosition(num, start);
        _lineRenderer.SetPosition(num + 1, endPos);
    }
}

#endregion





/**
 * 1. LaserDrain 
 *  -> Charging Laser Energy
 *      -> OverCharged -> Explode
 * **/