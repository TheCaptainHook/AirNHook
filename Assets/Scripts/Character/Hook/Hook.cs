using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class Hook : Player
{
    [field: SerializeField] private Grappling _grappling;
    private Transform _grabbedItem;
    
    private static readonly int IsGrabbing = Animator.StringToHash("IsGrabbing");

    protected override IEnumerator Co_DetectInteraction()
    {
        var shortestDistance = float.MaxValue;
        var offset = new Vector3(0, 0.45f);
        Collider2D closestTarget = null;
        
        while (true)
        {
            yield return null;
            if(_grabbedItem is not null)
                continue;
            
            var collisions = Physics2D.OverlapCircleAll(transform.position + offset, _detectDistance, _interactableLayer);

            if (collisions.Length == 0)
            {
                if (_latestTarget is null) continue;

                try { _latestTarget.GetComponent<IInteractable>().HideEButton(); }
                catch (MissingReferenceException e) { Managers.UI.HideUI<UI_ShowEButton>(); }
                _latestTarget = null;
                continue;
            }

            foreach (var collision in collisions)
            {
                //TODO 벽에 가로막혔을 경우 체크
                var targetDistance = Vector2.Distance(transform.position + offset, collision.transform.position);
                if (targetDistance < shortestDistance)
                {
                    // 후크가 잡고 있는 물체 처리
                    if (collision.TryGetComponent<IInteractable>(out var inhalable) && !inhalable.CanInteract()) continue;
                    
                    shortestDistance = targetDistance;
                    closestTarget = collision;
                }
            }

            if (_latestTarget is not null)
            {
                if (ReferenceEquals(_latestTarget, closestTarget))
                {
                    shortestDistance = float.MaxValue;
                    continue;
                }

                _latestTarget.GetComponent<IInteractable>().HideEButton();
            }

            _latestTarget = closestTarget;
            _latestTarget.GetComponent<IInteractable>().ShowEButton();
            shortestDistance = float.MaxValue;
        }
    }

    protected override void Interaction()
    {
        if (_grabbedItem is not null)
        {
            try { _grabbedItem.GetComponent<IInteractable>().Interaction(_grabPoint); }
            catch(MissingReferenceException e) { ReleaseItem(); }
            _grabbedItem = null;
            _animator.SetBool(IsGrabbing, false);
        }
        else if (_latestTarget is not null)
        {
            if(!_latestTarget.TryGetComponent<IInteractable>(out var interactable) || !interactable.CanInteract()) return;

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
        if (!isLocalPlayer) return;
        
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
        CmdIncreaseDeathCount();
        _movement.IsDead = true;
        _rigidbd.constraints = RigidbodyConstraints2D.FreezeAll;
        _collider2D.enabled = false;
        if(_grabbedItem is not null)
            Interaction();
    }
}
