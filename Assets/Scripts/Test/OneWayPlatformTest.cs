using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatformTest : MonoBehaviour
{
    public PlatformEffector2D platformEffector2D;
    [SerializeField] private Rigidbody2D _rb;

    private void FixedUpdate()
    {
        Test();
    }

    public void Test()
    {
        var upVector = Vector2.up;
        var boxUpVector = _rb.transform.up;

        var angle = Vector2.SignedAngle(upVector, boxUpVector);

        platformEffector2D.rotationalOffset = -angle;

        Debug.Log("_rb.transform = " + _rb.transform);
    }
}
