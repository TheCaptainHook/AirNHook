using System.Collections.Generic;
using UnityEngine;

public class NewCameraShake : MonoBehaviour
{
    private class ShakeRequest
    {
        public float intensity;
        public float endTime;

        public ShakeRequest(float intensity, float duration)
        {
            this.intensity = intensity;
            this.endTime = Time.time + duration;
        }

        public void UpdateDuration(float newDuration)
        {
            if (newDuration > (endTime - Time.time))
                endTime = Time.time + newDuration;
        }
    }

    private Dictionary<CameraShakeType, ShakeRequest> _activeShakes = new();
    private Vector3 _originalPosition = Vector3.zero;
    private ShakeRequest _strongestShake = null;
    private Stack<ShakeRequest> _shakePool = new();
    private bool _isShaking = false;
    public Transform cameraHolder;

    private void Start()
    {
        Managers.Game.cameraShake = this;
    }

    public void RequestShake(CameraShakeType sourceType, float intensity, float duration)
    {
        ShakeRequest newShake;

        if (_activeShakes.TryGetValue(sourceType, out newShake))
        {
            newShake.UpdateDuration(duration);
        }
        else
        {
            newShake = GetShakeRequest(intensity, duration);
            _activeShakes[sourceType] = newShake;
        }

        if (_strongestShake == null || intensity > _strongestShake.intensity)
            _strongestShake = newShake;

        if (_activeShakes.Count > 0)
            _isShaking = true;
    }

    private ShakeRequest GetShakeRequest(float intensity, float duration)
    {
        if (_shakePool.Count > 0)
        {
            ShakeRequest request = _shakePool.Pop();
            request.intensity = intensity;
            request.endTime = Time.time + duration;
            return request;
        }
        return new ShakeRequest(intensity, duration);
    }

    private void ReleaseShakeRequest(ShakeRequest target)
    {
        _shakePool.Push(target);
    }

    private void Update()
    {
        if (!_isShaking) return;

        float maxIntensity = 0;
        ShakeRequest maxShake = null;
        CameraShakeType? expiredKey = null;

        foreach (var kvp in _activeShakes)
        {
            if (Time.time > kvp.Value.endTime)
            {
                expiredKey = kvp.Key;
            }
            else if (kvp.Value.intensity > maxIntensity)
            {
                maxIntensity = kvp.Value.intensity;
                maxShake = kvp.Value;
            }
        }

        if (expiredKey.HasValue)
        {
            ReleaseShakeRequest(_activeShakes[expiredKey.Value]);
            _activeShakes.Remove(expiredKey.Value);

            if (_activeShakes.Count <= 0)
                _isShaking = false;
        }

        _strongestShake = maxShake;

        if (_strongestShake != null)
        {
            float shakeAmount = _strongestShake.intensity * 0.1f;

            cameraHolder.position = _originalPosition + new Vector3(
                (Mathf.PerlinNoise(Time.time * 10, 0) - 0.5f) * shakeAmount,
                (Mathf.PerlinNoise(0, Time.time * 10) - 0.5f) * shakeAmount,
                0);
        }
        else
        {
            cameraHolder.position = _originalPosition;
        }
    }
}
