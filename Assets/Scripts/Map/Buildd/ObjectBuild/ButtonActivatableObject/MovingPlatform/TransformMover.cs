
using Mirror;
using UnityEngine;



public class TransformMover : NetworkBehaviour
{
    //Refs
    private Collider2D col;
    private Rigidbody2D rb;
    private NetworkRigidbodyUnreliable2D netRb;
    //Platform Detect
    [SerializeField] LayerMask movingPlatformMask = 1 << 15;
    [SerializeField] float skin = 0.2f;
    private RaycastHit2D[] _hits = new RaycastHit2D[2];
    private ContactFilter2D _filter;

    [SyncVar] public uint _platformId; // 입/퇴장 시점 공유용(옵션)
   
    private void Awake()
    {
        col =GetComponent<Collider2D>();    
        rb= GetComponent<Rigidbody2D>();
        netRb = GetComponent<NetworkRigidbodyUnreliable2D>();

        _filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = movingPlatformMask,
            useTriggers = true
        };
    }

    //-----0813
    void FixedUpdate()
    {
        if (!isOwned) return;

        float dist = col.bounds.extents.y + skin;
        int count = Physics2D.Raycast(col.bounds.center, Vector2.down, _filter, _hits, dist);

        if (count > 0)
        {
            if (_hits[0].collider.TryGetComponent(out MovingPlatform mp))
            {
                rb.position += mp.dir;
                return;
            }
            if(_hits[0].collider.TryGetComponent(out WDMP_Net wdmp))
            {
                rb.position +=  wdmp._c_dir * wdmp._c_curStep;
                return;
            }

        }

    }


}
