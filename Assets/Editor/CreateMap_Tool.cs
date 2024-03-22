using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class CreateMap_Tool : EditorWindow
{
    Texture2D headerSectionTexture;
    Texture2D objectSectionTexture;

    [Header("Section")]
    Rect headerSection;
    Rect modeSction;
    Rect objectSection;

    Color headerSectionColor = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f);
    Color objectSectonColor = new Color(0, 0, 0,1);

    int objectSectionPot;
    static string path = "Prefabs/MapEditor/Object";
    List<GameObject> list = new();

    [Header("GUI Style")]
    GUIStyle _GUIStyle_Text;
    GUIStyle _GUIStyle_Cell;


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

    }

    #endregion

    private void OnGUI()
    {
        DrawLayouts();

        DrawHeader();
        DrawMode();

        if(ModeToggle)
        {
            DrawContent(); // Draw Object
        }
        else { Debug.Log("Tile"); }

       


    }

    #region Draw

    private void DrawLayouts()
    {
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = Screen.width;
        headerSection.height = 50;
        GUI.DrawTexture(headerSection, headerSectionTexture);

        objectSection.x = 0;
        objectSection.y = 100;
        objectSection.width = Screen.width;
        objectSection.height = 200;
        GUI.DrawTexture(objectSection, objectSectionTexture);

        modeSction.x = 0;
        modeSction.y = 50;
        modeSction.width = Screen.width;
        modeSction.height = 100;

    }

    private void DrawHeader()
    {
        GUILayout.BeginArea(headerSection);
        GUILayout.Label("TEST SECTION");
        GUILayout.EndArea();

    }
    private void DrawMode()
    {
        GUILayout.BeginArea(modeSction);
        GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
        if (GUI.Button(new Rect(10,0,100,50),"Tile Mode"))
        {
            ModeToggle = false;
        }
        if (GUI.Button(new Rect(120, 0, 100, 50), "Object Mode"))
        {
            ModeToggle = true;

        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void DrawContent()
    {
        GUILayout.BeginArea(objectSection);


        list = new(Resources.LoadAll<GameObject>(path));
        List<GUIContent> contentsList = new();

        if(ModeToggle == true)
        {
            foreach (GameObject obj in list)
            {
                Texture2D texture = AssetPreview.GetAssetPreview(obj);
                contentsList.Add(new GUIContent(texture));
            }

            //objectSectionPot = GUILayout.SelectionGrid(objectSectionPot, contentsList.ToArray(), 6,guiStyle);


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
                    Debug.Log($"{index}, {list[index]}");
                }
                GUILayout.Label(list[index].name, _GUIStyle_Text);

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
        else { Debug.Log("Tile"); }


       
    }

    #endregion



}
