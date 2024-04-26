using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatformTest : MonoBehaviour
{
    public PlatformEffector2D platformEffector2D;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private BoxCollider2D _boxCollider2D;

    private void FixedUpdate()
    {
        OneWayPlatformSetting();
    }

    public void OneWayPlatformSetting()
    {
        var upVector = Vector2.up;
        var boxUpVector = _rb.transform.up;

        var angle = Vector2.SignedAngle(upVector, boxUpVector);

        platformEffector2D.rotationalOffset = -angle;

        //오브젝트가 움직이고있는상태일때
        if (_rb.velocity.magnitude >= 0.1f)
        {
            _boxCollider2D.enabled = false;
        }
        //오브젝트가 정지한 상태일때
        else
            _boxCollider2D.enabled = true;
    }
}
