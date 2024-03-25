using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    Material dissolveMaterial;
    BoxCollider2D _collider;
    Rigidbody2D _rb;
    BuildObj buildObj;

    float dissolveRate = 0.005f;
    private void Awake()
    {
        dissolveMaterial = GetComponent<SpriteRenderer>().material;
        _collider = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        buildObj = GetComponent<BuildObj>();
        buildObj.OnDissolveAction += Dissolve;
    }

    public void Dissolve(Vector2 pot)
    {
        StartCoroutine(Co_Dissolve(pot));
    }

    IEnumerator Co_Dissolve(Vector2 pot)
    {
        float percent = 1;
        _collider.enabled = false;
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



}
