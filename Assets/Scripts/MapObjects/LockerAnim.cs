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
    private Animator _animator;
    public Vector2 offset;

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
        _player = accessor.root.gameObject;

        var playerSM = _player.GetComponent<PlayerSM>();
        playerSM.canControl = false;

        var playerRigidbody = _player.GetComponent<Rigidbody2D>();
        playerRigidbody.gravityScale = 0f;
        playerRigidbody.velocity = Vector3.zero;

        playerSM.transform.position = transform.position + new Vector3(0, 1f);

        var playerSortingGroup = _player.GetComponent<SortingGroup>();
        playerSortingGroup.sortingOrder = 0;
        CmdChangeSortingOrder(_player);

        _animator.SetTrigger(Changing);
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interacting(bool value)
    {
        return;
    }

    public void ChangeCharacter()
    {
        if (_player is not null && _player.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            CmdChangeCharacter(_player);
            _animator.SetTrigger("Restock");
        }

        _player = null;
    }

    [Command(requiresAuthority = false)]
    private void CmdChangeCharacter(GameObject player)
    {
        Managers.Network.ReplacePlayer(player.GetComponent<NetworkIdentity>().connectionToClient,
            _characterType,
            transform.position + new Vector3(0, 0.2f));
    }

    [Command(requiresAuthority = false)]
    private void CmdChangeSortingOrder(GameObject player)
    {
        RpcChangeSortingOrder(player);
    }

    [ClientRpc(includeOwner = false)]
    private void RpcChangeSortingOrder(GameObject player)
    {
        var playerSortingGroup = player.GetComponent<SortingGroup>();
        playerSortingGroup.sortingOrder = 0;
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
