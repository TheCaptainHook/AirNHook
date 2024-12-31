using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraGlobalVolumeController : MonoBehaviour
{
    private Volume _Volume;
    private Vignette _Vignette;
    private LensDistortion _LensDistortion;

    private Coroutine InnerFogCoroutine;
    private Coroutine DeathVignetteCoroutine;
    private bool isInFog;


    private Vector3 previousViewport;
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _Volume = GetComponent<Volume>();

        if (_Volume.profile.TryGet(out Vignette vignette))
        {
            _Vignette = vignette;
        }
        if (_Volume.profile.TryGet(out LensDistortion lensDistortion))
        {
            _LensDistortion = lensDistortion;
        }
    }


    #region Portal

    public void PortalSpace_TimeTransitionEffect()
    {
        StartCoroutine(PSTTECoroutine());

    }
    IEnumerator PSTTECoroutine()
    {
        _LensDistortion.active = true;
        float percent = 0;
        // holding camera
        Camera.main.GetComponent<PlayerCameraView>().notFollowCam = true;
        while(percent < 1)
        {
            percent += Time.fixedDeltaTime*1.8f;
            _Volume.weight = percent;
            yield return null;
        }
        //Transform position
        Camera.main.GetComponent<PlayerCameraView>().SetCamerPosition();
        while (percent > 0)
        {
            percent -= Time.fixedDeltaTime * 1.8f;
            _Volume.weight = percent;
            yield return null;
        }
        
        _Volume.weight = 0;
        _LensDistortion.active = false;
    }
    #endregion

    private void Update()
    {
        if (isInFog)
        {
            Vector3 viewport = GetViewPort();
            if (previousViewport != viewport)
            {
                previousViewport = viewport;
                _Vignette.center.value = (Vector2)previousViewport;
            }
        }
    }

    #region Fog
    public void InnerFog(bool inout)
    {
        _Volume.weight = 1;

        if (InnerFogCoroutine != null)
        {
            StopCoroutine(InnerFogCoroutine);
        }

        if (inout)
        {
            InnerFogCoroutine = StartCoroutine(VignetteCoroutine_In());
        }
        else
        {
           InnerFogCoroutine = StartCoroutine(VignetteCoroutine_Out());
        }

    }

    private Vector3 GetViewPort()
    {
        if (!Managers.Game.Player) return Vector3.zero;

        return Camera.main.WorldToViewportPoint(Managers.Game.Player.transform.position);

    }
    IEnumerator VignetteCoroutine_In()
    {
        if(!_Vignette.active) _Vignette.active = true;
        isInFog = true;

        float percent = _Vignette.intensity.value;
        while (percent < 1) 
        {
            percent += Time.fixedDeltaTime;
            _Vignette.intensity.value = percent;
            yield return null;
        }
        _Vignette.intensity.value = 1;
        InnerFogCoroutine = null;
    }
    IEnumerator VignetteCoroutine_Out()
    {
 
        float percent = _Vignette.intensity.value;
        while(percent > 0)
        {
            percent -= Time.fixedDeltaTime;
            _Vignette.intensity.value= percent;
            yield return null;
        }
        _Vignette.intensity.value = 0;
        InnerFogCoroutine = null;
        _Vignette.active = false;
        _Volume.weight = 0;
        isInFog = false;
    }
    #endregion

    #region DeathVignette

    public void DeathVignette(bool inout)
    {
        if ( isInFog ) return;
        
        _Volume.weight = 0.65f;

        if (DeathVignetteCoroutine != null)
        {
            StopCoroutine(DeathVignetteCoroutine);
        }

        if (inout)
        {
            DeathVignetteCoroutine = StartCoroutine(DeathVignetteCoroutine_In(0.3f, 0.7f));
        }
        else
        {
            DeathVignetteCoroutine = StartCoroutine(DeathVignetteCoroutine_Out());
        }

    }
    IEnumerator DeathVignetteCoroutine_In(float targetIntensity = 1.0f, float speed = 1.0f)
    {
        if (!_Vignette.active) _Vignette.active = true;

        float percent = _Vignette.intensity.value;
        while (percent < targetIntensity)
        {
            percent += Time.fixedDeltaTime * speed;
            _Vignette.intensity.value = Mathf.Clamp01(percent); // 값 제한
            yield return null;
        }
        _Vignette.intensity.value = targetIntensity;
        DeathVignetteCoroutine = null;
    }

    IEnumerator DeathVignetteCoroutine_Out(float targetIntensity = 0.0f, float speed = 1.0f)
    {
        float percent = _Vignette.intensity.value;
        while (percent > targetIntensity)
        {
            percent -= Time.fixedDeltaTime * speed;
            _Vignette.intensity.value = Mathf.Clamp01(percent); // 값 제한
            yield return null;
        }
        _Vignette.intensity.value = targetIntensity;
        if (targetIntensity <= 0)
        {
            _Vignette.active = false;
            _Volume.weight = 0;
        }
        DeathVignetteCoroutine = null;
    }

    #endregion
}
