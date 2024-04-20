using Mirror;
using UnityEngine;

public class Hook : Player
{
    [field: SerializeField] private Grappling _grappling;
    private Transform _grabbedItem;
    
    private static readonly int IsGrabbing = Animator.StringToHash("IsGrabbing");
    
    protected override void Interaction()
    {
        if (_grabbedItem is not null)
        {
            try { _grabbedItem.GetComponent<IInteractable>().Interaction(_grabPoint); }
            catch(MissingReferenceException e) { }
            _grabbedItem = null;
            _animator.SetBool(IsGrabbing, false);
        }
        else if (_latestTarget is not null)
        {
            if(!_latestTarget.TryGetComponent<IInteractable>(out var interactable)) return;

            if (interactable.GetObjectType() == ObjectTypeEnum.Grab)
            {
                _grabbedItem = _latestTarget.transform;
                _animator.SetBool(IsGrabbing, true);
                CmdObjectAuthoritySet(_grabbedItem.GetComponent<NetworkIdentity>());
            }
            interactable.Interaction(_grabPoint);
        }
    }

    public void ReleaseItem()
    {
        Debug.Log("a");
        if (_grabbedItem is null) return;
        
        Debug.Log("a");
        _grabbedItem = null;
        _animator.SetBool(IsGrabbing, false);
    }
    
    [Command(requiresAuthority = false)]
    private void CmdObjectAuthoritySet(NetworkIdentity id)
    {
        id.RemoveClientAuthority();
        id.AssignClientAuthority(connectionToClient);
    }

    public override void TakeDamage()
    {
        if(!isLocalPlayer)
            return;
        
        if (isDead) return;
        isDead = true;
        _grappling.StopRope();
        _animator.SetTrigger(IsDead);
        Debug.Log("사망하였습니다.");
        _movement.IsDead = true;
        _rigidbd.constraints = RigidbodyConstraints2D.FreezeAll;
        _collider2D.enabled = false;
        if(_grabbedItem is not null)
            Interaction();
    }
}
