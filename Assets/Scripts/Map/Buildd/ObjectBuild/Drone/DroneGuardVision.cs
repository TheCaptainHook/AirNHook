using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Engines;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DroneGuardVision : MonoBehaviour
{

    private bool _OnFind;

    // private float _MinRot = 0;
    private float _MaxRot = 120;

    [SerializeField] GameObject _FoundTargetObj;

    [SerializeField] float _RotSpeed;



    [Header("View Field")]
    
    private Mesh _Mesh;
    public float _Fov; //30 ,View Area
    public int _RayCount; // Mesh Point
    public float _Angle; // 0
    private float AngleIncrease => _Fov/_RayCount;
    public float _ViewDistance;

    public Vector3[] _Vertices;
    public Vector2[] _UV;
    public int[] _Triangles;





    private  void Start(){
        _Mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _Mesh;
        StartCoroutine(GuardVisionPrograss());

    }


    IEnumerator GuardVisionPrograss(){

        while(true){
            while(_OnFind){
                if(_FoundTargetObj == null){
                    _OnFind = false;
                }
                Debug.Log("Find Target");
                yield return null;
            }

           _Angle = Mathf.PingPong(Time.time *_RotSpeed,_MaxRot) * -1;
        
          yield return null;

        }
    }


    //TEST
    private void Update(){
      CreateMesh();
    }


    #region  Create View Field
    private void CreateMesh(){
       Vector2 curPot = transform.position;
        //INIT
        _Vertices = new Vector3[_RayCount+2];
        _UV = new Vector2[_Vertices.Length];
        _Triangles = new int[_RayCount *3];

        _Vertices[0] = transform.InverseTransformPoint(curPot);
        int verticesIdx = 1;
        int trianglesIdx = 0;
        //INIT


        for(int i = 0; i<=_RayCount; i++){
            Vector3 vertex;
            
            RaycastHit2D hit = Physics2D.Raycast(curPot,GetVectorFromAngle(_Angle),_ViewDistance);

            if(hit.collider == null){
                // vertex = transform.position + GetVectorFromAngle(_Angle) * _ViewDistance;
                vertex = transform.InverseTransformPoint((Vector3)curPot + GetVectorFromAngle(_Angle) * _ViewDistance);
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
             _Angle -= AngleIncrease;
        }

        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        _Mesh.triangles = _Triangles;

    }
    #endregion



    #region  Util
    private Vector3 GetVectorFromAngle(float angle){
        //Convert Radian from angle
        float radian = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(radian),Mathf.Sin(radian)).normalized;
    }
    #endregion

}
