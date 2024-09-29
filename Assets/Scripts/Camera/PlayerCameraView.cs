using System;

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

    private int testCount;
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
   
    // private bool onDefaultMode;
    private bool onTwoPlayer;
    private bool onFadeZoom;

    // private Coroutine _FadeZoomCoroutine;
    //  private Coroutine _AdjustCameraSizeCoroutine;

    float _Zoom;

    [Header("Follow Camera")]
    private float _smoothSpeed = 0.5f;
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

    private bool CheckTwoPlayer(){
        if(Player != null && OtherPlayer != null){
          return true;      
        } 
        return false;
    }


    private void Update(){
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        _ViewMode = Ch_ViewMode(scroll);

        switch(_ViewMode){
            case ViewMode.Wide:
                WideViewMode();
            break;
            case ViewMode.Default:
                OnDefaultMode();
            break;
        }
             
        if(_ViewMode == ViewMode.Default){
            //MouseZoomInOut;
            InGameZoomInAndOut(scroll);
        }
	
    }

    #region REFACTORING
    private ViewMode Ch_ViewMode(float scroll){
        if(OtherPlayer == null) return ViewMode.Default;

        if(IsDistanceWithinThreshold(_TriggerDistance) && IsDistanceWithinThreshold() <= 20 && scroll >= 0 && mainCamera.orthographicSize >= _MaxZoom){
            return ViewMode.Wide;
        }else{
            return ViewMode.Default;
        }
    }

    private void WideViewMode(){
        if(marker.gameObject.activeSelf){
             marker.gameObject.SetActive(false);
        }

        Vector3 center = (Player.position + OtherPlayer.position) / 2;
        FollowCamera(center);
        
        var p1 =  IsObjectInView(Player, increasedCameraViewRate);
        var p2 = IsObjectInView(OtherPlayer, increasedCameraViewRate);

        if(!p1.isInView || !p2.isInView){
             AdjustCameraView(true);
        }
    }

    private void FadeZoom(float target){
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize,target,ref _floatVelocity,_smoothSpeed,float.MaxValue,Time.deltaTime);
         
    }

    
    private void AdjustCameraView(bool isIncreasing){
       
        // float viewRate = isIncreasing ? increasedCameraViewRate : decreasedCameraViewRate; 
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

    private void OnDefaultMode(){
        CheckOtherPlayerAndMarker();

        FollowCamera(Player);
    }

    #region Util
    private void CheckOtherPlayerAndMarker(){
        if(OtherPlayer != null){
            if(!IsObjectInView(OtherPlayer,1).isInView){
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
     private void InGameZoomInAndOut(float scroll)
    {
        mainCamera.orthographicSize  = Mathf.Clamp(mainCamera.orthographicSize,_MinZoom,_MaxZoom);
        _Zoom = mainCamera.orthographicSize + scroll;

        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize,_Zoom,ref _floatVelocity,0.1f,float.MaxValue,Time.deltaTime);
        
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


    (bool isInView,Vector3 viewPort) IsObjectInView(Transform obj,float rate)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(obj.position);
        
        // 뷰포트 좌표는 (0, 0)에서 (1, 1) 사이에 있음
        // z값은 카메라 앞에 있을 경우 양수, 뒤에 있을 경우 음수임
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
    float IsDistanceWithinThreshold(){
         if(OtherPlayer == null) return 0;

        Vector3 worldDistance = Player.position - OtherPlayer.position;
        // 카메라의 가로 세로 비율에 따라 거리 조정
        Vector3 adjustedDistance = new Vector3(worldDistance.x / mainCamera.aspect, worldDistance.y, worldDistance.z);
        // 조정된 거리를 이용해 크기 계산
        float adjustedMagnitude = Mathf.Floor(adjustedDistance.magnitude * 100) /100f;

        // Debug.Log($"[FLOAT]  distance : {worldDistance}, adjustedDistance : {adjustedDistance}, magnutude : {adjustedMagnitude}");
        return adjustedMagnitude;
    }

    #endregion
}
