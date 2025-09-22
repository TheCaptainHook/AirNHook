using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PairAuthDoor_Eye : MonoBehaviour
{
    [SerializeField] private Transform _eyeTransform;
    [SerializeField] private Material _scanMat;
    [SerializeField] private PairAuthDoor _pairAuthDoor;
    private MeshFilter _mf;
    private MeshRenderer _mr;
    private Mesh _mesh;

    //public Vector3 p0;
    //public Vector3 p1;
    //public Vector3 p2;

    public LayerMask _hitMask;
    public float _maxDistance = 50f;
    public int _rayCount = 20;


    public bool _leftOrRight; // true : Left , false : Right

    void Awake()
    {
        _mf = GetComponent<MeshFilter>();
        _mr = GetComponent<MeshRenderer>();
        _mesh ??= new Mesh { name = "PairAuthDoor_Eye_Mesh" };
        _mf.sharedMesh = _mesh;
        _mr.sharedMaterial = _scanMat;
    }
    public float scanSpeed = 3f;
    public IEnumerator ScaningCoroutine()
    {
        float percent = 0f;
        while (percent < 1f)
        {
            percent += Time.deltaTime * scanSpeed;
            float offsetAngle = Mathf.SmoothStep(_offsetAngle_Min, _offsetAngle_Max, percent);
            UpdateTriangleMesh(offsetAngle);
            yield return null;
        }

        StartCoroutine(CloseScan());
    }
    /**
    Right : -70~-50
    Left : -110 ~ - 130
    **/
    public float partialFov = 30f; // 원하는 각도
    public float _offsetAngle_Min;
    public float _offsetAngle_Max;
    private void UpdateTriangleMesh(float offsetAngle)
    {
        Vector3 origin = _eyeTransform.position;
        float startAngle = offsetAngle - partialFov / 2f;
        float angleIncrease = partialFov / _rayCount;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();


        vertices.Add(transform.InverseTransformPoint(origin)); 

        int vertexIndex = 1;
        for (int i = 0; i <= _rayCount; i++)
        {
            float angle = startAngle + i * angleIncrease;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * _eyeTransform.right;

            Vector3 endPoint = origin + dir * _maxDistance;
            var hit = Physics2D.Raycast(origin, dir, _maxDistance, _hitMask);


            if (hit.collider != null)
            {
                endPoint = hit.point;
            }
            else continue;

            //----------Player Check, And Player Hold
            if (hit.collider.TryGetComponent(out PlayerSM player))
            {
                _pairAuthDoor.DetectPlayer(hit.collider);
            }
            //----------Player Check

            // 로컬 변환
            vertices.Add(transform.InverseTransformPoint(endPoint));

            if (i > 0)
            {
                triangles.Add(0);
                triangles.Add(vertexIndex - 1);
                triangles.Add(vertexIndex);
            }
            vertexIndex++;
        }

        _mesh.Clear();
        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = triangles.ToArray();
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        
    }
    private IEnumerator CloseScan()
    {
        if (_mesh == null || _mesh.vertexCount == 0)
            yield break;

        List<Vector3> verts = new List<Vector3>(_mesh.vertices);

        while (verts.Count > 1)
        {
            if(_leftOrRight) // left
            {
                verts.RemoveAt(verts.Count - 1);
            }
            else //right
            {
                verts.RemoveAt(1);
            }

            List<int> tris = new List<int>();
            for (int i = 1; i < verts.Count - 1; i++)
            {
                tris.Add(0);
                tris.Add(i);
                tris.Add(i + 1);
            }

            // 메쉬 갱신
            _mesh.Clear();
            _mesh.vertices = verts.ToArray();
            _mesh.triangles = tris.ToArray();
            _mesh.RecalculateBounds();
            _mesh.RecalculateNormals();

            yield return null; // 일정 시간마다 하나씩 제거
        }

        // 다 닫히면 메쉬 제거
        MeshClear();
    }
    public void MeshClear()
    {
        _mesh.Clear();
        _mesh.vertices = Array.Empty<Vector3>();
        _mesh.triangles = Array.Empty<int>();
        _mesh.uv = Array.Empty<Vector2>();
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
    }
}
