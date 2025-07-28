using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PingWheel_Item_Marker : MonoBehaviour
{

    private PlayerCameraView playerCameraView;
    private PlayerCameraView PCV
    {
        get
        {
            playerCameraView ??= Camera.main.GetComponent<PlayerCameraView>();
            return playerCameraView;
        }
    }

    [ReadOnly]
    public Vector2 pingPosition;
    public void Setting_PingPosition(Vector2 pot)
    {
        pingPosition = pot;
        onPing = true;
    }

  
    private bool onPing;
    void Update()
    {
        if (onPing)
        {
            Update_Ping();
        }
    }

    private void Update_Ping()
    {
        if (Check_ViewPort(pingPosition))
        {
            transform.position = pingPosition;
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeScale(true));
        }
        else
        {
            SetPingItem();
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeScale(false));
        }
    }


    private void SetPingItem()
    {
        var viewport = Camera.main.WorldToViewportPoint(pingPosition);
        var edgeViewportPosition = ConvertViewport(viewport);
        var worldPos = Camera.main.ViewportToWorldPoint(edgeViewportPosition);

        transform.position = new Vector3(worldPos.x, worldPos.y, 0);
        // TargetRotation(pingPosition);
    }
    private Vector3 maxScale = Vector3.one;
    private Vector3 minScale = new Vector3(.75f,.75f,.75f);
    private Coroutine fadeCoroutine;
    private float duration = 0.5f;
    
    IEnumerator FadeScale(bool inOut)
    {
        Vector3 target = inOut ? maxScale : minScale;

        if (transform.localScale == target)
        {
            fadeCoroutine = null;
            yield break;
        }

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localScale = Vector3.Lerp(transform.localScale, target, t);
            yield return null;
        }
        transform.localScale = target;
        fadeCoroutine = null;

    }

   

    #region  Util
    private bool Check_ViewPort(Vector2 pot)
    {
        var viewport = Camera.main.WorldToViewportPoint(pot);

        return viewport.x >= 0f && viewport.x <= 1f &&
                viewport.y >= 0f && viewport.y <= 1f;

    }
    private Vector3 ConvertViewport(Vector3 viewport)
    {
        Vector3 edgeViewportPosition = viewport;

        edgeViewportPosition.x = Mathf.Clamp01(viewport.x);
        edgeViewportPosition.y = Mathf.Clamp01(viewport.y);

        // Add padding to keep it slightly inside the edges
        edgeViewportPosition.x = Mathf.Clamp(edgeViewportPosition.x, 0.0625f, 0.95f);
        edgeViewportPosition.y = Mathf.Clamp(edgeViewportPosition.y, 0.0625f, 0.9f);

        return edgeViewportPosition;
    }

    private void TargetRotation(Vector3 target)
    {
        Vector3 dir = transform.position - Managers.Game.Player.transform.position;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        transform.rotation = targetRotation;
    }
    
    float _MaxScale = 1;
    float _MinScale = 0.5f;
    private void TargetScale()
    {
        float t = PCV.GetZoomRatio();
        float scaleRatio = Mathf.Lerp(_MinScale, _MaxScale, t);
        transform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);
    }
    #endregion

    
}
