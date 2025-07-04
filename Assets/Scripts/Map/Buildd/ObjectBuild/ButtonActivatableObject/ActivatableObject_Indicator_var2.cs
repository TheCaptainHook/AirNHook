using System.Collections;
using System.Collections.Generic;
using ANH_MapEditor;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class ActivatableObject_Indicator_var2 : MonoBehaviour
{
    //--------------------------------------------------------------------------------Renewal 0704
    public Stack<ActivatableObject_Indicator_var2_Item> itemWaitStack;
    private List<Indicator_var2_DrawLineUtility> itemCurActiveList;
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

    }

    //--------------------------------------------------------------------------------Renewal 0704


    public void SetApplyActive(uint id, int curActiveBtn, int inc)
    {
        Transform target = NetworkClient.spawned.TryGetValue(id, out var targetObject) ? targetObject.transform : null;
        if (target == null) return;

        if (inc == 1)
        {
            if (itemWaitStack.Count == 0)
            {

                return;
            }

            var lineUtility = GetLine();
            var item = itemWaitStack.Pop();
            //itemCurActiveList.Add((id, item, data));
            itemCurActiveList.Add(lineUtility);
            lineUtility.SettingAndDrawLine(id, item, target, ()=> SetApplyActive(1));
            
        }
        else
        {
            for (int i = 0; i < itemCurActiveList.Count; i++)
            {
                if (itemCurActiveList[i].id == id)
                {
                    var curActiveData = itemCurActiveList[i];
                    itemCurActiveList.RemoveAt(i);

                    curActiveData.Erase(this,()=>SetApplyActive(-1));
                    return;
                }
            }
        }


        
    }

    public void SetApplyActive(int inc)
    {
        if (inc == 1) curActiveRequirAmount++;
        else curActiveRequirAmount--;

        if (curActiveRequirAmount < 0) curActiveRequirAmount = 0;

        if (curActiveRequirAmount == activeRequirAmount)
        {
            foreach(var item in itemCurActiveList)
            {
                item.Fade(true);
            }


            if (NetworkServer.active)
                entity.Activation();
          
        }
        else
        {       
            foreach (var item in itemCurActiveList)
            {
                item.Fade(false);
            }

            if (NetworkServer.active)
                entity.Deactivated();
        }
    }
    //--------------------------------------------------------------------------------Renewal 0704


    private Vector2 curItem_Space;
    private float item_Space = 0.3f;

    private ActivatableObject_Indicator_var2_Item CreateItem()
    {
        var item = Instantiate(activatableObject_Indicator_var2_Item_Prefab, container).GetComponent<ActivatableObject_Indicator_var2_Item>();;
        item.transform.localPosition = curItem_Space;
        curItem_Space += -(Vector2)container.up * item_Space;

        //item.indicator_Var2 = this;
        return item;
    }


    #region  Line
    public Queue<Indicator_var2_DrawLineUtility> lineQueue;

    private Indicator_var2_DrawLineUtility GetLine()
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

    private Indicator_var2_DrawLineUtility CreateNewLine()
    {
        var line = new GameObject("Line").AddComponent<LineRenderer>();
        var path = line.AddComponent<PathFinder>();
        path.obstacleLayer = 1 << 6;
        line.positionCount = 0;
        line.transform.SetParent(linePoolingContainer);
        line.startWidth = 0.1f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.red;
        line.endColor = Color.red;
        line.sortingLayerName = "Map/Tiles";
        line.sortingOrder = 3;


        return line.AddComponent<Indicator_var2_DrawLineUtility>();
    }
    #endregion

}
