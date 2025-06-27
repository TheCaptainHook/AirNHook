using System.Collections;
using System.Collections.Generic;
using Mirror;
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
    [ReadOnly]
    public int curActiveRequirAmount;

    [ReadOnly]
    public ActivatableObjectEntity entity;

    public void Setting(ActivatableObjectEntity entity)
    {
        this.entity = entity;
        parent = entity.gameObject.transform;
        var offset = parent.rotation * (parent.localScale * entity.indicatorOffset_val_2);

        transform.position = parent.position + offset;
        transform.rotation = parent.rotation;

        container.localScale = parent.localScale;

        var termTr = MapEditor.Instance.dontSaveObjectTransform;

        transform.SetParent(termTr);
        gameObject.SetActive(false);

        activeRequirAmount = entity.ButtonActivatedObjectStruct.activeRequirAmount;

        itemDic = new();
    }


    private bool isConditionSatisfied = false;

    //inc 1 -> active, inc -1 -> deactive
    public void SetApplyActive(uint id, int curActiveBtn, int inc)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        // if (curActiveBtn == activeRequirAmount) //조건 충족
        // {
        //     isConditionSatisfied = true;

        //     if (!itemDic.ContainsKey(id)) //아이디 없을때
        //     {
        //         CreateNewItem(id, 2); //조건 충족용 라인 제거
        //         foreach (var item in itemDic.Values)
        //         {
        //             if (item.targetId == id) continue;
        //             if (item.onActive) item.SetActive(2);
        //         }
        //     }
        //     else //아이디 존재
        //     {
        //         var curItem = itemDic[id];
        //         if (inc == 1)
        //         {
        //             curItem.onActive = true;
        //         }
        //         if (inc == -1)
        //         {
        //             curItem.SetActive(inc);
        //         }

        //         foreach (var item in itemDic.Values)
        //         {
        //             if (item.targetId == id) continue;
        //             if (item.onActive) item.SetActive(2);
        //         }
        //     }

        //     return;
        // }
        // //After the condition is satisfied, when it is deactivated or reactivated
        // if (isConditionSatisfied)
        // {
        //     if (itemDic.ContainsKey(id))
        //     {
        //         var curItem = itemDic[id];
        //         curItem.SetActive(inc);

        //         foreach (var item in itemDic.Values)
        //         {
        //             if (item == curItem) continue;
        //             if (item.onActive) item.SetActive(3);
        //         }
        //     }
        //     else
        //     {
        //         var curItem = CreateNewItem(id);

        //         foreach (var item in itemDic.Values)
        //         {
        //             if (item == curItem) continue;
        //             if (item.onActive && !item.onDraw) item.SetActive(3);
        //         }

        //     }
        //     isConditionSatisfied = false;
        //     return;
        // }

        if (itemDic.ContainsKey(id))
        {
            itemDic[id].SetActive(inc);
        }
        else
        {
            CreateNewItem(id);
        }

        if (NetworkServer.active)
            StartCoroutine(ConditionCheckCo(id, inc));
    }

    // private Coroutine conditionCheckCoroutine;

    IEnumerator ConditionCheckCo(uint id, int inc) //Server
    {
        if (inc != 1)
        {
            curActiveRequirAmount += inc;

            if (curActiveRequirAmount != activeRequirAmount) entity.Deactivated();
            else entity.Activation();

            yield break;
        }

        var item = itemDic[id];
        yield return new WaitUntil(() => !item.onPrograss);
        curActiveRequirAmount += inc;

        //---------------------------------------------RPC, Satisfy Effect Rpc
        if (curActiveRequirAmount == activeRequirAmount)
        {
            entity.Activation();
        }
        else entity.Deactivated();
        //---------------------------------------------RPC, Satisfy Effect Rpc

    }


    #region Satisfy Condition [Server]
    private float satisfiedEffectWaitDelaySec = 3;
    private Coroutine satisfiedCoroutine;

    [ClientRpc]
    private void Rpc_SatisfyEffect()
    {
        
    }

    private IEnumerator SatisfyEffectCo()
    {
        float percent = 0;
        while (percent < satisfiedEffectWaitDelaySec)
        {
            percent += Time.deltaTime;
            yield return null;
        }

    }

    #endregion

    private ActivatableObject_Indicator_var2_Item CreateNewItem(uint id, int inc = 1)
    {
        var item = CreateItem();
        itemDic[id] = item;
        item.Setting(id, inc);
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
