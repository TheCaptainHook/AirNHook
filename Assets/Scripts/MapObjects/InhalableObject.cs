using Mirror;
using UnityEngine;

public class InhalableObject : NetworkBehaviour, IInhalable
{
    private Transform _accessor;
    private bool _isDestroyed;
    private GameObject _permissionPlayer;
    private object _lock = new object();
    protected Rigidbody2D _rigidbody;
    protected RigidbodyType2D _originType;
    protected RigidbodyConstraints2D _originRot;
    protected float _gravityScale;
    protected float _stoppedTime;
    protected GameObject _inhalingPlayer;

    protected Transform _fixedPoint;
    [SyncVar] protected bool _isFixed;

    [Header("Inhale")]
    [field: SerializeField] private float _inhalePower = 20f;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        _originType = _rigidbody.bodyType;
        _originRot = _rigidbody.constraints;
        _gravityScale = _rigidbody.gravityScale;
    }

    private void Update()
    {
        ClientAuthorityPass();
    }

    protected virtual void FixedUpdate()
    {
        if (_isFixed)
            Fixing();
    }

    private void ClientAuthorityPass()
    {
        if (!isOwned || isServer || _isFixed) return;

        if (_rigidbody.velocity.magnitude <= 0.5f && _rigidbody.angularVelocity <= 0.5f)
        {
            _stoppedTime += Time.deltaTime;

            if (_stoppedTime >= 3f)
            {
                AuthorityToServer();
                _stoppedTime = 0f;
            }
        }
        else
        {
            _stoppedTime = 0f;
        }
    }

    private void AuthorityToServer()
    {
        if (isServer) return;

        Managers.Command.AuthorityToServer(netId);
    }

    #region Inhalation
    public bool CanInhale()
    {
        return !_isFixed && !_isDestroyed;
    }

    public void Fixed(bool value)
    {
        if (_isDestroyed) return;

        _isFixed = value;
        CmdChangeFixedState(value);

        if (_isFixed)
        {
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
        }
    }

    public void Fixing()
    {
        if (_isDestroyed) return;

        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _rigidbody.freezeRotation = true;
    }

    public void Inhalation(Transform accessor)
    {
        _accessor = accessor;
    }

    public virtual bool Inhaling(bool value, GameObject player)
    {
        Debug.Log("1");
        if (value == false)
        {
            Debug.Log("2");
            CmdRemovePermissionPlayer();
            Debug.Log("3");
            return true;
        }

        if (AddPermissionPlayer(player))
        {
            Debug.Log("4");
            return true;
        }
        else
        {
            Debug.Log("5");
            return false;
        }
    }

    public virtual void Shooting(Vector2 force)
    {
        if (_isDestroyed) return;

        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _stoppedTime = 0f;
    }

    public virtual void StopInhale(GameObject accessor)
    {
        if (_permissionPlayer != null && !ReferenceEquals(_permissionPlayer, accessor)) return;

        if (_isDestroyed) return;

        Fixed(false);
        CmdRemovePermissionPlayer();
        _rigidbody.drag = 0f;
        _rigidbody.gravityScale = _gravityScale;
        _rigidbody.freezeRotation = false;
    }
    #endregion

    public Transform GetFixedPointRootTransform()
    {
        if (_accessor == null) return null;
        return _accessor.root;
    }

    private bool AddPermissionPlayer(GameObject player, bool isGrab = false)
    {
        lock (_lock)
        {
            if (_permissionPlayer == null)
            {
                _permissionPlayer = player;
                return true;
            }
            else if (ReferenceEquals(_permissionPlayer, player))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Command(requiresAuthority = false)]
    protected void CmdRemovePermissionPlayer()
    {
        lock (_lock)
        {
            _permissionPlayer = null;
        }

        if (_permissionPlayer == null)
            Invoke(nameof(CheckPermissionPlayer), 0.1f);
    }

    private void CheckPermissionPlayer()
    {
        if (_isFixed && _permissionPlayer == null)
        {
            _isFixed = false;
            CmdChangeFixedState(false);
        }
    }

    [Command(requiresAuthority = false)]
    protected void CmdChangeFixedState(bool value)
    {
        _isFixed = value;
    }
}
