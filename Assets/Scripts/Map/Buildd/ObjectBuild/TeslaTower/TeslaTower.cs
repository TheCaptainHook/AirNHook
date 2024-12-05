using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO 0723 Develop code Line : 51,101

enum Insulator
{
    LightningRod,
    LeverHead
}
public class TeslaTower : BuildObj
{
    [CustomHeader("TeslaTower")]
    [SerializeField] ParticleSystem[] chargeEffects;
    [SerializeField] ParticleSystem[] lightningEffects;
    [SerializeField] Transform lightningBox;
    [SerializeField] TeslaBezierCurve bezierCurve;
    [SerializeField] Transform lineRendererContainer;
    [SerializeField] Material lightningShaderMat;
    public float lightningRate;
    public bool onCharge;

    Queue<GameObject> lineRendererQueue;

    private int enterBoundaryPlayerAmount;
    public int EnterBoundaryPlayerAmount
    {
        get
        {
            return enterBoundaryPlayerAmount;
        }
        set
        {
            enterBoundaryPlayerAmount += value;

            enterBoundaryPlayerAmount = Mathf.Clamp(enterBoundaryPlayerAmount, 0, 2);
            if (enterBoundaryPlayerAmount == 0)
            {
                StopChargeEffect();
            }
            else
            {
                StartChargeEffect();
            }
            

        }
    }

    //TODO 0723
    [SerializeField] float detectionRadiusX=4;// 감지 범위의 X축 반지름
    [SerializeField] float detectionRadiusY=3; // 감지 범위의 Y축 반지름
    private Vector3 detectOffset = new Vector3(0, 1.5f);
    private HashSet<GameObject> detectedObjects = new HashSet<GameObject>();
    private System.Type[] _DetectObjComponentTypes = { typeof(PlayerSM), typeof(BuildObj) };


    private float maxLightningRate = 1f;
    private float curLightningRate = 0;
    //TODO 0723


    private void Start()
    {
        lineRendererQueue = new Queue<GameObject>();

        for (int i = 0; i < 3; i++)
        {
            GameObject obj = new GameObject("LineRenderer");
            LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 1f;
            lineRenderer.sortingLayerName = "ForeGround";
            lineRenderer.sortingOrder = 100;
            lineRenderer.material = lightningShaderMat;
            obj.transform.SetParent(lineRendererContainer);
            obj.SetActive(false);
            lineRendererQueue.Enqueue(obj);
        }

    }

    private void Update()
    {
        DetectObjectsWithComponents(_DetectObjComponentTypes);


        if (onCharge)
        {
            curLightningRate += Time.deltaTime;
            if (curLightningRate >= maxLightningRate)
            {
                Check_DetectObjectsAndLightning();
                curLightningRate = 0;
            }
        }   
    }


