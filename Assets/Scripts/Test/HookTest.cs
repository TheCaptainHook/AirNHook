using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookTest : MonoBehaviour
{
    private GrapplingHookTest _grappling;
    public DistanceJoint2D _joint2D;
    public LayerMask layerMask;
    
    // Start is called before the first frame update
    void Start()
    {
        _grappling = GameObject.Find("Player").GetComponent<GrapplingHookTest>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            _joint2D.enabled = true;
            _grappling.isAttach = true;
        }
    }
}
