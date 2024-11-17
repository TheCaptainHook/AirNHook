using System;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(WDMP_Path))]
public class WeightDetectionMoveingPlatform : ActivatableObjectEntity
{
    [CustomHeader("Weight Detection Moving Platform")]
    [ReadOnly]
    public Vector2 path;
    
    #region  TEST
    private Vector2 orgPot;
    #endregion
    private float rayLength;

    #region Main
    private RaycastHit2D[] leftHit;
    private RaycastHit2D[] rightHit;
    [ReadOnly]
    [SerializeField] Transform leftPoint;
    [ReadOnly]
    [SerializeField] Transform rightPoint;
    public float moveDistance;
    public float moveSpeed;
    private float maxRotate = 30;
    private bool onActive; 

    [SerializeField] LayerMask layerMask;

    
    private float weight;
    private Vector2 dir;
    private float step;

    
    private float minDis_Clamp; //Compare orgPot, path.
    private float maxDis_Clamp; //Compare orgPot, path.
    private Vector2 curTargetPot; //next move point.
    private float releaseCount =3;
    private float curReleaseCount;
    #endregion

    
   #region  Components
   private Rigidbody2D rb;
   private Collider2D bodyCol;
   #endregion

    #region  Test Init
    private void Init(){
        orgPot = transform.position;
        path  = GetPath();
        curTargetPot = orgPot;

        rayLength = bodyCol.bounds.size.x/2f;
    }
    #endregion


    #region  Get,Set
    public override T GetData<T>()
    {
        if(typeof(T)==typeof(ButtonActivatableObjectStruct)){
            return (T)(object)new ButtonActivatableObjectStruct(id,activeRequirAmount,transform.position,transform.rotation,transform.localScale,moveDistance,moveSpeed);
        }
        
        return default(T);

    }
    public override async void  SetData<T>(T data)
    {
        try{
            if (typeof(T) == typeof(ButtonActivatableObjectStruct))
            {
                ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
                ButtonActivatedObjectStruct = objData;
                moveDistance = objData.moveDistance;
                moveSpeed = objData.moveSpeed;
                Init();
          
            }
        }catch{
                Debug.Log($"ERROR,{typeof(T)}");
        }
        
        if(Application.isPlaying){
            Util util  = new Util();
            await util.Delay(()=>{CheckActiveRequirAmount();});
        }
    }
    #endregion

    #region  Activatable Object Entity
    protected override void Activation()
    {
        onActive = true;
    }
    protected override void Deactivated()
    {
        onActive = false;
    }
    #endregion

    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
        bodyCol = GetComponent<Collider2D>();

    }

    // private void Start(){//TEST CODE
    //     Init(); 
    //     onActive = true;
    // }

    private void Update(){
        ShootRay();
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


        if(leftHit.Length == 0 && rightHit.Length == 0){
            curReleaseCount+=Time.fixedDeltaTime;
            if(curReleaseCount >= releaseCount){
                transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.identity,Time.fixedDeltaTime);
            }
        }else{
            curReleaseCount = 0;
        }


        weight = (lw-rw)/10f;
        //tilt platform
        Rotate(weight);
        //move platform
        if(moveDistance == 0) return;
        if(!onActive) return;

        dir = transform.rotation.z == 0 ? Vector2.zero : transform.rotation.z>0 ? -Vector2.right : Vector2.right;
        
        if(CheckMaxAndMinClamp()){
           step = moveSpeed * Time.fixedDeltaTime;
        }else{
            step = 0;
        }

        MoveTowards();   
        MoveTowards(leftHit);
        MoveTowards(rightHit);
            

    }
    private void Rotate(float weight){
        Vector3 euler = transform.rotation.eulerAngles;
        euler.z += weight;
        euler.z = Mathf.Clamp(euler.z > 180 ? euler.z - 360 : euler.z,-30,30);
        transform.rotation = Quaternion.Euler(euler);
    }
    private void MoveTowards(){

        curTargetPot = rb.position + dir;
        curTargetPot.x = Mathf.Clamp(curTargetPot.x,minDis_Clamp,maxDis_Clamp);

        rb.position = Vector2.MoveTowards(rb.position,curTargetPot,step);
    }
    private void MoveTowards(RaycastHit2D[] hits){
            foreach(RaycastHit2D hit in hits){
                if(hit.collider.TryGetComponent(out Rigidbody2D component)){
                component.position = Vector2.MoveTowards(component.position,component.position + dir,step);
            }
        }
    }
    

   


#region  Util
private Vector2 GetPath(){
    if(moveDistance == 0) return orgPot;
    Vector2 target = new Vector2(orgPot.x + moveDistance,orgPot.y);

    minDis_Clamp = orgPot.x > target.x ? target.x : orgPot.x;
    maxDis_Clamp = orgPot.x < target.x ? target.x : orgPot.x;
    return target;

}

private float Weight(RaycastHit2D hit){
    float dis = Mathf.Floor(Vector3.Distance(transform.position,hit.point)*100)/100;
    float mass = hit.collider.GetComponent<Rigidbody2D>().mass;
    return dis*mass;
}
private bool CheckMaxAndMinClamp(){
        if(dir == Vector2.right){
            if(curTargetPot.x >= maxDis_Clamp){
                return false;
            }
        }else if(dir == -Vector2.right){
            if(curTargetPot.x <= minDis_Clamp){
                return false;
            }
        }else if(dir == Vector2.zero ){
            return false;
        }

        return true;
}
   

#endregion
}
