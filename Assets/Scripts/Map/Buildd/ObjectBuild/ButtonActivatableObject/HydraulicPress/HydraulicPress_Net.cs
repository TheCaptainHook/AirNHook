using System.Collections;
using Mirror;
using UnityEngine;

public class HydraulicPress_Net : NetworkBehaviour
{
    public bool onSync;
    private HydraulicPress Main => GetComponent<HydraulicPress>();

    [Server]
    public void Server_Press(bool onOff)
    {
        Rpc_Press(onOff);
    }
    [ClientRpc]
    private void Rpc_Press(bool onOff)
    {
        if(onOff) PressOn();
        else PressRelease();
        
    }



    #region  Init Sync
    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(Main.ObjectData);
    }
    [ClientRpc]
    public void Rpc_InitSync(ObjectData data)
    {
        if (onSync) return;
        transform.position = data.position;
        transform.rotation = data.quaternion;
        onSync = true;

    }
    [Command(requiresAuthority = false)]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!onSync) Cmd_InitSync();
    }
    #endregion

    #region  Press
    [Header("Light")]
    [SerializeField] SpriteRenderer lightSpriteRenderer;
    [SerializeField] Sprite greenSprite;
    [SerializeField] Sprite redSprite;
    [SerializeField] Material _GreenLightMat;
    [SerializeField] Material _RedLightMat;
    
    [Header("Press")]
    [SerializeField] Transform rayPoint;
    [SerializeField] float rayDistance;
    [SerializeField] Transform pressTr;
    [SerializeField] BoxCollider2D pressCol;
    [SerializeField] float pressSpeed = 1.5f;
    private float minPressLength = -1.85f;
    private float maxPressLength = 2.5f;
    private Vector2 minColOffset = new Vector2(-0.2f,0);
    private Vector2 minColSize = new Vector2(0,0.6f);
    private Vector2 maxColOffset = new Vector2(-2.4f,0);
    private Vector2 maxColSize = new Vector2(4.4f,0.6f);
    private Coroutine pressOnCoroutine;
    private Coroutine pressReleaseCoroutine;

    private RaycastHit2D hit;
    [SerializeField] LayerMask layerMask;

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayPoint.position,pressTr.right*rayDistance);
    }
#endif
    private IEnumerator PressOnCo()
    {
        
        lightSpriteRenderer.sprite = greenSprite;
        lightSpriteRenderer.material = _GreenLightMat;
        
        float x = pressTr.localPosition.x;
        float percent = (x-minPressLength) / (maxPressLength-minPressLength);

        while(percent <1)
        {
            hit = Physics2D.Raycast(rayPoint.position, pressTr.right, rayDistance, layerMask);
            if (hit.collider == null)
            {
                x += Time.deltaTime * pressSpeed;
                pressTr.localPosition = new Vector3(x,0,0);

                percent = (x-minPressLength) / (maxPressLength-minPressLength);
                pressCol.offset = Vector2.Lerp(minColOffset,maxColOffset,percent);
                pressCol.size = Vector2.Lerp(minColSize,maxColSize,percent);
            }
            else
            {
                //Stop Press
                pressOnCoroutine = null;
                yield break;
                //Stop Press
            }
            yield return null;
        }
        pressTr.localPosition = new Vector3(maxPressLength,0,0);
        pressCol.offset= maxColOffset;
        pressCol.size = maxColSize;

        pressOnCoroutine = null;
    }

    private IEnumerator PressReleaseCo() 
    {
        float x = pressTr.localPosition.x;
        lightSpriteRenderer.sprite = redSprite;
        lightSpriteRenderer.material = _RedLightMat;
        float percent = (x-minPressLength) / (maxPressLength-minPressLength);

        while(x > minPressLength)
        {
            pressTr.localPosition = new Vector3(x,0,0);
            x -= Time.deltaTime * pressSpeed;
            
            percent = (x-minPressLength) / (maxPressLength-minPressLength);

            pressCol.offset = Vector2.Lerp(minColOffset,maxColOffset,percent);
            pressCol.size = Vector2.Lerp(minColSize,maxColSize,percent);

            yield return null;
        }
        pressTr.localPosition = new Vector3(minPressLength,0,0);
        pressCol.offset= minColOffset;
        pressCol.size = minColSize;

        pressReleaseCoroutine = null;

    }

    private void PressOn()
    {
        Main.SteamOn(); 

        if(pressReleaseCoroutine != null)
        {
            StopCoroutine(pressReleaseCoroutine);
            pressOnCoroutine = null;
        }
        pressOnCoroutine = StartCoroutine(PressOnCo());
    }
    private void PressRelease()
    {

        Main.SteamOn();
        if(pressOnCoroutine != null)
        {
            StopCoroutine(pressOnCoroutine);
            pressOnCoroutine = null;
        }
        pressReleaseCoroutine = StartCoroutine(PressReleaseCo());
    }

    #endregion
    
}
