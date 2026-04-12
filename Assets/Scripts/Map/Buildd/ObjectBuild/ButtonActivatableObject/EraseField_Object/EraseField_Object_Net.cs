using Mirror;
using System.Collections;
using UnityEngine;

public class EraseField_Object_Net : ActivatableObject_Net_Entity
{
    [SerializeField] GameObject base_2_Field;
    // [SerializeField] GameObject main_Field;
    // [SerializeField] GameObject main_Light;

    [SerializeField] SpriteRenderer main_Field_Sprite;

    private Color color = new Color(71 / 255f, 239 / 255f, 1, 1);
    private Color nonCol = new Color(71 / 255f, 239 / 255f, 1, 0);

    protected override void Active()
    {
        Active(false);
    }
    protected override void Deactive()
    {
        Active(true);
    }

    [ClientRpc]
    protected override void Rpc_ChangeOnActive(bool onOff)
    {
        if (onOff) Active();
        else Deactive();
        
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
        // main_Light.SetActive(onOff);
    }
}
