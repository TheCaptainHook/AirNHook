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
    public void Setting(ActivatableObjectEntity entity, ActivatableObject_Net_Entity net)
    {
        parent = entity.transform;
        // transform.position = parent.localScale * net.data.indicatorStruct.indicator_1_position;
        transform.position = net.data.indicatorStruct.indicator_1_position;
        
        transform.localScale = Vector2.one;
        // bgImg.transform.localScale = Vector2.one;

        var termTr = MapEditor.Instance.dontSaveObjectTransform;

        transform.SetParent(termTr);

        activeRequirAmount = net.data.activeRequirAmount;
        text.text = $"{0}/{activeRequirAmount}";
    }

    #region  Specific Encapsulation Field 
    private TransportItemEntity transportItemEntity;
    public void Setting(TransportItemEntity entity)
    {
        parent = entity.transform;

        transform.position = parent.position + new Vector3(0, 1.5f);

        // bgImg.transform.localScale = parent.localScale;
        transform.localScale = Vector2.one;
        // bgImg.transform.localScale = Vector2.one;

        transform.SetParent(parent);

        activeRequirAmount = entity.data.activeRequireAmount;
        text.text = $"{0}/{activeRequirAmount}";
    }
    public void SetApplyActive_EncapsulationField(int curActiveAmount)
    {
        if (!transportItemEntity.EncapsulationField.isCapsuling) return;
        
        if (curActiveAmount == activeRequirAmount) //satisfy condition
        {
            gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeSelf) gameObject.SetActive(true);
        text.text = $"{curActiveAmount}/{activeRequirAmount}";

    }

    #endregion

    public void SetApplyActive(int curActiveAmount)
    {
        if (curActiveAmount == activeRequirAmount) //satisfy condition
        {

        }

        text.text = $"{curActiveAmount}/{activeRequirAmount}";


    }

    #region  Clean
    public void Clean()
    {
        
    }
#endregion


}
