using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableObject_Indicator_var2 : MonoBehaviour
{
    private Dictionary<uint, ActivatableObject_Indicator_var2_Item> itemDic;
    private Transform parent;

    [Header("Prefab")]
    [SerializeField] GameObject activatableObject_Indicator_var2_Item_Prefab;

    [SerializeField] Transform container;
    [ReadOnly]
    public int activeRequirAmount;
    public void Setting(ActivatableObjectEntity entity)
    {
        parent = entity.gameObject.transform;
        var offset = parent.rotation * (parent.localScale * entity.indicatorOffset_val_2);
        transform.position = parent.position + offset;
        transform.rotation = parent.rotation;

        container.localScale = parent.localScale;

        transform.SetParent(parent);
        gameObject.SetActive(false);

        activeRequirAmount = entity.ButtonActivatedObjectStruct.activeRequirAmount;

        itemDic = new();

    }

    public void SetApplyActive(uint id,int curActiveAmount,int inc)
    {
    

        if (!gameObject.activeSelf) gameObject.SetActive(true);

        if (itemDic.ContainsKey(id))
        {
            itemDic[id].SetActive(inc);
            
        }
        else
        {
            //딕셔너리 추가, 경로 생성 저장
            var item = CreateItem();
            itemDic[id] = item;
            item.Setting(id);
        }
    }

    private float curItem_Space = 0;
    private float item_Space = 0.3f;

    private ActivatableObject_Indicator_var2_Item CreateItem()
    {
        var item = Instantiate(activatableObject_Indicator_var2_Item_Prefab,container);
        item.transform.localPosition = new Vector3(0,-curItem_Space,0);
        curItem_Space += item_Space;

        return item.GetComponent<ActivatableObject_Indicator_var2_Item>();
    }
 
}