    #region DetectObjects
//     private void DetectObjectsWithComponents(System.Type[] componentTypes)
// {
//     float maxRadius = Mathf.Max(detectionRadiusX, detectionRadiusY);
//     Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, maxRadius);

//     Vector2 position = (Vector2)transform.position + detectOffset;
//     HashSet<GameObject> currentDetectedObjects = new HashSet<GameObject>();

//     foreach (Collider2D collider in colliders)
//     {
//         GameObject obj = collider.gameObject;

//         if (obj == gameObject || currentDetectedObjects.Contains(obj)) continue;

//         Vector2 objPosition = obj.transform.position;

//         foreach (var type in componentTypes)
//         {
//             if (obj.GetComponent(type) != null)
//             {
//                 if (IsInsideEllipse(position, objPosition, detectionRadiusX, detectionRadiusY))
//                 {
//                     currentDetectedObjects.Add(obj);
//                     break;
//                 }
//             }
//         }
//     }

//     // 새로 탐지된 객체
//     var newDetectedObjects = new HashSet<GameObject>(currentDetectedObjects);
//     newDetectedObjects.ExceptWith(detectedObjects);
//     detectedObjects.UnionWith(newDetectedObjects);

//     // 떠난 객체
//     detectedObjects.IntersectWith(currentDetectedObjects);
// }
    //TODO 0723
    private void DetectObjectsWithComponents(System.Type[] componentTypes)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Mathf.Max(detectionRadiusX, detectionRadiusY));
        HashSet<GameObject> currentDetectedObjects = new HashSet<GameObject>();
        Vector2 position = transform.position + detectOffset;
        CheckDetectObjectsIsInsideEllipse(componentTypes,currentDetectedObjects,colliders,position);
        detectedObjects.IntersectWith(currentDetectedObjects);
    }

    private void CheckDetectObjectsIsInsideEllipse(
        System.Type[] componentTypes,
        HashSet<GameObject> currentDetectedObjects,
        Collider2D[] colliders,
        Vector2 position
        )
    {
        
        Vector2 objPosition;
        foreach (Collider2D collider in colliders)
        {
            GameObject obj = collider.gameObject;

            if (collider.gameObject == gameObject) continue;

            foreach (var type in componentTypes)
            {
                var component = obj.GetComponent(type);
                
                if (component != null)
                {
                    // Vector2 position = transform.position + detectOffset;
                    objPosition = obj.transform.position;

                    // 타원 내에 있는지 체크
                    if (IsInsideEllipse(position, objPosition, detectionRadiusX, detectionRadiusY))
                    {
                        currentDetectedObjects.Add(obj);

                        if (!detectedObjects.Contains(obj))
                        {
                            detectedObjects.Add(obj);
                        }
                        break;
                    }
                }
            }
        }
    }

    private void Check_DetectObjectsAndLightning()
    {
        Debug.Log("TEST 1");
        if (detectedObjects.Count > 0 && onCharge)
        {
            #region take care IInsulator, delet this region code line
            foreach(var obj in detectedObjects) //Find LightningRod
            {
                if(obj.TryGetComponent(out LightningRod lightningRod))
                {
                    DrawLineRenderer(obj.transform, lightningRod.hitPoint);
                    lightningRod.Electric();
                    return;
                }
            }
            #endregion

            foreach(var obj in detectedObjects)
            {
                if(obj.TryGetComponent(out BuildObj buildObj))
                {
                    if (CheckInsulator(buildObj.id))
                    {
                        DrawLineRenderer(obj.transform, obj.transform);
                        buildObj.TakeDamage();

                        // 플레이어가 오브젝트를 들고있는 경우 제대로 작동 안댐
                        return;
                    }
                    
                }
            }

            foreach(var obj in detectedObjects)
            {
                Debug.Log(obj.name);
                if(obj.layer == LayerMask.NameToLayer("Player"))
                {
                    DrawLineRenderer(obj.transform, obj.transform);
                    obj.GetComponent<PlayerSM>().TakeDamage(DamageType.Electric);
                }
            }

        }
    }



    //TODO 0723
    private bool CheckInsulator(int id)
    {
        string name = Managers.Data.mapData.mapObjectDataDictionary[id].name;
        return Enum.IsDefined(typeof(Insulator), name);
    }

    private bool IsInsideEllipse(Vector2 center, Vector2 point, float radiusX, float radiusY)
    {
        float dx = point.x - center.x;
        float dy = point.y - center.y;
        return (dx * dx) / (radiusX * radiusX) + (dy * dy) / (radiusY * radiusY) <= 1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // 디버그 타원의 색상을 빨간색으로 설정

        // 타원의 세그먼트 수
        int segments = 100;
        Vector3[] points = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;
            float x = Mathf.Cos(angle) * detectionRadiusX;
            float y = Mathf.Sin(angle) * detectionRadiusY;
            points[i] = new Vector3(transform.position.x + x, transform.position.y + y, 0) + detectOffset;
        }

        for (int i = 0; i < segments; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }
    #endregion

    #region Effect

    public void StartChargeEffect()
    {
        onCharge = true;
        foreach (ParticleSystem ps in chargeEffects)
        {
            ps.Play();
        }

    }

    public void StopChargeEffect()
    {
        onCharge = false;
        foreach (ParticleSystem ps in chargeEffects)
        {
            ps.Stop();
        }
    }

    private void StartLightningEffect()
    {
        foreach (ParticleSystem ps in lightningEffects)
        {
            ps.Play();
        }
    }

    #endregion


    public void Lightning(GameObject target)
    {
        ///
        /// If the Lightning Rod is within the attack range
        /// Unconditionally, a Lightning Rod attack.
        ///
        if (target.TryGetComponent(out LightningRod lightningRod1))
        {
            DrawLineRenderer(target.transform, lightningRod1.hitPoint);
            lightningRod1.Electric();
            return;
        }

        if (target.gameObject.TryGetComponent(out BuildObj buildObj))
        {
            DrawLineRenderer(target.transform, target.transform);
            buildObj.TakeDamage();
            return;
        }


        if (target.TryGetComponent(out Hook hook))
        {
            LightningRod lightningRod = hook.GetGrabbedItem<LightningRod>();
            if(lightningRod != null)
            {
                DrawLineRenderer(target.transform, lightningRod.hitPoint);
                return;
            }
            
        }

        if(target.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage();
        }


        //if (target.TryGetComponent(out Air air))
        //{
        //    Debug.Log("Air");
           
        //}

       

        return;


    }



    private void DrawLineRenderer(Transform target,Transform hitPoint)
    {
       Vector3 dir = (target.position - transform.position).normalized;
        GameObject newObj = lineRendererQueue.Dequeue();
        newObj.SetActive(true);
        StartLightningEffect();
        bezierCurve.Generator(newObj.GetComponent<LineRenderer>(), lightningBox.position, lightningBox.position + dir * 3, hitPoint.position);
        lineRendererQueue.Enqueue(newObj);
    }

}
