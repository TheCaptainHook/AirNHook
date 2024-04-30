using System;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering;

public class InteractableObject : NetworkBehaviour, IInteractable, IInhalable
{
    // grab release
    [Header("Grab n Release")]
    private Rigidbody2D _rigidbody;
    private Transform _fixedPoint;
    private RigidbodyType2D _originType;
    private RigidbodyConstraints2D _originRot;
    [field: SerializeField] private ObjectTypeEnum _objectType = ObjectTypeEnum.Grab;
    private float _gravityScale;
    [SyncVar] private bool _isFixed;
    [SyncVar] private bool _canInteract = true;
    
    // e button ui
    [Header("E Button UI")]
    private UI_Base _eButtonUI;
    [field: SerializeField] private Vector2 _offset;
    private Vector3 _previous;
    
    // inhale
    [Header("Inhale")]
    [field: SerializeField] private float _inhalePower = 20f;
    
    // sorting layer
    private SortingGroup _sortingGroup;
    private int _originSortingLayerID;
    private const string GrabObj = "GrabObj";
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _sortingGroup = GetComponent<SortingGroup>();
    }

    private void Start()
    {
        _originType = _rigidbody.bodyType;
        _originRot = _rigidbody.constraints;
        _gravityScale = _rigidbody.gravityScale;
        
        _originSortingLayerID = _sortingGroup.sortingLayerID;
    }

    private void Update()
    {
        var velocity = ((transform.position - _previous).magnitude) / Time.deltaTime;
        _previous = transform.position;
        if(velocity <= 0.1f)
            _rigidbody.velocity = Vector2.zero;
        
        if (isOwned && _isFixed && _fixedPoint is not null)
        {
            _rigidbody.velocity = Vector2.zero;
            transform.position = _fixedPoint.position;
        }
        
        if (!_isFixed && _eButtonUI is not null)
        {
            _eButtonUI.transform.position = transform.position + (Vector3)_offset;
        }
    }

    private void FixedUpdate()
    {
        if(!isOwned || _fixedPoint is null) return;

        Inhale();
    }

    #region IInteractable
    public void Interaction(Transform accessor)
    {
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
    
    protected virtual void Grab()
    {
        _isFixed = true;
        _canInteract = false;
        ChangeState(true);
        HideEButton();
        
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        transform.rotation = Quaternion.identity;
        _sortingGroup.sortingLayerName = GrabObj;
    }

    public virtual void Release()
    {
        _isFixed = false;
        _canInteract = true;
        ChangeState(false);
        ShowEButton();

        _rigidbody.bodyType = _originType;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.gravityScale = _gravityScale;
        _fixedPoint = null;
        _rigidbody.constraints = _originRot;
        _sortingGroup.sortingLayerID = _originSortingLayerID;
    }
    
    public void Destroyed()
    {
        _isFixed = false;
        _canInteract = false;
        CmdChangeFixedState(false);
        CmdChangeInteractState(false);
        
        if(_eButtonUI is not null)
            HideEButton();

        _rigidbody.bodyType = _originType;
        
        if (_fixedPoint is not null && _fixedPoint.root.TryGetComponent<Hook>(out var hook))
            hook.ReleaseItem();

        _fixedPoint = null;
        _rigidbody.constraints = _originRot;
        Managers.Command.AuthorityToServer(netId);
    }
    
    public bool CanInteract()
    {
        return _canInteract;
    }

    public void Interacting(bool value)
    {
        _canInteract = !value;
    }

    public ObjectTypeEnum GetObjectType()
    {
        return _objectType;
    }

    public void ShowEButton()
    {
        if(!_canInteract) return;
        
        _eButtonUI = Managers.UI.ShowUI<UI_ShowEButton>();
        _eButtonUI.transform.position = transform.position + (Vector3)_offset;
    }
    
    public void HideEButton()
    {
        _eButtonUI = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }
    #endregion

    #region IInhalation
    public void Inhalation(Transform accessor)
    {
        _fixedPoint = accessor;
        transform.rotation = Quaternion.identity;
    }

    public void StopInhale()
    {
        _fixedPoint = null;
        _rigidbody.gravityScale = _gravityScale;
    }

    public void Fixed(bool value)
    {
        _isFixed = value;
        _canInteract = !value;
    }

    public void Inhaling(bool value)
    {
        _canInteract = !value;
    }

    public void Shooting(Vector2 force)
    {
        _fixedPoint = null;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.gravityScale = _gravityScale;
        _rigidbody.AddForce(force, ForceMode2D.Impulse);
    }

    public bool CanInhale()
    {
        return !_isFixed;
    }

    private void Inhale()
    {
        var direction = (_fixedPoint.position - transform.position).normalized;
        var power = _inhalePower * Time.fixedDeltaTime;
        _rigidbody.gravityScale = 0f;
        _rigidbody.AddForce(direction * power);
    }
    #endregion
    
    #region Command
    private void ChangeState(bool value)
    {
        CmdChangeFixedState(value);
        CmdChangeInteractState(!value);
    }
    
    [Command(requiresAuthority = false)]
    private void CmdChangeFixedState(bool value)
    {
        _isFixed = value;
    }

    [Command(requiresAuthority = false)]
    private void CmdChangeInteractState(bool value)
    {
        _canInteract = value;
    }
    #endregion
}
