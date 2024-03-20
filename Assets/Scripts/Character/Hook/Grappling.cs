using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grappling : MonoBehaviour
{
    [Header("Player")]
    public Player playerMovement;
    public DistanceJoint2D distanceJoint;
    private Vector2 _playerPosition;
    public float climbSpeed = 3f;
    
    private InputActions _inputActions;
    private Vector2 _mousePosition;
    private float _vertical;
    private bool _grappleAttached;
    private bool _distanceSet;
    private bool _isActioning;

    [Header("Rope")]
    public Transform ropeStartPos;
    public LineRenderer ropeRenderer;
    private float _ropeMaxDistance = 5f;
    private List<Vector2> _ropePositions = new List<Vector2>();
    
    [Header("Hook")]
    public GameObject hookAnchor;
    public Transform hookStartPos;
    private Rigidbody2D _hookAnchorRb;
    public LayerMask hookLayerMask;
    
    private void Awake()
    {
        distanceJoint.enabled = false;
        _playerPosition = transform.position;
        _hookAnchorRb = hookAnchor.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _inputActions = GetComponent<Player>().input;
        
        _inputActions.playerActions.Look.performed += OnLook;
        _inputActions.playerActions.Look.canceled += OnLook;
        _inputActions.playerActions.VerticalMove.started += OnVerticalMove;
        _inputActions.playerActions.Action.started += OnMainAction;
        _inputActions.playerActions.Action.canceled += OnMainAction;
        _inputActions.playerActions.SubAction.started += OnSubAction;
    }

    private void Update()
    {
        HandleRopeLength();
    }

    private void FixedUpdate()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(_mousePosition.x, _mousePosition.y, 0f));
        var facingDirection = mousePos - transform.position;
        var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
            aimAngle = Mathf.PI * 2 + aimAngle;
        
        Vector2 aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
        _playerPosition = transform.position;
        
        if (!_grappleAttached)
        {
            playerMovement.isSwinging = false;
        }
        else
        {
            playerMovement.isSwinging = true;
            playerMovement.ropeHook = _ropePositions.Last();
        }

        HandleInput(aimDirection);
        UpdateRopePositions();
    }
    
    private void ResetRope()
    {
        distanceJoint.enabled = false;
        _grappleAttached = false;
        playerMovement.isSwinging = false;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, ropeStartPos.position);
        ropeRenderer.SetPosition(1, ropeStartPos.position);
        _ropePositions.Clear();
        hookAnchor.transform.position = hookStartPos.position;
    }
    
    private void UpdateRopePositions()
    {
        if (!_grappleAttached)
            return;

        ropeRenderer.positionCount = _ropePositions.Count + 1;

        for (var i = ropeRenderer.positionCount - 1; i >= 0; i--)
        {
            if (i != ropeRenderer.positionCount - 1) // if not the Last point of line renderer
            {
                ropeRenderer.SetPosition(i, _ropePositions[i]);
                
                if (i == _ropePositions.Count - 1 || _ropePositions.Count == 1)
                {
                    var ropePosition = _ropePositions[_ropePositions.Count - 1];
                    if (_ropePositions.Count == 1)
                    {
                        _hookAnchorRb.transform.position = ropePosition;
                        if (!_distanceSet)
                        {
                            distanceJoint.distance = Vector2.Distance(transform.position, ropePosition);
                            _distanceSet = true;
                        }
                    }
                    else
                    {
                        _hookAnchorRb.transform.position = ropePosition;
                        if (!_distanceSet)
                        {
                            distanceJoint.distance = Vector2.Distance(transform.position, ropePosition);
                            _distanceSet = true;
                        }
                    }
                }
                else if (i - 1 == _ropePositions.IndexOf(_ropePositions.Last()))
                {
                    var ropePosition = _ropePositions.Last();
                    _hookAnchorRb.transform.position = ropePosition;
                    if (!_distanceSet)
                    {
                        distanceJoint.distance = Vector2.Distance(transform.position, ropePosition);
                        _distanceSet = true;
                    }
                }
            }
            else
            {
                ropeRenderer.SetPosition(i, ropeStartPos.position);
            }
        }
    }
    
    private void HandleRopeLength()
    {
        if (_grappleAttached)
        {
            if (Vector2.Distance(transform.position, hookAnchor.transform.position) <= 0.2f)
            {
                ResetRope();
            }
            else if (transform.position.y > hookAnchor.transform.position.y)
            {
                distanceJoint.distance -= Time.deltaTime * 14f;
            }
            
            if (_vertical > 0f)
            {
                distanceJoint.distance -= Time.deltaTime * climbSpeed;
            }
            else if (_vertical < 0f && distanceJoint.distance < _ropeMaxDistance)
            {
                var distance = distanceJoint.distance + Time.deltaTime * climbSpeed;
                distanceJoint.distance = Mathf.Min(distance, _ropeMaxDistance);
            }
        }
    }
    
    private void HandleInput(Vector2 aimDirection)
    {
        if (_isActioning)
        {
            if (_grappleAttached) return;
            ropeRenderer.enabled = true;

            var hit = Physics2D.Raycast(_playerPosition, aimDirection, _ropeMaxDistance, hookLayerMask);
        
            if (hit.collider != null)
            {
                _grappleAttached = true;
                if (!_ropePositions.Contains(hit.point))
                {
                    transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 2f), ForceMode2D.Impulse);
                    _ropePositions.Add(hit.point);
                    distanceJoint.distance = Vector2.Distance(_playerPosition, hit.point);
                    distanceJoint.enabled = true;
                }
            }
            else
            {
                ropeRenderer.enabled = false;
                _grappleAttached = false;
                distanceJoint.enabled = false;
            }
        }
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        _mousePosition = context.ReadValue<Vector2>();
    }
    
    public void OnVerticalMove(InputAction.CallbackContext context)
    {
        _vertical = context.ReadValue<Vector2>().y;
    }
    
    public void OnMainAction(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
            _isActioning = true;
        else if (context.phase == InputActionPhase.Canceled)
            _isActioning = false;
    }
    
    public void OnSubAction(InputAction.CallbackContext context)
    {
        ResetRope();
    }
}
