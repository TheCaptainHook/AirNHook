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
    public ActivatableObject_Net_Entity net;

    public void Setting(ActivatableObjectEntity entity,ActivatableObject_Net_Entity net)
    {
        this.entity = entity;
        this.net = net;

        parent = entity.gameObject.transform;
        var offset = parent.rotation * (parent.localScale * entity.indicatorOffset_val_2);

        transform.position = parent.position + offset;
        transform.rotation = parent.rotation;

        container.localScale = parent.localScale;

        var termTr = MapEditor.Instance.dontSaveObjectTransform;

        transform.SetParent(termTr);
        gameObject.SetActive(false);

        activeRequirAmount = net.data.activeRequirAmount;

        itemDic = new();
    }

    //inc 1 -> active, inc -1 -> deactive
    Coroutine conditionCheckCo;
    public void SetApplyActive(uint id, int curActiveBtn, int inc)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        if (itemDic.ContainsKey(id))
        {
            if(inc ==1)
            {
                if(curActiveBtn > activeRequirAmount)
                {
                    itemDic[id].SetActive(3);
                }else
                {
                    itemDic[id].SetActive(inc);
                }
                  
            } else
            {

                itemDic[id].SetActive(inc);
            }
               
        }
        else
        {
            if (curActiveBtn > activeRequirAmount)
            {
                CreateNewItem(id, 3);
            }
            else
            {
                CreateNewItem(id);
            }
               
        }

        curActiveRequirAmount += inc;

        if (conditionCheckCo != null) StopCoroutine(conditionCheckCo);
        conditionCheckCo = StartCoroutine(ConditionCheckCo(id, inc));


    }
     
    // private Coroutine conditionCheckCoroutine;

    IEnumerator ConditionCheckCo(uint id, int inc) //Server
    {
        if (inc != 1)
        {  
            if (curActiveRequirAmount != activeRequirAmount)
            {
                //---------Stop SatisfyEffectCo Recover RPC
                if (satisfiedCoroutine != null)
                {
                    StopCoroutine(satisfiedCoroutine);
                    satisfiedCoroutine = null;
                }
                SatisfyEffectRecover();
                //---------Stop SatisfyEffectCo Recover

                if (NetworkServer.active)
                    entity.Deactivated();
            }
            else
            {
                //---------Start SatisfyEffectCo RPC
                if (satisfiedCoroutine != null) StopCoroutine(satisfiedCoroutine);
                satisfiedCoroutine = StartCoroutine(SatisfyEffectCo());
                //---------Start SatisfyEffectCo
                if (NetworkServer.active)
                    entity.Activation();
            }

            yield break;
        }

        var item = itemDic[id];
        yield return new WaitUntil(() => !item.onPrograss);


        if (curActiveRequirAmount == activeRequirAmount)
        {
            //---------Start SatisfyEffectCo RPC
            if (satisfiedCoroutine != null) StopCoroutine(satisfiedCoroutine);
            satisfiedCoroutine = StartCoroutine(SatisfyEffectCo());
            //---------Start SatisfyEffectCo
            if (NetworkServer.active)
                entity.Activation();
        }
        else
        {
            //---------Stop SatisfyEffectCo Recover RPC
            if (satisfiedCoroutine != null)
            {
                StopCoroutine(satisfiedCoroutine);
                satisfiedCoroutine = null;
            }

            SatisfyEffectRecover();
            //---------Stop SatisfyEffectCo Recover
            if (NetworkServer.active)
                entity.Deactivated();
        }

    }

    #region Satisfy Condition [Server]
    private float satisfiedEffectWaitDelaySec = 1;
    private Coroutine satisfiedCoroutine;

    // private void Rpc_SatisfyEffect()
    // {

    // }

    private IEnumerator SatisfyEffectCo()
    {
        float percent = 0;
        while (percent < satisfiedEffectWaitDelaySec)
        {
            percent += Time.deltaTime;
            yield return null;
        }

        //Rpc Fade Out Line
        foreach(var item in itemDic.Values)
        {
            if(item.onDraw)
            {
                item.SatisfyCondition_FadeOutLine();
            }
        }
        //Rpc Fade Out Line

        satisfiedCoroutine = null;
    }

    public void SatisfyEffectRecover()
    {
        foreach (var item in itemDic.Values)
        {
            item.FadeRecover();
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
