

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mono.CecilX.Cil;

[CustomEditor(typeof(ActivatableObjectEntity),true)]
public class ActivatableObject_Indicator_Helper_Editor : Editor
{

    ActivatableObjectEntity entity;

    private void OnEnable()
    {
        entity = target as ActivatableObjectEntity;

        itemList = new();

        if(entity !=null && entity.gameObject.scene.IsValid())
        EditorApplication.update += EditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= EditorUpdate;

        if (entity != null)
        {
            indicator = INDICATOR.NONE;
            curActiveRequirAmount = 0;
            DestroyAll();
        }

        entity = null;
    }
    public bool isHorizontal;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(20);

        if(onIndicator)
        {
            indicator_1 = (ActivatableObject_Indicator_var1)EditorGUILayout.ObjectField("indicator_1", indicator_1, typeof(ActivatableObject_Indicator_var1), true);
            indicator_2 = (ActivatableObject_Indicator_var2)EditorGUILayout.ObjectField("indicator_2", indicator_2, typeof(ActivatableObject_Indicator_var2), true);
            if (indicator_2 != null)
            {
                isHorizontal = EditorGUILayout.Toggle("is HorizonTal", isHorizontal);
            }
            entity.indicatorOffset_val_1 = pre_indicator_1_pot;
            entity.indicatorOffset_val_2 = pre_indicator_2_pot;
        }
       
    }


    INDICATOR indicator = INDICATOR.NONE;
    int curActiveRequirAmount =0;
    bool onIndicator;
    private void EditorUpdate()
    {
        if (entity == null) return;

        if (entity.indicator != indicator)
        {
            indicator = entity.indicator;
            ClearIndicator(indicator);
        }


        if(onIndicator)
        {
            if (debugTrnasform == null) CreateDebugTransform();

            CreateIndicator(indicator);

            if (curActiveRequirAmount != entity.activeRequirAmount)
            {
                if (indicator_1 != null)
                {

                }

                if(indicator_2 != null)
                {
                    for (int i = 0; i < entity.activeRequirAmount; i++)
                    {
                        var item = CreateItem();
                    }
                    Indicator_2_Item_Sort();
                       
                }


                curActiveRequirAmount = entity.activeRequirAmount;
            }

            CheckIndicatorPosition();
        }
    }

    private void ClearIndicator(INDICATOR indicator)
    {
        switch(indicator)
        {
            case INDICATOR.NONE:
                if (debugTrnasform != null) Editor.DestroyImmediate(debugTrnasform.gameObject);
                curActiveRequirAmount = 0;
                itemList.Clear();
                onIndicator = false;
                break;
            case INDICATOR.TEXT:
                if (indicator_2 != null) Editor.DestroyImmediate(indicator_2.gameObject);
                itemList.Clear();
                onIndicator = true;
                break;
            case INDICATOR.MARK:
                if (indicator_1 != null) Editor.DestroyImmediate(indicator_1.gameObject);
                isHorizontal = true;
                onIndicator = true;
                break;
            default:
                onIndicator = true;
                break;
        }
    }
    private Transform debugTrnasform;
    public ActivatableObject_Indicator_var1 indicator_1;
    public Vector3 pre_indicator_1_pot;
    public ActivatableObject_Indicator_var2 indicator_2;
    public Vector3 pre_indicator_2_pot;


    private void DestroyAll()
    {
        if(debugTrnasform != null)
        {
            Editor.DestroyImmediate(debugTrnasform.gameObject);
        }
    }
    private void CreateDebugTransform()
    {
        GameObject go = new GameObject("DebugTransform");
        go.transform.SetParent(entity.gameObject.transform);
        debugTrnasform = go.transform;
    }

    private void CreateIndicator(INDICATOR indicator)
    {
        switch (indicator)
        {
            case INDICATOR.TEXT:
                if (indicator_1 == null) indicator_1 = CreateIndicator_1();
                break;
            case INDICATOR.MARK:
                if (indicator_2 == null) indicator_2 = CreateIndicator_2();
                break;
            case INDICATOR.BOTH:
                if (indicator_1 == null) indicator_1 = CreateIndicator_1();
                if (indicator_2 == null) indicator_2 = CreateIndicator_2();
                break;

        }

    }

    private void CheckIndicatorPosition()
    {
        if (indicator_1 != null && indicator_1.transform.position != pre_indicator_1_pot)
        {
            pre_indicator_1_pot = indicator_1.transform.position;
        }

        if(indicator_2 != null && indicator_2.transform.position != pre_indicator_2_pot)
        {
            pre_indicator_2_pot = indicator_2.transform.position;
        }
    }
    private ActivatableObject_Indicator_var1 CreateIndicator_1()
    {
        var indicator = ResourceManager.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_1_Path);
        var item = Instantiate(indicator).GetComponent<ActivatableObject_Indicator_var1>();
        item.transform.SetParent(debugTrnasform);
        pre_indicator_1_pot = item.transform.position;
        return item;
    }
    private ActivatableObject_Indicator_var2 CreateIndicator_2()
    {
        var indicator = ResourceManager.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_2_Path);
        var item = Instantiate(indicator).GetComponent<ActivatableObject_Indicator_var2>();
        item.transform.SetParent(debugTrnasform);
        pre_indicator_2_pot = item.transform.position;

        if (itemList == null) itemList = new();
        if (entity.activeRequirAmount != itemList.Count)
        {
            for(int i = 0; i<entity.activeRequirAmount;i++)
            {
                var newItem = CreateItem();
            }

            Indicator_2_Item_Sort();
        }

        return item;
    }

    #region Indicator_2
    private float item_Space = 0.3f;
    private List<ActivatableObject_Indicator_var2_Item> itemList;

    private ActivatableObject_Indicator_var2_Item CreateItem()
    {
        var source = Resources.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_2_Item);
        var item = Instantiate(source,indicator_2.transform).GetComponent<ActivatableObject_Indicator_var2_Item>();
        itemList.Add(item);
        return item;
    }
    private void Indicator_2_Item_Sort()
    {
        if (itemList == null || itemList.Count == 0 || indicator_2 == null)
            return;

        float totalLength = item_Space * (itemList.Count - 1); // 총 길이
        Vector3 startPos = indicator_2.transform.position - new Vector3(totalLength / 2f, 0, 0); // 시작점

        if (isHorizontal)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                var item = itemList[i];
                item.transform.position = startPos + new Vector3(item_Space * i, 0, 0);
            }
        }
        else
        {
            for (int i = 0; i < itemList.Count; i++)
            {

            }
        }
    }
    #endregion
}
#endif