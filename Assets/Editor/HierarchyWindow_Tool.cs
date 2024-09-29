
using UnityEngine;
using UnityEditor;
public class HierarchyWindow_Tool :EditorWindow

{
    GUIStyleGenerator _GUIStyleGenerator = new GUIStyleGenerator();


    Texture2D headerSectionTexture;
    Color headerSectionColor = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f);

    Texture2D mainSectionTexture;
    Color mainSectionColor = new Color(223f/255f,245f/255f,131f/255f,1f);

    [Header("Section")]
    Rect headerSction;
    Rect mainSection;
    Rect mainInObjectSection;
    Rect mainInInteractedObjectSection;

    [Header("GUI Style")]
    GUIStyle _GUIStyle_Text;
    GUIStyle _GUIStyle_TitleText;


    [MenuItem("Window/MapEditor Tool/Hierarchy Check Tool")]
    public static void ShowWindow()
    {
        HierarchyWindow_Tool _HWT = (HierarchyWindow_Tool)GetWindow(typeof(HierarchyWindow_Tool));
        _HWT.minSize = new Vector2(350, 500);
        _HWT.maxSize = new Vector2(350, 700);
        _HWT.Show();
    }





    private void OnEnable()
    {
        InitTexture();
        // Init();
    }


    private void OnGUI()
    {
        DrawLayout();

        DrawHeader();

    }



    #region Init

    private void InitTexture()
    {
        headerSectionTexture = new Texture2D(1, 1);
        headerSectionTexture.SetPixel(0, 0, headerSectionColor);
        headerSectionTexture.Apply();

        mainSectionTexture = new Texture2D(1, 1);
        mainSectionTexture.SetPixel(0, 0, mainSectionColor);
        mainSectionTexture.Apply();

    }
    // private void Init()
    // {
    //     _GUIStyle_TitleText = _GUIStyleGenerator.Generator(20, Color.green, TextAnchor.MiddleCenter,80,350);
    // }
    #endregion


    #region   Draw

    private void DrawLayout()
    {
        headerSction = new Rect(0, 0, 350, 80);
        GUI.DrawTexture(headerSction, headerSectionTexture);
        mainSection = new Rect(0, 90, 350, 350);
        GUI.DrawTexture(mainSection, mainSectionTexture);
    }

    private void DrawHeader()
    {
        GUILayout.BeginArea(headerSction);
        GUILayout.Label("Hierarchy Window",_GUIStyle_TitleText);


        GUILayout.EndArea();
    }



    private void DrawMain()
    {
        GUILayout.BeginArea(mainSection);

        GUILayout.EndArea();
    }
    #endregion


}
