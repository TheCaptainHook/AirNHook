using Mirror;
using System.Collections;
using UnityEngine;

public class EraseField_Character_Net : ActivatableObject_Net_Entity
{
    [SerializeField] GameObject base_2_Field;
    // [SerializeField] GameObject main_Field;
    // [SerializeField] GameObject main_Light;

    [SerializeField] SpriteRenderer main_Field_Sprite;

    private Color color = Color.red;
    private Color nonCol = new Color(1, 0, 0, 0);

    protected override void Active()
    {
        Active(false);
    }
    protected override void Deactive()
    {
        Active(true);
    }
    
    [Server]
    public override void Server_PlayUniqueEffect(uint id)
    {
        var item = NetworkClient.spawned.TryGetValue(id, out var identity) ? identity : null;
        if (item != null)
        {
            TRpc_PlayUniqueEffect(identity.connectionToClient, item.gameObject);
        }
    }
    [TargetRpc]
    private void TRpc_PlayUniqueEffect(NetworkConnection con,GameObject obj)
    {
        if (obj.TryGetComponent(out IDamageable component)) component.TakeDamage(DamageType.Fire);
    }

    private void Active(bool onOff)
    {
        SetEffect(onOff);
        onActive = onOff;
        if (effectCoroutine != null) StopCoroutine(effectCoroutine);
        effectCoroutine = StartCoroutine(EffectCo(onOff));

        Col.enabled = onOff;
    }

    Coroutine effectCoroutine;
    float percent = 0;
    IEnumerator EffectCo(bool onOff)
    {
        while (percent < 1)
        {
            percent += Time.fixedDeltaTime;

            if (onOff) main_Field_Sprite.color = Color.Lerp(nonCol, color, percent);
            else main_Field_Sprite.color = Color.Lerp(color, nonCol, percent);

            yield return null;
        }
        percent = 0;

        if (onOff) main_Field_Sprite.color = color;
        else main_Field_Sprite.color = nonCol;
    }

    private void SetEffect(bool onOff)
    {
        base_2_Field.SetActive(onOff);
        //main_Field.SetActive(onOff);
        // main_Light.SetActive(onOff);
    }
}
