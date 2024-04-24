using System;
using Mirror;
using UnityEngine;

public class LockerAnim : NetworkBehaviour, IInteractable
{
    [SerializeField] private GameObject _disappearingObj;
    [SerializeField] private CharacterType _characterType;
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
    
    //이하 애니메이션 테스트용 코드
    
    private void Update()
    {
        if (IsChanging)
        {
            _animator.SetTrigger(Changing);
        }
    }

    public void DestroyGO()
    {
        Destroy(_disappearingObj);
    }

    public void Interaction(Transform accessor)
    {
        CmdChangeCharacter(accessor.root.gameObject);
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Fixed(bool value)
    {
        return;
    }

    [Command(requiresAuthority = false)]
    private void CmdChangeCharacter(GameObject player)
    {
        Managers.Network.ReplacePlayer(player.GetComponent<NetworkIdentity>().connectionToClient,
            _characterType,
            transform.position + new Vector3(0, 0.2f));
    }

    public ObjectTypeEnum GetObjectType()
    {
        return _objectType;
    }

    public void ShowEButton()
    {
        var eButtonUI = Managers.UI.ShowUI<UI_ShowEButton>();
        eButtonUI.transform.SetParent(transform);
        eButtonUI.transform.localPosition = offset;
    }

    public void HideEButton()
    {
        Managers.UI.HideUI<UI_ShowEButton>();
    }
}
