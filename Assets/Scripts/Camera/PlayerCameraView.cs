using System;
using System.Collections;
using System.ComponentModel.Design;
using UnityEngine;


//TODO 0817 DEVELOP CODE LINE : 


public class PlayerCameraView : MonoBehaviour
{
    
    Camera mainCamera;

    //Test Code
    //[SerializeField] Transform Player;
    //[SerializeField] Transform OtherPlayer;
    //Release Code
    private Transform Player
    {
        get { return Managers.Game.Player?.transform; }
    }
    private Transform OtherPlayer => Managers.Game.OtherPlayer?.transform;


    [Header("Info")]
    //need two player coord, update()
    [SerializeField] float _TriggerDistance; //7
    [SerializeField] float _MaxDistance; // 12

    [SerializeField] float _MinZoom; //6
    [SerializeField] float _MaxZoom; //8

    [Header("Main Logic")]
    private bool onPrograss; //change cameraSize methode prograss
    private bool onMarker;
    private bool onWideMode;
    private bool onDefaultMode;
    private bool onTwoPlayer;
    private bool onFadeZoom;
    private bool onUserControlZoom;
    //private Vector3 beforeCameraPosition;
    //private Vector3 beforeCameraSize;

    private float beforeDistance;

    private Coroutine _FadeZoomCoroutine;
     private Coroutine _AdjustCameraSizeCoroutine;

    float _Zoom;

    [Header("Follow Camera")]
    private float _smoothSpeed = 0.5f;
    private Vector3 _vecVelocity = Vector3.zero;
    private float _vecSpeed = 0;

    private float cameraOrthograpicSizeAdd = 0.1f;

    [Header("Camera Size")]
    private float increasedCameraViewRate = .95f;
    private float decreasedCameraViewRate = .95f;


    [SerializeField] PlayerCameraViewMarker marker; //TODO 0817



    private void Awake()
    {
        mainCamera = Camera.main;
        
    }

    private void Update() 
    {
        if(Player == null) return;

        // InGameZoomInAndOut();
        SetCameraAngle();
    }
  
    private void Reset()
    {
        mainCamera.orthographicSize = _MinZoom;
    }

    private bool CheckTwoPlayer(){
        if(Player != null && OtherPlayer != null){
            if(_FadeZoomCoroutine!=null){
                StopCoroutine(_FadeZoomCoroutine);
                _FadeZoomCoroutine = null;
            }
          return true;      
        } 
        return false;
    }
    private void SetCameraAngle()
    {
       onTwoPlayer = CheckTwoPlayer();

       if (IsDistanceWithinThreshold(_TriggerDistance)) 
        {
            onWideMode = true;
            Debug.Log("WideView Mode");
            if (marker.gameObject.activeSelf)
            {
                marker.gameObject.SetActive(false);
                onMarker = false;
            }

            WideViewMode();


        }
        else//
        {
            Debug.Log("default Mode");
            onWideMode = false;
            OnDefaultMode();
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
        if(marker.gameObject.activeSelf){
            marker.gameObject.SetActive(false);
        }

        Vector3 center = (Player.position + OtherPlayer.position) / 2;
        FollowCamera(center);

        float distance = GetDistance();

        //Check camera Viewport

        if (!onPrograss && Mathf.Abs(distance - beforeDistance) > 0.001f)
        {
            if(distance > beforeDistance )
            {
                if(_AdjustCameraSizeCoroutine == null)
                {
                    _AdjustCameraSizeCoroutine = StartCoroutine(AdjustCameraSize(true));
                }
            
            }
            else
            {

                if (_AdjustCameraSizeCoroutine == null)
                {
                    _AdjustCameraSizeCoroutine = StartCoroutine(AdjustCameraSize(false));
                }
            }

        }

        beforeDistance = distance;
    }



   private void OnMarkerMode() //TODO 0817
    {
        //marker Setting
        if (!marker.gameObject.activeSelf)
        {
            marker.gameObject.SetActive(true);
            marker.SettingCam(OtherPlayer);
        }
        else
        {
            marker.SettingCam(OtherPlayer);
        }
        

        FollowCamera(Player);
    }
    private void OnDefaultMode(){
        beforeDistance = 0;
        CheckOtherPlayerAndMarker();

        FollowCamera(Player);
    }

    #region Util
    private void CheckOtherPlayerAndMarker(){
        if(OtherPlayer != null){
            if(!IsObjectInView(OtherPlayer,1)){
                 if (marker.gameObject.activeSelf){
                    marker.SettingCam(OtherPlayer);
                }else{
                    marker.gameObject.SetActive(true);
                    marker.SettingCam(OtherPlayer);
                }
            }else{
                marker.gameObject.SetActive(false);
            }
        }else{
            if(marker.gameObject.activeSelf){
                marker.gameObject.SetActive(false);
            }
        }
    }
     private void InGameZoomInAndOut()
    {
        if(onFadeZoom) return;
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll == 0 && _Zoom>= _MaxDistance){
            onUserControlZoom = false;
        }else if(scroll != 0){
            _Zoom -= scroll * 1;
            onUserControlZoom = true;
            StopAllCoroutines();
            _Zoom = Mathf.Clamp(_Zoom, _MinZoom, _MaxZoom);
            mainCamera.orthographicSize = _Zoom;

        }
        
        

       

        
        // mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, _Zoom, ref _vecVelocity, _smoothSpeed);
        
    }


