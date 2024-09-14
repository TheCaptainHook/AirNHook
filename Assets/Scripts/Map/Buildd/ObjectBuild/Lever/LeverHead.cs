using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverHead : BuildObj
{
    //Material dissolveMaterial;
    //effect
    private Vector2 _firstPos;
    private InteractableObject _interactableObject;

    private void Awake()
    {
        DissolveInitSetting();
        // _interactableObject = GetComponent<InteractableObject>();
        
    }

    private void Start()
    {
        _firstPos = transform.position;
    }


    public void AttachToLevelBody(Transform transform)
    {
        //_collider.enabled = false;
        //_rb.velocity = Vector2.zero;
        //_rb.gravityScale = 0;
        //_rb.bodyType = RigidbodyType2D.Kinematic;
        //_rb.freezeRotation = true;

        //this.transform.SetParent(transform);
        //this.transform.rotation = Quaternion.Euler(0, 0, 0);
        //this.transform.localPosition = Vector2.zero;
        _collider.enabled = false;
        _rb.simulated = false;

        //this.transform.SetParent(transform);
        this.transform.rotation = Quaternion.Euler(0, 0, 0);
        this.transform.position = transform.position;
    }

    public void AttachToLevelBody()
    {
        _interactableObject.Destroyed();
        _collider.enabled = false;
        _rb.simulated = false;
    }

    public void DetachToLevelBody()
    {
        _collider.enabled = true;
        _rb.simulated = true;
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0f;
        transform.SetParent(MapEditor.Instance.objectTransform);
    }

    #region Effect

    // public void Dissolve(Vector2 pot)
    // {
    //     if (MapEditor.Instance.mapEditorState != MapEditorState.NoEditor)
    //     {
    //         StartCoroutine(Co_Dissolve(orgPosition));
    //     }
    //     else
    //     {
    //         StartCoroutine(Co_Dissolve(_firstPos));
    //     }

    // }

    // IEnumerator Co_Dissolve(Vector2 pot)
    // {
    //     float percent = 1;
    //     _collider.enabled = false;
    //     _rb.velocity = Vector2.zero;
    //     _rb.gravityScale = 0;
    //     while (percent > 0)
    //     {
    //         percent -= dissolveRate;
    //         //dissolveMaterial.SetFloat("_DissolveAmount", percent);
    //         yield return null;
    //     }

    //     transform.position = pot;

    //     while (percent < 1)
    //     {
    //         percent += dissolveRate;
    //         //dissolveMaterial.SetFloat("_DissolveAmount", percent);
    //         yield return null;
    //     }
    //     _collider.enabled = true;
    //     _rb.gravityScale = 1;
    //     GetComponent<InteractableObject>().Respawned();
    // }
    #endregion

    //public void Activation()
    //{
    //    //Animation
    //}
    //public void Deactivation()
    //{
    //    //Animation
    //}

    public override void TurnOff()
    {
        base.TurnOff();

        _rb.velocity = Vector2.zero;
        _rb.gravityScale = 0;


    }

    public override void TurnOn()
    {
        base.TurnOn();

        _rb.gravityScale = 1;
    }

}
