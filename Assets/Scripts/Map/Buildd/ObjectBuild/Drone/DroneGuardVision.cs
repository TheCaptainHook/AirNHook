using System.Collections;
using System.Data;
using Org.BouncyCastle.Crypto.Engines;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Assertions.Must;


public class DroneGuardVision : MonoBehaviour
{

    public bool _OnFind;

    // private float _MinRot = 0;
    private float _MaxRot = 130;

    // [SerializeField] GameObject _FoundTargetObj;

    [SerializeField] float _RotSpeed;
    public LayerMask layerMask;


    [Header("View Field")]
    MeshFilter meshFilter;
    private Mesh _Mesh;
    [SerializeField] float _Fov; //40 ,View Area
    [SerializeField] int _RayCount; //30 Mesh Point
    private float _Angle; // 0
    // private float previousAngle;
    private float AngleIncrease => _Fov/_RayCount;
    [SerializeField] float _ViewDistance; //10

    private Vector3[] _Vertices;
    private Vector2[] _UV;
    private int[] _Triangles;

    [SerializeField] Drone_Laser drone_Laser;
    [SerializeField] Transform lightTr;
    [SerializeField] LineRenderer lineRenderer;

    private void Awake(){
        meshFilter = GetComponent<MeshFilter>();
        _Mesh = new Mesh();
        drone_Laser.PrograssAction += GuardVisionPrograss;
        drone_Laser.BrokenAction += Broken;
    }

    private void GuardVisionPrograss(){
         
        StartCoroutine(GuardVisionPrograssCo());
    }
    IEnumerator GuardVisionPrograssCo(){
        float percent = 0;
        while(!drone_Laser.IsBroken){
            if(_OnFind){
                if(drone_Laser.target != null){
                    drone_Laser.Stop();
                    Vector2 dir = (drone_Laser.target.transform.position - transform.position).normalized;
                    float deg = GetAngleFromVector(dir);
                    _Angle = deg;
                    CreateMesh(deg+20);
                    drone_Laser.RotLazerAnimation(deg);
                    if(!drone_Laser.OnLazer){
                        yield return new WaitForSeconds(0.1f);
                        drone_Laser.TurnOnLazer();
                    }
                }
                // _Angle = previousAngle;
            }else{
                DelMesh();
                drone_Laser.TurnOffLazer();
                drone_Laser.Go();
                percent += Time.deltaTime;
                _Angle = (Mathf.PingPong(percent *_RotSpeed,_MaxRot)+20) * -1;
            }

        GuardVisionRay(_Angle);
        yield return null;

        }

    }

    private void Broken(){
        StopAllCoroutines();
        GetComponent<MeshFilter>().mesh = null;
    }


    private void GuardVisionRay(float _Angle){
        Vector2 curPot = transform.position;
        float newAngle = _Angle;
        Vector2 dir =GetVectorFromAngle(newAngle);
        RaycastHit2D hit = Physics2D.Raycast(curPot,dir,_ViewDistance,layerMask);
        Debug.DrawRay(curPot,GetVectorFromAngle(newAngle)*_ViewDistance,Color.red);
        
        if(hit.collider != null){
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")){
                lineRenderer.positionCount = 0;
                _OnFind = true;
                drone_Laser.target = hit.collider.gameObject;
            }else{
                DrawLine(hit.point);
                Debug.DrawRay(curPot,dir * hit.point,Color.green);
                _OnFind = false;
            }
        }else{
            DrawLine(curPot+dir*_ViewDistance);
            _OnFind = false;
        }
        
    }
    #region  Create View Field
    private void CreateMesh(float _Angle){
        _Mesh = new Mesh();
        meshFilter.mesh = _Mesh;
        Vector2 curPot = transform.position;
        //INIT
        _Vertices = new Vector3[_RayCount+2];
        _UV = new Vector2[_Vertices.Length];
        _Triangles = new int[_RayCount *3];
        float newAngle = _Angle;
        _Vertices[0] = transform.InverseTransformPoint(curPot);
        int verticesIdx = 1;
        int trianglesIdx = 0;
        Vector2 dir;
        //INIT
        
        for(int i = 0; i<=_RayCount; i++){
            Vector3 vertex;
            dir = GetVectorFromAngle(newAngle);
            RaycastHit2D hit = Physics2D.Raycast(curPot,dir,_ViewDistance,layerMask);

            if(hit.collider == null){
                vertex = transform.InverseTransformPoint((Vector3)curPot + GetVectorFromAngle(newAngle) * _ViewDistance);
            }else{
                vertex = transform.InverseTransformPoint((Vector3)hit.point);
            }
            
            _Vertices[verticesIdx] = vertex;

            if(i>0){
                _Triangles[trianglesIdx] = 0;
                _Triangles[trianglesIdx+1] = verticesIdx-1;
                _Triangles[trianglesIdx+2] = verticesIdx;
                trianglesIdx +=3;
            }
             
             verticesIdx++;
            //  _Angle -= AngleIncrease;
                newAngle -= AngleIncrease;    
        }

        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        _Mesh.triangles = _Triangles;

    }
    #endregion


    #region  Util
    private void DelMesh(){
        
        if(meshFilter.mesh != null){
            meshFilter.mesh = null;
        }
    }
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
    #endregion

}
