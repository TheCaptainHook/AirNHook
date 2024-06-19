using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRod : BuildObj
{

    float dissolveRate = 0.05f;
    public Transform hitPoint;

    public bool onElectric;
    [SerializeField] float maxDurationRate; //Electric Duration
    public float curDurationRate;

    [SerializeField] ParticleSystem[] particles;


    [Header("Components")]
    Collider2D _collider;
    Rigidbody2D _rb;


    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        OnDissolveAction += Dissolve;

        curDurationRate = maxDurationRate;
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
            Activate();
            StartCoroutine(Timer());
        }
        else
        {
            curDurationRate = maxDurationRate;
        }

    }


   public void Activate()
    {
        onElectric = true;
        foreach (ParticleSystem ps in particles)
        {
            ps.Play();
        }
    }
    public void Deactivate()
    {
        onElectric = false;
        foreach (ParticleSystem ps in particles)
        {
            ps.Stop();
            
        }
    }





    IEnumerator Timer()
    {
        while(curDurationRate > 0 && onElectric)
        {
            curDurationRate -= Time.deltaTime;
            yield return null;
        }
        Deactivate();
        curDurationRate = maxDurationRate;
    }
}
