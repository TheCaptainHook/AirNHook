using System;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering;

public class LockerAnim : NetworkBehaviour, IInteractable
{
    [SerializeField] private GameObject _disappearingObj;
    [SerializeField] private CharacterType _characterType;
    [SerializeField] private SpriteRenderer _doorSpriteRenderer;
    private GameObject _player;
    private ObjectTypeEnum _objectType = ObjectTypeEnum.Interaction;
    private NetworkAnimator _animator;
    [SyncVar] private bool _isRestock = true;
    public Vector2 offset;

    #region StringCache
    private static readonly int Changing = Animator.StringToHash("Changing");
    #endregion
    
    public bool IsChanging = false;
    
    public event Action OnChangingAnimation;
    
    private void Awake()
    {
        _animator = GetComponent<NetworkAnimator>();
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

    public void Interacting(bool value)
    {
        return;
    }

    [Command(requiresAuthority = false)]
    public void CmdTryChangeCharacter(GameObject target)
    {
        if (!_isRestock) return;

        _isRestock = false;
        _player = target;

        RpcPlayerStuckToLocker(_player.GetComponent<NetworkIdentity>().connectionToClient, target);

        CmdChangeSortingOrder(_player);

        _animator.SetTrigger(Changing);
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

    [Command(requiresAuthority = false)]
    private void CmdChangeSortingOrder(GameObject player)
    {
        RpcChangeSortingOrder(player);
    }

    [ClientRpc]
    private void RpcChangeSortingOrder(GameObject player)
    {
        var playerSortingGroup = player.GetComponent<SortingGroup>();
        playerSortingGroup.sortingOrder = 0;
    }

    [Command(requiresAuthority = false)]
    public void CmdChangeCharacter()
    {
        Managers.Network.ReplacePlayer(_player.GetComponent<NetworkIdentity>().connectionToClient,
            _characterType,
            transform.position + new Vector3(0, 0.2f));
        _animator.SetTrigger("Restock");
    }

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
}
