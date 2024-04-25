using System.Collections;
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
    private float _gravityScale;
    [SyncVar] private bool _isFixed;
    [SyncVar] private bool _canInhale;
    [SyncVar] private bool _isDestroyed;
    [field: SerializeField] private float _inhalePower = 20f;

    private UI_Base _eButtonUI;
    public Vector2 offset;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _originType = _rigidbody2D.bodyType;
        _originRot = _rigidbody2D.constraints;
        _releaseLayerMask = _rigidbody2D.excludeLayers;
        _fixedPoint = null;
    }

    private void Start()
    {
        _gravityScale = _rigidbody2D.gravityScale;
    }

    private void Update()
    {
        if (!_isFixed && _eButtonUI is not null)
        {
            _eButtonUI.transform.position = transform.position + (Vector3)offset;
        }
        
        if(!isOwned) return;
        
        if (_isFixed && _fixedPoint is not null)
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

    public bool CanInteract()
    {
        return !(_isDestroyed || _isFixed);
    }

    public void Fixed(bool value)
    {
        _isFixed = value;
        ChangeFixedState(value);
    }

    public ObjectTypeEnum GetObjectType()
    {
        return _objectType;
    }

    protected virtual void Grab()
    {
        ChangeFixedState(true);
        ChangeCanInhaleState(false);
        HideEButton();
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.velocity = new Vector2(0, 0);
        transform.rotation = Quaternion.identity;
        CmdSetExcludeLayer(grabLayerMask);
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public virtual void Release()
    {
        ChangeFixedState(false);
        ChangeCanInhaleState(false);
        ShowEButton();
        _rigidbody2D.bodyType = _originType;
        _fixedPoint = null;
        CmdSetExcludeLayer(_releaseLayerMask);
        _rigidbody2D.constraints = _originRot;
    }

    public void Destroyed()
    {
        _isDestroyed = true;
        _isFixed = false;
        _canInhale = true;
        ChangeDestroyedState(true);
        ChangeFixedState(false);
        ChangeCanInhaleState(true);
        _rigidbody2D.bodyType = _originType;
        if (_fixedPoint is not null && _fixedPoint.root.TryGetComponent<Hook>(out var hook))
        {
            hook.ReleaseItem();
        }

        _fixedPoint = null;
        CmdSetExcludeLayer(_releaseLayerMask);
        _rigidbody2D.constraints = _originRot;
    }

    public void Inhalation(Transform accessor)
    {
        if (_fixedPoint is not null || _canInhale || _isFixed || _isDestroyed)
            return;

        _fixedPoint = accessor;
        
        transform.rotation = Quaternion.identity;
        ChangeCanInhaleState(true);
    }

    public void StopInhale()
    {
        ChangeCanInhaleState(false);
        ChangeFixedState(false);
        _fixedPoint = null;
        _rigidbody2D.gravityScale = _gravityScale;
        CmdSetExcludeLayer(_releaseLayerMask);
    }

    public void Shooting(Vector2 force)
    {
        ChangeCanInhaleState(false);
        ChangeFixedState(false);
        _fixedPoint = null;
        _rigidbody2D.gravityScale = _gravityScale;
        _rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        CmdSetExcludeLayer(_releaseLayerMask);
    }

    public bool CanInhale()
    {
        return !(_isDestroyed || _isFixed);
    }

    private void Inhale()
    {
        var direction = (_fixedPoint.position - transform.position).normalized;
        var power = _inhalePower * Time.fixedDeltaTime;
        _rigidbody2D.gravityScale = 0f;
        _rigidbody2D.AddForce(direction * power);
        CmdSetExcludeLayer(grabLayerMask);
            
        if (Vector2.Distance(_fixedPoint.position, transform.position) > 0.2f) return;

        ChangeCanInhaleState(false);
        ChangeFixedState(true);
    }

    [Command(requiresAuthority = false)]
    private void ChangeDestroyedState(bool value)
    {
        _isDestroyed = value;
        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(2f);
        ChangeDestroyedState(false);
    }

    [Command(requiresAuthority = false)]
    private void ChangeFixedState(bool value)
    {
        _isFixed = value;
    }
    
    [Command(requiresAuthority = false)]
    private void ChangeCanInhaleState(bool value)
    {
        _canInhale = value;
    }
    
    [Command(requiresAuthority = false)]
    private void CmdSetExcludeLayer(LayerMask layerMask)
    {
        RpcSetExcludeLayer(layerMask);
    }

    [ClientRpc]
    private void RpcSetExcludeLayer(LayerMask layerMask)
    {
        _rigidbody2D.excludeLayers = layerMask;
    }

    public void ShowEButton()
    {
        if(_isFixed || _isDestroyed) HideEButton();
        
        _eButtonUI = Managers.UI.ShowUI<UI_ShowEButton>();
        _eButtonUI.transform.SetParent(null);
        _eButtonUI.transform.position = transform.position + (Vector3)offset;
    }
    
    public void HideEButton()
    {
        _eButtonUI = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }
}
