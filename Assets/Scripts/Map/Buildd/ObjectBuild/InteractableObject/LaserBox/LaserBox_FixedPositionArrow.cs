using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBox_FixedPositionArrow : MonoBehaviour
{
    [SerializeField]Transform parent;
    private Quaternion initialWorldRotation;
        private Vector3 localOffset;
    void Start()
    {
                initialWorldRotation = transform.rotation;

    }

    void LateUpdate()
    {
                transform.rotation = initialWorldRotation;
    }
}
