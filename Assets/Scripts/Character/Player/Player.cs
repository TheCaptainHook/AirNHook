using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.RigidbodyConstraints2D;

public class Player : NetworkBehaviour, IDamageable
{
    //점프 버퍼
    [SerializeField] private float _jumpBufferTime = 0.2f;
    private float _jumpBufferCount;

    //코요테시간
    [SerializeField] private float _coyoteTime = 0.2f;
    private float _coyoteTimeCount;

    //플레이어 이동 및 점프
    [SerializeField] private Rigidbody2D _rigidbd;
    [SerializeField] private Transform _floorCheck;
    //땅 체크
    [SerializeField] private LayerMask _floorLayer;
    public PlayerInput playerInput { get; private set; }
    
    //플레이어 점프 체크
    private bool _isJumping;

    //점프타임
    private float _jumpingTime;

    private float _horizontal;
    private float _speed = 4f;
    private float _jumpingPower = 10f;
    
    private Animator _animator;
    private Collider2D _collider2D;
    
    //사망 체크
    [SerializeField] private bool _isDead = false;
    
    [SerializeField] private Transform _charPivot;

    #region StringCache
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsDead = Animator.StringToHash("IsDead");
    private static readonly int IsRespawning = Animator.StringToHash("IsRespawning");
    private static readonly int OnRespawnEnd = Animator.StringToHash("OnRespawnEnd");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsStayJumping = Animator.StringToHash("IsStayJumping");

    #endregion
    
    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _rigidbd = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 땅 체크
        if (IsFloor())
        {
            _coyoteTimeCount = _coyoteTime;
            _jumpingTime = 0.025f;
        }
        else
        {
            _coyoteTimeCount -= Time.deltaTime;
            _jumpingTime -= Time.deltaTime;
        }

        //점프를 눌렀을 때
        if (_isJumping)
        {
            _jumpBufferCount = _jumpBufferTime;
        }
        else
        {
            _jumpBufferCount -= Time.deltaTime;
        }
        
        MoveAnimation();
    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        playerInput.playerActions.Move.started += Move;
        playerInput.playerActions.Jump.started += JumpStarted;
        playerInput.playerActions.Jump.canceled += JumpCanceled;
    }

    private void FixedUpdate()
    {
        if (!_isDead)
        {
            _rigidbd.velocity = new Vector2(_horizontal * _speed, _rigidbd.velocity.y);
            IsLeftHead();
            IsRightHead();
            
            //점프
            if (_coyoteTimeCount > 0f && _jumpBufferCount > 0f && _jumpingTime > 0f)
            {
                _rigidbd.velocity = new Vector2(_rigidbd.velocity.x, _jumpingPower);
                _jumpBufferCount = 0f;
            }
        }
        else
        {
            _rigidbd.velocity = Vector2.zero;
        }
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
    }

    private void MoveAnimation()
    {
        //이동에 따라 애니메이션 제어
        _animator.SetBool(IsMoving, _horizontal != 0 && IsFloor());
        _animator.SetBool(IsJumping, _horizontal != 0 && _isJumping || _horizontal != 0 && !IsFloor());
        _animator.SetBool(IsStayJumping, _horizontal == 0 && _isJumping || _horizontal == 0 && !IsFloor());
        // CharPivot 오브젝트의 로테이션을 사용하여 플립
        if (_horizontal < 0)
        {
            _charPivot.rotation = Quaternion.Euler(0f, 180f, 0f); // 왼쪽으로 이동할 때 캐릭터를 뒤집음
        }
        else if (_horizontal > 0)
        {
            _charPivot.rotation = Quaternion.Euler(0f, 0f, 0f); // 오른쪽으로 이동할 때 캐릭터를 되돌림
        }
    }

    public void JumpStarted(InputAction.CallbackContext context)
    {
        _jumpingTime = 0.025f;
        _isJumping = true;
    }

    public void JumpCanceled(InputAction.CallbackContext context)
    {
        if (_rigidbd.velocity.y >= 0f)
        {
            _rigidbd.velocity = new Vector2(_rigidbd.velocity.x, _rigidbd.velocity.y * 0.6f);
            _coyoteTimeCount = 0f;
        }
        _isJumping = false;
    }

    //점프체크
    private bool IsFloor()
    {
        //Ray발사
        for (int i = -1; i < 2; i++)
        {
            if (Physics2D.Raycast(transform.position + (Vector3.right * 0.5f * i), Vector2.down, 0.1f, _floorLayer))
            {
                return true;
            }
        }
        return false;
    }

    //오른쪽 머리 충돌체크
    private bool IsRightHead()
    {
        //레이 발사 / 정상 작동
        for (int i = -2; i < 3; i++)
        {
            if(Physics2D.Raycast(transform.position + (Vector3.right * 0.175f * i), Vector2.up, 1.5f, _floorLayer))
            {
                if(i == 2)
                {
                    transform.position = new Vector3(transform.position.x - 0.26f, transform.position.y);
                }
                return true;
            }
        }
        return false;
    }

    //왼쪽 머리 충돌체크
    private bool IsLeftHead()
    {
        for (int j = -2; j < 3; j++)
        {
            if (Physics2D.Raycast(transform.position + (Vector3.right * -0.175f * j), Vector2.up, 1.5f, _floorLayer))
            {
                if (j == 2)
                {
                    transform.position = new Vector3(transform.position.x + 0.26f, transform.position.y);
                }
                return true;
            }
        }
        return false;
    }
    
    // 사망 메서드
    public void TakeDamage()
    {
        if (!_isDead)
        {
            Debug.Log("사망하였습니다.");
            // 여기에 필요한 사망 처리
            _animator.SetTrigger(IsDead);
            _isDead = true;
            _rigidbd.constraints = FreezeAll;
            _collider2D.enabled = false;
        }
    }

    private void Respawning()
    {
        Debug.Log("리스포닝");
        
        transform.position = Managers.Network.startPos[0].position;
        _animator.SetTrigger(IsRespawning);
    }

    private void RespawnEnd()
    {
        Debug.Log("리스폰끝");
        _animator.SetTrigger(OnRespawnEnd);
        _isDead = false;
        _collider2D.enabled = true;
        _rigidbd.constraints = None;
        _rigidbd.freezeRotation = true;
    }

    [Command]
    public void CmdEmote(string emoteName)
    {
        var prefab = Managers.Network.spawnPrefabDict[emoteName];
        var go = Instantiate(prefab, gameObject.transform.position, Quaternion.identity);
        NetworkServer.Spawn(go);
        go.name = prefab.name;
        RpcEmote(go);
    }
    
    [ClientRpc]
    public void RpcEmote(GameObject go)
    {
        var sort = go.GetComponent<SortingGroup>();
        sort.sortingOrder = isLocalPlayer ? 8 : 7;
        go.transform.parent = gameObject.transform;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for(int i = -2; i < 3; i++)
        {
            Gizmos.DrawRay(transform.position + (Vector3.right * -0.175f * i), Vector2.up * 1.5f);
        }
    }
}
