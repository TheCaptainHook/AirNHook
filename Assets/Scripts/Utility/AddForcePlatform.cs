

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AddForcePlatform : MonoBehaviour
{
    [CustomHeader("AddForcePlatform")]
    public float _LimitVelocity;

    [Header("Ray")]
    public LayerMask layerMask;

    [Header("Main")]
    public HashSet<DetectObj> _PreviousDetactObjects;


    private MovingPlatform_Net movingPlatform_Net;
    private MovingPlatform_Net MovingPlatform_Net
    {
        get
        {
            if(movingPlatform_Net == null)movingPlatform_Net = GetComponent<MovingPlatform_Net>();
            return movingPlatform_Net;
        }
    }

    [Header("INIT")]
    private float w;
    private float h;
    
    Collider2D _collider;
    Collider2D _Collider {
        get{
            if(_collider == null){
                _collider = GetComponent<Collider2D>();
                return _collider;
            }
            return _collider;
        }
    }
    Rigidbody2D rb;
    Rigidbody2D _Rb{
        get{
            if(rb == null){
                rb = GetComponent<Rigidbody2D>();
                return rb;
            }
            return rb;
        }
    }

    [SerializeField] MovingPlatform _MovingPlatform;

    private void Update(){
        
        var wh = GetColliderWH();
        w = wh.width;
        h = wh.height;

        _PreviousDetactObjects = DetectObjectsInRaycast();

    }
    // private void FixedUpdate(){
    //     _PreviousDetactObjects = DetectObjectsInRaycast();
    // }

    public void Init(){
        _PreviousDetactObjects = new();
        _MovingPlatform.MoveAction+=AddForce;
    }

    #region  Main
    

    private HashSet<DetectObj> DetectObjectsInRaycast(){

        HashSet<DetectObj> set = new();
        HashSet<GameObject> visitedObj = new();

        Debug.DrawRay(GetRayStart(),Vector2.right*w,Color.red);
        RaycastHit2D[] hits = Physics2D.RaycastAll(GetRayStart(),Vector2.right,w,layerMask);
        foreach(RaycastHit2D hit in hits){
            if(hit.collider != null && hit.collider.gameObject != gameObject){
                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Default")) continue;
                if(visitedObj.Add(hit.collider.gameObject)){
                    DetectObj detectObj = new DetectObj(hit.collider.gameObject);
                    set.Add(detectObj);
                    Debug.Log($"{detectObj.obj.name}");
                }
                
            }
        }

        List<DetectObj> newDetectObjs = new();

        foreach(DetectObj obj in set){
            var detect = GetDetectObjRayStart(obj);
            RaycastHit2D[] hits2 = Physics2D.RaycastAll(detect.startPot,Vector2.right,detect.width,layerMask);
            foreach(RaycastHit2D hit in hits2){
                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Default")) continue;
                if(hit.collider != null && hit.collider.gameObject != gameObject){
                    if(visitedObj.Add(hit.collider.gameObject)){
                        DetectObj detectObj = new DetectObj(hit.collider.gameObject);
                        newDetectObjs.Add(detectObj);
                        
                    }
                   
                }
            }
            
        }

        foreach (DetectObj newObj in newDetectObjs)
        {
            set.Add(newObj);
            Debug.Log($"{newObj.obj.name}");
        }

        return set;
    }

    public void AddForce(Vector2 vec){
        if(_PreviousDetactObjects.Count <=0) return;
        foreach(DetectObj obj in _PreviousDetactObjects){
            Vector2 curPot = obj._Rb.position;
            Vector2 target = curPot + vec;
            obj._Rb.position = Vector2.MoveTowards(curPot,target,MovingPlatform_Net.step);
        }
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
        cur.y += h + 0.1f;
        return cur;
    }
    private (Vector2 startPot,float width) GetDetectObjRayStart(DetectObj obj){
    
      Vector2 pot = obj.obj.transform.position;
      float detectobjX = obj._Col.bounds.size.x/2;
      pot.x -= detectobjX;
      pot.y += h + 0.1f;
      Debug.DrawRay(pot,Vector2.right*obj._Col.bounds.size.x,Color.red);
      return (pot,obj._Col.bounds.size.x);
      
    }
    #endregion

   
}


public class DetectObj{
    public GameObject obj;
    public Rigidbody2D _Rb;
    public Collider2D _Col;

    public DetectObj(GameObject obj){
        this.obj = obj;
        _Rb = obj.GetComponent<Rigidbody2D>();
        _Col = obj.GetComponent<Collider2D>();
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

}
