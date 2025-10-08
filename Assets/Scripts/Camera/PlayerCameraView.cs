using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


//TODO 0928 DEVELOP CODE LINE : 09,17~


public enum ViewMode{  //TODO 0928
    Default,
    Wide
}


public class PlayerCameraView : MonoBehaviour
{
    //TODO 0928
    private ViewMode _ViewMode;
    Camera mainCamera;
    float tolerance = 0.001f;
    //Test Code
    //[SerializeField] Transform Player;
    //[SerializeField] Transform OtherPlayer;
    //Release Code

    public CameraGlobalVolumeController _CameraGlobalVolumeController;
    public CameraImageEffects _CameraImageEffects;
    private Transform Player
    {
        get {
            if (Managers.Game.Player == null)
            {
                return null;
            }

            return Managers.Game.Player.transform;


        }
    }

    private Transform OtherPlayer{
        get{
             try{
                return Managers.Game.OtherPlayer?.transform;
            }catch{
                return null;
            }
        }
    } 


    [Header("Info")]
    //need two player coord, update()
    [SerializeField] float _TriggerDistance; //7
    // [SerializeField] float _MaxDistance; // 12

    [SerializeField] float _MinZoom; //3
    [SerializeField] float _MaxZoom; //10

    [Header("Main Logic")]
   
    // private bool onDefaultMode;
    // private bool onTwoPlayer;
    // private bool onFadeZoom;

    // private Coroutine _FadeZoomCoroutine;
    //  private Coroutine _AdjustCameraSizeCoroutine;

    float _Zoom;

    [Header("Follow Camera")]
    public bool notFollowCam;
    private float _smoothSpeed = 0.4f;
    [Header("Camera Zoom")]
    private Vector3 _vecVelocity = Vector3.zero;
    private float _floatVelocity = 0;
    

    private float cameraOrthograpicSizeAdd = 0.05f;

    [Header("Camera Size")]
    private float increasedCameraViewRate = 0.96f;
    // private float decreasedCameraViewRate = .97f;


    [SerializeField] PlayerCameraViewMarker marker; //TODO 0817


    private void Awake()
    {
        mainCamera = Camera.main;
        
    }

    private void Reset()
    {
        mainCamera.orthographicSize = _MinZoom;
    }

    private float scroll;
    public bool onChangeModeDefaultFromWide;
    private Coroutine smoothZoomToDefaultCo;
    private void LateUpdate()
    {
        if (Player == null) return;
        scroll = Input.GetAxis("Mouse ScrollWheel");
        ViewMode previousMode = _ViewMode;
        _ViewMode = Ch_ViewMode(scroll);

        if (previousMode == ViewMode.Wide && _ViewMode == ViewMode.Default && !onChangeModeDefaultFromWide)
        {
            onChangeModeDefaultFromWide = true;
            smoothZoomToDefaultCo = StartCoroutine(SmoothZoomToDefault());
        }

        if (onChangeModeDefaultFromWide && mainCamera.orthographicSize < _MaxZoom)
        {
            if (smoothZoomToDefaultCo != null) StopCoroutine(smoothZoomToDefaultCo);
            onChangeModeDefaultFromWide = false;
        }

        switch (_ViewMode)
        {
            case ViewMode.Wide:
                WideViewMode();
                break;
            case ViewMode.Default:
                DefaultViewMode();
                break;
        }

        if (_ViewMode == ViewMode.Default)
        {
            InGameZoomInAndOut(scroll);
        }
    }

    #region REFACTORING

    private ViewMode Ch_ViewMode(float scroll)
    {
        if (OtherPlayer == null) return ViewMode.Default;

        if (IsDistanceWithinThreshold(_TriggerDistance)
            && IsDistanceWithinThreshold() <= 20
            && scroll >= 0
            && mainCamera.orthographicSize >= _MaxZoom)
        {
            return ViewMode.Wide;
        }
        else
        {
            return ViewMode.Default;
        }
    }


    private IEnumerator SmoothZoomToDefault() {
    float targetZoom = _MaxZoom - 0.1f;
    while (mainCamera.orthographicSize > targetZoom) {
        mainCamera.orthographicSize = Mathf.SmoothDamp(
            mainCamera.orthographicSize, 
            targetZoom, 
            ref _floatVelocity, 
            _smoothSpeed, 
            float.MaxValue, 
            Time.deltaTime
        );

        yield return null;
    }
    smoothZoomToDefaultCo = null;
}

    private void DefaultViewMode(){
        if(!onChangeModeDefaultFromWide){
            CheckOtherPlayerAndMarker();
        }
        FollowCamera(Player);
    }

    private void WideViewMode(){
        if(marker.gameObject.activeSelf){
             marker.gameObject.SetActive(false);
        }
        if(_ViewMode == ViewMode.Default) return;
        if(onChangeModeDefaultFromWide) return;

        Vector3 center = (Player.position + OtherPlayer.position) / 2;
        FollowCamera(center);
        
        var p1 = IsObjectInView(Player, increasedCameraViewRate);
        var p2 = IsObjectInView(OtherPlayer, increasedCameraViewRate);

        if(!p1.isInView || !p2.isInView){
             AdjustCameraView(true);
        }
    }

