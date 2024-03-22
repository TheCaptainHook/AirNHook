using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Vector3 _origin;
    private Vector3 _difference;
    private Camera _cam;
    private Vector3 _playerPos;
    
    private float _zoom;
    private float _zoomMultiplier = 4f;
    private float _minZoom = 2f;
    private float _maxZoom = 8f;
    private float _velocity = 0f;
    private float _smoothTime = 0.25f;
    private float _smoothSpeed = 0.25f;
    
    private void Start()
    {
        _cam = Camera.main;
        _zoom = _cam.orthographicSize;
    }

    private void Update()
    {
        try
        {
            if (NetworkClient.ready && Managers.Game.Player != null)
                _playerPos = Managers.Game.Player.transform.position;
        }
        catch (NullReferenceException e) { }
    }

    private void FixedUpdate()
    {
        if (Managers.Game.CurrentState is GameState.Game or GameState.Lobby)
        {
            FollowPlayer();
        }
    }

    private void LateUpdate()
    {
        if (Managers.Game.CurrentState is GameState.Editor)
        {
            PanCamera();
            //ZoomInAndOut();
        }
    }

    private void PanCamera()
    {
        //마우스 월드 스페이스 시작 지점 저장
        if (Input.GetMouseButtonDown(1))
        {
            _origin = _cam.ScreenToWorldPoint(Input.mousePosition);
        }
        
        //드래그 오리진 지점과 새로운 지점간 거리 차이 계산
        if (!Input.GetMouseButton(1)) return;
        _difference = _origin - _cam.ScreenToWorldPoint(Input.mousePosition);
            
        //해당 지점으로 이동
        _cam.transform.position += _difference;
    }

    private void ZoomInAndOut()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        _zoom -= scroll * _zoomMultiplier;
        _zoom = Mathf.Clamp(_zoom, _minZoom, _maxZoom);
        _cam.orthographicSize = Mathf.SmoothDamp(_cam.orthographicSize, _zoom, ref _velocity, _smoothTime);
    }

    private void FollowPlayer()
    {
        try
        {
            var player = Managers.Game.Player;
            
            var position = transform.position;
            _playerPos = new Vector3(_playerPos.x, _playerPos.y, position.z);
            var smoothPos = Vector3.Lerp(position, _playerPos, _smoothSpeed);
            transform.position = smoothPos;
        }
        catch(NullReferenceException e) { }
    }
}
