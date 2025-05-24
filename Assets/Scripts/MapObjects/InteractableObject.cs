using System;
using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class InteractableObject : NetworkBehaviour, IInteractable, IInhalable
{
    // grab release
    [Header("Grab n Release")]
    private Transform _accessor;
    protected Rigidbody2D _rigidbody;
    protected Collider2D _collider;
    protected Transform _fixedPoint;
    protected RigidbodyType2D _originType;
    protected RigidbodyConstraints2D _originRot;
    [field: SerializeField] protected ObjectTypeEnum _objectType = ObjectTypeEnum.Grab;
    [SerializeField] protected float _gravityScale;
    [SerializeField][SyncVar] protected bool _isFixed;
    [SerializeField][SyncVar] protected bool _canInteract = true;
    [SerializeField][SyncVar] protected bool _canGrab = true;
    [SerializeField][SyncVar] protected bool _isDestroyed;
    protected bool _isGrab;
    protected float _stoppedTime;

    // e button ui
    [Header("E Button UI")]
    private UI_Base _eButtonUI;
    [field: SerializeField] private Vector2 _offset;
    private Vector3 _previous;

    // inhale
    [Header("Inhale")]
    [field: SerializeField] private float _inhalePower = 20f;

    // sorting layer
    protected SortingGroup _sortingGroup;
    protected int _originSortingLayerID;
    private const string GrabObj = "GrabObj";

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _sortingGroup = GetComponent<SortingGroup>();
    }

    private void Start()
    {
        _originType = _rigidbody.bodyType;
        _originRot = _rigidbody.constraints;
        _gravityScale = _rigidbody.gravityScale;
        _originSortingLayerID = _sortingGroup.sortingLayerID;
    }

    protected void Update()
    {
        ClientAuthorityPass();

        if (!_isFixed && _eButtonUI is not null)
        {
            _eButtonUI.transform.position = transform.position + (Vector3)_offset;
        }
    }

    private void FixedUpdate()
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

    #region IInteractable
    public void Interaction(Transform accessor)
    {
        _accessor = accessor;

        if (_isFixed)
            Release();
        else
            Grab();
    }

    protected virtual void Grab()
    {
        _stoppedTime = 0f;
        _isFixed = true;
        _isGrab = true;
        _canInteract = false;
        _canGrab = false;
        CmdChangeFixedState(true);
        CmdChangeInteractState(false);
        CmdChangeGrabState(false);
        HideEButton();

        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        transform.rotation = Quaternion.identity;
        _sortingGroup.sortingLayerName = GrabObj;
        CmdChangeSortingLayer(true);
    }

    public virtual void Release()
    {
        _stoppedTime = 0f;
        _isFixed = false;
        _isGrab = false;
        _canInteract = true;
        _canGrab = true;
        CmdChangeFixedState(false);
        CmdChangeInteractState(true);
        CmdChangeGrabState(true);
        ShowEButton();

        _rigidbody.bodyType = _originType;
        _rigidbody.gravityScale = _gravityScale;
        _rigidbody.constraints = _originRot;
        _sortingGroup.sortingLayerID = _originSortingLayerID;
        CmdChangeSortingLayer(false);
    }

    public void Destroyed()
    {
        _stoppedTime = 0f;
        _canInteract = false;
        _canGrab = false;
        CmdChangeFixedState(false);
        CmdChangeInteractState(false);
        CmdChangeGrabState(false);

        if (_eButtonUI is not null)
            HideEButton();

        _rigidbody.bodyType = _originType;

        if (_isGrab)
        {
            _isGrab = false;
            _isFixed = false;
            if (Managers.Game.Player.TryGetComponent<HookSM>(out var hook))
                hook.ReleaseItem();
            else if (Managers.Game.Player.TryGetComponent<AirSM>(out var air))
                air.StopGun();
        }

        _rigidbody.constraints = _originRot;
        Managers.Command.AuthorityToServer(netId);
    }

    public void Respawned()
    {
        _canInteract = true;
        _canGrab = true;
        _isFixed = false;
        CmdChangeFixedState(false);
        CmdChangeInteractState(true);
        CmdChangeGrabState(true);
        //Release();
        //StopInhale();
    }

    public bool CanInteract()
    {
        return _canInteract && _canGrab && !_isDestroyed;
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
        if (!_canInteract) return;

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
    public void Inhalation(Transform accesor)
    {
        _accessor = accesor;
        //_canInteract = false;
        //_canGrab = false;
        //CmdChangeInteractState(false);
        //CmdChangeGrabState(false);
    }

    public void StopInhale()
    {
        if (_isDestroyed) return;

        Fixed(false);
        _rigidbody.drag = 0f;
        _rigidbody.gravityScale = _gravityScale;
        _rigidbody.freezeRotation = false;
    }

    public void Fixed(bool value)
    {
        if (_isDestroyed) return;

        _isFixed = value;
        _isGrab = value;
        _canInteract = !value;
        _canGrab = !value;
        CmdChangeFixedState(value);
        CmdChangeInteractState(!value);
        CmdChangeGrabState(!value);

        if (_isFixed)
        {
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
        }
    }

    public void Fixing()
    {
        if (_isDestroyed) return;

        //_canInteract = false;
        //CmdChangeInteractState(false);
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _rigidbody.freezeRotation = true;
        _rigidbody.Sleep();
    }

    public void Inhaling(bool value)
    {
        //_canInteract = !value;
        _canGrab = !value;
        CmdChangeGrabState(!value);
        //CmdChangeInteractState(!value);
    }

    public void Shooting(Vector2 force)
    {
        if (_isDestroyed) return;

        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _rigidbody.Sleep();
        _stoppedTime = 0f;
        //_canInteract = true;
        //_canGrab = true;
        //CmdChangeInteractState(true);
        //CmdChangeGrabState(true);
    }

    public bool CanInhale()
    {
        return !_isFixed && !_isDestroyed && _canInteract;
    }
    #endregion

    public Transform GetFixedPointRootTransform()
    {
        if (_accessor == null) return null;
        return _accessor.root;
    }

    #region Command
    protected void ChangeState(bool value)
    {
        CmdChangeFixedState(value);
        CmdChangeInteractState(!value);
    }

    [Command(requiresAuthority = false)]
    protected void CmdChangeFixedState(bool value)
    {
        _isFixed = value;
    }

    [Command(requiresAuthority = false)]
    private void CmdChnageDestroyState(bool value)
    {
        _isDestroyed = value;
    }

    [Command(requiresAuthority = false)]
    protected void CmdChangeInteractState(bool value)
    {
        _canInteract = value;
    }

    [Command(requiresAuthority = false)]
    protected void CmdChangeGrabState(bool value)
    {
        _canGrab = value;
    }

    [Command(requiresAuthority = false)]
    protected void CmdChangeSortingLayer(bool isGrab)
    {
        RpcChangeSortingLayer(isGrab);
    }

    [ClientRpc]
    private void RpcChangeSortingLayer(bool isGrab)
    {
        if (isGrab)
            _sortingGroup.sortingLayerName = GrabObj;
        else
            _sortingGroup.sortingLayerID = _originSortingLayerID;
    }
    #endregion

    #region Dissolve
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    float dissolveRate = 0.015f;
    [Command(requiresAuthority = false)]
    public void Cmd_Dissolve()
    {
        CmdChnageDestroyState(true);
        Managers.Command.AuthorityToServer(netId);
        Rpc_Dissolve();
    }
    [ClientRpc]
    private void Rpc_Dissolve()
    {
       
        var root = GetFixedPointRootTransform();

        if (root != null)
        {
            if (root.TryGetComponent(out HookSM hook)) hook.ReleaseItem();
            else if (root.TryGetComponent(out AirSM air)) air.StopGun();
        }

    
        var buildObj = GetComponent<BuildObj>();
        StartCoroutine(Co_Dissolve(buildObj.position));
    }

    IEnumerator Co_Dissolve(Vector2 pot)
    {
        var buildObj = GetComponent<BuildObj>();
        if (buildObj == null) yield break;

        buildObj.canRespawn = false;

        float percent = 1;
        _rigidbody.gravityScale = 0;
        _collider.enabled = false;
        _rigidbody.velocity = Vector2.zero;
        while (percent > 0)
        {
            percent -= dissolveRate;
            buildObj.DissolveMaterial.SetFloat(DissolveAmount, percent);
            yield return null;
        }

        if (NetworkServer.active)
        {
            if (buildObj.isTransportItem)
            {
                if (buildObj.carrierTransform != null)
                {
                    buildObj.Connection_TransportItem();
                    Debug.Log($"InteractableObject, Co_Dissolve, name :{gameObject.name}");
                }
                   
            }
            else
            {
                _rigidbody.position = pot;

            }
        }

        while (percent < 1)
        {
            percent += dissolveRate;
            buildObj.DissolveMaterial.SetFloat(DissolveAmount, percent);
            yield return null;
        }

        if (!buildObj.isTransportItem)
        {
            _collider.enabled = true;
            _rigidbody.gravityScale = _gravityScale;  
        }

        GetComponent<InteractableObject>().Respawned();
        buildObj.canRespawn = true;
        CmdChnageDestroyState(false);
    }
    #endregion

}
