using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCameraView : MonoBehaviour
{
    // Camera position -> Mathf.Abs(player1.positon - player2.position) /2
    // when two plyaer distance
    // how to check when i player1? player2?

    //plyaer not control camera zooom,
    //other player distacne check, control camera zoom


    //CameraHolder in CameraFollow Script => PlayerCameraView
    //CameraMove -> PlayerCameraView 


    //Take Care Task    1. Method prograss when condition are met
    //                  2. 
    
    Camera mainCamera;

    [Header("Test Code")]
    [SerializeField] Transform Player;
    [SerializeField] Transform OtherPlayer;

    //private Transform Player => Managers.Game.Player.transform;
    //private Transform OtherPlayer => Managers.Game.OtherPlayer.transform;


    [Header("Info")]
    //need two player coord, update()
    [SerializeField] float _TriggerDistance; //7
    [SerializeField] float _MaxDistance; // 16

    [SerializeField] float _MinZoom; //8
    [SerializeField] float _MaxZoom; //10

    [Header("Main Logic")]
    private bool onPrograss; //change cameraSize methode prograss
    private bool onMarker;

    private Vector3 beforeCameraPosition;
    private Vector3 beforeCameraSize;

    private float beforeDistance;

    private Coroutine _FadeZoomCoroutine;
    private Coroutine _CameraSizeChangeCoroutine;


    [Header("Follow Camera")]
    private float _smoothSpeed = 0.5f;
    private Vector3 _vecVelocity = Vector3.zero;
    private float _vecSpeed = 0;

    private float cameraOrthograpicSizeAdd = 0.1f;

    [Header("Camera Size")]
    private float increasedCameraViewRate = .9f;
    private float decreasedCameraViewRate = .92f;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.M))//TODO TEST CODE
        {
            GetDistance();
        }

        SetCameraAngle();

    }

    private void Reset()
    {
        mainCamera.orthographicSize = _MinZoom;
    }


    private void SetCameraAngle()
    {
        if (OtherPlayer == null)
        {
            FollowCamera(Player);
            return;
        }

        float distance = GetDistance(Player.position,OtherPlayer.position);

        if (IsDistanceWithinThreshold(_MaxDistance)) // Mark
        {
            OnMarkerMode();
        }
        else if (IsDistanceWithinThreshold(_TriggerDistance))  //IsDistanceWithInThreshold 부터 작업하기, onmark 작업하기
        {
            onMarker = false;
            WideViewMode();
            //
        }else//
        {
            onPrograss = false;
            onMarker = false;

            FollowCamera(Player);
        }

    }

    #region TEST CODE

    private float GetDistance(Vector3 point1, Vector3 point2)
    {
        return Vector3.Distance(point1, point2);
    }

    private float GetDistance()
    {
        float distance = Vector3.Distance(Player.position, OtherPlayer.position);
        return distance;
    }

    #endregion


    private void WideViewMode()
    {
        Vector3 center = (Player.position + OtherPlayer.position) / 2;
        FollowCamera(center);

        float distance = GetDistance();

        //Check camera Viewport


        if (!onPrograss)
        {
            if(distance > beforeDistance)
            {
                StartCoroutine(FadeCameraSizeIncreased());
            }
            else
            {
                StartCoroutine(FadeCameraSizeDecreased());
            }

        }

        beforeDistance = distance;
    }



   private void OnMarkerMode()
    {
        mainCamera.orthographicSize = _MinZoom;
        FollowCamera(Player);
    }

    #region Util

    #region Follow Camera
    private void FollowCamera(Transform target)
    {
        try
        {
            if (target == null) return;

            var _playerPos = new Vector3(target.position.x, target.position.y + 1f, -1);
            transform.position = Vector3.SmoothDamp(transform.position, _playerPos, ref _vecVelocity, _smoothSpeed,
                float.MaxValue, Time.fixedDeltaTime);
        }
        catch (Exception)
        {
            // ignored
        }
    }




    private void FollowCamera(Vector3 target)
    {
        var _playerPos = new Vector3(target.x, target.y + 1f, -1);
        transform.position = Vector3.SmoothDamp(transform.position, _playerPos, ref _vecVelocity, _smoothSpeed,
            float.MaxValue, Time.fixedDeltaTime);
    }


    #endregion

    #region Camera Size
    IEnumerator FadeCameraSizeIncreased()
    {
        onPrograss = true;

        bool p1CameraInView = IsObjectInView(Player,increasedCameraViewRate);
        bool p2CameraInView = IsObjectInView(OtherPlayer, increasedCameraViewRate);

        while (!p1CameraInView || !p2CameraInView)
        {
            float cameraSize = mainCamera.orthographicSize + cameraOrthograpicSizeAdd;
            //cameraSize = Math.Clamp(cameraSize, _MinZoom, _MaxZoom);
   
            if(_FadeZoomCoroutine != null)
            {
                StopCoroutine(_FadeZoomCoroutine);
                _FadeZoomCoroutine = StartCoroutine(FadeZoom(cameraSize));
            }
            else
            {
                _FadeZoomCoroutine = StartCoroutine(FadeZoom(cameraSize));
            }

            p1CameraInView = IsObjectInView(Player, increasedCameraViewRate);
            p2CameraInView = IsObjectInView(OtherPlayer, increasedCameraViewRate);

            yield return null;
        }

        onPrograss = false;

    }

    IEnumerator FadeCameraSizeDecreased()
    {
        onPrograss = true;

        bool p1CameraInView = IsObjectInView(Player,decreasedCameraViewRate);
        bool p2CameraInView = IsObjectInView(OtherPlayer, decreasedCameraViewRate);

        while (p1CameraInView && p2CameraInView)
        {
            float cameraSize = mainCamera.orthographicSize - cameraOrthograpicSizeAdd;
            cameraSize = Math.Clamp(cameraSize, _MinZoom, 100);

            if (_FadeZoomCoroutine != null)
            {
                StopCoroutine(_FadeZoomCoroutine);
                _FadeZoomCoroutine = StartCoroutine(FadeZoom(cameraSize));
            }
            else
            {
                _FadeZoomCoroutine = StartCoroutine(FadeZoom(cameraSize));
            }

            p1CameraInView = IsObjectInView(Player, decreasedCameraViewRate);
            p2CameraInView = IsObjectInView(OtherPlayer, decreasedCameraViewRate);
            yield return null;
        }

        onPrograss = false;
    }


    IEnumerator FadeZoom(float target)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime*3;
            mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, target, ref _vecSpeed, _smoothSpeed, float.MaxValue, percent);
            //mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, target, percent);
            yield return null;
        }

        mainCamera.orthographicSize = target;
        _FadeZoomCoroutine = null;
    }
    #endregion



    bool IsObjectInView(Transform obj,float rate)
    {
        // 월드 좌표를 뷰포트 좌표로 변환
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(obj.position);

        // 뷰포트 좌표는 (0, 0)에서 (1, 1) 사이에 있음
        // z값은 카메라 앞에 있을 경우 양수, 뒤에 있을 경우 음수임
        bool isInView = viewportPoint.x >= 0 && viewportPoint.x <= rate &&
                        viewportPoint.y >= 0 && viewportPoint.y <= rate &&
                        viewportPoint.z > 0;

        return isInView;
    }


    bool IsDistanceWithinThreshold(float thresholdDistance)
    {
        // 두 오브젝트 간의 월드 좌표 거리 계산
        Vector3 worldDistance = Player.position - OtherPlayer.position;

        // 카메라의 가로 세로 비율에 따라 거리 조정
        Vector3 adjustedDistance = new Vector3(worldDistance.x / mainCamera.aspect, worldDistance.y, worldDistance.z);

        // 조정된 거리를 이용해 크기 계산
        float adjustedMagnitude = adjustedDistance.magnitude;

        Debug.Log($"distance : {worldDistance}, adjustedDistance : {adjustedDistance}, magnutude : {adjustedMagnitude}");

        // 조정된 거리가 특정 임계값(thresholdDistance) 이내인지 확인
        return adjustedMagnitude >= thresholdDistance;
    }

    #endregion
}
