using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class EncapsulationField : MonoBehaviour
{
    private BuildObj obj;
    private BuildObj Main { get { obj ??= GetComponent<BuildObj>(); return obj; } }

    private Transform parent => transform.parent;
    private TransportItemEntity entity;
    public TransportItemEntity Net
    {
        get
        {
            entity ??= GetComponent<TransportItemEntity>();
            return entity;
        }
    }

    private ParentConstraint pc;
    private ParentConstraint ParentConstraint
    {
        get { pc ??= GetComponent<ParentConstraint>(); return pc; }
    }

    #region  Main
    [Header("Save Data Field")]
    public bool onEncapsulationItem;
    public int activeRequirAmount;
    public INDICATOR indicator;


    [ReadOnly]
    public int curActiveRequirAmount;
    private GameObject capsuleObject;
    [ReadOnly]
    public bool isCapsuling;
    #endregion


    private Collider2D mainCol;
    private Rigidbody2D mainRb;
    void Awake()
    {
        mainCol = GetComponent<Collider2D>();
        mainRb = GetComponent<Rigidbody2D>();

        obstacleLayerMask = 1 << 6;
    }

    #region Indicator
    public ActivatableObject_Indicator_var1 indicator_1;
    public ActivatableObject_Indicator_var2 indicator_2;

    private ActivatableObject_Indicator_var1 Create_Indicator_var_1()
    {
        var source = ResourceManager.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_1_Path);
        var item = Instantiate(source).GetComponent<ActivatableObject_Indicator_var1>();
        item.Setting(Net);

        return item;
    }

    private ActivatableObject_Indicator_var2 Create_Indicator_var_2()
    {
        var source = ResourceManager.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_2_Path);
        var item = Instantiate(source).GetComponent<ActivatableObject_Indicator_var2>();
        item.Setting(this, Net);
        return item;
    }

    #endregion

    #region  Capsuling
    private Transform orgParent;
    Bounds mainColliderBounds;
    public void Capsuling(Vector2 startPot) //Rpc
    {
        Main.canRespawn = false;
        isCapsuling = true;

        //Main Object Setting
        transform.position = startPot;
        transform.rotation = Quaternion.identity;

        StartCoroutine(CapsullingCoroutine(startPot));
    }
    private IEnumerator CapsullingCoroutine(Vector2 startPot)
    {
        yield return new WaitForFixedUpdate();

        mainColliderBounds = mainCol.bounds;
        var distance = CheckUPAndDownDistance();

        mainRb.simulated = false;
        mainCol.enabled = false;
        //Main Object Setting

        if (distance > 0)
        {
            yield return StartCoroutine(MoveCapsuleCo(distance));
        }
        else
        {
            SettingCapsule();
        }

        //----------------------------INDICATOR SETTING 0709 
        CheckIndicator(Net.data.indicator);
  

        //----------------------------INDICATOR SETTING 0709 
    }
    private void CheckIndicator(INDICATOR indicator)
    {
        switch (indicator)
        {
            case INDICATOR.TEXT:
                indicator_1 ??= Create_Indicator_var_1();
                break;
            case INDICATOR.MARK:
                indicator_2 ??= Create_Indicator_var_2();
                break;
            case INDICATOR.BOTH:
                break;
            default:
                break;
        }
    }

    private void SettingCapsule()
    {
        // Capsule Object Setting
        if (capsuleObject == null)
        {
            capsuleObject = Instantiate(Resources.Load<GameObject>(GlobalText.CAPSULE_OBJECT)); //default : false, Polling
        }

        capsuleObject.transform.position = transform.position;
        // Capsule Object Setting
        orgParent = parent; //---Main cashing org parent


        //capsuleObject Appearance Animation 
        if (!capsuleObject.activeSelf) //Animation
            capsuleObject.SetActive(true);

        TransformParentNull();
        capsuleObject.transform.SetParent(orgParent);
        Main.transform.SetParent(capsuleObject.transform);

        // Size Change Effect(Coroutine)
        Vector2 d = new Vector2(transform.position.x, transform.position.y - mainCol.offset.y);
        transform.position = d;
        // Size Change Effect

        //capsuleObject Appearance Animation 
    }


    private IEnumerator MoveCapsuleCo(float moveValue)
    {
        var start = transform.position;
        var end = transform.position + new Vector3(0, moveValue, 0);
        float t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime * 1.5f;
            float smoothedT = 1 - Mathf.Pow(1 - t, 3);
            transform.position = Vector3.Lerp(start, end, smoothedT);
            yield return null;
        }

        transform.position = end;
        SettingCapsule();
    }

    #endregion


    public void UnCapsuling() //Call Only Server,RPC
    {
        //capsuleObject Disappearance Animation 
        //Return Size
        //Return Size
        //capsuleObject Disappearance Animation 

        //Return parent
        TransformParentNull();
        Main.transform.SetParent(orgParent);
        capsuleObject.transform.SetParent(Main.transform);
        //Return parent

        //TEST
        capsuleObject.SetActive(false);
        //TEST

        mainCol.enabled = true;
        mainRb.simulated = true;

        Net.Reset_Interacable();

        isCapsuling = false;
        Main.canRespawn = true;
    }

    #region Main
    public void ApplyActive(int inc,uint id = 9999) //only Server
    {
        curActiveRequirAmount += inc;
       
            if (Net.data.indicator == INDICATOR.TEXT)
            {
                Net.Rpc_ApplyActive_Sync_var1(curActiveRequirAmount);
            }
            else if (Net.data.indicator == INDICATOR.MARK)
            {
                Net.Rpc_ApplyActive_Sync_var2(inc, id);

                return;
            }
            else if (Net.data.indicator == INDICATOR.BOTH)
            {
                return;
            }
            

            if (curActiveRequirAmount == activeRequirAmount)
            {
                // UnCapsuling();
                if (!isCapsuling)
                {
                    return;
                }

                Net.Rpc_UnCapsuling();
            }
        


    }


    public void CapsulReset() //only server
    {
        if (curActiveRequirAmount == activeRequirAmount)
        {
            Net.Reset_Interacable();
            return;
        }

        Net.Rpc_Capsuling();

    }

    #endregion

    #region  Util
    private float maxSpace = 2;
    private float maxRayLenght = 2;
    [ReadOnly]
    public LayerMask obstacleLayerMask;
    private Collider2D[] colResult = new Collider2D[3];
    Vector2 boxCenter;
    Vector2 boxSize;

    private float CheckUPAndDownDistance()
    {
        boxCenter = new Vector2(mainColliderBounds.center.x, mainColliderBounds.min.y - (maxRayLenght / 2f));
        boxSize = new Vector2(0.5f, maxRayLenght);
        int downHitCount = Physics2D.OverlapBoxNonAlloc(boxCenter, boxSize, 0, colResult, obstacleLayerMask);
        if (downHitCount > 0)
        {
            float moveValue = 0;
            float result = GetNearestCollider(downHitCount);

            if (result < maxSpace)
            {
                moveValue = maxSpace - result;
            }
            else return 0;

            boxCenter = new Vector2(mainColliderBounds.center.x, mainColliderBounds.max.y + (maxRayLenght / 2f));
            boxSize = new Vector2(0.5f, maxRayLenght);
            Array.Clear(colResult, 0, downHitCount);

            int upHitCount = Physics2D.OverlapBoxNonAlloc(boxCenter, boxSize, 0, colResult, obstacleLayerMask);

            if (upHitCount > 0)
            {
                float result2 = GetNearestCollider(upHitCount);
                Debug.Log("UP Hit Count");
                if (result2 < moveValue)
                {
                    return moveValue - result2;
                }
                else return moveValue;

            }
            else
            {
                return moveValue;
            }
        }
        else
        {
            return 0;
        }
    }
    private float GetNearestCollider(int hitCount)
    {
        float minDistance = Mathf.Infinity;
        for (int i = 0; i < hitCount; i++)
        {
            var dis = Mathf.Abs(colResult[i].ClosestPoint(transform.position).y - transform.position.y);
            if (minDistance > dis)
            {
                minDistance = dis;
                Debug.Log($"{colResult[i].name}, {minDistance}");
            }

        }

        return minDistance;
    }
    

    private void TransformParentNull()
    {
        capsuleObject.transform.SetParent(null);
        Main.transform.SetParent(null);
    }
    
    #endregion

}
