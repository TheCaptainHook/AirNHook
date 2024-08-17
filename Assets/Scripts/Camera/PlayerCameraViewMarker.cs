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
            edgeViewportPosition.x = 0.1f;
        }
        else if(viewport.x > 1)
        {
            edgeViewportPosition.x = 0.9f;
        }


        if(viewport.y < 0)
        {
            edgeViewportPosition.y = 0.2f;
        }
        else if(viewport.y > 1)
        {
            edgeViewportPosition.y = .8f;
        }

        Debug.Log($"Viewport : {edgeViewportPosition}, normalized : {edgeViewportPosition.normalized}");

        edgeViewportPosition = Camera.main.ViewportToWorldPoint(edgeViewportPosition);
        TargetRotation(player.position);

        return new Vector3(edgeViewportPosition.x, edgeViewportPosition.y, 0);

    }



    private void TargetRotation(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        transform.rotation = targetRotation;
    }


}
