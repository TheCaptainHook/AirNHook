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
    
    [SerializeField] Transform holder;
    [SerializeField] AnimationCurve curve;
    
    private Vector3 velocity = Vector3.zero;

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

     /*public IEnumerator Co_Shake(float duration, float magnitude)
    {
        Debug.Log("Shake");
        Vector3 originPos = new Vector3(0, 0, 0);
        Vector3 left = new Vector3(-1f,.5f,0);
        Vector3 right = new Vector3(1f,-.5f,0);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            Vector3 targetPosition = Vector3.Lerp(left,right,curve.Evaluate(elapsedTime/duration));

            holder.position = Vector3.SmoothDamp(holder.position, targetPosition, ref velocity, 0.1f); 

            yield return null;
        }

        holder.position = originPos;
    }*/

     public IEnumerator Co_Shake(float duration, float magnitude)
     {
         Debug.Log("Shake");
         Vector3 originPos = new Vector3(0, 0, 0);
         float elapsedTime = 0f;

         float minMagnitude = magnitude * 0.5f;

         while (elapsedTime < duration)
         {
             elapsedTime += Time.deltaTime;

             float randomX = Random.Range(-magnitude, magnitude) * (Random.Range(0, 2) == 0 ? 1 : -1);
             float randomY = Random.Range(-magnitude, magnitude) * (Random.Range(0, 2) == 0 ? 1 : -1);

             randomX = Mathf.Clamp(randomX, -magnitude, -minMagnitude);
             randomY = Mathf.Clamp(randomY, -magnitude, -minMagnitude);

             Vector3 randomOffset = new Vector3(randomX, randomY, 0);
             Vector3 targetPosition = originPos + randomOffset;

             holder.position = targetPosition;
             
             yield return null;
         }
         holder.position = originPos;
     }

}
