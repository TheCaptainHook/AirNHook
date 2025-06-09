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
    private TransportItemEntity Net
    {
        get
        {
            entity ??= GetComponent<TransportItemEntity>();
            return entity;
        }
    }

    #region  Main
    [Header("Save Data Field")]
    public bool onEncapsulationItem;
    public int activeRequirAmount;
    [ReadOnly]
    public int curActiveRequirAmount;
    private GameObject capsuleObject;
    [ReadOnly]
    public bool isCapsuling;
    #endregion


    public Collider2D mainCol;
    public Rigidbody2D mainRb;
    void Awake()
    {
        mainCol = GetComponent<Collider2D>();
        mainRb = GetComponent<Rigidbody2D>();

        obstacleLayerMask = 1 << 6;
    }

    //TEST
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && onEncapsulationItem)
        {
            UnCapsuling();
        }
        if (Input.GetKeyDown(KeyCode.O) && onEncapsulationItem)
        {
            Capsuling(Main.position);
        }
    }
    //TEST



    #region  Capsuling
    private Transform orgParent;
    Bounds mainColliderBounds;
    public void Capsuling(Vector2 startPot) //Rpc
    {
        Main.canRespawn = false;
        isCapsuling = true;

        //Main Object Setting
        transform.position = startPot;
        mainColliderBounds = mainCol.bounds;
        var distance = CheckUPAndDownDistance();

        mainRb.simulated = false;
        mainCol.enabled = false;
        //Main Object Setting

        if (distance > 0)
        {
            StartCoroutine(MoveCapsuleCo(distance));
        }
        else
        {
            SettingCapsule();
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

        TransformParentNull();
        capsuleObject.transform.SetParent(orgParent);
        Main.transform.SetParent(capsuleObject.transform);


        //capsuleObject Appearance Animation 
            // Size Change Effect
            // Size Change Effect
        //TEST
        if (!capsuleObject.activeSelf) //Animation
            capsuleObject.SetActive(true);
        //TEST
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


    public void UnCapsuling() //Call Only Server
    {
        //capsuleObject Disappearance Animation 

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

        isCapsuling = false;
        Main.canRespawn = true;
    }


    #region Requir 
    public void ApplyActive(int amount) //only Server
    {
        curActiveRequirAmount += amount;
        if (curActiveRequirAmount == activeRequirAmount)
        {
            // UnCapsuling();
            if (!isCapsuling) return;
            Net.Rpc_UnCapsuling();
        }

    }


    public void CapsulReset() //only server
    {
        if (!onEncapsulationItem) return;
        if (curActiveRequirAmount == activeRequirAmount) return;

        Net.Rpc_Capsuling();

    }

    #endregion

    #region  Util
    private float maxSpace = 2;
    private float maxRayLenght = 2;
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
