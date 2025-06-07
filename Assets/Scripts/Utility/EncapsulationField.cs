using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        obstacleLayerMask = ~0;
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
    public void Capsuling()
    {
        // float moveValue = CheckUPAndDownDistance();
        // Debug.Log($"33333 {moveValue}");

        isCapsuling = true;
        //Main Object Setting
        mainRb.simulated = false;
        mainCol.enabled = false;
        //Main Object Setting

        // Capsule Object Setting
        if (capsuleObject == null)
        {
            capsuleObject = Instantiate(Resources.Load<GameObject>(GlobalText.CAPSULE_OBJECT));
        }
        
       
        //capsuleObject Appearance Animation 
        capsuleObject.transform.position = transform.position;
        //TEST
        if(!capsuleObject.activeSelf)
        capsuleObject.SetActive(true);
        //TEST

        //capsuleObject Appearance Animation 

        //---Main cashing org parent
        orgParent = parent;
        TransformParentNull();

        capsuleObject.transform.SetParent(orgParent);
        Main.transform.SetParent(capsuleObject.transform);
        //---Main cashing org parent

        
        // if (moveValue != 0)
        // {
        //     Debug.Log($"Moveing Capsule : {moveValue}");
        //     StartCoroutine(MoveCapsuleCo(moveValue));
        //     // capsuleObject.transform.position += new Vector3(0, moveValue, 0);
        // }
        // Capsule Object Setting
    }
    

    private IEnumerator MoveCapsuleCo(float moveValue)
    {
        var start = capsuleObject.transform.position;
        var end = capsuleObject.transform.position += new Vector3(0, moveValue, 0);
        float t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime * 1.5f;
            float smoothedT = 1 - Mathf.Pow(1 - t, 3);
            capsuleObject.transform.position = Vector3.Lerp(start, end, smoothedT);
            yield return null;
        }
        capsuleObject.transform.position = end;
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
    private float maxSpace = 1;
    private float maxRayLenght = 2;
    public LayerMask obstacleLayerMask;
    private float skin = 0.01f;


    
    private float CheckUPAndDownDistance()
    {
        var startBottom = new Vector2(Main._collider.bounds.center.x, Main._collider.bounds.min.y - skin);
        var startTop = new Vector2(Main._collider.bounds.center.x, Main._collider.bounds.max.y + skin);

        var downRay = Physics2D.Raycast(startBottom, Vector2.down, maxRayLenght, obstacleLayerMask);
        var upRay = Physics2D.Raycast(startTop, Vector2.up, maxRayLenght, obstacleLayerMask);

        if (downRay.collider == null)
        {
            return 0;
        }
        else
        {
            float moveValue = 0;

            var downPoint = downRay.point;
            var distanceD = GetDistance(downPoint, transform.position);
            Debug.Log($"11 Down : {distanceD}");
            if (distanceD < maxSpace)
                moveValue += maxSpace - distanceD;

            if (upRay.collider != null)
            {
                var point = upRay.point;
                var distanceU = GetDistance(point, transform.position);
                Debug.Log($"222 up : {distanceU}");
                if (distanceU >= moveValue)
                {
                    return moveValue;
                }
                else
                {
                    return distanceU;
                }
            }
            return moveValue;
        }
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
