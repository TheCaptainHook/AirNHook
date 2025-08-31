using System.Collections;
using UnityEngine;

public class CameraShakeObject : MonoBehaviour
{
    private float _sleepInterval = 1f;
    private Vector3 _offset = new Vector3(0, 0.5f);
    private WaitForSeconds _waitForSeconds;
    private WaitForSeconds _waitForSleep;
    public LayerMask layerMask;
    public float shakeInterval = 0.1f;
    public float radius = 1f;
    public float intensity = 1f;
    public float minDistance = 0.2f;
    public float duration = 1f;

    private Coroutine shackeCoroutine;

    void Awake()
    {
        _waitForSeconds = new WaitForSeconds(shakeInterval);
        _waitForSleep = new WaitForSeconds(_sleepInterval);
    }

    void OnEnable()
    {
        if (shackeCoroutine == null)
        {
            shackeCoroutine = StartCoroutine(CameraShake());
        }
    }
    
    void OnDisable()
    {
        StopAllCoroutines();
        shackeCoroutine = null;
    }

    private IEnumerator CameraShake()
    {
        while (true)
        {
            yield return _waitForSeconds;

            var collisions = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);

            if (collisions.Length == 0)
            {
                yield return _waitForSleep;
                continue;
            }

            foreach (var collision in collisions)
            {
                if (!collision.gameObject.Equals(Managers.Game.Player)) continue;

                var distance = Vector2.Distance(transform.position, collision.transform.position + _offset);

                float power;
                if (distance <= minDistance)
                    power = intensity;
                else
                    power = intensity * (1 - (distance - minDistance) / (radius - minDistance));

                Managers.Game.cameraShake.RequestShake(gameObject, power, duration);
            }
        }
    }
}
