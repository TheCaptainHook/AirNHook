using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Tilemaps;
public class CreateMap_Tool : EditorWindow
{

    //test
    MapEditor curMapEditor;
    bool isMapEditor;
    GameObject obj;
    //test

    List<GameObject> objLists;
    RuleTile ruleTile;

    Texture2D headerSectionTexture;
    Texture2D objectSectionTexture;

    [Header("Section")]
    Rect headerSection;
    Rect modeSction;
    Rect objectSection;

    Color headerSectionColor = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f);
    Color objectSectonColor = new Color(0, 0, 0,1);

    int objectSectionPot;

    [Header("GUI Style")]
    GUIStyle _GUIStyle_Text;
    GUIStyle _GUIStyle_Cell;
    GUIStyle _GUIStyle_HeadTitleText;

    [Header("Mode")]
    bool modeToggle;
    public bool ModeToggle {
        get { return modeToggle; }
        set { if (modeToggle != value) { modeToggle = value; }
        } }

    [MenuItem("Window/Create Map Tool")]
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
        objLists = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/MapEditor/Object"));
        ruleTile = Resources.Load<RuleTile>("Arts/Tiles/1");
        InitTextures();
        InitGUIStyle();
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
        }

        DrawLayouts();

        DrawHeader();
        DrawMode();

        if (!ModeToggle) { DrawObjectContent();} // Draw Object}
        //else { DrawTileContent();}
       


    }
    #region Draw

    private void DrawLayouts()
    {
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = 350;
        headerSection.height = 80;
        GUI.DrawTexture(headerSection, headerSectionTexture);

        modeSction.x = 0;
        modeSction.y = 80;
        modeSction.width = 350;
        modeSction.height = 120;

        objectSection.x = 0;
        objectSection.y = 120;
        objectSection.width = 350;
        objectSection.height = 220;
        GUI.DrawTexture(objectSection, objectSectionTexture);

    }

    private void DrawHeader()
    {
        GUILayout.BeginArea(headerSection);
        GUILayout.Label("Map Create Tool",_GUIStyle_HeadTitleText);
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
                Debug.Log("맵있음");
            }

        }

        Event evt = Event.current;
        //if (GUI.Button(new Rect(50,50,200,30), "select", EditorStyles.objectFieldThumb.name + "Overlay2"))
        //{

        //    EditorGUIUtility.ShowObjectPicker<GameObject>(obj, false, "MapEditor", 1);
        //    evt.Use();
        //}
        obj = EditorGUIUtility.GetObjectPickerObject() as GameObject;
        HandleUtility.Repaint();

        GUILayout.EndArea();

    }
    private void DrawMode()
    {
        GUILayout.BeginArea(modeSction);
        GUILayout.BeginHorizontal(GUILayout.Width(350));
        //if (GUI.Button(new Rect(70,5,100,30),"Tile Mode"))
        //{
        //    ModeToggle = true;
        //}

        if (GUI.Button(new Rect(125, 5, 100, 30), "Object"))
        {
            ModeToggle = false;

        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void DrawObjectContent()
    {
        GUILayout.BeginArea(objectSection);

        List<GUIContent> contentsList = new();

            foreach (GameObject obj in objLists)
            {
                Texture2D texture = AssetPreview.GetAssetPreview(obj);
                contentsList.Add(new GUIContent(texture));
            }

            //objectSectionPot = GUILayout.SelectionGrid(objectSectionPot, contentsList.ToArray(), 6,_GUIStyle_Cell);

            float screenWidth = 240f;
            int index = 0;
            float curWidth = 0;
            foreach (GUIContent content in contentsList)
            {
                if (curWidth == 0)
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
                }

                if (GUILayout.Button(content, _GUIStyle_Cell))
                {
                    
                    Debug.Log($"{index}, {objLists[index]}");
                CreateObject(index);
                
                }
                GUILayout.Label(objLists[index].name, _GUIStyle_Text);

                if (curWidth > screenWidth - 10)
                {
                    curWidth = 0;
                    index++;
                    GUILayout.EndHorizontal();
                    continue;
                }
                curWidth += _GUIStyle_Cell.fixedWidth;
                index++;

            }
     
        GUILayout.EndArea();

    }
    //private void DrawTileContent()
    //{
    //    GUILayout.BeginArea(objectSection);
    //    Texture2D ttt = AssetPreview.GetAssetPreview(ruleTile);
    //    if (GUILayout.Button(new GUIContent(ttt), _GUIStyle_Cell))
    //    {
    //        Debug.Log("asdas");
    //    }
    //    GUILayout.EndArea();
    //}

    #endregion


    #region Function
    void CreateObject(int i)
    {
        GameObject obj = objLists[i];
        switch (obj.name)
        {
            case "SpawnPoint":
                FindObj(curMapEditor.dontSaveObjectTransform, obj);
                Selection.activeGameObject = Instantiate(objLists[i], curMapEditor.dontSaveObjectTransform);
                break;
            case "ExitPoint":
                FindObj(curMapEditor.exitDoorObjectTransform, obj);
                Selection.activeGameObject = Instantiate(objLists[i], curMapEditor.exitDoorObjectTransform);
                break;
            case "BtnActivated":
                Selection.activeGameObject = Instantiate(obj, curMapEditor.dontSaveObjectTransform);
                break;
            case "BtnActivatedDoor":
                Selection.activeGameObject = Instantiate(obj, curMapEditor.interactionObjectTransform);
                break;
            default:
                Selection.activeGameObject = Instantiate(obj, curMapEditor.objectTransform);
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
    #endregion


}
