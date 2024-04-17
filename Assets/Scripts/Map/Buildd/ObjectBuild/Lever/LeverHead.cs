using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverHead : BuildObj
{
    Collider2D _collider;
    Rigidbody2D _rb;

    //effect
    float dissolveRate = 0.005f;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        //OnDissolveAction += Dissolve;
    }



    public void AttachToLevelBody(Transform transform)
    {
        _collider.enabled = false;
        _rb.velocity = Vector2.zero;
        _rb.gravityScale = 0;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.freezeRotation = true;

        this.transform.SetParent(transform);
        this.transform.rotation = Quaternion.Euler(0, 0, 0);
        this.transform.localPosition = Vector2.zero;
    }

    public void DetachToLevelBody()
    {
        
    }

    #region Effect

    public void Dissolve(Vector2 pot)
    {
        if (MapEditor.Instance.mapEditorState != MapEditorState.NoEditor)
        {
            EditorMode_Destroy();
        }
        else
        {
            StartCoroutine(Co_Dissolve(pot));
        }

    }

    IEnumerator Co_Dissolve(Vector2 pot)
    {
        float percent = 1;
        _collider.enabled = false;
        _rb.velocity = Vector2.zero;
        _rb.gravityScale = 0;
        while (percent > 0)
        {
            percent -= dissolveRate;
            //dissolveMaterial.SetFloat("_DissolveAmount", percent);
            yield return null;
        }

        transform.position = pot;

        while (percent < 1)
        {
            percent += dissolveRate;
            //dissolveMaterial.SetFloat("_DissolveAmount", percent);
            yield return null;
        }
        _collider.enabled = true;
        _rb.gravityScale = 1;
    }
    #endregion

    public void Activation()
    {
        //Animation
    }
    public void Deactivation()
    {
        //Animation
    }


}
