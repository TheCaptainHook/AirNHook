using UnityEngine;

public class SpikeBall : InhalableObject
{
    [field: SerializeField] private Collider2D _coll;
    [field: SerializeField] private GameObject _breakingColl;
    [field: SerializeField] private float _minVelocity;
    [field: SerializeField] private LayerMask _defaultLayer;
    [field: SerializeField] private LayerMask _modifiedLayer;
    public GameObject shootingPlayer;

    protected override void Start()
    {
        base.Start();
        _breakingColl.SetActive(false);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        VelocityCheck();
    }

    public override bool Inhaling(bool value, GameObject player)
    {
        shootingPlayer = player;
        return base.Inhaling(value, player);
    }

    public override void Shooting(Vector2 force)
    {
        base.Shooting(force);

        _breakingColl.SetActive(true);
        Invoke("MakeShootingPlayerNull", 0.1f);
    }

    public override void StopInhale(GameObject accssor)
    {
        base.StopInhale(accssor);
        shootingPlayer = null;
    }

    private void VelocityCheck()
    {
        if (!_breakingColl.activeSelf) return;

        if (_rigidbody.velocity.magnitude <= _minVelocity)
        {
            _breakingColl.SetActive(false);
            _coll.includeLayers = _defaultLayer;
            _coll.forceSendLayers = _defaultLayer;
            _coll.forceReceiveLayers = _defaultLayer;
        }
        else
        {
            _breakingColl.SetActive(true);
            _coll.includeLayers = _modifiedLayer;
            _coll.forceSendLayers = _modifiedLayer;
            _coll.forceReceiveLayers = _modifiedLayer;
        }
    }

    private void MakeShootingPlayerNull()
    {
        shootingPlayer = null;
    }
}
