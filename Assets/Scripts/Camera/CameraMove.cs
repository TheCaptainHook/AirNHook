using System;
using Mirror;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Vector3 _origin;
    private Vector3 _difference;
    private Camera _cam;
    private Transform _player => Managers.Game.Player.transform;
    
    [Header("Zoom")]
    private float _zoom;
    private float _zoomMultiplier = 4f;
    private float _minZoom = 7f;
    private float _maxZoom = 20f;
    private float _iGMinZoom = 3f;
    private float _iGMaxZoom = 10f;


    private float _velocity = 0f;
    private float _smoothTime = 0.25f;
    // private float _smoothSpeed = 0.25f;
    private Vector3 _vecVelocity = Vector3.zero;
    
    private void Start()
    {
        _cam = Camera.main;
        _zoom = _cam.orthographicSize;
    }

    private void Update()
    {
        if (Managers.Game.CurrentState is GameState.Game or GameState.Lobby)
        {
            //FollowPlayer();
            InGameZoomInAndOut();
        }
    }

    private void LateUpdate()
    {
        if (Managers.Game.CurrentState is GameState.Editor)
        {
            if (!MapEditor.Instance.placeMentSystem.onEnterMapEditorUi)
            {
                PanCamera();
                ZoomInAndOut();
            }
           
        }
    }

    private void PanCamera()
    {
        //마우스 월드 스페이스 시작 지점 저장
        if (Input.GetMouseButtonDown(2))
        {
            _origin = _cam.ScreenToWorldPoint(Input.mousePosition);
        }
        
        //드래그 오리진 지점과 새로운 지점간 거리 차이 계산
        if (!Input.GetMouseButton(2)) return;
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
    
    private void InGameZoomInAndOut()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        _zoom -= scroll * _zoomMultiplier;
        _zoom = Mathf.Clamp(_zoom, _iGMinZoom, _iGMaxZoom);
        _cam.orthographicSize = Mathf.SmoothDamp(_cam.orthographicSize, _zoom, ref _velocity, _smoothTime);
    }

    // private void FollowPlayer()
    // {
    //     try
    //     {
    //         if (_player == null) return;
    //
    //         var _playerPos = new Vector3(_player.position.x, _player.position.y + 1f, transform.position.z);
    //         transform.position = Vector3.SmoothDamp(transform.position, _playerPos, ref _vecVelocity, _smoothSpeed,
    //             float.MaxValue, Time.fixedDeltaTime);
    //     }
    //     catch (Exception)
    //     {
    //         // ignored
    //     }
    // }



    #region Util
    //todo 0527
    public void Camera_MoveToObjectPosition(Vector3 position)
    {
        Camera.main.gameObject.transform.position = position;
    }


    //todo 0527
    #endregion

}
