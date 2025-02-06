using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;


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
    private float maxRotate = 40;
    private bool onActive; 

    [SerializeField] LayerMask layerMask;
    [SerializeField] GameObject rail_Prefabs;
    [SerializeField] LineRenderer rail_Line;
    
    private float weight;
    //private Vector2 dir;
    //private float step;

    
    //private float minDis_Clamp; //Compare orgPot, path.
    //private float maxDis_Clamp; //Compare orgPot, path.
    private Vector2 curTargetPot; //next move point.
    private float releaseCount =1;
    private float curReleaseCount;
    #endregion

    
   #region  Components
   private Rigidbody2D rb;
   private Collider2D bodyCol;
   private Animator animator;
   private WDMP_Net WDMP_Net => GetComponent<WDMP_Net>();
   #endregion
    
    #region Animation
    private readonly int leftDown = Animator.StringToHash("LeftDown");
    private readonly int rightDown = Animator.StringToHash("RightDown");
    #endregion
    
    private void Init(){
        WDMP_Net.Server_SetMoveDistance(ButtonActivatedObjectStruct.moveDistance);

        orgPot = transform.position;
        path  = GetPath();
        curTargetPot = orgPot;


        bodyCol = GetComponent<Collider2D>();
        rayLength = bodyCol.bounds.size.x/2f;

        // CreateRail();
    }
 

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

                //moveDistance = objData.moveDistance;
                //WDMP_Net.Server_SetMoveDistance(objData.moveDistance);
                moveSpeed = objData.moveSpeed;

          
            }
        }catch(Exception ex){
                Debug.Log($"ERROR,{typeof(T)},{ex}");
        }
        
        if(Application.isPlaying){
            Init();
            // CreateRail();
            
            
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
        animator = GetComponent<Animator>();

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

    bool onMove;
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

        //recover tilt
        if(leftHit.Length == 0 && rightHit.Length == 0){
            curReleaseCount+=Time.deltaTime;
            if(curReleaseCount >= releaseCount){
                onMove = false;
                transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.identity,Time.fixedDeltaTime);
            }
        }else{
            curReleaseCount = 0;
            onMove = true;
        }
        if(!onMove) return;
            
        weight = (lw-rw)/10f;
        //tilt animation
        TiltAnimationSet(lw,rw);
        //tilt platform
        Rotate(weight);
        //move platform
        if(WDMP_Net.moveDistance == 0) return;

        //dir = transform.rotation.z == 0 ? Vector2.zero : transform.rotation.z>0 ? -Vector2.right : Vector2.right;
        Vector2 dir = transform.rotation.z == 0 ? Vector2.zero : transform.rotation.z > 0 ? -Vector2.right : Vector2.right;
        WDMP_Net.Server_SetDir(dir);

        if (CheckMaxAndMinClamp()){
            //step = moveSpeed * rate * Time.fixedDeltaTime;
            WDMP_Net.Server_SetStep(moveSpeed * rate * Time.fixedDeltaTime);
        }else{
            //step = 0;
            WDMP_Net.Server_SetStep(0);
        }

        MoveTowards();   
        MoveTowards(leftHit);
        MoveTowards(rightHit);
            

    }
    float rate = 0;
    // z>0 : left , z<0 :right
    private void Rotate(float weight){
        Vector3 euler = transform.rotation.eulerAngles;
        euler.z += weight;

        if(euler.z > 180){
            euler.z -= 360;
        }

        euler.z = Mathf.Clamp(euler.z , -maxRotate,maxRotate);
        rate = Mathf.Abs(euler.z) / maxRotate;

        transform.rotation = Quaternion.Euler(euler);
    }
    private void MoveTowards(){

        //curTargetPot = rb.position + dir;
        curTargetPot = rb.position + WDMP_Net.dir;
        //curTargetPot.x = Mathf.Clamp(curTargetPot.x,minDis_Clamp,maxDis_Clamp);
        curTargetPot.x = Mathf.Clamp(curTargetPot.x, WDMP_Net.minDis_Clamp, WDMP_Net.maxDis_Clamp);
        //rb.position = Vector2.MoveTowards(rb.position,curTargetPot,step);
        rb.position = Vector2.MoveTowards(rb.position, curTargetPot, WDMP_Net.step);
    }
    private void MoveTowards(RaycastHit2D[] hits){
            foreach(RaycastHit2D hit in hits){
                if(hit.collider.TryGetComponent(out Rigidbody2D component)){
                //component.position = Vector2.MoveTowards(component.position,component.position + dir,step);
                component.position = Vector2.MoveTowards(component.position, component.position + WDMP_Net.dir, WDMP_Net.step);
            }
        }
    }
    

   


#region  Util
private void TiltAnimationSet(float l,float r){
     if(l > r){//left
            animator.SetBool(leftDown,true);
            animator.SetBool(rightDown,false);
        }else if(l < r){//right
            animator.SetBool(leftDown,false);
            animator.SetBool(rightDown,true);
        }else{
            animator.SetBool(leftDown,false);
            animator.SetBool(rightDown,false);
        }
}

private Vector2 GetPath(){
    if(WDMP_Net.moveDistance == 0) return orgPot;
    Vector2 target = new Vector2(orgPot.x + WDMP_Net.moveDistance, orgPot.y);

    float minDis_Clamp = orgPot.x > target.x ? target.x : orgPot.x;
    float maxDis_Clamp = orgPot.x < target.x ? target.x : orgPot.x;

    WDMP_Net.Server_SetClamp(minDis_Clamp,maxDis_Clamp);

    return target;

}

private float Weight(RaycastHit2D hit){
        if (hit.collider.TryGetComponent(out HookSM hook))
        {
            if (hook.isSwinging)
            {
                return 0;
            }
           
        }
        if (hit.collider.TryGetComponent(out Rigidbody2D component))
        {
            float dis = Mathf.Floor(Vector3.Distance(transform.position, hit.point) * 100) / 100;
            float mass = component.mass;
            return dis * mass;
        }

        return 0;
   
}
private bool CheckMaxAndMinClamp(){
        if(WDMP_Net.dir == Vector2.right){
            if(curTargetPot.x > WDMP_Net.maxDis_Clamp)
            {
                return false;
            }
        }else if(WDMP_Net.dir == -Vector2.right){
            if(curTargetPot.x < WDMP_Net.minDis_Clamp)
            {
                return false;
            }
        }else if(WDMP_Net.dir == Vector2.zero ){
            return false;
        }

        return true;
}
private void CreateRail(){ //rail node, rail lineRenderer
        Transform parents = MapEditor.Instance.dontSaveObjectTransform;
        Transform container = new GameObject("Rail_Container").transform;
        container.SetParent(parents);

        LineRenderer line = Instantiate(rail_Line,container);
        //Draw Line
        DrawLine(line);

        GameObject railNode_1 = Instantiate(rail_Prefabs,container);
        railNode_1.transform.position = line.GetPosition(0);
        GameObject railNode_2 = Instantiate(rail_Prefabs,container);
        railNode_2.transform.position = line.GetPosition(1);
    }
    private void DrawLine(LineRenderer line){
        line.positionCount = 2;
        line.SetPosition(0,transform.position);
        Vector2 target = new Vector2(transform.position.x + WDMP_Net.moveDistance,transform.position.y);
        line.SetPosition(1,target);
           
    }
   

#endregion
}
