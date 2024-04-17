using Mirror;
using UnityEngine;

public class InteractableObject : NetworkBehaviour, IInteractable, IInhalable
{
    private Transform _fixedPoint;
    private Rigidbody2D _rigidbody2D;
    [field: SerializeField] private ObjectTypeEnum _objectType = ObjectTypeEnum.Grab;
    public LayerMask grabLayerMask;
    private LayerMask _releaseLayerMask;
    private RigidbodyType2D _originType;
    private RigidbodyConstraints2D _originRot;
    [SyncVar] private bool _isFixed;
    [SyncVar] private bool _canInhale;
    [field: SerializeField] private float _inhalePower = 20f;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _originType = _rigidbody2D.bodyType;
        _originRot = _rigidbody2D.constraints;
        _releaseLayerMask = _rigidbody2D.excludeLayers;
        _fixedPoint = null;
    }

    private void OnDisable()
    {
        if(NetworkServer.active)
            CmdRemoveAuthority();
    }

    [Command(requiresAuthority = false)]
    private void CmdRemoveAuthority()
    {
        netIdentity.RemoveClientAuthority();
    }

    private void Update()
    {
        if(!isOwned || _fixedPoint is null) return;
        
        if (_isFixed)
        {
            _rigidbody2D.velocity = Vector2.zero;
            transform.position = _fixedPoint.position;
        }
    }

    private void FixedUpdate()
    {
        if(!isOwned || _fixedPoint is null) return;
        
        if (_canInhale)
        {
            Inhale();
        }
    }

    public void Interaction(Transform accessor)
    {
        if (_fixedPoint is not null && !ReferenceEquals(_fixedPoint, accessor))
            return;

        if (_isFixed)
        {
            Release();
        }
        else
        {
            _fixedPoint = accessor;
            Grab();
        }
    }

    public ObjectTypeEnum GetObjectType()
    {
        return _objectType;
    }

    private void Grab()
    {
        _isFixed = true;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.velocity = new Vector2(0, 0);
        transform.rotation = Quaternion.identity;
        _rigidbody2D.excludeLayers = grabLayerMask;
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Release()
    {
        _isFixed = false;
        _canInhale = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _fixedPoint = null;
        _rigidbody2D.excludeLayers = _releaseLayerMask;
        _rigidbody2D.constraints = _originRot;
    }

    public void Inhalation(Transform accessor)
    {
        if (_fixedPoint is not null || _canInhale || _isFixed)
            return;

        _fixedPoint = accessor;
        
        transform.rotation = Quaternion.identity;
        _canInhale = true;
    }

    public void StopInhale()
    {
        _canInhale = false;
        _isFixed = false;
        _fixedPoint = null;
        _rigidbody2D.gravityScale = 1f;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
    }

    public void Shooting(Vector2 force)
    {
        _canInhale = false;
        _isFixed = false;
        _fixedPoint = null;
        _rigidbody2D.gravityScale = 1f;
        _rigidbody2D.AddForce(force, ForceMode2D.Impulse);
    }

    public bool CanInhale()
    {
        return !_isFixed;
    }

    private void Inhale()
    {
        var direction = (_fixedPoint.position - transform.position).normalized;
        var power = _inhalePower * Time.fixedDeltaTime;
        _rigidbody2D.gravityScale = 0f;
        _rigidbody2D.AddForce(direction * power);
        
        if (Vector2.Distance(_fixedPoint.position, transform.position) > 0.15f) return;
        
        _canInhale = false;
        _isFixed = true;
    }
}
