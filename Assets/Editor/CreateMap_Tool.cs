using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using Unity.VisualScripting;
using System.Text;
using UnityEditor.Build.Reporting;

public enum ModeType
{
    Tile,
    Object,
    Scenes,
    BackGround,
    Other
}

public class CreateMap_Tool : EditorWindow
{

    MapEditor curMapEditor;
    bool isMapEditor;
    GameObject obj;
 
    // 
    /// <summary>
    ///Load in Prefabs/MapEditor Directory. 
    /// 1. If you have added data to the data table, you must create a list.
    /// 2. Add ModeType
    /// 3. When OnEnable(), data is received from the Prefabs/MapEditor path.
    /// 4. If you need Mode Change Button, Create in DrawMode().
    /// 5. Add GUIContent and Label in DrawObjectContent() 
    /// </summary>
    List<GameObject> objLists;
    List<GameObject> sceneObjLists;
    List<GameObject> otherObjLists;
    List<GameObject> backgroundObjLists;

    RuleTile ruleTile;

    Texture2D headerSectionTexture;
    Texture2D objectSectionTexture;

    string saveSpritePath = Path.Combine(Application.dataPath, "Resources/Arts/Sprites/PreviewSprites");
    

    [Header("Section")]
    float viewWidth;
    Rect headerSection;
    Rect modeSction;
    Rect objectSection;
    Rect generatorObjectPreviewSpriteSection;

