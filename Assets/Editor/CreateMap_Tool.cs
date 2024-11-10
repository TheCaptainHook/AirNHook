using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Mono.CecilX;
using Unity.VisualScripting;

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

    //test
    MapEditor curMapEditor;
    bool isMapEditor;
    GameObject obj;
    //test
    
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

    // int objectSectionPot;

    [Header("GUI Style")]
    GUIStyle _GUIStyle_Text;
    GUIStyle _GUIStyle_Cell;
    GUIStyle _GUIStyle_HeadTitleText;
    GUIStyle _GUIStyle_Tooltip;
    

    [Header("Mode")]
    ModeType modeType;

    [Header("Scroll")]
    Vector2 scrollPosition;
    //bool modeToggle;
    //public bool ModeToggle {
    //    get { return modeToggle; }
    //    set { if (modeToggle != value) { modeToggle = value; }
    //    } }

    [MenuItem("Tools/MapEditor Tool/Create Object Tool")]
    public static void ShowWindow()
    {
        CreateMap_Tool ct = (CreateMap_Tool)GetWindow(typeof(CreateMap_Tool));
        ct.minSize = new Vector2(350, 500);
        ct.maxSize = new Vector2(350, 700);
        ct.Show();
    }

    /// <summary>
    /// Similar Start() or Awake()
    /// </summary>
    private void OnEnable()
    {
        objLists = GetResourcesList("Object");
        sceneObjLists = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/MapEditor/Scenes"));
        backgroundObjLists = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/MapEditor/Background"));
        otherObjLists = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/MapEditor/Other"));
    }
    bool isGUIStyleInitialized;
    private void Init_TextureAndGUI(){
        if(isGUIStyleInitialized) return;
        isGUIStyleInitialized = true;
        InitTextures();
        InitGUIStyle();
    }

    private List<GameObject> GetResourcesList(string type){
        List<GameObject> list  = new();
        foreach(var obj in Resources.LoadAll<GameObject>($"Prefabs/MapEditor/{type}")){
            if(obj.TryGetComponent(out BuildObj component)){
                if(component.id ==313) continue;
                if(component.id ==320) continue;
                if(component.id ==321) continue;
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

    #region REFECTORINGCODE 0510

    private void DrawLayouts()
    {
        viewWidth = EditorGUIUtility.currentViewWidth;
        headerSection = new Rect(0, 0, viewWidth, 80); //350
        GUI.DrawTexture(headerSection, headerSectionTexture);
        modeSction = new Rect(0, 80, viewWidth, 120);
        objectSection = new Rect(0, 120, viewWidth, 320);
        GUI.DrawTexture(objectSection, objectSectionTexture);
        generatorObjectPreviewSpriteSection = new Rect(0, 450, viewWidth, 550);

    }

  
    #endregion


    
    private float GetPosition(float layoutWidth){
        return (viewWidth -layoutWidth)/2;
        
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
        GUILayout.BeginHorizontal();

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
       
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();
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
    } //todo TEST REFECTORING CODE 0503

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
 // foreach (GUIContent content in contentsList) //
        // {
        //     if (curWidth == 0)
        //     {
        //         GUILayout.BeginHorizontal(GUILayout.Width(screenWidth));
        //     }
        
        //     if (GUILayout.Button(content, _GUIStyle_Cell))
        //     {
        //         CreateObject(index);
        //     }

        //     CreateLabel(modeType, index);

        //     if (curWidth > screenWidth - 10)
        //     {
        //         curWidth = 0;
        //         index++;
        //         GUILayout.EndHorizontal();
        //         continue;
        //     }
        //     else if (index == contentsList.Count - 1)
        //     {
        //         GUILayout.EndHorizontal();
        //     }
        //     curWidth += _GUIStyle_Cell.fixedWidth;
        //     index++;

        // }


    private void SettingContents(List<GUIContent> contentsList,float screenWidth,ref int index,ref float curWidth){
        foreach (GUIContent content in contentsList) //
        {
            if (curWidth == 0)
            {
                GUILayout.BeginHorizontal(GUILayout.Width(screenWidth));
            }
            GUIContent btnContent = new GUIContent(content.image,"");
            if (GUILayout.Button(btnContent, _GUIStyle_Cell))
            {
                CreateObject(index);
            }

            // CreateLabel(modeType, index);
            Rect lastRect = GUILayoutUtility.GetLastRect();
            // 마우스가 버튼 위에 있을 때 툴팁을 보여줍니다.
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
            else if (index == contentsList.Count - 1)
            {
                GUILayout.EndHorizontal();
            }
            curWidth += _GUIStyle_Cell.fixedWidth;
            index++;

        }
    }

    //todo TEST REFECTORING CODE 0503
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
    private void SelectActiveOBJ_OtherType(GameObject obj){
        string[] str = obj.name.Split("_");
        Transform curTr = curMapEditor.otherContainer;
        OtherContainer otherContainer = curTr.GetComponent<OtherContainer>();

        for(int i =0;i<str.Length-1;i++){
            Transform transform =curTr.Find(str[i]);
            if(transform == null){
                transform = new GameObject(str[i]).transform;
                transform.SetParent(curTr);
            }
            curTr = transform;
        }
        otherContainer.SetGroup(curTr);
        SelectActiveOBJ(obj,curTr);
    }
    void CreateObject(int i)
    {
        GameObject obj = modeType == ModeType.Object ? objLists[i] : modeType == ModeType.Scenes ? sceneObjLists[i]: modeType == ModeType.Other ? otherObjLists[i] : backgroundObjLists[i];
        BuildObj buildObj = obj.GetComponent<BuildObj>();
        //GameObject obj = objLists[i];

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
            case 305:
            case 304:
            case 322:
            case 330:
            case 331:
            case 332:
                SelectActiveOBJ(obj, curMapEditor.buttonActivatableObjectTransform);
                break;
            case 306:
            case 312:
            case 324:
            case 329:
                SelectActiveOBJ(obj,curMapEditor.buttonObjectTransform);
                break;
            case 1003:
                SelectActiveOBJ(obj, curMapEditor.triggerDialogueTransform);
                break;
            case 325:
            case 326:
            case 327:
            case 328:
                SelectActiveOBJ(obj,curMapEditor.droneTransform);
            break;
            default:
                SelectActiveOBJ(obj, curMapEditor.objectTransform);
                break;
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
        if(obj.GetComponent<BuildObj>().id == 312){
            Instantiate(Resources.Load<GameObject>("Prefabs/MapEditor/Object/LeverHead"),curMapEditor.objectTransform);
        }

        Selection.activeGameObject = Instantiate(obj, transform);
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


}
