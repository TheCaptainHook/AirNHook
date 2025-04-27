using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO 0723 Develop code Line : 51,101

enum Insulator
{
    LightningRod,
    LeverHead,
    Battery
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
    [SerializeField] AnimationCurve lightningLineCurve;
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
    [SerializeField] float detectionRadiusX=5;// 감지 범위의 X축 반지름
    [SerializeField] float detectionRadiusY=4.5f; // 감지 범위의 Y축 반지름
    private Vector3 detectOffset = new Vector3(0, 1.5f);
    private HashSet<GameObject> detectedObjects = new HashSet<GameObject>();
    private System.Type[] _DetectObjComponentTypes = { typeof(PlayerSM), typeof(BuildObj) };


    private float maxLightningRate = 2f;
    [ReadOnly]
    public float curLightningRate = 0;
    //TODO 0723


    private void Start()
    {
        lineRendererQueue = new Queue<GameObject>();

        for (int i = 0; i < 3; i++)
        {
            GameObject obj = new GameObject("LineRenderer");
            LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
            //lineRenderer.startWidth = 0.1f;
            lineRenderer.widthCurve = lightningLineCurve;
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
        curLightningRate += Time.deltaTime;

        if (onCharge)
        {   
            DetectObjectsWithComponents(_DetectObjComponentTypes);
            if (curLightningRate >= maxLightningRate && detectedObjects.Count > 0)
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
        // Vector2 position = transform.position + detectOffset;
        Vector2 position = (Vector2)transform.TransformPoint(detectOffset);
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
        if (detectedObjects.Count > 0 && onCharge)
        {
            #region take care IInsulator, delet this region code line
            foreach(var obj in detectedObjects) //Find LightningRod
            {
                if(obj.TryGetComponent(out LightningRod lightningRod))
                {
                    DrawLineRenderer(lightningBox, lightningRod.hitPoint);
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
                        DrawLineRenderer(lightningBox, obj.transform);
                        buildObj.TakeDamage(DamageType.Electric);

                        return;
                    }
                    
                }
            }

            foreach(var obj in detectedObjects)
            {
                if(obj.layer == LayerMask.NameToLayer("Player"))
                {
                    DrawLineRenderer(lightningBox, obj.transform);
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
        // float dx = point.x - center.x;
        // float dy = point.y - center.y;
        // return (dx * dx) / (radiusX * radiusX) + (dy * dy) / (radiusY * radiusY) <= 1;
            // 1. 월드 좌표에서 타원 중심과 검사할 점 사이의 차이를 구합니다.
        Vector2 diff = point - center;
        
        // 2. 오브젝트의 회전 각도를 라디안 단위로 구합니다.
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        
        // 3. 차이 벡터를 오브젝트의 로컬 좌표계로 변환하기 위해 역회전시킵니다.
        float cos = Mathf.Cos(-angle);
        float sin = Mathf.Sin(-angle);
        float localX = diff.x * cos - diff.y * sin;
        float localY = diff.x * sin + diff.y * cos;
        
        // 4. 표준 타원 방정식 적용 (로컬 좌표에서 타원은 축에 평행)
        return (localX * localX) / (radiusX * radiusX) + (localY * localY) / (radiusY * radiusY) <= 1f;
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


    // public void Lightning(GameObject target)
    // {
    //     ///
    //     /// If the Lightning Rod is within the attack range
    //     /// Unconditionally, a Lightning Rod attack.
    //     ///
    //     if (target.TryGetComponent(out LightningRod lightningRod1))
    //     {
    //         DrawLineRenderer(target.transform, lightningRod1.hitPoint);
    //         lightningRod1.Electric();
    //         return;
    //     }

    //     if (target.gameObject.TryGetComponent(out BuildObj buildObj))
    //     {
    //         DrawLineRenderer(target.transform, target.transform);
    //         buildObj.TakeDamage();
    //         return;
    //     }


    //     if (target.TryGetComponent(out HookSM hook))
    //     {
    //         Transform item = hook.GetGrabbedItem();
    //         LightningRod lightningRod = item.GetComponent<LightningRod>();
    //         if (lightningRod != null)
    //         {
    //             DrawLineRenderer(target.transform, lightningRod.hitPoint);
    //             return;
    //         }
            
    //     }

    //     if(target.TryGetComponent(out IDamageable damageable))
    //     {
    //         damageable.TakeDamage(DamageType.Electric);
    //     }

    //     return;


    // }



    private void DrawLineRenderer(Transform start,Transform hitPoint)
    {
    //    Vector3 dir = (target.position - transform.position).normalized;
        GameObject newObj = lineRendererQueue.Dequeue();
        newObj.SetActive(true);
        StartLightningEffect();
        StartCoroutine(DrawLineRendererCoroutine(start.position,(Vector2)hitPoint.position));
        // bezierCurve.Generator(newObj.GetComponent<LineRenderer>(), lightningBox.position, lightningBox.position + dir * 3, hitPoint.position);
        lineRendererQueue.Enqueue(newObj);
    }
    private float electricSpeed =5;
    private IEnumerator DrawLineRendererCoroutine(Vector2 start,Vector2 end)
    {
        float percent = 0;
        var line = lineRendererQueue.Dequeue().GetComponent<LineRenderer>();
        line.gameObject.SetActive(true);
        
        line.positionCount =2;
        line.SetPosition(0,start);
        
        while(percent <1)
        {
            percent += Time.deltaTime * electricSpeed;
            var pot = Vector2.Lerp(start,end,percent);
            line.SetPosition(1,pot);
            yield return null;
        }

        while(percent>0)
        {
            percent -= Time.deltaTime * 3;
            var pot = Vector2.Lerp(end,start,percent);
            line.SetPosition(0,pot);
            yield return null;
        }
        line.positionCount = 0;
        line.gameObject.SetActive(false);
        lineRendererQueue.Enqueue(line.gameObject);
    }

}
