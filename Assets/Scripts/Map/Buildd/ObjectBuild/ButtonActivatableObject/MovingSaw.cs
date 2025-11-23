using UnityEngine;


public class MovingSaw : DroneEntity
{
    [Header("Main")]

    // [SerializeField] private GameObject _greenLight;

    #region  SawObj
    [SerializeField] Animator _animator;
    public float addForcePower;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 객체가 IDamageable 인터페이스를 가지고 있는지 확인
        if (other.TryGetComponent(out IDamageable damageable) && !turnOff)
        {
            var rb = other.TryGetComponent(out Rigidbody2D _rb) ? _rb : null;
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                
                rb.AddForce(GetTargetDir(other) * addForcePower, ForceMode2D.Impulse);
            }
                damageable.TakeDamage();
        }
    }


    public override void Init()
    {
        Managers.Sound.PlaySound3D(GlobalText.SAW_SOUND_LOOP, transform, 0.35f, true, true);
        _animator.SetBool("onActive", true);
    }

    private Vector2 GetTargetDir(Collider2D target)
    {
        return (target.transform.position - transform.position).normalized;
    
    }
    #endregion


}

