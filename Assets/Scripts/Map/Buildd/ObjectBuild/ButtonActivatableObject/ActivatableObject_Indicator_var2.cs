using System.Collections;
using System.Collections.Generic;
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

    #region Default
    public void Setting(ActivatableObjectEntity entity, ActivatableObject_Net_Entity net)
    {
        this.entity = entity;
        this.net = net;
        mainTr = entity.gameObject.transform;

        // var offset = mainTr.rotation * (mainTr.localScale * entity.indicatorOffset_val_2);
        transform.position = net.data.indicatorStruct.indicator_2_position;
        transform.rotation = mainTr.rotation;

        container.localScale = mainTr.localScale;
        container.rotation = transform.rotation;

        var termTr = MapEditor.Instance.dontSaveObjectTransform;

        transform.SetParent(termTr);
        // gameObject.SetActive(false);

        itemWaitStack = new();
        itemCurActiveList = new();

        activeRequirAmount = net.data.activeRequirAmount;

        CreateItemAndSorting(net.data);

    }
    #endregion

    #region  Path Chacking
    public bool notObstacle = false;
    public void PathChacking(uint targetID) //Rpc
    {
        StartCoroutine(Encapsulation_WaitItemReadyCo(targetID));
    }
    private IEnumerator Encapsulation_WaitItemReadyCo(uint targetID)
    {
        yield return new WaitUntil(() => _isEncapsulation_ItemReady);

        if (itemWaitStack.Count == 0) yield break;

        var item = itemWaitStack.Pop();
        var lineUtility = GetLine();
        lineUtility.PathChacking(this, targetID, item);
    }
   
    #endregion


    #region  Encapsulation Field
    public EncapsulationField encapsulationField;
    public TransportItemEntity transportItemEntity;
    private Vector2 encapsulationOffset = new Vector2(0, -1f);
    public void Setting(EncapsulationField encapsulationField, TransportItemEntity transportItemEntity)
    {
        this.encapsulationField = encapsulationField;
        this.transportItemEntity = transportItemEntity;
        mainTr = transportItemEntity.gameObject.transform;

        transform.position = transportItemEntity.data.position + encapsulationOffset;

        var termTr = MapEditor.Instance.dontSaveObjectTransform;
        transform.SetParent(termTr);

        itemWaitStack = new();
        itemCurActiveList = new();

        activeRequirAmount = transportItemEntity.data.activeRequireAmount;

        CreateItemAndSorting(transportItemEntity.data);
       
    }
   
   
    private float FloorTo2DecimalPlaces(float num)
    {
        return Mathf.Floor(num * 100) / 100f;
    }
    public void SetApplyActive_EncapsulationField(int inc, uint id)
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
            lineUtility.SettingAndDrawLine(id, item, target, () => SetApplyActive_EncapsultationField(1),notObstacle);

        }
        else
        {
            for (int i = 0; i < itemCurActiveList.Count; i++)
            {
                if (itemCurActiveList[i].id == id)
                {
                    var curActiveData = itemCurActiveList[i];
                    itemCurActiveList.RemoveAt(i);

                    curActiveData.Erase(this, () => SetApplyActive_EncapsultationField(-1));
                    return;
                }
            }
        }
    }
    private void SetApplyActive_EncapsultationField(int inc)
    {
        if (inc == 1) curActiveRequirAmount++;
        else curActiveRequirAmount--;

        if (curActiveRequirAmount < 0) curActiveRequirAmount = 0;

        if (curActiveRequirAmount == activeRequirAmount)
        {
            foreach (var item in itemCurActiveList)
            {
                item.Fade(true);
            }

            if (!encapsulationField.isCapsuling) return;


            if (NetworkServer.active)
                transportItemEntity.Rpc_UnCapsuling();

        }
        else
        {
            foreach (var item in itemCurActiveList)
            {
                item.Fade(false);
            }
            
        }

    }
    #endregion
    //Encapsulation Field
    private bool _isEncapsulation_ItemReady;
    private void CreateItemAndSorting(ObjectData data)
    {
        float totalLength = item_Space * (data.activeRequireAmount - 1); // 총 길이
        Vector3 startPot = transform.position - new Vector3(FloorTo2DecimalPlaces(totalLength / 2f), 0, 0);
        for (int i = activeRequirAmount - 1; i >= 0; i--)
        {
            var item = CreateItem();
            itemWaitStack.Push(item);
            item.transform.position = startPot + new Vector3(item_Space * i, 0, 0);
        }

        _isEncapsulation_ItemReady = true;
    }

    //Default
    private float item_Space = .5f;
    private void CreateItemAndSorting(ButtonActivatableObjectStruct data)
    {
        float totalLength = item_Space * (data.activeRequirAmount - 1); // 총 길이
        Vector3 startPos = data.indicatorStruct.isHorizontal ?
            transform.position - new Vector3(totalLength / 2f, 0, 0) :
            transform.position - new Vector3(0, totalLength / 2f, 0);

        for (int i = activeRequirAmount - 1; i >= 0; i--)
        {
            var item = CreateItem();
            itemWaitStack.Push(item);
            Vector3 offset = data.indicatorStruct.isHorizontal ? new Vector3(item_Space * i, 0, 0) : new Vector3(0, item_Space * i, 0);
            item.transform.position = startPos + transform.rotation * offset;
        }
            
        _isEncapsulation_ItemReady = true;
    
    }
   
    //--------------------------------------------------------------------------------Renewal 0704


    public void SetApplyActive(uint id, int curActiveBtn, int inc) //all client
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
            lineUtility.SettingAndDrawLine(id, item, target, () => SetApplyActive(1),notObstacle);

        }
        else
        {
            for (int i = 0; i < itemCurActiveList.Count; i++)
            {
                if (itemCurActiveList[i].id == id)
                {
                    var curActiveData = itemCurActiveList[i];
                    itemCurActiveList.RemoveAt(i);

                    curActiveData.Erase(this, () => SetApplyActive(-1));
                    return;
                }
            }
        }



    }

    public void SetApplyActive(int inc) //Check curActiveRequirAmount
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


    // private Vector2 curItem_Space;
    // private float item_Space = 0.3f;

    private ActivatableObject_Indicator_var2_Item CreateItem()
    {
        var item = Instantiate(activatableObject_Indicator_var2_Item_Prefab, container).GetComponent<ActivatableObject_Indicator_var2_Item>();;
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

        //Line Visual
        line.startWidth = 0.1f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.green;
        line.endColor = Color.green;
        //Line Visual

        line.sortingLayerName = "Map/Tiles";
        line.sortingOrder = 3;


        return line.AddComponent<Indicator_var2_DrawLineUtility>();
    }
    #endregion

}
