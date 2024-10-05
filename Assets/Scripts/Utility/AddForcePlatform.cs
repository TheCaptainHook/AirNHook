

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AddForcePlatform : MonoBehaviour
{
    [CustomHeader("AddForcePlatform")]
    public float _LimitVelocity;

    [Header("Ray")]
    private float rayLength;
    public LayerMask layerMask;

    [Header("Main")]
    public HashSet<DetectObj> _PreviousDetactObjects;
    

    [Header("INIT")]
    private float w;
    private float h;
    Collider2D _Collider;
    Rigidbody2D _Rb;

    [SerializeField] MovingPlatform _MovingPlatform;


    private void Awake(){
        _Collider = GetComponent<Collider2D>();
        _Rb = GetComponent<Rigidbody2D>();
        _PreviousDetactObjects = new();

        var wh = GetColliderWH();
        w = wh.width;
        h = wh.height;

        _MovingPlatform.MoveAction+=AddForce;
    }

    private void Update(){
        //test
        if(Input.GetKeyDown(KeyCode.A)){
           foreach(DetectObj obj in _PreviousDetactObjects){
            Debug.Log(obj.obj.name);
           }
        }
    }
    private void FixedUpdate(){
    //    CheckDetectObjectSet(DetectObjectsInRaycast());
        _PreviousDetactObjects = DetectObjectsInRaycast();
    }

    #region  Main
    
    private void CheckDetectObjectSet(HashSet<DetectObj> curSet){
        // HashSet<DetectObj> detectObjs = new(_PreviousDetactObjects);
        // detectObjs.ExceptWith(curSet);

        // foreach(DetectObj obj in detectObjs){ 
        //     Debug.Log(obj.obj.name);
        //     obj.ReturnRbMass();
        // }

        // curSet.UnionWith(_PreviousDetactObjects);

        foreach(DetectObj detectObj in curSet){
            Debug.Log(detectObj.obj.name);
            // detectObj._Rb.position += _MovingPlatform.dir * _MovingPlatform.step;
        }
        

        _PreviousDetactObjects = curSet;

    }

    private HashSet<DetectObj> DetectObjectsInRaycast(){

        HashSet<DetectObj> set = new();
        RaycastHit2D[] hits = Physics2D.RaycastAll(GetRayStart(),Vector2.right,w,layerMask);
        foreach(RaycastHit2D hit in hits){
            if(hit.collider != null && hit.collider.gameObject != gameObject){
                // var att = GetDetectObjAttribute(hit.collider);
                DetectObj detectObj = new DetectObj(hit.collider.gameObject);
                set.Add(detectObj);
            }
        }
        // Debug.DrawRay(GetRayStart(),Vector2.right * w,Color.red);

        return set;
    }

    public void AddForce(Vector2 vec){
        if(_PreviousDetactObjects.Count <=0) return;
        foreach(DetectObj obj in _PreviousDetactObjects){
            Vector2 curPot = obj._Rb.position;
            obj._Rb.position = Vector2.MoveTowards(curPot,curPot+ConvertVec(_MovingPlatform.dir),_MovingPlatform.step);
        }
    }
    private Vector2 ConvertVec(Vector2 vec){
        Vector2 newVec = vec;
        if(vec.y>0){
            newVec.y = 0.1f;
        }

        return newVec;

    }
    public bool CheckObject(GameObject obj){
        foreach(DetectObj dtobj in _PreviousDetactObjects){
            if(dtobj.obj == obj) return true;
        }
        return false;
    }
    #endregion

    #region  Util
    private (float width,float height) GetColliderWH(){
        float width = _Collider.bounds.size.x;
        float height = _Collider.bounds.size.y;

        return (width,height);
    }


    private Vector2 GetRayStart(){
        Vector2 cur = transform.position;
        cur.x  -= w/2;
        cur.y += h*0.6f;
        return cur;
    }

    // private (GameObject obj,float mass) GetDetectObjAttribute(Collider2D col){
    //     GameObject obj = col.gameObject;
    //     float mass = 0;
    //     if(obj.TryGetComponent(out Rigidbody2D component)){
    //         mass = component.mass;
    //     }

    //     return (obj,mass);
    // }
    #endregion


   
}


public class DetectObj{
    public GameObject obj;
    public Rigidbody2D _Rb;

    public DetectObj(GameObject obj){
        this.obj = obj;
        _Rb = obj.GetComponent<Rigidbody2D>();
    }

    public override int GetHashCode()
    {
        return obj.GetHashCode();
    }
    public override bool Equals(object curObj)
    {
         if (curObj is DetectObj otherObj)
        {
            return obj == otherObj.obj;
        }
        return false;
    }

    // public void MassChangeAndVelocityZero(Vector2 parentsRBVelocity){
    //     _Rb.velocity =parentsRBVelocity;
    //     if(!Mathf.Approximately(_Rb.mass,0.001f)){
    //         _Rb.mass = 0.001f;
    //     }
        
    // }

}
