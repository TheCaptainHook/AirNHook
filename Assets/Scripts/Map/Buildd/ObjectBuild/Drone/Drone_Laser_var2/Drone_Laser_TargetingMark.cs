using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Drone_Laser_TargetingMark : MonoBehaviour
{ 
    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    Coroutine defaultLineCoroutine;
    IEnumerator LineDefault()
    {
        while(true)
        {
            line.SetPosition(0, lightTr.position);
            yield return null;
        }
    }

    [Header("targeting")]
    [SerializeField] Transform lightTr;
    [SerializeField] Drone_Laser_var2 main;
    private Vector2 targetPosition;
    public void Targeting(Vector2 targetPosition)
    {
       if(!gameObject.activeSelf)
       {
        gameObject.SetActive(true);
       }
        this.targetPosition = targetPosition;

        if(targetingCoroutine == null)
        {
            defaultLineCoroutine = StartCoroutine(LineDefault());
            targetingCoroutine = StartCoroutine(TargetingCo());

        }
    }
    private Coroutine targetingCoroutine;
    private Vector2 velocity;
    float duration => main.laserTargetingMarkMovingRate; // 이동 시간
    IEnumerator TargetingCo()
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);

            // transform.position = Vector2.Lerp(transform.position, targetPosition, speedUpT);
            transform.position = Vector2.SmoothDamp(transform.position, targetPosition, ref velocity, t);
            line.SetPosition(1, transform.position);
            yield return null;
        }
        targetingCoroutine = null;
        
        transform.position = targetPosition;
        line.SetPosition(0, lightTr.position);
        line.SetPosition(1,transform.position);
        
    }

    public void Reset()
    {
        StopAllCoroutines();
        targetingCoroutine = null;
        defaultLineCoroutine = null;

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            transform.position = lightTr.position;
            line.SetPosition(1,lightTr.position);
        }   
    }
}
