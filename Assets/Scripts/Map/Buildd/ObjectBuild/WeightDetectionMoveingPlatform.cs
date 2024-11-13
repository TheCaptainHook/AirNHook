
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightDetectionMoveingPlatform : MonoBehaviour
{
    [CustomHeader("Weight Detection Moving Platform")]
    [ReadOnly]
    public Vector2 path;
    
    #region  TEST
    private Vector2 orgPot;
    #endregion

    private HashSet<DetectObj> leftSet;
    private HashSet<DetectObj> rightSet;
    private float rayLength;

    #region Main
    private RaycastHit2D[] leftHit;
    private RaycastHit2D[] rightHit;
    [SerializeField] Transform leftPoint;
    [SerializeField] Transform rightPoint;
    public float moveDistance;
    public float moveSpeed;
    private float maxRotate = 30;
    private bool onActive;

    [SerializeField] LayerMask layerMask;

    private float weight;
    private Vector2 dir;
    private float step;
    #endregion

    
   #region  Components
   private Rigidbody2D rb;
   private Collider2D bodyCol;
   #endregion

    #region  Test Init
    private void TestInit(){
        orgPot = transform.position;
        path  = GetPath();
    }
    #endregion

    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
        bodyCol = GetComponent<Collider2D>();
        leftSet = new();
        rightSet = new();


        TestInit();
    }

    private void Start(){
        rayLength = bodyCol.bounds.size.x/2 - 0.1f;
    }

    private void Update(){
        ShootRay();


        if(Input.GetKeyDown(KeyCode.B)){
            foreach(RaycastHit2D hit in leftHit){
                Debug.Log($"name: {hit.collider.name}\npoint : {hit.point}\ndiff:{Vector3.Distance(transform.position,hit.point)}");
            }
        }
        if(Input.GetKeyDown(KeyCode.N)){
            foreach(RaycastHit2D hit in rightHit){
                Debug.Log($"name: {hit.collider.name}\npoint : {hit.point}\ndiff:{Vector3.Distance(transform.position,hit.point)}");
            }
        }
    }
    // - : right
    // + : left

    private void ShootRay(){
        float lw = 0;
        float rw = 0;

        Debug.DrawRay(leftPoint.position,-transform.right*rayLength,Color.red);
        Debug.DrawRay(rightPoint.position,transform.right*rayLength,Color.blue);
        leftHit = Physics2D.RaycastAll(leftPoint.position,-transform.right,rayLength,layerMask);
        rightHit = Physics2D.RaycastAll(rightPoint.position,transform.right,rayLength,layerMask);

        foreach(RaycastHit2D hit in leftHit){
            lw += Weight(hit);
        }

        foreach(RaycastHit2D hit in rightHit){
            rw += Weight(hit);
        }
        weight = (lw-rw)/10f;
        transform.Rotate(0,0,weight);

        
    }
    private void MoveTowards(){
        dir = transform.rotation.z > 0 ? -Vector2.right : Vector2.right;
        step = moveSpeed * Time.fixedDeltaTime;
        rb.position = Vector2.MoveTowards(rb.position,rb.position +dir,step);
    }

   private float Weight(RaycastHit2D hit){
    float dis = Mathf.Floor(Vector3.Distance(transform.position,hit.point)*100)/100;
    float mass = hit.collider.GetComponent<Rigidbody2D>().mass;
    return dis*mass;
   }


    // public void Prograss(){

    // }

    // IEnumerator PrograssCo(){
        
    //     Vector2 targetPosition = path;
    //     while(true){
    //         while(!onActive){
    //             yield return null;
    //         }

        

    //     }
    // }


#region  Util
private Vector2 GetPath(){
    return new Vector2(orgPot.x + moveDistance,orgPot.y);
}

#endregion
}
