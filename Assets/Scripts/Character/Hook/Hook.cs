using Mirror;
using UnityEngine;

public class Hook : Player
{
    [field: SerializeField] private Grappling _grappling;
    
    private static readonly int IsGrabbing = Animator.StringToHash("IsGrabbing");
    
    protected override void Interaction()
    {
        if (_grabbedItem != null)
        {
            _grabbedItem.GetComponent<IInteractable>().Interaction(_grabPoint);
            CmdGrabInteraction();
            _grabbedItem = null;
            _animator.SetBool(IsGrabbing, false);
        }
        else if (_latestTarget != null)
        {
            var interactable = _latestTarget.GetComponent<IInteractable>();

            if (interactable.GetObjectType() == ObjectTypeEnum.Grab)
            {
                _grabbedItem = _latestTarget.transform;
                _animator.SetBool(IsGrabbing, true);
                CmdGrabInteraction();
            }
            interactable.Interaction(_grabPoint);
        }
    }
    
    [Command(requiresAuthority = false)]
    private void CmdGrabInteraction()
    {
        RpcGrabInteraction();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcGrabInteraction()
    {
        _latestTarget.GetComponent<IInteractable>().Interaction(_grabPoint);
    }

    public override void TakeDamage()
    {
        if(!isLocalPlayer)
            return;
        
        
        if (_isDead) return;
        _isDead = true;
        _grappling.StopRope();
        _animator.SetTrigger(IsDead);
        Debug.Log("사망하였습니다.");
        _movement.IsDead = true;
        _rigidbd.constraints = RigidbodyConstraints2D.FreezeAll;
        _collider2D.enabled = false;
    }
}
