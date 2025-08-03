using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class LaserObject_Net : ActivatableObject_Net_Entity
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private GameObject _endVFX;

    protected override void Active()
    {
        _endVFX.SetActive(true);
        _lineRenderer.enabled = true;
    }
    protected override void Deactive()
    {
        _endVFX.SetActive(false);
        _lineRenderer.enabled = false;
    }



}
