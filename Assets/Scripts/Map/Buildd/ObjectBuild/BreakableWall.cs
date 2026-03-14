using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BreakableWall : BuildObj
{
    // private Rigidbody2D _rb;
    private Animator _animator;
    // [SerializeField] private Collider2D _collider;
    [SerializeField] private ParticleSystem _BrokenPartsParticles;
    [SerializeField] private ParticleSystem _DustParticles;

    public float health = 5f;
    
    private static readonly int DestroyTrigger = Animator.StringToHash("DestroyTrigger");
    private static readonly int Crumbling = Animator.StringToHash("Crumbling");

    [Header("Only use Editor mode")]
    private static readonly int Recovery = Animator.StringToHash("Recovery");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        // _rb = GetComponent<Rigidbody2D>();
    }

    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        Managers.Sound.PlaySound3D(GlobalText.HIT_SOUND, transform.position, 0.45f);
        health -= 1f;
        switch (health)
        {
            case 3f:
                _animator.SetTrigger(Crumbling);
                break;
            case <= 0f:
                DestroyWall();
                break;
        }
    }

    private void DestroyWall()
    {
        Managers.Sound.PlaySound3D(GlobalText.ROCK_DESTROY_SOUND, transform.position, 0.4f);
        _animator.SetTrigger(DestroyTrigger);
        Col.enabled = false;
    }

    public void CrumbleParticles()
    {
        _BrokenPartsParticles.Play();
    }

    public void BreakParticles()
    {
        _DustParticles.Play();
        _BrokenPartsParticles.Play();
    }
    
    public override void TurnOff()
    {
        base.TurnOff();
        if(health <= 0)
        {
            Reset();
        }
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
    }
    public override void TurnOn()
    {
        base.TurnOn();
        _rb.gravityScale = 1;
    }


    public override void Reset()
    {
        health = 5f;
        Col.enabled = true;
        _animator.SetTrigger(Recovery);
    }



}
