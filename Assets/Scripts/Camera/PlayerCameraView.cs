using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;

    private Transform Player => Managers.Game.Player.transform;
    private Transform OtherPlayer => Managers.Game.OtherPlayer.transform;


    [Header("Info")]
    //need two player coord, update()
    [SerializeField] float _TriggerDistance; //12
    [SerializeField] float _MaxDistance; // 25

    [SerializeField] float _MinZoom; //8
    [SerializeField] float _MaxZoom; //10

    [Header("Main Logic")]
    private bool onPrograss;
    private bool onMarker;

    private Vector3 beforeCameraPosition;
    private Vector3 beforeCameraSize;

    private Coroutine _CameraPositionMoveCoroutine;
    private Coroutine _CameraSizeChangeCoroutine;


    [Header("Follow Camera")]
    private float _smoothSpeed = 0.25f;
    private Vector3 _vecVelocity = Vector3.zero;



    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.M))//TODO TEST CODE
        {
            Debug.Log(GetDistance(player1.transform.position, player2.transform.position));
        }
    }

    private void Reset()
    {
        mainCamera.orthographicSize = _MinZoom;
    }


    private void SetCameraAngle()
    {
        if(OtherPlayer == null)
        {

            FollowCamera(Player);
            return;
        }


        float distance = GetDistance(player1.transform.position, player2.transform.position);
        

        if (distance > _TriggerDistance) // CameraAngle
        {
            onPrograss = true;
        } else if (distance > _MaxDistance) //Mark
        {
            onPrograss = true;
            onMarker = true;
        }else// Camrea reset and pos
        {
            onPrograss = false;
            onMarker = false;
        }


        if (onPrograss && onMarker)
        {

        }
        else if(onPrograss)
        {

        }
        else
        {
            Reset();
            FollowCamera(Player);
        }




        //Camera position change

        //Check player distance and Change Camera size

    }



    #region TEST CODE

private Vector3 GetCenterPosition(Vector3 pot1, Vector3 pot2)
    {
        return (pot1 + pot2) / 2;
    }

    private float GetDistance(Vector3 point1, Vector3 point2)
    {
        return Vector3.Distance(point1, point2);
    }

    #endregion




    //IEnumerator MoveCamera()
    //{

    //}

    //IEnumerator ChangeCameraSize()
    //{

    //}


    private void FollowCamera(Transform target)
    {
        try
        {
            if (Player == null) return;

            var _playerPos = new Vector3(target.position.x, target.position.y + 1f, target.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, _playerPos, ref _vecVelocity, _smoothSpeed,
                float.MaxValue, Time.fixedDeltaTime);
        }
        catch (Exception)
        {
            // ignored
        }
    }


}
