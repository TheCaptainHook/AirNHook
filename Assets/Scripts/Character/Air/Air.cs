using UnityEngine;

public class Air : Player
{
    [field: SerializeField] private AirGunNet _airGun;
    
    public override void TakeDamage()
    {
        if(!isLocalPlayer)
            return;
        
        _animator.SetTrigger(IsDead);
        
        if (isDead) return;
        Debug.Log("사망하였습니다.");
        // 여기에 필요한 사망 처리
        // _animator.SetTrigger(IsDead);
        CmdIncreaseDeathCount();
        isDead = true;
        _movement.IsDead = true;
        _rigidbd.constraints = RigidbodyConstraints2D.FreezeAll;
        _collider2D.enabled = false;
        _airGun.Reset();
    }

    public override void Respawning()
    {
        base.Respawning();
        _airGun.Reset();
        _airGun.canHandle = true;
    }
}
