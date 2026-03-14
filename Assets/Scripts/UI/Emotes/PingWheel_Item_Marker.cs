
using UnityEngine;

public class PingWheel_Item_Marker : MonoBehaviour
{
    [ReadOnly]
    public Vector2 pingPosition;
    public void Setting_PingPosition(Vector2 pot)
    {
        pingPosition = pot;
        onPing = true;
    }

    [SerializeField] GameObject _arrowGo; 

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
            if (_arrowGo.activeSelf) _arrowGo.SetActive(false);
            transform.position = pingPosition;

            if (transform.localScale.sqrMagnitude <= maxScale.sqrMagnitude)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, maxScale, Time.deltaTime * 2);
            }
            else transform.localScale = maxScale;
        }
        else
        {
            SetPingItem();
        }
    }

    private void SetPingItem()
    {
        var viewport = Camera.main.WorldToViewportPoint(pingPosition);
        var edgeViewportPosition = ConvertViewport(viewport);
        var worldPos = Camera.main.ViewportToWorldPoint(edgeViewportPosition);

        transform.position = new Vector3(worldPos.x, worldPos.y, 0);

        if (transform.localScale.sqrMagnitude >= minScale.sqrMagnitude)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, minScale, Time.deltaTime * 2);
        }
        else transform.localScale = minScale;

        // TargetRotation(pingPosition);
        TargetRotationArrow(pingPosition);
        if (!_arrowGo.activeSelf) _arrowGo.SetActive(true);
    }
    private Vector3 maxScale = Vector3.one;
    private Vector3 minScale = new Vector3(.65f,.65f,.65f);
   
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

    private void TargetRotationArrow(Vector3 target)
    {
        var dir = (target - Managers.Game.Player.transform.position).normalized;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _arrowGo.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    
    #endregion

    
}
