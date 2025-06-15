using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivatableObject_Indicator_var1 : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image bgImg;

    private Transform parent;
    private Quaternion initialWorldRotation;
    void Awake()
    {
        initialWorldRotation = transform.rotation;
    }
    void LateUpdate()
    {
        transform.rotation = initialWorldRotation;
    }

    [ReadOnly]
    public int activeRequirAmount;
    public void Setting(ActivatableObjectEntity entity)
    {
        parent = entity.transform;

        var offset = parent.rotation  * (parent.localScale * entity.indicatorOffset_val_1);
        transform.position = parent.position + offset;

        bgImg.transform.localScale = parent.localScale;

        transform.SetParent(parent);
        gameObject.SetActive(false);

        activeRequirAmount = entity.ButtonActivatedObjectStruct.activeRequirAmount;
    }
  
    public void SetApplyActive(int curActiveAmount)
    {
        var num = activeRequirAmount - curActiveAmount;

        if (num == 0) //satisfy condition
        {
            //Disappear  Coroutine
            gameObject.SetActive(false);
            //Disappear  Coroutine
            return;
        }

        if (num == activeRequirAmount) //reset
        {
            gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeSelf) gameObject.SetActive(true);
        text.text = num.ToString();


    }



}
