

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ActivatableObjectEntity),true)]
public class ActivatableObject_Indicator_Helper_Editor : Editor
{

    ActivatableObjectEntity entity;

    private void OnEnable()
    {
        if (Application.isPlaying) return;

        entity = target as ActivatableObjectEntity;

        itemList = new();

        if (entity != null && entity.gameObject.scene.IsValid())
        {
            // if (debugTrnasform == null) CreateDebugTransform();
            InitSetting(entity.ButtonActivatedObjectStruct);

            EditorApplication.update += EditorUpdate;

        }
       
    }
    private void InitSetting(ButtonActivatableObjectStruct data)
    {
        IndicatorStruct idata = entity.indicatorStruct; 
        if (debugTrnasform == null) CreateDebugTransform();
        if (idata.indicator == INDICATOR.NONE) return;

        Debug.Log($"{idata.indicator}");

        switch (idata.indicator)
        {
            case INDICATOR.TEXT:
                if (indicator_1 == null) indicator_1 = CreateIndicator_1();
                indicator_1.transform.position = idata.indicator_1_position;
                break;
            case INDICATOR.MARK:
                if (indicator_2 == null) indicator_2 = CreateIndicator_2();
                indicator_2.transform.position = idata.indicator_2_position;
                indicator_2.transform.rotation = data.quaternion;
                break;
            case INDICATOR.BOTH:
                if (indicator_1 == null) indicator_1 = CreateIndicator_1();
                indicator_1.transform.position = idata.indicator_1_position;
                if (indicator_2 == null) indicator_2 = CreateIndicator_2();
                indicator_2.transform.position = idata.indicator_2_position;
                indicator_2.transform.rotation = data.quaternion;
                break;

        }
        
    }

    private void OnDisable()
    {
        if (Application.isPlaying) return;

        EditorApplication.update -= EditorUpdate;

        if (entity != null)
        {
            indicator = INDICATOR.NONE;
            curActiveRequirAmount = 0;
            DestroyAll();
        }

        entity = null;
    }
    // private void EditorQuit()
    // {
    //      EditorApplication.update -= EditorUpdate;

    //     if (entity != null)
    //     {
    //         indicator = INDICATOR.NONE;
    //         curActiveRequirAmount = 0;
    //         DestroyAll();
    //     }

    //     entity = null;
    // }
    // public bool isHorizontal;
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying) return;
        
        base.OnInspectorGUI();

        EditorGUILayout.Space(20);

        if (onIndicator)
        {
            indicator_1 = (ActivatableObject_Indicator_var1)EditorGUILayout.ObjectField("indicator_1", indicator_1, typeof(ActivatableObject_Indicator_var1), true);
            if (indicator_1 != null)
            {
                entity.indicatorOffset_val_1 = EditorGUILayout.Vector2Field("   Indicator_1_Offset", entity.indicatorOffset_val_1);
            }

            indicator_2 = (ActivatableObject_Indicator_var2)EditorGUILayout.ObjectField("indicator_2", indicator_2, typeof(ActivatableObject_Indicator_var2), true);
            if (indicator_2 != null)
            {
                entity.isHorizontal = EditorGUILayout.Toggle("is HorizonTal", entity.isHorizontal);
                entity.indicatorOffset_val_2 = EditorGUILayout.Vector2Field("   Indicator_2_Offset", entity.indicatorOffset_val_2);
            }

            
            EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("SAVE"))
                {
                    entity.GetIndicatorStruct();
                }   
                GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

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
            if (curActiveRequirAmount != entity.activeRequirAmount)
            {
                if (indicator_1 != null)
                {

                }

                if(indicator_2 != null)
                {

                    int num = entity.activeRequirAmount - curActiveRequirAmount;
                    if (num > 0)
                    {
                        for (int i = 0; i < entity.activeRequirAmount - curActiveRequirAmount; i++)
                        {
                            var item = CreateItem();
                        }
                    }
                    else
                    {
                        for (int i = itemList.Count - 1; i >= entity.activeRequirAmount; i--) 
                        {
                            Editor.DestroyImmediate(itemList[i].gameObject);
                            itemList.RemoveAt(i);
                        }

                    }

                }      
                curActiveRequirAmount = entity.activeRequirAmount;
            }

            CheckIndicatorPosition();

            Indicator_2_Item_Sort();

        }
    }

    private void ClearIndicator(INDICATOR indicator)
    {
        switch(indicator)
        {
            case INDICATOR.NONE:
                if (debugTrnasform != null) Editor.DestroyImmediate(debugTrnasform.gameObject);
                indicator = INDICATOR.NONE;
                curActiveRequirAmount = 0;
                itemList.Clear();
                onIndicator = false;
                break;
            case INDICATOR.TEXT:
                if (indicator_2 != null) Editor.DestroyImmediate(indicator_2.gameObject);
                if (indicator_1 == null) indicator_1 = CreateIndicator_1();
                itemList.Clear();
                onIndicator = true;
                break;
            case INDICATOR.MARK:
                if (indicator_1 != null) Editor.DestroyImmediate(indicator_1.gameObject);
                if(indicator_2 == null) indicator_2 = CreateIndicator_2();
                // entity.isHorizontal = true;
                onIndicator = true;
                break;
            case INDICATOR.BOTH:
                if (indicator_1 == null) indicator_1 = CreateIndicator_1();
                if (indicator_2 == null) indicator_2 = CreateIndicator_2();
                // entity.isHorizontal = true;
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
        debugTrnasform = go.transform;
    }

   

    private void CheckIndicatorPosition()
    {
        if(indicator_1 != null)
        {
            entity.indicatorOffset_val_1 = indicator_1.transform.position;
        }
        
        if(indicator_2 != null)
        {
            entity.indicatorOffset_val_2 = indicator_2.transform.position;
            indicator_2.transform.rotation = entity.transform.rotation;

        }
    }

    #region  Create Indicator 1,2
    
    private ActivatableObject_Indicator_var1 CreateIndicator_1()
    {
        var indicator = ResourceManager.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_1_Path);
        var item = Instantiate(indicator).GetComponent<ActivatableObject_Indicator_var1>();
        item.transform.SetParent(debugTrnasform);

        item.transform.position = entity.transform.position;

        pre_indicator_1_pot = item.transform.position;
      
        return item;
    }
    private ActivatableObject_Indicator_var2 CreateIndicator_2()
    {
        var indicator = ResourceManager.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_2_Path);
        var item = Instantiate(indicator).GetComponent<ActivatableObject_Indicator_var2>();
        item.transform.SetParent(debugTrnasform);

        item.transform.position = entity.transform.position;

        pre_indicator_2_pot = item.transform.position;

        
        if (itemList == null) itemList = new();


        return item;
    }
    #endregion

    #region Indicator_2
    private float item_Space = 0.5f;
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
        Vector3 startPos = entity.isHorizontal ? 
            indicator_2.transform.position - new Vector3(totalLength / 2f, 0, 0) : 
            indicator_2.transform.position - new Vector3(0, totalLength / 2f, 0);

        if (entity.isHorizontal)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                var item = itemList[i];
                item.transform.position = startPos + indicator_2.transform.rotation * new Vector3(item_Space * i, 0, 0) ;

            }
        }
        else
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                var item = itemList[i];
                item.transform.position = startPos + indicator_2.transform.rotation * new Vector3(0, item_Space * i, 0);
            }
        }

    }
    #endregion
}


#region Util


#endregion
#endif