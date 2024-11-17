using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class StrongBox : BuildObj
{
    // private Material _dissolveMaterial;
    // private Rigidbody2D _rb;
    // private Collider2D _collider;
    // float dissolveRate = 0.015f;
    public float health = 5f;

    // [SerializeField] SpriteRenderer spriteRenderer;
    
    // private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");


    private void Awake()
    {
        // _dissolveMaterial = spriteRenderer.material;
        // _rb = GetComponent<Rigidbody2D>();
        // _collider = GetComponent<Collider2D>();

        // OnInteractableObjectRelease += GetComponent<InteractableObject>().Release;
        // OnDissolveAction += Dissolve;
        DissolveInitSetting();
    }

    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        health -= 1f;
        if (health <= 0f)
        {
            base.TakeDamage();
            health = 5f;
        }
    }
    
    // public void Dissolve(Vector2 pot)
    // {
    //     if(MapEditor.Instance.mapEditorState != MapEditorState.NoEditor)
    //     {
    //         StartCoroutine(Co_Dissolve(orgPosition));
    //     }
    //     else
    //     {
    //         StartCoroutine(Co_Dissolve(pot));
    //     }
        
    // }

    // IEnumerator Co_Dissolve(Vector2 pot)
    // {
    //     float percent = 1;
    //     _collider.enabled = false;
    //     _rb.velocity = Vector2.zero;
    //     _rb.gravityScale = 0;
    //     while (percent> 0)
    //     {
    //         percent -= dissolveRate;
    //         _dissolveMaterial.SetFloat(DissolveAmount, percent);
    //         yield return null;
    //     }
       
    //     transform.position = pot;

    //     while(percent < 1)
    //     {
    //         percent += dissolveRate;
    //         _dissolveMaterial.SetFloat(DissolveAmount, percent);
    //         yield return null;
    //     }
    //     _collider.enabled = true;
    //     _rb.gravityScale = 1;
    //     GetComponent<InteractableObject>().Respawned();
        
    //     if (MapEditor.Instance.mapEditorState == MapEditorState.Object)
    //     {
    //         TurnOff();
    //     }
    // }


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
