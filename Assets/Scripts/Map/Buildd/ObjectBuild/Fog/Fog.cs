using System;

using UnityEngine;

public class Fog : BuildObj
{
    [CustomHeader("Fog")]
    public Vector2 size;

    private ParticleSystem _MainParticle;
    private BoxCollider2D _Colider;

    [ReadOnly]
    [SerializeField] GameObject innerFogEffect;


    //10 : 10 = 40
    //20 : 20 = 160
    // a*b*0.4

    #region Particle
    private Vector3 shapeScale;
    #endregion

    public void Init()
    {
        _MainParticle = GetComponent<ParticleSystem>();
        _Colider = GetComponent<BoxCollider2D>();
    }

    #region Get,Set
    public override void SetData<T>(T data)
    {
        if(typeof(T) == typeof(ObjectData)){
            ObjectData objData = (ObjectData)(object)data;
            SetData(objData);
            Init();
            SetParticleSetting();
        }
    }
    public override void SetData(ObjectData data)
    {
        base.SetData(data);
        size = data.size;
        
    }
    public override T GetData<T>()
    {
        if(typeof(T)==typeof(ObjectData)){
            return (T)(object)new ObjectData(id,transform.position,size);
        }

       return default(T);
    }
    #endregion


    #region Particle    
    public void SetParticleSetting(){
        SetParticleShapeScale();
        SetParticleEmissionRate();
    }
    private void SetParticleEmissionRate()
    {
        float rate = size.x * size.y * 0.5f;
        var emission = _MainParticle.emission;
        emission.rateOverTime = rate;
    }
    private void SetParticleShapeScale()
    {
        _Colider.size = size;
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
        GameObject fogEffect = Managers.Pooling.D_GetItem(innerFogEffect);   
        try{
            if(fogEffect == null) return;
            fogEffect.transform.position = collision.transform.position;
            fogEffect.transform.eulerAngles = new Vector3(fogEffect.transform.rotation.x, deg, 0);
            fogEffect.SetActive(true);
            fogEffect.GetComponent<IPooling>().D_ReleaseToPool();
            // StartCoroutine(InnerFogCoroutine(fog));
        }catch(Exception ex){
            Debug.Log(ex);
        }
       

    }

}
