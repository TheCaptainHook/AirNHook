using Mirror;
using UnityEngine;

public class InhalableObject : NetworkBehaviour, IInhalable
{
    protected Rigidbody2D _rigidbody;
    protected RigidbodyType2D _originType;
    protected RigidbodyConstraints2D _originRot;
    protected float _gravityScale;
    private float _stoppedTime;

    protected Transform _fixedPoint;
    [SyncVar] protected bool _isFixed;

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
        return true;
    }

    public void Fixed(bool value)
    {
        _stoppedTime = 0f;
        _rigidbody.drag = 0f;
        _isFixed = value;
    }

    public void Inhalation(Transform accessor)
    {
        _stoppedTime = 0f;
        _fixedPoint = accessor;
    }

    public bool Inhaling(bool value, GameObject player)
    {
        return true;
    }

    public virtual void Shooting(Vector2 force)
    {
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _rigidbody.Sleep();
        _stoppedTime = 0f;
    }

    public void StopInhale(GameObject accssor)
    {
        _stoppedTime = 0f;
        _fixedPoint = null;
        _rigidbody.drag = 0f;
        _rigidbody.gravityScale = _gravityScale;
        Fixed(false);
    }
    #endregion
}
