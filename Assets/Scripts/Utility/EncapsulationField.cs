using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncapsulationField : MonoBehaviour
{
    private BuildObj obj;
    private BuildObj Main { get { obj ??= GetComponent<BuildObj>(); return obj; } }

    private Transform parent => transform.parent;


    #region  Main
    [Header("Save Data Field")]
    public bool onEncapsulationItem;
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

        obstacleLayerMask = 1<<6;
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
            Capsuling();
        }  
    }
    //TEST


   

    private Transform orgParent;
    Bounds mainColliderBounds;
    public void Capsuling()
    {
        isCapsuling = true;

        //Main Object Setting
        // transform.position = Main.ObjectData.position;
        mainColliderBounds = mainCol.bounds;
        var distance = CheckUPAndDownDistance();
        Debug.Log(distance);

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
            capsuleObject = Instantiate(Resources.Load<GameObject>(GlobalText.CAPSULE_OBJECT)); //default : false
        }

        capsuleObject.transform.position = transform.position;
        // Capsule Object Setting
        orgParent = parent; //---Main cashing org parent

        TransformParentNull();
        capsuleObject.transform.SetParent(orgParent);
        Main.transform.SetParent(capsuleObject.transform);


        //capsuleObject Appearance Animation 
        //TEST
        if (!capsuleObject.activeSelf) //Animation
            capsuleObject.SetActive(true);
        //TEST
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

    private void TransformParentNull()
    {
        capsuleObject.transform.SetParent(null);
        Main.transform.SetParent(null);
    }
    public void UnCapsuling()
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
    }

    #region  Util
    private float maxSpace = 2;
    private float maxRayLenght = 2;
    public LayerMask obstacleLayerMask;



    private Collider2D[] colResult = new Collider2D[3];
    Vector2 boxCenter;
    Vector2 boxSize;

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     var bounds = Main._collider.bounds;

    //     var boxCenter = new Vector2(bounds.center.x, bounds.min.y - (maxRayLenght / 2f));
    //     var boxsize = new Vector2(0.5f, maxRayLenght);
    //     Gizmos.DrawCube(boxCenter, boxsize);
    // }
    private float CheckUPAndDownDistance()
    {
        boxCenter = new Vector2(mainColliderBounds.center.x, mainColliderBounds.min.y - (maxRayLenght/2f)) ;
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

            boxCenter = new Vector2(mainColliderBounds.center.x, mainColliderBounds.max.y + (maxRayLenght /2f));
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
    private float GetDistance(Vector2 target, Vector2 start)
    {
        return Mathf.Abs((target - start).y);
    }
    #endregion
    ///Capsuling
    /// 1. CapsulateField capsulateField = Instantiate(CapsulateField)
    /// 2. capsulateField.Setting()
    /// 3. ConstraintParent, Main : capsulateField, parts : Main.gameObject

    ///
    /// 
    /// 
    /// 1. Create Capsule Object,  string path, Find Capsule Object Logic, 
    /// 2. BuildObj col,rb setting,Check Vector2.Up,Down Ray
    /// 3. 
    /// 3. CapsuleObject SetParent( BuildObj.trasform.root)
    ///  
}
