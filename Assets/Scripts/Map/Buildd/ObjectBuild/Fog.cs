using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Fog : MonoBehaviour
{

    public Vector2 size;

    private ParticleSystem _MainParticle;
    private BoxCollider2D _Colider;

    [SerializeField] GameObject innerFogEffect;


    //10 : 10 = 40
    //20 : 20 = 160
    // a*b*0.4

    #region Particle
    private float rateOverTime;
    private Vector3 shapeScale;
    #endregion
    public void Init()
    {
        _MainParticle = GetComponent<ParticleSystem>();
        _Colider = GetComponent<BoxCollider2D>();
    }

    private void Awake()
    {
        Init();
    }

    private void Setting()
    {
        _Colider.size = size;
        SetParticleShapeScale(size);
        SetParticleEmissionRate();
    }

    #region Particle
    private void SetParticleEmissionRate()
    {
        float rate = size.x * size.y * 0.4f;
        var emission = _MainParticle.emission;
        emission.rateOverTime = rate;
    }
    private void SetParticleShapeScale(Vector2 size)
    {
        var shape = _MainParticle.shape;
        shape.scale = size;
    }
    #endregion


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            InnerFogSetting(collision);
            if(collision.TryGetComponent(out PlayerSM component))
            {
                Camera.main.GetComponent<PlayerCameraView>()._CameraGlobalVolumeController.InnerFog(true);
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            InnerFogSetting(collision);
            if (collision.TryGetComponent(out PlayerSM component))
            {
                Camera.main.GetComponent<PlayerCameraView>()._CameraGlobalVolumeController.InnerFog(false);
            }
        }
    }

    private void InnerFogSetting(Collider2D collision)
    {
        Vector2 dir= (collision.transform.position - transform.position).normalized;
        float deg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        GameObject fog = Managers.Pooling.D_GetItem(innerFogEffect);   
        fog.transform.position = collision.transform.position;
        fog.transform.eulerAngles = new Vector3(fog.transform.rotation.x, deg, 0);
        fog.SetActive(true);
        StartCoroutine(InnerFogCoroutine(fog));

    }

    IEnumerator InnerFogCoroutine(GameObject innerFogEffect)
    {
        yield return new WaitForSeconds(2);
        innerFogEffect.SetActive(false);
        Managers.Pooling.D_ReleaseToPool(innerFogEffect);
    }
   
}
