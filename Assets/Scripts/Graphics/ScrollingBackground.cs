using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    private Transform _cam;
    private Vector3 _camStartPos;
    private float _distance;

    private GameObject[] _backgrounds;
    private Material[] _mat;
    private float[] _backSpeed;
    
    [SerializeField][Range(0f, 1f)] private float _scrollSpeed = 0.2f;

    private float _farthestback;
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    private void Awake()
    {
        _cam = Camera.main.transform;
        _camStartPos = _cam.position;

        int backCount = transform.childCount;
        _mat = new Material[backCount];
        _backSpeed = new float[backCount];
        _backgrounds = new GameObject[backCount];

        for (int i = 0; i < backCount; i++)
        {
            _backgrounds[i] = transform.GetChild(i).gameObject;
            _mat[i] = _backgrounds[i].GetComponent<Renderer>().material;
        }
        BackSpeedCalculate(backCount);
    }

    //TODO : 이 계산을 카메라와 z거리 말고는 뭐가 있을까 = Z거리를 넣는다고 그래픽적 효과가 없는데 랜더링 과정만 늘어나는데
    private void BackSpeedCalculate(int backCount)
    {
        for (int i = 0; i < backCount; i++)
        {
            if ((_backgrounds[i].transform.position.z - _cam.position.z) > _farthestback)
            {
                _farthestback = _backgrounds[i].transform.position.z - _cam.position.z;
            }
        }

        for (int i = 0; i < backCount; i++)
        {
            _backSpeed[i] = 1 - (_backgrounds[i].transform.position.z - _cam.position.z) / _farthestback;
        }
    }

    private void LateUpdate()
    {
        _distance = _cam.position.x - _camStartPos.x;
        transform.position = new Vector3(_cam.position.x, transform.position.y, 0);
        
        for (int i = 0; i < _backgrounds.Length; i++)
        {
            float speed = _backSpeed[i] * _scrollSpeed;
            _mat[i].SetTextureOffset(MainTex, new Vector2(_distance, 0) * speed);
        }
    }
}
