using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GrabbableObject : NetworkBehaviour
{
    public Transform player;
    private Rigidbody2D _rigidbody2D;
    [SyncVar]
    public bool isGrabbed;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        
    }

    public void Grab()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.velocity = new Vector2(0, 0);
        transform.parent = player;
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        CmdGrab();
    }

    [Command(requiresAuthority = false)]
    public void CmdGrab()
    {
        RpcGrab();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcGrab()
    {
        isGrabbed = true;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.velocity = new Vector2(0, 0);
        transform.parent = player;
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    public void Release()
    {
        isGrabbed = false;
        transform.parent = null;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        CmdRelease();
    }

    [Command(requiresAuthority = false)]
    public void CmdRelease()
    {
        RpcRelease();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcRelease()
    {
        isGrabbed = false;
        transform.parent = null;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
    }
}
