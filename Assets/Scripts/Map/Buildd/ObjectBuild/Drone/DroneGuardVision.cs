using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DroneTrackState
  {
    GUARD,
    TRACKING,
    LOSTTARGET,
    ONATTACK
  }

public class DroneGuardVision : MonoBehaviour
{

    public bool _OnFind;

    // private float _MinRot = 0;
    private float _MaxRot = 130;

    // [SerializeField] GameObject _FoundTargetObj;

    [SerializeField] float _RotSpeed;
    public LayerMask layerMask;
    private float _Angle; // 0
    [SerializeField] float _ViewDistance; //10
    private PathFinder pathFinder;
    // [Header("View Field")]
    // MeshFilter meshFilter;
    // private Mesh _Mesh;
    // [SerializeField] float _Fov; //40 ,View Area
    // [SerializeField] int _RayCount; //30 Mesh Point
    
    // // private float previousAngle;
    // private float AngleIncrease => _Fov/_RayCount;
    

    // private Vector3[] _Vertices;
    // private Vector2[] _UV;
    // private int[] _Triangles;

    [SerializeField] Drone_Laser drone_Laser;
    [SerializeField] Transform lightTr;
    [SerializeField] LineRenderer lineRenderer;

    private void Awake(){
        // meshFilter = GetComponent<MeshFilter>();
        // _Mesh = new Mesh();
        // drone_Laser.PrograssAction += GuardVisionPrograss;
        pathFinder = GetComponent<PathFinder>();
        droneTrackState = DroneTrackState.GUARD;
        drone_Laser.BrokenAction += Broken;
        moveSpeed = drone_Laser.moveSpeed;

    }

//----------------------------------------------------------------------------------------250104
  private float moveSpeed;
  private DroneTrackState droneTrackState;
  
  public void SwitchDroneTrackState(){ // droneLaser fixedUpdate
    switch(droneTrackState){
      case DroneTrackState.GUARD:
        GuardPrograss();
      break;
      case DroneTrackState.TRACKING:
        TrackingPrograss();
        Debug.Log("Trackging");
      break;
      case DroneTrackState.LOSTTARGET:
        Debug.Log("Lost Target");
      //Return path -> Go() -> GUARD
      break;
      case DroneTrackState.ONATTACK:
        Debug.Log("On Attack");
      //UpdateLaser
      break;
    }
  }
  #region  Guard
   float time;
   private void GuardPrograss()
    {
        if(!_OnFind)
        {
            time += Time.deltaTime;
            _Angle = (Mathf.PingPong(time *_RotSpeed,_MaxRot)+20) * -1;  
        }else
        {
            _Angle = GetAngleFromTargetPositionDir();
        }

        
        drone_Laser.RotLazerAnimation(_Angle);
        GuardVisionRay(_Angle); 
    }

    private void FindTarget(GameObject target)
    {
        if(!_OnFind)
        {
            _OnFind = true;
            drone_Laser.Stop();
        }
        drone_Laser.target = target;  
    }
    private void LostTarget(){
        if(_OnFind)
        {
            _OnFind = false;
            drone_Laser.Go();
            drone_Laser.target = null;
        }
    }
    private float senseTargetRate =2;
    private float curSenseTargetRate;
    private void GuardVisionRay(float _Angle){
        Vector2 curPot = transform.position;
        float newAngle = _Angle;
        Vector2 dir =GetVectorFromAngle(newAngle);
        RaycastHit2D hit = Physics2D.Raycast(curPot,dir,layerMask);
    
        if(hit.collider != null){
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")){
               
                FindTarget(hit.collider.gameObject);
                curSenseTargetRate += Time.deltaTime;
                DrawLine(hit.point);

                if(curSenseTargetRate >= senseTargetRate)
                {
                    droneTrackState = DroneTrackState.TRACKING;
                }
                
            }else{
           
                LostTarget();

                curSenseTargetRate = 0;
                hitPoint = hit.point;
                DrawLine(hit.point);
            }
        }
    }
  #endregion

  #region  Tracking
  private float trackingSenseRate =20;
  private float curTrackingSenseRate;
  private void TrackingVisionRay(float _Angle)
  {
        Vector2 curPot = transform.position;
        float newAngle = _Angle;
        Vector2 dir =GetVectorFromAngle(newAngle);
        RaycastHit2D hit = Physics2D.Raycast(curPot,dir,layerMask);

        if(hit.collider != null)
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")){
                if(drone_Laser.target != hit.collider.gameObject)
                {
                    drone_Laser.target = hit.collider.gameObject;
                }

                Rigidbody2D _rb = drone_Laser.GetComponent<Rigidbody2D>();
                
                // Vector2 newDir = (drone_Laser.target.transform.position - transform.position).normalized;
                _rb.AddForce(dir * 50,ForceMode2D.Force);
                
                if (_rb.velocity.magnitude > moveSpeed)
                {
                    _rb.velocity = _rb.velocity.normalized * moveSpeed;
                }
                if(GetTargetDistance(drone_Laser.target)<= 10)
                {
                    droneTrackState = DroneTrackState.ONATTACK;
                }

                DrawLine(hit.point);
           
                
            }else{
                hitPoint = hit.point;
                if(drone_Laser.target != null)
                {
                    Rigidbody2D _rb = drone_Laser.GetComponent<Rigidbody2D>();
                    _rb.velocity = Vector2.zero;
                    drone_Laser.target = null;
                }
                
                curTrackingSenseRate += Time.deltaTime;
                if(curTrackingSenseRate >= trackingSenseRate)
                {
                    droneTrackState = DroneTrackState.LOSTTARGET;
                    curTrackingSenseRate = 0;
                }

                DrawLine(hit.point);
            }
        }
  }

   private void TrackingPrograss()
    {
        if(drone_Laser.target == null)
        {
            time += Time.deltaTime;
            float newAngle = Mathf.PingPong(time * _RotSpeed, 40) - 20 + _Angle;
            TrackingVisionRay(newAngle);
        }else
        {
            _Angle = GetAngleFromTargetPositionDir();
            TrackingVisionRay(_Angle);
        }
        
        

    }
   
  #endregion

  #region  Lost Target
   private void LostTargetPrograss()
    {
        
    }
  #endregion

  #region  On Attack
   private void OnAttackPrograss() //attack range 70,sqrMagnitude
    {

    }
  #endregion

   
   


