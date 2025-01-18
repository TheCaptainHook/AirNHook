using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransformUnreliable))]
[RequireComponent(typeof(NetworkRigidbodyUnreliable2D))]
[RequireComponent(typeof(SortingGroup))]
public class InteractableObject_LadderObject : InteractableObject
{
    private LadderObject ladderObject;

    protected override void Awake()
    {
        base.Awake();
        ladderObject = GetComponent<LadderObject>();
    }

    public override void Release()
    {
        base.Release();
    }
}
