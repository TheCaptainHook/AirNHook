using System.Collections;
using System.Collections.Generic;
using ANH_MapEditor;
using Mirror;
using UnityEngine;

public class ActivatableObject_Indicator_var2 : MonoBehaviour
{
    // private Dictionary<uint, ActivatableObject_Indicator_var2_Item> itemDic;
    //--------------------------------------------------------------------------------Renewal 0704
    public Stack<ActivatableObject_Indicator_var2_Item> itemWaitStack;
    private List<(uint id, ActivatableObject_Indicator_var2_Item item,Indicator_2_DrawLineStruct data)> itemCurActiveList;
    [ReadOnly]
    public int curItemListIndex;

    [SerializeField] Transform linePoolingContainer;
    //--------------------------------------------------------------------------------Renewal 0704
    private Transform mainTr;

    [Header("Prefab")]
    [SerializeField] GameObject activatableObject_Indicator_var2_Item_Prefab;

    [SerializeField] Transform container;
    [ReadOnly]
    public int activeRequirAmount;
    [ReadOnly]
    public int curActiveRequirAmount;

    [ReadOnly]
    public ActivatableObjectEntity entity;
    [ReadOnly]
    public ActivatableObject_Net_Entity net;

    public void Setting(ActivatableObjectEntity entity, ActivatableObject_Net_Entity net)
    {
        this.entity = entity;
        this.net = net;
        mainTr = entity.gameObject.transform;

        var offset = mainTr.rotation * (mainTr.localScale * entity.indicatorOffset_val_2);

        transform.position = mainTr.position + offset;
        transform.rotation = mainTr.rotation;

        container.localScale = mainTr.localScale;
        container.rotation = transform.rotation;

        var termTr = MapEditor.Instance.dontSaveObjectTransform;

        transform.SetParent(termTr);
        // gameObject.SetActive(false);

        activeRequirAmount = net.data.activeRequirAmount;

        itemWaitStack = new();
        itemCurActiveList = new();
        //--------------------------------------------------------------------------------Renewal 0704
        for (int i = activeRequirAmount - 1; i >= 0; i--)
        {
            itemWaitStack.Push(CreateItem());
        }
        //--------------------------------------------------------------------------------Renewal 0704

        // itemDic = new();
    }

    //--------------------------------------------------------------------------------Renewal 0704


    public void SetApplyActive(uint id, int curActiveBtn, int inc)
    {
        Transform target = NetworkClient.spawned.TryGetValue(id, out var targetObject) ? targetObject.transform : null;
        if (target == null) return;

        if (inc == 1)
        {
            if (itemWaitStack.Count == 0) return;
            var data = new Indicator_2_DrawLineStruct(target, GetLine());
            var item = itemWaitStack.Pop();
            itemCurActiveList.Add((id, item, data));
            item.SettingAndDraw(data);
        }
        else
        {
            for (int i = 0; i < itemCurActiveList.Count; i++)
            {
                if (itemCurActiveList[i].id == id)
                {
                    var curActiveData = itemCurActiveList[i];
                    itemCurActiveList.RemoveAt(i);

                    curActiveData.item.Erase(curActiveData);
                    return;
                }
            }
        }
    }

    public void SetApplyActive(int inc)
    {
        if (inc == 1) curActiveRequirAmount++;
        else curActiveRequirAmount--;

        if (curActiveRequirAmount == activeRequirAmount)
        {
            if (NetworkServer.active)
                entity.Activation();
            else entity.Deactivated();
        }
    }
    //--------------------------------------------------------------------------------Renewal 0704

    //inc 1 -> active, inc -1 -> deactive
    // Coroutine conditionCheckCo;
    // public void SetApplyActive(uint id, int curActiveBtn, int inc)
    // {
    //     if (!gameObject.activeSelf) gameObject.SetActive(true);

    //     if (itemDic.ContainsKey(id))
    //     {
    //         if(inc ==1)
    //         {
    //             if(curActiveBtn > activeRequirAmount)
    //             {
    //                 itemDic[id].SetActive(3);
    //             }else
    //             {
    //                 itemDic[id].SetActive(inc);
    //             }

    //         } else
    //         {

    //             itemDic[id].SetActive(inc);
    //         }

    //     }
    //     else
    //     {
    //         if (curActiveBtn > activeRequirAmount)
    //         {
    //             CreateNewItem(id, 3);
    //         }
    //         else
    //         {
    //             CreateNewItem(id);
    //         }

    //     }

    //     curActiveRequirAmount += inc;

    //     if (conditionCheckCo != null)
    //     {
    //         StopCoroutine(conditionCheckCo);
    //         conditionCheckCo = null;
    //     }

    //     conditionCheckCo = StartCoroutine(ConditionCheckCo(id, inc));


