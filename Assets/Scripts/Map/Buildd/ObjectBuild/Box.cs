using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : BuildObj
{
    private Material _dissolveMaterial;
    private Rigidbody2D _rb;
    private Collider2D _collider;
    float dissolveRate = 0.015f;

    [SerializeField] SpriteRenderer spriteRenderer;


    private void Awake()
    {
        _dissolveMaterial = spriteRenderer.material;
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
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
            _dissolveMaterial.SetFloat("_DissolveAmount", percent);
            yield return null;
        }
       
        transform.position = pot;

        while(percent < 1)
        {
            percent += dissolveRate;
            _dissolveMaterial.SetFloat("_DissolveAmount", percent);
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
