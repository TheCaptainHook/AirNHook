using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_Laser_TargetingMark : MonoBehaviour
{ 
    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    [Header("targeting")]
    [SerializeField] Transform lightTr;
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
            targetingCoroutine = StartCoroutine(TargetingCo());
        }
    }
    private Coroutine targetingCoroutine;
    private Vector2 velocity;
    IEnumerator TargetingCo()
    {
        float duration = 0.2f; // 이동 시간
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);

            // transform.position = Vector2.Lerp(transform.position, targetPosition, speedUpT);
            transform.position = Vector2.SmoothDamp(transform.position, targetPosition, ref velocity, t);
            line.SetPosition(1,targetPosition);
            yield return null;
        }
        targetingCoroutine = null;
        transform.position = targetPosition;
        line.SetPosition(1,targetPosition);
        
    }

    public void Reset()
    {
        StopAllCoroutines();
        if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            transform.position = lightTr.position;
            line.SetPosition(0,lightTr.position);
            line.SetPosition(1,lightTr.position);
        }   
    }
}
