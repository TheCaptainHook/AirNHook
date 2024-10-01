

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AddForcePlatform : MonoBehaviour
{
    [CustomHeader("AddForcePlatform")]
    public float _LimitVelocity;

    [Header("Ray")]
    private float rayLength;

    [Header("Main")]
    HashSet<DetectObj> _PreviousDetactObjects;
    

    [Header("INIT")]
    private float w;
    private float h;
    Collider2D _Collider;
    Rigidbody2D _Rb;


    private void Awake(){
        _Collider = GetComponent<Collider2D>();
        _Rb = GetComponent<Rigidbody2D>();
        _PreviousDetactObjects = new();

        var wh = GetColliderWH();
        w = wh.width;
        h = wh.height;
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
       CheckDetectObjectSet(DetectObjectsInRaycast());
    }

    #region  Main
    
    private void CheckDetectObjectSet(HashSet<DetectObj> curSet){
        HashSet<DetectObj> detectObjs = new(_PreviousDetactObjects);
        detectObjs.ExceptWith(curSet);

        foreach(DetectObj obj in detectObjs){
            Debug.Log(obj.obj.name);
            obj.ReturnRbMass();
        }

        _PreviousDetactObjects = curSet;

    }

    private HashSet<DetectObj> DetectObjectsInRaycast(){

        HashSet<DetectObj> set = new();
        RaycastHit2D[] hits = Physics2D.RaycastAll(GetRayStart(),Vector2.right,w);
        foreach(RaycastHit2D hit in hits){
            if(hit.collider != null && hit.collider.gameObject != gameObject){
                var att = GetDetectObjAttribute(hit.collider);
                set.Add(new DetectObj(att.obj,att.mass));
            }
        }
        Debug.DrawRay(GetRayStart(),Vector2.right * w,Color.red);

        return set;
    }

    public void AddForce(){
        foreach(DetectObj obj in _PreviousDetactObjects){
            if(obj._Rb.mass != obj.mass){
                obj._Rb.mass = obj.mass;
            }

            obj._Rb.velocity = _Rb.velocity;
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
        cur.y += h*0.55f;
        return cur;
    }

    private (GameObject obj,float mass) GetDetectObjAttribute(Collider2D col){
        GameObject obj = col.gameObject;
        float mass = 0;
        if(obj.TryGetComponent(out Rigidbody2D component)){
            mass = component.mass;
        }

        return (obj,mass);
    }
    #endregion


   
}


public class DetectObj{
    public GameObject obj;
    public float mass;
    public Rigidbody2D _Rb;

    public DetectObj(GameObject obj,float mass){
        this.obj = obj;
        this.mass = mass;
        _Rb = obj.GetComponent<Rigidbody2D>();
    }

    public override int GetHashCode()
    {
        return obj.GetHashCode() ^ mass.GetHashCode();
    }
    public override bool Equals(object curObj)
    {
         if (curObj is DetectObj otherObj)
        {
            return obj == otherObj.obj && mass == otherObj.mass;
        }
        return false;
    }

    public void ReturnRbMass(){
        obj.GetComponent<Rigidbody2D>().mass = this.mass;
    }

}
