using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    private NetworkRigidbodyUnreliable2D netRb;
    private NetworkRigidbodyUnreliable2D NetRb { get { netRb ??= GetComponent<NetworkRigidbodyUnreliable2D>(); return netRb; } }

    #region  Main
    [Header("Save Data Field")]
    public bool onEncapsulationItem;
    public int activeRequirAmount;
    public INDICATOR indicator;


    [ReadOnly]
    public int curActiveRequirAmount;
    private CapsulObject capsuleObject;
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

    #region Indicator_2 Path Chaking
    public void Indicator_2PathChaking(GameObject targetObj)
    {
        var netId = targetObj.TryGetComponent(out NetworkIdentity identity) ? identity.netId : 9999;
        if (netId == 9999) return;

        Debug.Log("[2] Encapsulation Indicator 2 Path Chack->Net");
        Net.Server_Indicator_2_Path_Chacking(netId);
    }
    #endregion

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
            capsuleObject = Instantiate(Resources.Load<GameObject>(GlobalText.CAPSULE_OBJECT)).GetComponent<CapsulObject>(); //default : false, Polling
            capsuleObject.transform.position = Main.ObjectData.position;
        }
        Connection();

        capsuleObject.Resize(transform, mainColliderBounds);
        // Size Change Effect


    }

    private void Connection()
    {
        TransformParentNull();
        // Main.transform.SetParent(capsuleObject.insertTr);
        // capsuleObject.transform.SetParent(Main.transform);
        capsuleObject.transform.SetParent(MapEditor.Instance.networkingObjectTransform);
        Main.transform.SetParent(capsuleObject.insertTr);
        Main.transform.localPosition = new Vector2(0,-(mainCol.offset.y/2));

    }
    private void Disconnection()
    {
        TransformParentNull();
        Main.transform.SetParent(orgParent);
        capsuleObject.transform.SetParent(MapEditor.Instance.dontSaveObjectTransform);
        
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
        //Sound
        Managers.Sound.PlaySound3D(GlobalText.CAPSULE_UNCAPSULING, transform.position);
        //Sound

        //Return parent
        Disconnection();
        //Return parent

        capsuleObject.Recover(transform,()=> {
            mainCol.enabled = true;
            mainRb.simulated = true;
        });

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
