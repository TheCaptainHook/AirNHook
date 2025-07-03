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


    // private ActivatableObjectEntity entity;
    // private ActivatableObject_Net_Entity net;

    [ReadOnly]
    public int activeRequirAmount;
    public void Setting(ActivatableObjectEntity entity,ActivatableObject_Net_Entity net)
    {
        // this.entity = entity;
        // this.net = net;

        parent = entity.transform;

        var offset = parent.rotation  * (parent.localScale * entity.indicatorOffset_val_1);
        transform.position = parent.position + offset;

        bgImg.transform.localScale = parent.localScale;

        transform.SetParent(parent);

        activeRequirAmount = net.data.activeRequirAmount;
        text.text = $"{0}/{activeRequirAmount}";
    }
     public void Setting(TransportItemEntity entity)
    {
        // this.entity = entity;
        // this.net = net;

        parent = entity.transform;

        transform.position = parent.position + new Vector3(0, 1.5f);

        bgImg.transform.localScale = parent.localScale;

        transform.SetParent(parent);

        activeRequirAmount = entity.data.activeRequireAmount;
        text.text = $"{0}/{activeRequirAmount}";
    }
  
    public void SetApplyActive(int curActiveAmount)
    {
        if (curActiveAmount == activeRequirAmount) //satisfy condition
        {

        }

        //if (curActiveAmount == 0)
        //{
        //    gameObject.SetActive(false);
        //    return;
        //}

        //if (!gameObject.activeSelf) gameObject.SetActive(true);
        text.text = $"{curActiveAmount}/{activeRequirAmount}";


    }



}