    #region Follow Camera
    private void FollowCamera(Transform target)
    {
        try
        {
            if (target == null) return;

            var _playerPos = new Vector3(target.position.x, target.position.y + 1f, -1);
            if(Vector3.Distance(transform.position,target.position)>0.01f){
                 transform.position = Vector3.SmoothDamp(transform.position, _playerPos, ref _vecVelocity, _smoothSpeed,
                float.MaxValue, Time.fixedDeltaTime);
            }
           
        }
        catch (Exception)
        {
            // ignored
        }
    }




    private void FollowCamera(Vector3 target)
    {
        var _playerPos = new Vector3(target.x, target.y + 1f, -1);
        if(Vector3.Distance(transform.position,target)> 0.01f){
            transform.position = Vector3.SmoothDamp(transform.position, _playerPos, ref _vecVelocity, _smoothSpeed,
            float.MaxValue, Time.fixedDeltaTime);
        }
        
    }


    #endregion

    #region Camera Size
    IEnumerator AdjustCameraSize(bool isIncreasing)
    {
        if(Player == null || OtherPlayer == null) yield break;
        onPrograss = true;

        float viewRate = isIncreasing ? increasedCameraViewRate : decreasedCameraViewRate;
        float addCameraSize = isIncreasing ? cameraOrthograpicSizeAdd : -cameraOrthograpicSizeAdd;

        bool p1CameraInView = IsObjectInView(Player, viewRate);
        bool p2CameraInView = IsObjectInView(OtherPlayer, viewRate);

        while((isIncreasing && (!p1CameraInView || !p2CameraInView)) || (!isIncreasing && p1CameraInView && p2CameraInView))
        {
            
            float cameraSize = mainCamera.orthographicSize + addCameraSize;
            cameraSize = Math.Clamp(cameraSize, _MinZoom, 99);

            // yield return FadeZoom(cameraSize);
            mainCamera.orthographicSize = cameraSize;

            if(Player == null || OtherPlayer == null) break;

            p1CameraInView = IsObjectInView(Player, viewRate);
            p2CameraInView = IsObjectInView(OtherPlayer, viewRate);
          
            yield return null;
        }

        onPrograss = false;
        _AdjustCameraSizeCoroutine = null;
    }

    IEnumerator FadeZoom(float target)
    {
        onFadeZoom = true;

        float percent = 0;

        float orgSize = mainCamera.orthographicSize;
        while (percent < 1)
        {
            percent += Time.deltaTime * 5;


            float size = Mathf.Lerp(orgSize, target, percent);

            mainCamera.orthographicSize = size;

            yield return null;
        }
        
        mainCamera.orthographicSize = target;


        onFadeZoom = false;
        _FadeZoomCoroutine = null;
    }
    
    #endregion



    bool IsObjectInView(Transform obj,float rate)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(obj.position);

        // 뷰포트 좌표는 (0, 0)에서 (1, 1) 사이에 있음
        // z값은 카메라 앞에 있을 경우 양수, 뒤에 있을 경우 음수임
        bool isInView = viewportPoint.x >= 1-rate && viewportPoint.x <= rate &&
                        viewportPoint.y >= 1-rate && viewportPoint.y <= rate &&
                        viewportPoint.z > 0;

        return isInView;
    }


    bool IsDistanceWithinThreshold(float thresholdDistance)
    {
        if(!onTwoPlayer) return false;
        Vector3 worldDistance = Player.position - OtherPlayer.position;
        // 카메라의 가로 세로 비율에 따라 거리 조정
        Vector3 adjustedDistance = new Vector3(worldDistance.x / mainCamera.aspect, worldDistance.y, worldDistance.z);
        // 조정된 거리를 이용해 크기 계산
        float adjustedMagnitude = adjustedDistance.magnitude;

        Debug.Log($"distance : {worldDistance}, adjustedDistance : {adjustedDistance}, magnutude : {adjustedMagnitude}");

        return adjustedMagnitude >= thresholdDistance;  

    }

    #endregion
}
