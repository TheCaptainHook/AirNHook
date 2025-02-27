using UnityEngine;

public class CameraShakeObject : MonoBehaviour
{
    private float shakeInterval = 0.1f;
    private float lastShakeTime = 0f;
    private float radius;
    private Vector3 _offset = new Vector3(0, 0.5f);
    public float intensity = 1f;
    public float minDistance = 0.2f;
    public float duration = 1f;

    void Start()
    {
        radius = GetComponent<CircleCollider2D>().radius;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (Managers.Game.Player == null || !collision.gameObject.Equals(Managers.Game.Player)) return;

        if (Time.time - lastShakeTime < shakeInterval) return;
        
        var distance = Vector2.Distance(transform.position, collision.transform.position + _offset);

        float power;
        if (distance <= minDistance)
            power = intensity;
        else
            power = intensity * (1 - (distance - minDistance) / (radius - minDistance));

        Managers.Game.cameraShake.RequestShake(gameObject, power, duration);
        lastShakeTime = Time.time;
    }
}
