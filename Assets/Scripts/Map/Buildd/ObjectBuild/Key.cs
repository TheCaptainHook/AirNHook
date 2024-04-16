using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : BuildObj
{
    Material dissolveMaterial;
    BoxCollider2D _collider;
    Rigidbody2D _rb;
   
    float dissolveRate = 0.005f;
    private void Awake()
    {
        dissolveMaterial = GetComponent<SpriteRenderer>().material;
        _collider = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        OnDissolveAction += Dissolve;
    }

    public void Dissolve(Vector2 pot)
    {
        if(MapEditor.Instance.mapEditorState != MapEditorState.NoEditor)
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
        while (percent> 0)
        {
            percent -= dissolveRate;
            dissolveMaterial.SetFloat("_DissolveAmount", percent);
            yield return null;
        }
       
        transform.position = pot;

        while(percent < 1)
        {
            percent += dissolveRate;
            dissolveMaterial.SetFloat("_DissolveAmount", percent);
            yield return null;
        }
        _collider.enabled = true;
        _rb.gravityScale = 1;
    }





    public override void TurnOff()
    {
        base.TurnOff();
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;

    }
    public override void TurnOn()
    {
        base.TurnOn();
        _rb.gravityScale = 1;

    }


}
