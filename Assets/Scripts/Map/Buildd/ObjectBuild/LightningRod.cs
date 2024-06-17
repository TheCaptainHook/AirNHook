using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRod : BuildObj
{

    float dissolveRate = 0.05f;
    public Transform hitPoint;
    private bool onElectric;

    [SerializeField] ParticleSystem[] particles;


    [Header("Components")]
    Collider2D _collider;
    Rigidbody2D _rb;


    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        OnDissolveAction += Dissolve;
    }


    #region Effect

    public void Dissolve(Vector2 pot)
    {
        if (MapEditor.Instance.mapEditorState != MapEditorState.NoEditor)
        {
            StartCoroutine(Co_Dissolve(orgPosition));
        }
        else
        {
            StartCoroutine(Co_Dissolve(ObjectData.position));
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
        GetComponent<InteractableObject>().Respawned();
    }
    #endregion



    public void Electric()
    {
        if (!onElectric)
        {
            onElectric = true;
            Activate();
        }

    }


   public void Activate()
    {
        foreach(ParticleSystem ps in particles)
        {
            ps.Play();
        }
    }
    public void Deactivate()
    {
        foreach (ParticleSystem ps in particles)
        {
            ps.Stop();
        }
    }
}
