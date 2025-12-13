using Mirror;
using System.Collections;
using UnityEngine;

public class Destructible : NetworkBehaviour
{
    [SerializeField] private float _maxHealthPoint = 4f;
    [SyncVar] private float _healthPoint;

    [SerializeField] private SpriteRenderer[] _spriteRenderers;
    private Material[] _materials;
    private float _flashTime = 0.3f;

    private Coroutine _flashCoroutine;

    private void Start()
    {
        Init();

        _materials = new Material[_spriteRenderers.Length];

        for (var i = 0; i < _spriteRenderers.Length; i++)
        {
            _materials[i] = _spriteRenderers[i].material;
        }

        SetFlash(false);
    }

    private void Init()
    {
        StopAllCoroutines();
        SetFlash(false);
        _healthPoint = _maxHealthPoint;
    }

    public void TakeDestructionDamage(NetworkIdentity identity)
    {
        CmdReportDestructionHit();
    }

    [Command(requiresAuthority = false)]
    private void CmdReportDestructionHit()
    {
        _healthPoint--;

        if (_healthPoint < 0)
        {
            //Destroy
            return;
        }

        RpcFlash();
    }

    [ClientRpc]
    private void RpcFlash()
    {
        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);

        _flashCoroutine = StartCoroutine(Flasher());
    }

    private IEnumerator Flasher()
    {
        SetFlash(true);

        float currentFlashAmount = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < _flashTime)
        {
            elapsedTime += Time.deltaTime;

            currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / _flashTime));
            SetFlashAmount(currentFlashAmount);
            yield return null;
        }

        SetFlash(false);
    }

    private void SetFlash(bool value)
    {
        foreach (var renderer in _spriteRenderers)
        {
            renderer.enabled = value;
        }
    }

    private void SetFlashAmount(float amount)
    {
        foreach (var material in _materials)
        {
            material.SetFloat("_FlashAmount", amount);
        }
    }
}
