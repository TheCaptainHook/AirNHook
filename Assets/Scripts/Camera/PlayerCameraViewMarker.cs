using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraViewMarker : MonoBehaviour
{
    [SerializeField] Camera markingCam;
    [SerializeField] PlayerCameraView playerCamerView;
    [SerializeField] Vector3 offset;



    public void SettingCam(Transform player)
    {
        gameObject.transform.position = GetCameraEdgePosition(player);
        markingCam.transform.position = new Vector3(player.position.x, player.position.y + .6f, -1);
    }

    private Vector3 GetCameraEdgePosition(Transform player)
    {
        //Position Settong
        Vector3 viewport = Camera.main.WorldToViewportPoint(player.position);
        Vector3 edgeViewportPosition = ConvertViewport(viewport);
        edgeViewportPosition = Camera.main.ViewportToWorldPoint(edgeViewportPosition);
        //Rotate Setting
        TargetRotation(player.position);
        //Scale Setting
        TargetScale();

        return new Vector3(edgeViewportPosition.x, edgeViewportPosition.y, 0);
    }

    private Vector3 ConvertViewport(Vector3 viewport)
    {
        Vector3 edgeViewportPosition = viewport;

        if (viewport.x < 0)
        {
            edgeViewportPosition.x = 0.1f;
        }
        else if (viewport.x > 1)
        {
            edgeViewportPosition.x = 0.9f;
        }
        if (viewport.y < 0)
        {
            edgeViewportPosition.y = 0.2f;
        }
        else if (viewport.y > 1)
        {
            edgeViewportPosition.y = .8f;
        }

        return edgeViewportPosition;
    }

    private void TargetRotation(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        transform.rotation = targetRotation;
    }

    float _MaxScale = 1;
    float _MinScale = 0.5f;
    private void TargetScale()
    {
        float t = playerCamerView.GetZoomRatio();
        float scaleRatio = Mathf.Lerp(_MinScale, _MaxScale, t);
        transform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);
    }
    
}
