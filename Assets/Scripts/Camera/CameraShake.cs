using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    #region Cinemachine
    //
    // //임시 싱글톤
    // #region instance
    //
    // public static CameraShake instance = null; 
    //
    // private void Awake()
    // {
    //     if (instance == null)
    //     {
    //         instance = this;
    //     }
    //     else
    //     {
    //         if (instance != this)
    //             Destroy(this.gameObject); 
    //     }
    //     
    //     _cvc = GetComponent<CinemachineVirtualCamera>();
    // }
    //
    // #endregion
    //
    //
    // private CinemachineVirtualCamera _cvc;
    // //private float _shakeIntensity = 1f;
    // //private float _shakeTime = 0.2f;
    //
    // private float _timer;
    // private CinemachineBasicMultiChannelPerlin _cbmcp;
    //
    // private void Start()
    // {
    //     StopShake();
    // }
    //
    // public void ShakeCamera(float time, float intensity)
    // {
    //     CinemachineBasicMultiChannelPerlin _cbmcp = _cvc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    //     _cbmcp.m_AmplitudeGain = intensity;
    //
    //     _timer = time;
    // }
    //
    // private void StopShake()
    // {
    //     CinemachineBasicMultiChannelPerlin _cbmcp = _cvc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    //     _cbmcp.m_AmplitudeGain = 0f;
    //     _timer = 0;
    // }
    //
    // private void Update()
    // {
    //     if (_timer > 0)
    //     {
    //         _timer -= Time.deltaTime;
    //
    //         if (_timer <= 0)
    //         {
    //             StopShake();;
    //         }
    //     }
    // }

    #endregion
    
    #region instance
    public static CameraShake instance = null; 
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
                Destroy(this.gameObject); 
        }
    }
    #endregion

    public IEnumerator Co_Shake(float duration, float magnitude)
    {
        Debug.Log("Shake");
        Vector3 originPos = new Vector3(0, 0, 0);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float xOffset = Random.Range(-0.5f, 0.5f) * magnitude;
            float yOffset = Random.Range(-0.5f, 0.5f) * magnitude;

            transform.localPosition = new Vector3(xOffset, yOffset, originPos.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originPos;
    }
}
