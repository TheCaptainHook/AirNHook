using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PairAuthDoor_Eye : MonoBehaviour
{
    [SerializeField] private Transform _eyeTransform;
    [SerializeField] private Material _scanMat;
    [SerializeField] private PairAuthDoor _parentDoor;
    private MeshFilter _mf;
    private MeshRenderer _mr;
    private Mesh _mesh;

    public Vector3 p0;
    public Vector3 p1;
    public Vector3 p2;

    public LayerMask _hitMask;

    public float _maxDistance = 50f;
    public int _rayCount = 20;

    void Awake()
    {
        _mf = GetComponent<MeshFilter>();
        _mr = GetComponent<MeshRenderer>();
        _mesh ??= new Mesh { name = "PairAuthDoor_Eye_Mesh" };
        _mf.sharedMesh = _mesh;
        _mr.sharedMaterial = _scanMat;
    }

    public AirSM _air;
    public HookSM _hook;
    public IEnumerator DetectCoroutine()
    {
        float percent = 0f;
        while (percent < 1f)
        {
            percent += Time.deltaTime;
            float offsetAngle = Mathf.SmoothStep(_offsetAngle_Min, _offsetAngle_Max, percent);
            UpdateTriangleMesh(offsetAngle);
            yield return null;
        }
        _parentDoor.AuthPlayer(_air, _hook);
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


        vertices.Add(Vector3.zero); // 원점 (로컬 좌표 기준)

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

            if (hit.collider.TryGetComponent(out AirSM air))
            {
                _air = air;
                //LineRenderer
                //Panel on
            }
            else
            {
                //LineRenderer off
                //Panel Off
                _air = null;
            }
            if (hit.collider.TryGetComponent(out HookSM hook))
            {
                //LineRenderer
                //Panel on
                _hook = hook;
            }
            else
            {
                //LineRenderer off
                //Panel off
                _hook = null;
            }

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
    public void MeshClear()
    {
        _mesh.Clear();
        _air = null;
        _hook = null;
        _mesh.vertices = Array.Empty<Vector3>();
        _mesh.triangles = Array.Empty<int>();
        _mesh.uv = Array.Empty<Vector2>();
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
    }
}
