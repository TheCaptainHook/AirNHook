using UnityEngine;

public class SpikeBall : InhalableObject
{
    [field: SerializeField] private Collider2D _coll;
    [field: SerializeField] private GameObject _breakingColl;
    [field: SerializeField] private float _minVelocity;
    [field: SerializeField] private LayerMask _defaultLayer;
    [field: SerializeField] private LayerMask _modifiedLayer;

    protected override void Start()
    {
        base.Start();
        _breakingColl.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!isOwned) return;

        VelocityCheck();
    }

    public override void Shooting(Vector2 force)
    {
        base.Shooting(force);

        _breakingColl.SetActive(true);
    }

    private void VelocityCheck()
    {
        if (_rigidbody.velocity.magnitude <= _minVelocity)
            _breakingColl.SetActive(false);
    }
}
