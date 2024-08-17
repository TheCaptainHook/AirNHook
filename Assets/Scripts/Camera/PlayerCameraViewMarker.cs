using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraViewMarker : MonoBehaviour
{
    [SerializeField] Camera markingCam;

    [SerializeField] Vector3 offset;



    public void SettingCam(Transform player)
    {
        //gameObject Position Setting
        gameObject.transform.position = GetCameraEdgePosition(player);

        //markingCam Positiion Setting
        markingCam.transform.position = new Vector3(player.position.x, player.position.y+.6f, -1);
    }




    private Vector3 GetCameraEdgePosition(Transform player)
    {
        Vector3 viewport = Camera.main.WorldToViewportPoint(player.position);

        Vector3 edgeViewportPosition = viewport;

        if (viewport.x < 0)
        {
            edgeViewportPosition.x = 0;
        }
        else if(viewport.x > 1)
        {
            edgeViewportPosition.x = 1;
        }


        if(viewport.y < 0)
        {
            edgeViewportPosition.y = 0;
        }
        else if(viewport.y > 1)
        {
            edgeViewportPosition.y = 1;
        }



        

        Debug.Log($"Viewport : {edgeViewportPosition}, normalized : {edgeViewportPosition.normalized}");




        gameObject.transform.right = edgeViewportPosition.normalized;

        return Camera.main.ViewportToWorldPoint(edgeViewportPosition);

    }


}