    Color headerSectionColor = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f);
    Color objectSectonColor = new Color(0, 0, 0,1);

    [Header("GUI Style")]
    GUIStyle _GUIStyle_Text;
    GUIStyle _GUIStyle_Cell;
    GUIStyle _GUIStyle_HeadTitleText;
    GUIStyle _GUIStyle_Tooltip;
    

    [Header("Mode")]
    ModeType modeType;

    #region Search
    private string search_inputText;
    private bool onSearch;
    private Texture2D onImg;
    private Texture2D offImg;
    #endregion

    [Header("Scroll")]
    Vector2 scrollPosition;

    [MenuItem("Tools/MapEditor Tool/Create Object Tool")]
    public static void ShowWindow()
    {
        CreateMap_Tool ct = (CreateMap_Tool)GetWindow(typeof(CreateMap_Tool));
        ct.minSize = new Vector2(350, 500);
        ct.maxSize = new Vector2(350, 700);
        ct.Show();
    }


    private void OnEnable()
    {
        objLists = GetResourcesList("Object");
        sceneObjLists = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/MapEditor/Scenes"));
        backgroundObjLists = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/MapEditor/Background"));
        otherObjLists = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/MapEditor/Other"));

        onImg = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artwork/Sprites/Assets/UI Elements/White/1x/exit left.png");
        offImg =  AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artwork/Sprites/Assets/UI Elements/White/1x/exit right.png");
        search_inputText = "";
    }
    bool isGUIStyleInitialized;

    private void Init_TextureAndGUI()
    {
        if (isGUIStyleInitialized) return;
        isGUIStyleInitialized = true;
        InitTextures();
        InitGUIStyle();
    }

    private List<GameObject> GetResourcesList(string type)
    {
        List<GameObject> list = new();
        foreach (var obj in Resources.LoadAll<GameObject>($"Prefabs/MapEditor/{type}"))
        {
            if (obj.TryGetComponent(out BuildObj component))
            {
                if (component.id == 313) continue;
                if (component.id == 320) continue;
                if (component.id == 321) continue;
                list.Add(obj);

            }
        }

        return list;


    }

    #region  Init 
    private void InitTextures()
    {
        headerSectionTexture = new Texture2D(1, 1);
        headerSectionTexture.SetPixel(0, 0, headerSectionColor);
        headerSectionTexture.Apply();

        objectSectionTexture = new Texture2D(1, 1);
        objectSectionTexture.SetPixel(0, 0, objectSectonColor);
        objectSectionTexture.Apply();
    }

    private void InitGUIStyle()
    {
        _GUIStyle_Text = new GUIStyle();
        _GUIStyle_Text.fontSize = 10;
        _GUIStyle_Text.normal.textColor = Color.white;
        _GUIStyle_Text.focused.textColor = Color.blue;
        _GUIStyle_Text.fixedWidth = 5;
        _GUIStyle_Text.fixedHeight = 100;
        _GUIStyle_Text.padding = new RectOffset(-80,0, 10, 10);
        _GUIStyle_Text.alignment = TextAnchor.LowerLeft;
        _GUIStyle_Text.hover.textColor = Color.red;
        _GUIStyle_Text.active.textColor = Color.blue;
        
        _GUIStyle_Cell = new GUIStyle();
        _GUIStyle_Cell.focused.textColor = Color.blue;
        _GUIStyle_Cell.fixedWidth = 80;
        _GUIStyle_Cell.fixedHeight = 80;
        _GUIStyle_Cell.padding = new RectOffset(5, 10,10, 10);
        _GUIStyle_Cell.margin = new RectOffset(0, 5, 0, 0);
        _GUIStyle_Cell.hover.textColor = Color.red;
        _GUIStyle_Cell.active.textColor = Color.blue;

        _GUIStyle_HeadTitleText = new GUIStyle();
        _GUIStyle_HeadTitleText.fontSize = 20;
        _GUIStyle_HeadTitleText.normal.textColor = Color.green;
        _GUIStyle_HeadTitleText.fixedHeight = 40;

        // _GUIStyle_HeadTitleText.fixedWidth = viewWidth;
        _GUIStyle_HeadTitleText.alignment = TextAnchor.MiddleCenter;

        _GUIStyle_Tooltip = new GUIStyle(GUI.skin.label);
        _GUIStyle_Tooltip.fontSize = 12;
        _GUIStyle_Tooltip.normal.textColor = Color.white;
        _GUIStyle_Tooltip.fontStyle = FontStyle.Bold;
        _GUIStyle_Tooltip.alignment = TextAnchor.MiddleCenter;
        Texture2D texture = new Texture2D(1,1);
        texture.SetPixel(0,0,new Color(0,0,0,0.75f));
        texture.Apply();
        _GUIStyle_Tooltip.normal.background = texture;
    }

    #endregion

    private void OnGUI()
    {
        if (!isMapEditor)
        {
            curMapEditor = FindObjectOfType<MapEditor>();
            if (curMapEditor == null)
            {
                isMapEditor = false;
            }
            else isMapEditor = true;
        }
        Init_TextureAndGUI();

        DrawLayouts();

        DrawHeader();

        DrawMode(); // mode Change btn

        if(isMapEditor) DrawObjectContent(); 

        DrawGenratorObjectPreviewSpriteContent();
    }
    
    #region Draw

    private void DrawLayouts()
    {
        viewWidth = EditorGUIUtility.currentViewWidth;
        headerSection = new Rect(0, 0, viewWidth, 80); //350
        GUI.DrawTexture(headerSection, headerSectionTexture);
        modeSction = new Rect(0, 80, viewWidth, 150);
        objectSection = new Rect(0, 150, viewWidth, 320);
        GUI.DrawTexture(objectSection, objectSectionTexture);
        generatorObjectPreviewSpriteSection = new Rect(0, 500, viewWidth, 550);

    }


    private float GetPosition(float layoutWidth)
    {
        return (viewWidth - layoutWidth) / 2;

    }

    private void DrawHeader()
    {
        GUILayout.BeginArea(headerSection);
        GUILayout.Label("Object Create Tool",_GUIStyle_HeadTitleText);
        if(GUI.Button(new Rect(GetPosition(150),30,150,20),"Click [Create MapEditor]"))
        {
            MapEditor mapEditor = FindObjectOfType<MapEditor>();
            if (mapEditor == null)
            {
                GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/MapEditor/MapEditor"));
                Selection.activeGameObject = obj;
                curMapEditor = obj.GetComponent<MapEditor>();
                curMapEditor.Init();
                EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette");
            }
            else
            {
                Debug.Log("맵에디터 있음");
            }

        }
        GUILayout.EndArea();

    }
    private void DrawMode()
    {
        GUILayout.BeginArea(modeSction);
        GUILayout.BeginHorizontal(GUILayout.Width(viewWidth));

        GUILayout.FlexibleSpace();
        // GUILayout.BeginHorizontal();

        if(GUILayout.Button("Object",GUILayout.Width(85),GUILayout.Height(30))){
             modeType = ModeType.Object;
        }
        if(GUILayout.Button("Scene",GUILayout.Width(85),GUILayout.Height(30))){
             modeType = ModeType.Scenes;
        }
         if(GUILayout.Button("BackGround",GUILayout.Width(85),GUILayout.Height(30))){
             modeType = ModeType.BackGround;
        }
         if(GUILayout.Button("Other",GUILayout.Width(85),GUILayout.Height(30))){
             modeType = ModeType.Other;
        }
       
        // GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //Search Area
        
        HorizontalScope(()=>{
            GUILayout.FlexibleSpace();
            if(onSearch) 
            {
                search_inputText = GUILayout.TextField(search_inputText,GetTextFieldStyle(20,Color.white),GUILayout.Width(250),GUILayout.Height(30));
            }

            if(GUILayout.Button(new GUIContent(onSearch ? offImg : onImg),GUILayout.Width(30),GUILayout.Height(30))){
                onSearch = !onSearch;
                if(!onSearch) search_inputText = "";
            }

            
        });
        //Search Area

        GUILayout.EndArea();
    }
    #region todo TEST REFECTORING CODE 0503
    private void CreateContents(ModeType modeType, List<GUIContent> contents)
    {
        switch (modeType)
        {
            case ModeType.Object:
                foreach (GameObject obj in objLists)
                {
                    Texture2D texture = AssetPreview.GetAssetPreview(obj);
                    contents.Add(new GUIContent(texture,obj.name));
                }
                break;
            case ModeType.Scenes:
                foreach (GameObject obj in sceneObjLists)
                {
                    Texture2D texture = AssetPreview.GetAssetPreview(obj);
                    contents.Add(new GUIContent(texture,obj.name));
                }
                break;
            case ModeType.BackGround:
                foreach (GameObject obj in backgroundObjLists)
                {
                    Texture2D texture = AssetPreview.GetAssetPreview(obj);
                    contents.Add(new GUIContent(texture,obj.name));
                }
                break;
            case ModeType.Other:
                foreach (GameObject obj in otherObjLists)
                {
                    Texture2D texture = AssetPreview.GetAssetPreview(obj);
                    contents.Add(new GUIContent(texture,obj.name));
                    
                }
                break;
        }
    } 

    private void DrawObjectContent()
    {
        GUILayout.BeginArea(objectSection);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(viewWidth), GUILayout.Height(300));
        List<GUIContent> contentsList = new();

        CreateContents(modeType, contentsList);

        float screenWidth = viewWidth -10;
        int index = 0;
        float curWidth = 0;

        SettingContents(contentsList,screenWidth,ref index,ref curWidth);

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    #region  Searching
    private List<GUIContent> SearchingContents(List<GUIContent> main)
    {
        List<GUIContent> searchingList = new();
        StringBuilder sb = new();
        search_inputText = search_inputText.ToLower().Trim();

        foreach(var item in main)
        {
            sb.Clear().Append(item.tooltip.ToLower().Trim());

            if(sb.ToString().Contains(search_inputText))
            {
                searchingList.Add(item);
            }

        }
        return searchingList;
    }
    #endregion 
    private void SettingContents(List<GUIContent> contentsList,float screenWidth,ref int index,ref float curWidth){
        //Search Option
        List<GUIContent> searchContents = contentsList;
        // searchContents = contentsList;
        if(onSearch)
        {
            searchContents = SearchingContents(contentsList);
        }

        //Search Option
    
                    foreach (GUIContent content in searchContents) //
                    {
                        if (curWidth == 0)
                        {
                            GUILayout.BeginHorizontal(GUILayout.Width(screenWidth));
                        }
                        GUIContent btnContent = new GUIContent(content.image,content.tooltip);
                        if (GUILayout.Button(btnContent, _GUIStyle_Cell))
                        {
                            // CreateObject(index);
                            CreateObject(btnContent);
                        }

                        Rect lastRect = GUILayoutUtility.GetLastRect();

                        if (lastRect.Contains(Event.current.mousePosition))
                        {
                            GUI.Label(new Rect(lastRect.x, lastRect.y+20, lastRect.width, 20), content.tooltip,_GUIStyle_Tooltip);
                        }

                        if (curWidth > screenWidth - 10)
                        {
                            curWidth = 0;
                            index++;
                            GUILayout.EndHorizontal();
                            continue;
                        }
                        else if (index == searchContents.Count - 1)
                        {
                            GUILayout.EndHorizontal();
                        }
                        curWidth += _GUIStyle_Cell.fixedWidth;
                        index++;

                    }        
        
       
    }

    #endregion



    private void DrawGenratorObjectPreviewSpriteContent()
    {
        GUILayout.BeginArea(generatorObjectPreviewSpriteSection);
        if(GUI.Button(new Rect(GetPosition(200),0,200,30),"Genrator Object Preview Sprite"))
        {
            GeneratorObjPreviewSprite();
        }
        GUILayout.EndArea();
    }

    #endregion

    #region Function
    private void SelectActiveOBJ_OtherType(GameObject obj)
    {
        string[] str = obj.name.Split("_");
        Transform curTr = curMapEditor.otherContainer;
        OtherContainer otherContainer = curTr.GetComponent<OtherContainer>();

        for (int i = 0; i < str.Length - 1; i++)
        {
            Transform transform = curTr.Find(str[i]);
            if (transform == null)
            {
                transform = new GameObject(str[i]).transform;
                transform.SetParent(curTr);
            }
            curTr = transform;
        }
        otherContainer.SetGroup(curTr);
        SelectActiveOBJ(obj, curTr);
    }

    void CreateObject(GUIContent content)
    {
        GameObject obj = null;
        List<GameObject> list = GetTypeObjectList();
        foreach(GameObject item in list)
        {
            if(item.name == content.tooltip)
            {
                obj = item;
                break;
            }
        }
        if(obj == null){ Debug.Log("Can't Find Object"); return; }

        BuildObj buildObj = obj.GetComponent<BuildObj>();

         if(modeType == ModeType.BackGround){
            SelectActiveOBJ(obj,curMapEditor.backgroundObjectContainer);
            return;
        }
        if(modeType == ModeType.Other){
            SelectActiveOBJ_OtherType(obj);
            return;
        }

        switch (buildObj.id)
        {
            case 302:
                FindObj(curMapEditor.dontSaveObjectTransform, obj);
                SelectActiveOBJ(obj, curMapEditor.dontSaveObjectTransform);
                break;
            case 301:
                FindObj(curMapEditor.exitDoorObjectTransform, obj);
                SelectActiveOBJ(obj, curMapEditor.exitDoorObjectTransform);
                break;
            case 303:
            case 305:
            case 304:
            case 322:
            case 327:
            case 330:
            case 331:
            case 332:
            case 333:
            case 334:
            case 335:
            case 339:
            case 353:
            case 354:
            case 359:
            case 367:
            case 371:
            case 375:
                SelectActiveOBJ(obj, curMapEditor.buttonActivatableObjectTransform);
                break;
            case 306:
            case 312:
            case 324:
            case 329:
            case 341:
            case 345:
            case 336:
            case 362:
            case 372:
            case 378:
                SelectActiveOBJ(obj,curMapEditor.buttonObjectTransform);
                break;
            case 1003:
                SelectActiveOBJ(obj, curMapEditor.triggerDialogueTransform);
                break;
            case 325:
            case 326:            
            case 328:
            case 340:
            case 361:
                SelectActiveOBJ(obj,curMapEditor.droneTransform);
            break;
            case 338:
                SelectActiveOBJ(obj,curMapEditor.collectableContainer);
                break;
            default:
                SelectActiveOBJ(obj, curMapEditor.objectTransform);
                break;
        }
        
    }
    List<GameObject> GetTypeObjectList()
    {
        switch(modeType)
        {
            case ModeType.BackGround:
            return backgroundObjLists;
            case ModeType.Other:
            return otherObjLists;
            case ModeType.Scenes:
            return sceneObjLists;
            default:
            return objLists;
        }
    }

    void FindObj(Transform transform, GameObject obj)
    {
                foreach (Transform cur in transform)
                {
                    if (cur.GetComponent<BuildObj>().id == obj.GetComponent<BuildObj>().id)
                    {
                        Undo.DestroyObjectImmediate(cur.gameObject);
                    }
                }
    
    }

    void SelectActiveOBJ(GameObject obj,Transform transform)
    {
        GameObject prefab;
        if(obj.GetComponent<BuildObj>().id == 312){
           prefab = Instantiate(Resources.Load<GameObject>("Prefabs/MapEditor/Object/LeverHead"),curMapEditor.objectTransform);
           prefab.transform.position = GetSceneViewCenter();
        }

        prefab = Instantiate(obj, transform);
        prefab.transform.position = GetSceneViewCenter();

        Selection.activeGameObject = prefab;
    }
    private Vector3 GetSceneViewCenter(){
        SceneView sceneView = SceneView.lastActiveSceneView;
         
        
        if (sceneView != null)
        {
            Vector3 dir = sceneView.pivot;
            dir.z = 0;
            return dir;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void CreatePreviewSprite(ModeType modeType)
    {
        switch (modeType)
        {
            case ModeType.Object:
                CreatePreviewSprite(modeType, objLists);
                break;
            case ModeType.BackGround:
                CreatePreviewSprite(modeType, backgroundObjLists);
                break;
            case ModeType.Other:
                CreatePreviewSprite(modeType, otherObjLists);
                break;
        }
    }

    private void CreatePreviewSprite(ModeType modeType, List<GameObject> list)
    {
        foreach (GameObject obj in list)
        {
            string path = Path.Combine(saveSpritePath, $"{modeType}/{obj.name}.png");
            if (!FileExists(saveSpritePath, path))
            {
                Texture2D texture = AssetPreview.GetAssetPreview(obj);
                Texture2D transparentTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

                Color[] pixels = texture.GetPixels();
                Color backgroundColor = pixels[0];

                for (int i = 0; i < pixels.Length; i++)
                {
                    if (pixels[i] == backgroundColor) pixels[i].a = 0;
                }

                transparentTexture.SetPixels(pixels);
                transparentTexture.Apply();

                byte[] bytes = transparentTexture.EncodeToPNG();

                File.WriteAllBytes(path, bytes);

            }
        }
    }

    void GeneratorObjPreviewSprite() //REFECTORING CODE 0510
    {
        CreatePreviewSprite(ModeType.Object);
        CreatePreviewSprite(ModeType.BackGround);
        CreatePreviewSprite(ModeType.Other);

        AssetDatabase.Refresh();
    }


    bool FileExists(string path, string fileName)
    {
        // 경로 및 파일 이름 조합
        string fullPath = Path.Combine(path, fileName);
        Debug.Log(fullPath);
        // 파일 존재 여부 확인
        return File.Exists(fullPath);
    }

    #endregion


#region  Util
 private void VerticalScope(Action action,string label = "",GUIStyle style = null)
    {
        if (style == null)
        style = GUIStyle.none;

        using (new GUILayout.VerticalScope(label,style))
        {
            action?.Invoke();
        }
    }
    private void HorizontalScope(Action action,string label = "",GUIStyle style = null)
    {
        if (style == null)
        style = GUIStyle.none;

        using (new GUILayout.HorizontalScope(label,style))
        {
            action?.Invoke();
        }
    }
    private GUIStyle GetTextFieldStyle(int fontSize,Color fontColor,TextAnchor anchor = TextAnchor.MiddleLeft)
    {
        GUIStyle style = new GUIStyle(GUI.skin.textField);
        style.fontSize = fontSize; 
        style.normal.textColor = fontColor;  
        style.alignment = anchor;      
        return style;
    }
#endregion

}
