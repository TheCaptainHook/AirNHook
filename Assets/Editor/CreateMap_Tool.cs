using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

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
    Rect headerSection;
    Rect modeSction;
    Rect objectSection;
    Rect generatorObjectPreviewSpriteSection;

    Color headerSectionColor = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f);
    Color objectSectonColor = new Color(0, 0, 0,1);

    int objectSectionPot;

    [Header("GUI Style")]
    GUIStyle _GUIStyle_Text;
    GUIStyle _GUIStyle_Cell;
    GUIStyle _GUIStyle_HeadTitleText;

    

    [Header("Mode")]
    ModeType modeType;

    [Header("Scroll")]
    Vector2 scrollPosition;
    //bool modeToggle;
    //public bool ModeToggle {
    //    get { return modeToggle; }
    //    set { if (modeToggle != value) { modeToggle = value; }
    //    } }

    [MenuItem("Window/MapEditor Tool/Create Object Tool")]
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
        //todo 0415
        otherObjLists = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/MapEditor/Other"));
        //todo 0415
        //ruleTile = Resources.Load<RuleTile>("Arts/Tiles/1");
        InitTextures();
        InitGUIStyle();
    }


    private List<GameObject> GetResourcesList(string type){
        List<GameObject> list  = new();
        foreach(var obj in Resources.LoadAll<GameObject>($"Prefabs/MapEditor/{type}")){
            if(obj.TryGetComponent(out BuildObj component)){
                if(component.id ==308) continue;
                if(component.id ==313) continue;
                if(component.id ==315) continue;
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
        _GUIStyle_HeadTitleText.fixedHeight = 20;
        _GUIStyle_HeadTitleText.fixedWidth = 350;
        _GUIStyle_HeadTitleText.alignment = TextAnchor.MiddleCenter;

     


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
        headerSection = new Rect(0, 0, 350, 80);
        GUI.DrawTexture(headerSection, headerSectionTexture);
        modeSction = new Rect(0, 80, 350, 120);
        objectSection = new Rect(0, 120, 350, 320);
        GUI.DrawTexture(objectSection, objectSectionTexture);
        generatorObjectPreviewSpriteSection = new Rect(0, 500, 350, 550);

    }

  
    #endregion


    


    private void DrawHeader()
    {
        GUILayout.BeginArea(headerSection);
        GUILayout.Label("Object Create Tool",_GUIStyle_HeadTitleText);
        if(GUI.Button(new Rect(100,30,150,20),"Click [Create MapEditor]"))
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
        GUILayout.BeginHorizontal(GUILayout.Width(350));

        if (GUI.Button(new Rect(5, 5, 80, 30), "Object"))
        {
            modeType = ModeType.Object;
        }

        if (GUI.Button(new Rect(90, 5, 80, 30), "Scene"))
        {
            modeType = ModeType.Scenes;
        }

        if (GUI.Button(new Rect(175, 5, 80, 30), "BackGround"))
        {
            modeType = ModeType.BackGround;
        }

        if (GUI.Button(new Rect(260, 5, 80, 30), "Other"))
        {
            modeType = ModeType.Other;
        }

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
                    contents.Add(new GUIContent(texture));
                }
                break;
            case ModeType.Scenes:
                foreach (GameObject obj in sceneObjLists)
                {
                    Texture2D texture = AssetPreview.GetAssetPreview(obj);
                    contents.Add(new GUIContent(texture));
                }
                break;
            case ModeType.BackGround:
                foreach (GameObject obj in backgroundObjLists)
                {
                    Texture2D texture = AssetPreview.GetAssetPreview(obj);
                    contents.Add(new GUIContent(texture));
                }
                break;
            case ModeType.Other:
                foreach (GameObject obj in otherObjLists)
                {
                    Texture2D texture = AssetPreview.GetAssetPreview(obj);
                    contents.Add(new GUIContent(texture));
                }
                break;
        }
    } //todo TEST REFECTORING CODE 0503

    private void CreateLabel(ModeType modeType, int index)
    {
        switch (modeType)
        {
            case ModeType.Scenes:
                GUILayout.Label(sceneObjLists[index].name, _GUIStyle_Text);
                break;
            case ModeType.Object:
                GUILayout.Label(objLists[index].name, _GUIStyle_Text);
                break;
            case ModeType.Other:
                GUILayout.Label(otherObjLists[index].name, _GUIStyle_Text);
                break;
            case ModeType.BackGround:
                GUILayout.Label(backgroundObjLists[index].name, _GUIStyle_Text);
                break;
        }
    }

    private void DrawObjectContent()
    {
        GUILayout.BeginArea(objectSection);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(350), GUILayout.Height(300));
        List<GUIContent> contentsList = new();

        CreateContents(modeType, contentsList);

        float screenWidth = 240f;
        int index = 0;
        float curWidth = 0;
        foreach (GUIContent content in contentsList) //
        {
            if (curWidth == 0)
            {
                GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
            }

        
            if (GUILayout.Button(content, _GUIStyle_Cell))
            {
                CreateObject(index);
            }

            CreateLabel(modeType, index);

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

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    //todo TEST REFECTORING CODE 0503
    #endregion

    //private void DrawObjectContent()
    //{
    //    GUILayout.BeginArea(objectSection);
    //    scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(350), GUILayout.Height(300));

    //    List<GUIContent> contentsList = new();

    //    if (modeType == ModeType.Object)
    //    {
    //        foreach (GameObject obj in objLists)
    //        {
    //            Texture2D texture = AssetPreview.GetAssetPreview(obj);
    //            contentsList.Add(new GUIContent(texture));
    //        }
    //    }
    //    else if (modeType == ModeType.Scenes)
    //    {
    //        foreach (GameObject obj in sceneObjLists)
    //        {
    //            Texture2D texture = AssetPreview.GetAssetPreview(obj);
    //            contentsList.Add(new GUIContent(texture));
    //        }
    //    }
    //    else if (modeType == ModeType.BackGround)
    //    {
    //        foreach (GameObject obj in backgroundObjLists)
    //        {
    //            Texture2D texture = AssetPreview.GetAssetPreview(obj);
    //            contentsList.Add(new GUIContent(texture));
    //        }
    //    }
    //    else if (modeType == ModeType.Other)
    //    {
    //        foreach (GameObject obj in otherObjLists)
    //        {
    //            Texture2D texture = AssetPreview.GetAssetPreview(obj);
    //            contentsList.Add(new GUIContent(texture));
    //        }
    //    }

    //    //objectSectionPot = GUILayout.SelectionGrid(objectSectionPot, contentsList.ToArray(), 6,_GUIStyle_Cell);

    //    float screenWidth = 240f;
    //    int index = 0;
    //    float curWidth = 0;
    //    foreach (GUIContent content in contentsList)
    //    {
    //        if (curWidth == 0)
    //        {
    //            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
    //        }


    //        if (GUILayout.Button(content, _GUIStyle_Cell))
    //        {
    //            //if(modeToggle) Debug.Log($"{index}, {sceneObjLists[index]}");
    //            //else Debug.Log($"{index}, {objLists[index]}");

    //            CreateObject(index);

    //        }

    //        if (modeType == ModeType.Scenes) GUILayout.Label(sceneObjLists[index].name, _GUIStyle_Text);
    //        else if (modeType == ModeType.Object) GUILayout.Label(objLists[index].name, _GUIStyle_Text);
    //        else if (modeType == ModeType.Other) GUILayout.Label(otherObjLists[index].name, _GUIStyle_Text);
    //        else if (modeType == ModeType.BackGround) GUILayout.Label(backgroundObjLists[index].name, _GUIStyle_Text);

    //        if (curWidth > screenWidth - 10)
    //        {
    //            curWidth = 0;
    //            index++;
    //            GUILayout.EndHorizontal();
    //            continue;
    //        }
    //        else if (index == contentsList.Count - 1)
    //        {
    //            GUILayout.EndHorizontal();
    //        }
    //        curWidth += _GUIStyle_Cell.fixedWidth;
    //        index++;

    //    }
    //    //GUILayout.EndHorizontal();

    //    GUILayout.EndScrollView();
    //    GUILayout.EndArea();
    //}


    private void DrawGenratorObjectPreviewSpriteContent()
    {
        GUILayout.BeginArea(generatorObjectPreviewSpriteSection);
        if(GUI.Button(new Rect(70,0,200,30),"Genrator Object Preview Sprite"))
        {
            GeneratorObjPreviewSprite();
        }
        GUILayout.EndArea();
    }

    #endregion


    #region Function
    void CreateObject(int i)
    {
        GameObject obj = modeType == ModeType.Object ? objLists[i] : modeType == ModeType.Scenes ? sceneObjLists[i]: modeType == ModeType.Other ? otherObjLists[i] : backgroundObjLists[i];
        BuildObj buildObj = obj.GetComponent<BuildObj>();
        //GameObject obj = objLists[i];

        switch (buildObj.id)
        {
            case 302:
                FindObj(curMapEditor.dontSaveObjectTransform, obj);
                SelectActiveOBJ(objLists[i], curMapEditor.dontSaveObjectTransform);
                break;
            case 301:
                FindObj(curMapEditor.exitDoorObjectTransform, obj);
                SelectActiveOBJ(objLists[i], curMapEditor.exitDoorObjectTransform);
                break;
            case 305:
                SelectActiveOBJ(obj, curMapEditor.buttonActivatableObjectTransform);
                break;
            case 306:
            case 312:
                SelectActiveOBJ(obj,curMapEditor.buttonObjectTransform);
                break;
            case 1003:
                SelectActiveOBJ(obj, curMapEditor.triggerDialogueTransform);
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
