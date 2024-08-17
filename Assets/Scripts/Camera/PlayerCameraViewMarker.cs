using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraViewMarker : MonoBehaviour
{
    [SerializeField] Camera markingCam;


    public void SettingCam(Transform player)
    {
        //gameObject Position Setting


        //markingCam Positiion Setting
        markingCam.transform.position = new Vector3(player.position.x, player.position.y+.6f, -1);
    }

}
