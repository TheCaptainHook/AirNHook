

using Mirror;
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

    public bool onReady;
    private void Update(){
        //-------------------------------------250401
        if(!onReady) return;
        // var wh = GetColliderWH();
        // w = wh.width;
        // h = wh.height;
        //-------------------------------------250401
        _PreviousDetactObjects = DetectObjectsInRaycast();

    }


    public void Init(){
        _PreviousDetactObjects = new();

        //-------------------------------------250401
        var wh = GetColliderWH();
        w = wh.width;
        h = wh.height;

        // onReady = true;
        //-------------------------------------250401
    }

    #region  Main
    

    private HashSet<DetectObj> DetectObjectsInRaycast(){

        HashSet<DetectObj> set = new();
        HashSet<GameObject> visitedObj = new();

        Debug.DrawRay(GetRayStart(),Vector2.right*w,Color.red);
        //Check first floor
        DetectObjectsAlongRay(GetRayStart(),w,visitedObj,set);
        // RaycastHit2D[] hits = Physics2D.RaycastAll(GetRayStart(),Vector2.right,w,layerMask);
        // foreach(RaycastHit2D hit in hits){
        //     if(hit.collider != null && hit.collider.gameObject != gameObject){
        //         if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Default")) continue;
        //         if(visitedObj.Add(hit.collider.gameObject)){
        //             DetectObj detectObj = new DetectObj(hit.collider.gameObject);
        //             set.Add(detectObj);
        //             Debug.Log($"{detectObj.obj.name}");
        //         }
                
        //     }
        // }

        //Check second floor
        List<DetectObj> newDetectObjs = new();
        foreach (DetectObj obj in set)
        {
            var detect = GetDetectObjRayStart(obj);
            DetectObjectsAlongRay(detect.startPot, detect.width, visitedObj, newDetectObjs);
        }
        // List<DetectObj> newDetectObjs = new();
        // foreach(DetectObj obj in set){
        //     var detect = GetDetectObjRayStart(obj);
        //     RaycastHit2D[] hits2 = Physics2D.RaycastAll(detect.startPot,Vector2.right,detect.width,layerMask);
        //     foreach(RaycastHit2D hit in hits2){
        //         if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Default")) continue;
        //         if(hit.collider != null && hit.collider.gameObject != gameObject){
        //             if(visitedObj.Add(hit.collider.gameObject)){
        //                 DetectObj detectObj = new DetectObj(hit.collider.gameObject);
        //                 newDetectObjs.Add(detectObj);
                        
        //             }
                   
        //         }
        //     }
            
        // }

        foreach (DetectObj newObj in newDetectObjs)
        {
            set.Add(newObj);
        }

        return set;
    }

    void DetectObjectsAlongRay(Vector2 start, float width, HashSet<GameObject> visited, ICollection<DetectObj> collection)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(start, Vector2.right, width, layerMask);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;

            GameObject obj = hit.collider.gameObject;

            if (obj == gameObject) continue;
            if (obj.layer == LayerMask.NameToLayer("Default")) continue;

            if (visited.Add(obj))
            {
                var detectObj = new DetectObj(obj);
                collection.Add(detectObj);
                Debug.Log($"{detectObj.obj.name}");
            }
        }
    }

    float forcePower = 800;
    //public void AddForce(Vector2 vec,float step){
    //    if(_PreviousDetactObjects.Count <=0) return;
    //    foreach(DetectObj obj in _PreviousDetactObjects){

    //        if (obj.obj == null) continue;

    //         if(obj.obj.TryGetComponent(out NetworkIdentity component))
    //        {
    //            if (component.isOwned)
    //            {
    //                Vector2 curPot = obj._Rb.position;
    //                Vector2 target = curPot + vec;
    //                //obj._Rb.position = Vector2.MoveTowards(curPot, target, step);
    //                obj._Rb.MovePosition(obj._Rb.position+vec);
    //                //obj._Rb.AddForce(vec*800, ForceMode2D.Force);
    //                Debug.Log($"Name : {obj.obj.name} , isOwned : {component.isOwned}, ");
    //            }
    //        }

    //    }
    //}
    public void AddForce(Vector2 dir)
    {
        if (_PreviousDetactObjects.Count <= 0) return;


        foreach (DetectObj obj in _PreviousDetactObjects)
        {
            if (obj.obj == null) continue;

            if (obj.obj.TryGetComponent(out NetworkIdentity component))
            {
                if (component.isOwned)
                {
                    obj._Rb.MovePosition(obj._Rb.position + dir);
                    Debug.Log($"Name : {obj.obj.name} , isOwned : {component.isOwned}, ");
                }
            }

        }
    }
    /**
         서버에서 이동하고(Network Transform), AddForce ClientRpc

    **/

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
