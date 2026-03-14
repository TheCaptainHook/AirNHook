using System;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine;
using UnityEngine.Rendering;

public class LockerAnim : NetworkBehaviour, IInteractable
{
    [SerializeField] private GameObject _disappearingObj;
    [SerializeField] private CharacterType _characterType;
    [SerializeField] private SpriteRenderer _doorSpriteRenderer;
    private GameObject _player;
    private ObjectTypeEnum _objectType = ObjectTypeEnum.Interaction;
    private Animator _animator;
    [SyncVar] private bool _isRestock = true;
    public Vector2 offset;

    private readonly object _lock = new object(); // 동시 접근 방지용

    #region StringCache
    private static readonly int Changing = Animator.StringToHash("Changing");
    #endregion
    
    public bool IsChanging = false;
    
    public event Action OnChangingAnimation;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        OnChangingAnimation += SetTriggerChanging;
    }
    private void SetTriggerChanging()
    {
        IsChanging = true;
        _animator.SetTrigger(Changing);
    }
    public void CallOnChangingAnimation()
    {
        OnChangingAnimation?.Invoke();
    }

    public void DestroyGO()
    {
        Destroy(_disappearingObj);
    }

    public void Interaction(Transform accessor)
    {
        CmdTryChangeCharacter(Managers.Game.Player);
    }

    public bool CanInteract()
    {
        return _isRestock;
    }

    public bool Interacting(bool value, GameObject player)
    {
        return true;
    }

    [Command(requiresAuthority = false)]
    public void CmdTryChangeCharacter(GameObject target)
    {
        lock (_lock)
        {
            if (!_isRestock) return;

            _isRestock = false;
            _player = target;
        }

        RpcPlayerStuckToLocker(_player.GetComponent<NetworkIdentity>().connectionToClient, target);

        //CmdChangeSortingOrder(_player);
        RpcChangeSortingOrder(_player);
        _animator.SetTrigger(Changing);
        Invoke("CharacterChange", 1.2f);
    }

    [TargetRpc]
    private void RpcPlayerStuckToLocker(NetworkConnectionToClient conn, GameObject player)
    {
        var playerSM = player.GetComponent<PlayerSM>();
        playerSM.canControl = false;

        var playerRigidbody = player.GetComponent<Rigidbody2D>();
        playerRigidbody.gravityScale = 0f;
        playerRigidbody.velocity = Vector3.zero;

        playerSM.transform.position = transform.position + new Vector3(0, 1f);
    }

    [ClientRpc]
    private void RpcChangeSortingOrder(GameObject player)
    {
        var playerSortingGroup = player.GetComponent<PlayerSM>().sortingGroup;
        foreach (var sortingGroup in playerSortingGroup)
        {
            sortingGroup.sortingLayerName = "BackGround";
            sortingGroup.sortingOrder = 3;
        }
    }

    public void CharacterChange()
    {
        Managers.Network.ReplacePlayer(_player.GetComponent<NetworkIdentity>().connectionToClient,
            _characterType,
            transform.position + new Vector3(0, 0.2f));
        _animator.SetTrigger("Restock");
    }

    //[Command(requiresAuthority = false)]
    //public void CmdChangeCharacter()
    //{
    //    Managers.Network.ReplacePlayer(_player.GetComponent<NetworkIdentity>().connectionToClient,
    //        _characterType,
    //        transform.position + new Vector3(0, 0.2f));
    //    _animator.SetTrigger("Restock");
    //}

    public void Restocked()
    {
        _isRestock = true;
    }

    public ObjectTypeEnum GetObjectType()
    {
        return _objectType;
    }

    public void ShowEButton()
    {
        var eButtonUI = Managers.UI.ShowUI<UI_ShowEButton>();
        eButtonUI.transform.position = transform.position + (Vector3)offset;
    }

    public void HideEButton()
    {
        Managers.UI.HideUI<UI_ShowEButton>();
    }

    public void OnOpenEvent()
    {
        Managers.Sound.PlaySound3D(GlobalText.LOCKER_OPEN_SOUND, transform.position, 0.45f);
    }

    public void OnCloseEvent() 
    {
        Managers.Sound.PlaySound3D(GlobalText.LOCKER_CLOSE_SOUND, transform.position, 0.45f);
    }
}
