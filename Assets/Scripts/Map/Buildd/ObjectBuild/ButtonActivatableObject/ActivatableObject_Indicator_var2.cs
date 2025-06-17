using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private bool isConditionSatisfied = false;
    public void SetApplyActive(uint id, int curActiveBtn, int inc)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        //Condition Satisfied
        if (curActiveBtn == activeRequirAmount) //ex 1
        { 
            isConditionSatisfied = true;

            if (!itemDic.ContainsKey(id))
            {
                CreateNewItem(id, 2);
                foreach (var item in itemDic.Values)
                {
                    if(item.targetId == id) continue;
                    if(item.onActive)item.SetActive(2);
                }
            }
            else
            {
                // itemDic[id].SetActive(inc);
                var curItem = itemDic[id];
                if (inc == 1)
                {
                    curItem.onActive = true;
                }
                if (inc == -1)
                {
                    curItem.SetActive(inc);
                }

                foreach (var item in itemDic.Values)
                {
                    if (item.targetId == id) continue;
                    if (item.onActive) item.SetActive(2);
                }
            }
            
            return;
        }
        //Condition Satisfied

        
        //After the condition is satisfied, when it is deactivated or reactivated
        if (isConditionSatisfied)
        {
            //item iteral, find onActie == true, onDraw = false

            if (itemDic.ContainsKey(id))
            {
                var curItem = itemDic[id];
                curItem.SetActive(inc);

                foreach (var item in itemDic.Values)
                {
                    if (item == curItem) continue;
                    if (item.onActive) item.SetActive(3);
                }
            }
            else
            {
                var curItem = CreateNewItem(id);

                foreach (var item in itemDic.Values)
                {
                    if (item == curItem) continue;
                    if (item.onActive && !item.onDraw) item.SetActive(3);
                }
                
            }
            isConditionSatisfied = false;
            return;
        }
        //After the condition is satisfied, when it is deactivated or reactivated


        if (itemDic.ContainsKey(id))
        {
            itemDic[id].SetActive(inc);

        }
        else
        {
            //딕셔너리 추가, 경로 생성 저장
            // var item = CreateItem();
            // itemDic[id] = item;
            // item.Setting(id);
            CreateNewItem(id);
        }
    }

    private ActivatableObject_Indicator_var2_Item CreateNewItem(uint id,int inc = 1)
    {
        var item = CreateItem();
        itemDic[id] = item;
        item.Setting(id,inc);
        return item;
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
