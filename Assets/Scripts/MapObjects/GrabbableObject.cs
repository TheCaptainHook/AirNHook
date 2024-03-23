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

    [Command(requiresAuthority = false)]
    public void CmdGrab()
    {
        RpcGrab();
    }

    [ClientRpc]
    private void RpcGrab()
    {
        isGrabbed = true;
        transform.parent = player;
        transform.localPosition = player.position;
        transform.localRotation = player.rotation;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.velocity = new Vector2(0, 0);
    }

    [Command(requiresAuthority = false)]
    public void CmdRelease()
    {
        RpcRelease();
    }

    [ClientRpc]
    private void RpcRelease()
    {
        isGrabbed = false;
        transform.parent = null;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
    }
}
