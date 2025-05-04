using Mirror;
using UnityEngine;

public class ShakingEffectOnAirGun : NetworkBehaviour
{
    public LayerMask layerMask;
    private Vector3 _offset = new Vector3(0, 0.5f);
    private float timer = 0f;
    public float shakeInterval = 0.1f;
    public float radius = 1f;
    public float intensity = 1f;
    public float minDistance = 0.2f;
    public float duration = 0.5f;

    [SyncVar] public bool isShaking;

    private void Update()
    {
        if (!isShaking) return;

        if (timer < shakeInterval)
        {
            timer += Time.deltaTime;
            return;
        }
        timer = 0f;

        var collisions = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);

        if (collisions.Length == 0) return;

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

    public void StartShaking()
    {
        isShaking = true;
    }

    public void StopShaking()
    {
        isShaking = false;
    }
}
