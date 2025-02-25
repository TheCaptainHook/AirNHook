using UnityEngine;

public class CameraShakeObject : MonoBehaviour
{
    private float shakeInterval = 0.1f;
    private float lastShakeTime = 0f;
    public CameraShakeType shakeType;
    public float intensity = 1f;
    public float duration = 1f;

    void OnTriggerStay2D(Collider2D collision)
    {
        if (Managers.Game.Player == null || !collision.gameObject.Equals(Managers.Game.Player)) return;

        if (Time.time - lastShakeTime < shakeInterval) return;
        
        Managers.Game.cameraShake.RequestShake(shakeType, intensity, duration);
        lastShakeTime = Time.time;
    }
}