//----------------------------------------------------------------------------------------250104


    // private void GuardVisionPrograss(){
         
    //     StartCoroutine(GuardVisionPrograssCo());
    // }
    // IEnumerator GuardVisionPrograssCo(){
    //     float percent = 0;
    //     while(!drone_Laser.IsBroken){
    //         if(_OnFind){
    //             if(drone_Laser.target != null){
    //                 FindTarget(ref _Angle);
    //                 if(!drone_Laser.OnLazer){
    //                     yield return new WaitForSeconds(0.1f);
    //                     drone_Laser.TurnOnLazer();
    //                 }
    //             }
    //         }else{
    //             LostTarget(ref _Angle,ref percent);
    //         }

    //     GuardVisionRay(_Angle);
    //     yield return null;
    //     }

    // }

    private float GetAngleFromTargetPositionDir()
    {
        Vector2 dir = (drone_Laser.target.transform.position - transform.position).normalized;
        return GetAngleFromVector(dir);
    }
    // private void FindTarget(ref float _Angle)
    // {
    //     drone_Laser.Stop();
    //     Vector2 dir = (drone_Laser.target.transform.position - transform.position).normalized;
    //     float deg = GetAngleFromVector(dir);
    //     _Angle = deg;
    //     // CreateMesh(deg+20);
    //     drone_Laser.RotLazerAnimation(deg);
    // }
    // private void LostTarget(ref float _Angle,ref float percent)
    // {
    //     // DelMesh();
    //     drone_Laser.TurnOffLazer();
    //     drone_Laser.Go();
    //     percent += Time.deltaTime;
    //     _Angle = (Mathf.PingPong(percent *_RotSpeed,_MaxRot)+20) * -1;   
    // }

    private void Broken(){
        StopAllCoroutines();
        GetComponent<MeshFilter>().mesh = null;
    }

    public Vector2 hitPoint;
   


    
    #region  Create View Field
    // private void CreateMesh(float _Angle){
       
    //     Vector2 curPot = transform.position;
    //     float newAngle = _Angle;
    //     int verticesIdx = 1;
    //     int trianglesIdx = 0;
    //     Vector2 dir;
  
    //     CreateMeshInit();

    //     _Vertices[0] = transform.InverseTransformPoint(curPot);
        
    //     for(int i = 0; i<=_RayCount; i++){
    //         Vector3 vertex;
    //         dir = GetVectorFromAngle(newAngle);
    //         RaycastHit2D hit = Physics2D.Raycast(curPot,dir,_ViewDistance,layerMask);

    //         if(hit.collider == null){
    //             vertex = transform.InverseTransformPoint((Vector3)curPot + GetVectorFromAngle(newAngle) * _ViewDistance);
    //         }else{
    //             vertex = transform.InverseTransformPoint((Vector3)hit.point);
    //         }
            
    //         _Vertices[verticesIdx] = vertex;

    //         if(i>0){
    //             _Triangles[trianglesIdx] = 0;
    //             _Triangles[trianglesIdx+1] = verticesIdx-1;
    //             _Triangles[trianglesIdx+2] = verticesIdx;
    //             trianglesIdx +=3;
    //         }
             
    //          verticesIdx++;
    //         //  _Angle -= AngleIncrease;
    //             newAngle -= AngleIncrease;    
    //     }

    //     _Mesh.vertices = _Vertices;
    //     _Mesh.uv = _UV;
    //     _Mesh.triangles = _Triangles;

    // }
          //INIT
        //  _Mesh = new Mesh();
        // meshFilter.mesh = _Mesh;
        // _Vertices = new Vector3[_RayCount+2];
        // _UV = new Vector2[_Vertices.Length];
        // _Triangles = new int[_RayCount *3];
    // private void CreateMeshInit(){
    //     _Mesh = new Mesh();
    //     meshFilter.mesh = _Mesh;
    //     _Vertices = new Vector3[_RayCount+2];
    //     _UV = new Vector2[_Vertices.Length];
    //     _Triangles = new int[_RayCount *3];
    // }
    #endregion
    

    #region  Util
    // private void DelMesh(){
        
    //     if(meshFilter.mesh != null){
    //         meshFilter.mesh = null;
    //     }
    // }
    private Vector3 GetVectorFromAngle(float angle){
        //Convert Radian from angle
        float radian = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(radian),Mathf.Sin(radian)).normalized;
    }

    private float GetAngleFromVector(Vector3 dir){
        float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
        return angle;
    }

    private void DrawLine(Vector2 target){
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0,lightTr.position);
        lineRenderer.SetPosition(1,target);
    }

     private float GetTargetDistance(GameObject target)
    {
        float distance = (transform.position - target.transform.position).sqrMagnitude;
        Debug.Log(distance);
        return distance;
    }
    #endregion

}