    // }

    // private Coroutine conditionCheckCoroutine;

    // IEnumerator ConditionCheckCo(uint id, int inc) //Server
    // {
    //     if (inc != 1)
    //     {
    //         if (curActiveRequirAmount != activeRequirAmount)
    //         {
    //             //---------Stop SatisfyEffectCo Recover
    //             if (satisfiedCoroutine != null)
    //             {
    //                 StopCoroutine(satisfiedCoroutine);
    //                 satisfiedCoroutine = null;
    //             }
    //             SatisfyEffectRecover();
    //             //---------Stop SatisfyEffectCo Recover

    //             if (NetworkServer.active)
    //                 entity.Deactivated();
    //         }
    //         else
    //         {
    //             //---------Start SatisfyEffectCo
    //             if (satisfiedCoroutine != null) StopCoroutine(satisfiedCoroutine);
    //             satisfiedCoroutine = StartCoroutine(SatisfyEffectCo());
    //             //---------Start SatisfyEffectCo
    //             if (NetworkServer.active)
    //                 entity.Activation();
    //         }

    //         conditionCheckCo = null;
    //         yield break;
    //     }

    //     var item = itemDic[id];
    //     yield return new WaitUntil(() => !item.onPrograss);


    //     if (curActiveRequirAmount == activeRequirAmount)
    //     {
    //         //---------Start SatisfyEffectCo RPC
    //         if (satisfiedCoroutine != null) StopCoroutine(satisfiedCoroutine);
    //         satisfiedCoroutine = StartCoroutine(SatisfyEffectCo());
    //         //---------Start SatisfyEffectCo
    //         if (NetworkServer.active)
    //             entity.Activation();
    //     }
    //     else
    //     {
    //         //---------Stop SatisfyEffectCo Recover RPC
    //         if (satisfiedCoroutine != null)
    //         {
    //             StopCoroutine(satisfiedCoroutine);
    //             satisfiedCoroutine = null;
    //         }

    //         SatisfyEffectRecover();
    //         //---------Stop SatisfyEffectCo Recover
    //         if (NetworkServer.active)
    //             entity.Deactivated();
    //     }

    // }

    #region Satisfy Condition [Server]
    private float satisfiedEffectWaitDelaySec = 1;
    private Coroutine satisfiedCoroutine;

    // private void Rpc_SatisfyEffect()
    // {

    // }

    // private IEnumerator SatisfyEffectCo()
    // {
    //     float percent = 0;
    //     while (percent < satisfiedEffectWaitDelaySec)
    //     {
    //         percent += Time.deltaTime;
    //         yield return null;
    //     }

    //     //Rpc Fade Out Line
    //     foreach (var item in itemDic.Values)
    //     {
    //         if (item.onDraw)
    //         {
    //             item.SatisfyCondition_FadeOutLine();
    //         }
    //     }
    //     //Rpc Fade Out Line

    //     satisfiedCoroutine = null;
    // }

    // public void SatisfyEffectRecover()
    // {
    //     foreach (var item in itemDic.Values)
    //     {
    //         item.FadeRecover();
    //     }

    // }
    #endregion

    // private ActivatableObject_Indicator_var2_Item CreateNewItem(uint id, int inc = 1)
    // {
    //     var item = CreateItem();
    //     itemDic[id] = item;
    //     item.Setting(id, inc);
    //     return item;
    // }
    // private ActivatableObject_Indicator_var2_Item CreateNewItem()
    // {
    //     var item = CreateItem();
    //     return item;
    // }

    private Vector2 curItem_Space;
    private float item_Space = 0.3f;

    private ActivatableObject_Indicator_var2_Item CreateItem()
    {
        var item = Instantiate(activatableObject_Indicator_var2_Item_Prefab, container).GetComponent<ActivatableObject_Indicator_var2_Item>();;
        item.transform.localPosition = curItem_Space;
        curItem_Space += -(Vector2)container.up * item_Space;

        item.indicator_Var2 = this;
        return item;
    }


    #region  Line
    public Queue<LineRenderer> lineQueue;

    private LineRenderer GetLine()
    {
        if (lineQueue == null) lineQueue = new();

        if (lineQueue.Count > 0)
        {
            return lineQueue.Dequeue();
        }
        else
        {
            return CreateNewLine();
        }
    }
    private LineRenderer CreateNewLine()
    {
        var line = new GameObject("Line").AddComponent<LineRenderer>();;
        line.positionCount = 0;
        line.transform.SetParent(linePoolingContainer);
        line.startWidth = 0.1f;

        line.startColor = Color.red;
        line.endColor = Color.red;
        line.sortingLayerName = "Map/Tiles";
        line.sortingOrder = 3;

        return line;
    }
    #endregion

}