    private void FadeZoom(float target){
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize,target,ref _floatVelocity,_smoothSpeed,float.MaxValue,Time.deltaTime);
    }

    
    private void AdjustCameraView(bool isIncreasing){
        float addCameraSize = isIncreasing ? cameraOrthograpicSizeAdd : -cameraOrthograpicSizeAdd;
        float target = mainCamera.orthographicSize += addCameraSize;
        FadeZoom(target);   
     
    }


    #endregion

    #region TEST CODE

    // private float GetDistance(Vector3 point1, Vector3 point2)
    // {
    //     return Vector3.Distance(point1, point2);
    // }

    // private float GetDistance()
    // {
    //     float distance = Vector3.Distance(Player.position, OtherPlayer.position);
    //     return distance;
    // }

    #endregion


    //-----0408 Moving Platform SetParent
    public bool onCancel;
    //-----0408 Moving Platform SetParent
   
   #region  Mark
     private void CheckOtherPlayerAndMarker(){
        if (onCancel) return;

        if (OtherPlayer == null)
        {
            SetMarkerActive(false);
            return;
        }

        bool isInView = IsObjectInView(OtherPlayer, 1).isInView;
        SetMarkerActive(!isInView);

        if (!isInView) 
        {
            marker.SettingCam(OtherPlayer);
        }
    }

    private void SetMarkerActive(bool isActive) {
        if (marker.gameObject.activeSelf != isActive) {
            marker.gameObject.SetActive(isActive);
        }
    }
    public float GetZoomRatio()
    {
        return Mathf.InverseLerp(_MinZoom,_MaxZoom,mainCamera.orthographicSize);
    }

    #endregion
   
    #region Util
    
    private float zoomInOutSpeed = 1;
    private void InGameZoomInAndOut(float scroll)
    {
        var sm = Player.TryGetComponent(out PlayerSM psm) ? psm : null;

        if (sm == null) return;
        if (!sm.canAction) return;

        _Zoom = Math.Min(mainCamera.orthographicSize, _MaxZoom) + scroll*zoomInOutSpeed;
        _Zoom = Mathf.Clamp(_Zoom, _MinZoom, _MaxZoom);

        mainCamera.orthographicSize = _Zoom;

        if (Mathf.Abs(mainCamera.orthographicSize - _MaxZoom) < tolerance)
        {
            mainCamera.orthographicSize = _MaxZoom;
        }

        
    }


    #region Follow Camera
    public bool isCameraCenter;
    private void FollowCamera(Transform target) //Use Default Mode
    {
        try
        {
            if (target == null || notFollowCam)
            {
                isCameraCenter = false;
                return;
            }

            var _playerPos = new Vector3(target.position.x, target.position.y + 1f, -1);
            var dis = Vector2.Distance(transform.position,_playerPos);

            if(dis>0.01f)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _playerPos, ref _vecVelocity, _smoothSpeed,
                float.MaxValue, Time.fixedDeltaTime);
                isCameraCenter = dis < 1.5f;
            }

        }
        catch (Exception)
        {
            // ignored
        }
    }
    
    public void SetCamerPosition()
    {
        try
        {
            transform.position = Player.position + new Vector3(0,0,-1);
            if (notFollowCam) notFollowCam = false;
        }
        catch
        {
            Debug.Log("Can't find Player");
        }
       
    }


    private void FollowCamera(Vector3 target) //Use Wide Mode
    {
        var _playerPos = new Vector3(target.x, target.y + 1f, -1);
        if(Vector3.Distance(transform.position,target)> 0.01f){
            transform.position = Vector3.SmoothDamp(transform.position, _playerPos, ref _vecVelocity, _smoothSpeed,
            float.MaxValue, Time.fixedDeltaTime);
            

            
        }
        
    }


    #endregion


    (bool isInView,Vector3 viewPort) IsObjectInView(Transform obj,float rate)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(obj.position);

        bool isInView = viewportPoint.x >= 1-rate && viewportPoint.x <= rate &&
                        viewportPoint.y >= 1-rate && viewportPoint.y <= rate &&
                        viewportPoint.z > 0;
        return (isInView,viewportPoint);
    }


    bool IsDistanceWithinThreshold(float thresholdDistance)
    {
        if(OtherPlayer == null) return false;
        Vector3 worldDistance = Player.position - OtherPlayer.position;
        // 카메라의 가로 세로 비율에 따라 거리 조정
        Vector3 adjustedDistance = new Vector3(worldDistance.x / mainCamera.aspect, worldDistance.y, worldDistance.z);
        // 조정된 거리를 이용해 크기 계산
        float adjustedMagnitude = adjustedDistance.magnitude;

        // Debug.Log($"[BOOL] distance : {worldDistance}, adjustedDistance : {adjustedDistance}, magnutude : {adjustedMagnitude}");

        return adjustedMagnitude >= thresholdDistance;  

    }
    float IsDistanceWithinThreshold()
    {
        if (OtherPlayer == null) return 0;

        Vector3 worldDistance = Player.position - OtherPlayer.position;
        Vector3 adjustedDistance = new Vector3(worldDistance.x / mainCamera.aspect, worldDistance.y, worldDistance.z);
        float adjustedMagnitude = Mathf.Floor(adjustedDistance.magnitude * 100) / 100f;
        return adjustedMagnitude;
    }

    #endregion
}
