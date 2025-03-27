using System;
using System.Collections;
using System.ComponentModel;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering;

public class InteractableObject : NetworkBehaviour, IInteractable, IInhalable
{
    // grab release
    [Header("Grab n Release")]
    protected Rigidbody2D _rigidbody;
    protected Collider2D _collider;
    protected Transform _fixedPoint;
    protected RigidbodyType2D _originType;
    protected RigidbodyConstraints2D _originRot;
    [field: SerializeField] protected ObjectTypeEnum _objectType = ObjectTypeEnum.Grab;
    [SerializeField] protected float _gravityScale;
    [SyncVar] protected bool _isFixed;
    [SyncVar] protected bool _canInteract = true;
    protected bool _isGrab;

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
        //_gravityScale = 1;

        _originSortingLayerID = _sortingGroup.sortingLayerID;
    }

    protected void Update()
    {
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
        if (!isOwned || _fixedPoint is null || _isGrab) return;

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
        _isGrab = true;
        _canInteract = false;
        ChangeState(true);
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
        _isFixed = false;
        _isGrab = false;
        _canInteract = true;
        ChangeState(false);
        ShowEButton();

        _rigidbody.bodyType = _originType;
        //_rigidbody.velocity = Vector2.zero;
        _rigidbody.gravityScale = _gravityScale;
        _fixedPoint = null;
        _rigidbody.constraints = _originRot;
        _sortingGroup.sortingLayerID = _originSortingLayerID;
        CmdChangeSortingLayer(false);
    }

    public void Destroyed()
    {
        _isFixed = false;
        _isGrab = false;
        _canInteract = false;
        CmdChangeFixedState(false);
        CmdChangeInteractState(false);

        if (_eButtonUI is not null)
            HideEButton();

        _rigidbody.bodyType = _originType;

        if (_fixedPoint is not null && _fixedPoint.root.TryGetComponent<HookSM>(out var hook))
            hook.ReleaseItem();

        _fixedPoint = null;
        _rigidbody.constraints = _originRot;
        Managers.Command.AuthorityToServer(netId);
    }

    public void Respawned()
    {
        _canInteract = true;
        CmdChangeInteractState(true);
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
    public void Inhalation(Transform accessor)
    {
        _fixedPoint = accessor;
        transform.rotation = Quaternion.identity;
    }

    public void StopInhale()
    {
        _fixedPoint = null;
        _rigidbody.drag = 0f;
        _rigidbody.gravityScale = _gravityScale;
    }

    public void Fixed(bool value)
    {
        _rigidbody.drag = 0f;
        _isFixed = value;
        _isGrab = false;
        _canInteract = !value;
    }

    public void Inhaling(bool value)
    {
        _canInteract = !value;
    }

    public void Shooting(Vector2 force)
    {
        _fixedPoint = null;
        _rigidbody.drag = 0f;
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
        _rigidbody.drag = 10f;
        _rigidbody.gravityScale = 0f;
        _rigidbody.AddForce(direction * power);
    }
    #endregion

    public Transform GetFixedPointRootTransform()
    {
        if (_fixedPoint == null) return null;
        return _fixedPoint.root;
    }

    #region Command
    protected void ChangeState(bool value)
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
        Rpc_Dissolve();
    }
    [ClientRpc]
    private void Rpc_Dissolve()
    {
        var root = GetFixedPointRootTransform();
        if (root != null) if (root.TryGetComponent(out HookSM hook)) hook.ReleaseItem();
        if (!CanInteract()) Release();

        var buildObj = GetComponent<BuildObj>();
        StartCoroutine(Co_Dissolve(buildObj.position));
    }

    IEnumerator Co_Dissolve(Vector2 pot)
    {
        var buildObj = GetComponent<BuildObj>();
        if (buildObj == null) yield break;

        buildObj.canRespawn = false;

        float percent = 1;
        _collider.enabled = false;
        _rigidbody.simulated = false;
        _rigidbody.gravityScale = 0;
        _rigidbody.velocity = Vector2.zero;
        while (percent > 0)
        {
            percent -= dissolveRate;
            buildObj.DissolveMaterial.SetFloat(DissolveAmount, percent);
            yield return null;
        }

        if(NetworkServer.active)
        {
            if (buildObj.isTransportItem)
            {
                if (buildObj.carrierTransform != null)
                    // SettingTransportItem(carrierTransform);
                    buildObj.Connection_TransportItem();
                // SettingTransportItem(carrierTransform);
            }
        }
        if(!buildObj.isTransportItem) transform.position = pot;


        while (percent < 1)
        {
            percent += dissolveRate;
            buildObj.DissolveMaterial.SetFloat(DissolveAmount, percent);
            yield return null;
        }

        if (!buildObj.isTransportItem)
        {
            _collider.enabled = true;
            _rigidbody.gravityScale = 1;  
        }
        _rigidbody.simulated = true;

        GetComponent<InteractableObject>().Respawned();

        //CustomEditor
        // if (MapEditor.Instance.mapEditorState == MapEditorState.Object)
        // {
        //     TurnOff();
        // }

        buildObj.canRespawn = true;

    }

    #endregion

}
