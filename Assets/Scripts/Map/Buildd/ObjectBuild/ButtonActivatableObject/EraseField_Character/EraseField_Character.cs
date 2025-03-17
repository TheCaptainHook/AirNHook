using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraseField_Character : ActivatableObjectEntity
{
    [CustomHeader("EraseField Character")]
    [SerializeField] GameObject base_2_Field;
    [SerializeField] GameObject main_Field;
    [SerializeField] GameObject main_Light;

    [SerializeField] SpriteRenderer main_Field_Sprite;

    private Color color = Color.red;
    private Color nonCol = new Color(1, 0, 0, 0);

    private Collider2D Col;
    private void Awake()
    {
        Col = GetComponent<Collider2D>();
    }

    public override async void SetData<T>(T data)
    {
        try
        {
            if (typeof(T) == typeof(ButtonActivatableObjectStruct))
            {
                ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
                ButtonActivatedObjectStruct = objData;

            }
        }
        catch
        {
            Debug.Log($"ERROR,{typeof(T)}");
        }

        if (Application.isPlaying)
        {
            Util util = new Util();
            await util.Delay(() => { CheckActiveRequirAmount(); });
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out PlayerSM component))
        {
            component.TakeDamage();
        }
    }

    Coroutine effectCoroutine;
    float percent = 0;
    IEnumerator EffectCo(bool onOff)
    {  
        while(percent<1)
        {
            percent += Time.fixedDeltaTime;

            if(onOff) main_Field_Sprite.color = Color.Lerp(nonCol, color, percent);
            else main_Field_Sprite.color = Color.Lerp(color, nonCol, percent);

            yield return null;
        }
        percent = 0;

        if (onOff) main_Field_Sprite.color = color;
        else main_Field_Sprite.color = nonCol;
    }
    protected override void Activation()
    {
        SetEffect(true);
        if (effectCoroutine != null) StopCoroutine(effectCoroutine);
        effectCoroutine = StartCoroutine(EffectCo(true));

        Col.enabled = true;
    }
    protected override void Deactivated()
    {
        SetEffect(false);
        
        if (effectCoroutine != null) StopCoroutine(effectCoroutine);
        effectCoroutine = StartCoroutine(EffectCo(false));

        Col.enabled = false;
    }



    private void SetEffect(bool onOff)
    {
        base_2_Field.SetActive(onOff);
        //main_Field.SetActive(onOff);
        main_Light.SetActive(onOff);
    }

}
