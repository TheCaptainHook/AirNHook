using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor(typeof(Puzzle_1_Helper))]
public class Puzzle_1_Editor : Editor
{
    enum Mode
    {
        Parts,
        Item,
        Hint
    }



    Puzzle_1 puzzle_1;
    Puzzle_1_Helper helper;
    Mode mode;

    Texture2D checkBtnTexture;
    string checkBtnPath = "Assets/Resources/Arts/Icons/check-mark.png";
    GUIContent checkContent;


    #region Style
    private GUIStyle buttonStyle;

    private Color partsNumberColor;
    private Color itemNumberColor;
    private Color hintNumberColor;
    #endregion


    private void OnEnable(){
        checkBtnTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(checkBtnPath);
        helper = (Puzzle_1_Helper)target;

        helper.Init();
        puzzle_1 = helper.GetComponent<Puzzle_1>();

        checkContent = new GUIContent(checkBtnTexture);
    }
    private void OnDisable()
    {
        helper.Destory();
    }


    private void CheckPartsItemHint()
    {
        partsNumberColor = helper.GetPartsField().number > 0 ? Color.green : Color.red;
        itemNumberColor = (helper.GetItemField() != helper.GetPartsField().number) ?
            Color.red :
             (helper.GetItemField() == helper.GetPartsField().number) && helper.GetItemField() != 0 ? Color.green : Color.red; 

    }
    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        CheckPartsItemHint();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
            GUILayout.Label("Parts\t: ",GetGUIStyle_Label(Color.white));
            GUILayout.Label($"{helper.GetPartsField().number}", GetGUIStyle_Label(partsNumberColor));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            GUILayout.Label("Item\t: ", GetGUIStyle_Label(Color.white));
            GUILayout.Label($"{helper.GetItemField()}", GetGUIStyle_Label(itemNumberColor));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            GUILayout.Label("Hint\t: ", GetGUIStyle_Label(Color.white));
            GUILayout.Label($"X", GetGUIStyle_Label(hintNumberColor));
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();


        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        DrawHeader_Mode();


        switch (mode) 
        {
            case Mode.Parts:
                DrawSection_Parts();
                break;
            case Mode.Item:
                DrawSection_Item();
                break;
            case Mode.Hint:
                break;
        }

        //
       

    }

    #region Draw
    private void DrawHeader_Mode()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (Button("Parts"))
        {
            mode = Mode.Parts;
        }
        if (Button("Items"))
        {
            mode = Mode.Item;
        }
        if (Button("Hint"))
        {
            mode = Mode.Hint;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private void DrawSection_Parts()
    {
      
        EditorGUILayout.LabelField("Parts", GetGUIStyle_Label(Color.white, 20, FontStyle.Bold));

        EditorGUILayout.BeginVertical(new GUIStyle(GUI.skin.window));
        //+,- Button
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (Button("+", 20, 20))
        {
            helper.Add_Parts();
        }
        if (Button("-", 20, 20))
        {
            helper.RemoveParts();
        }

        var field = helper.GetPartsField();

        EditorGUILayout.EndHorizontal();
        //Draw Parts
        for(int i = 0; i < field.number; i++)
        {
            DrawObejctField(i,helper.GetParts(i));
        }

        EditorGUILayout.EndVertical();
    }
    private void DrawSection_Item()
    {
    
        EditorGUILayout.LabelField("Item", GetGUIStyle_Label(Color.white, 20, FontStyle.Bold));
        EditorGUILayout.BeginVertical(new GUIStyle(GUI.skin.window));
        //+,- Button
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (Button("+", 20, 20))
        {
            helper.Add_Item();
        }
        if (Button("-", 20, 20))
        {
            helper.Remove_Item();
        }
        EditorGUILayout.EndHorizontal();

        //Draw Item
        for(int i =0; i < helper.GetItemField(); i++)
        {
            DrawObejctField(i,helper.GetItem(i));
        }

        EditorGUILayout.EndVertical();
    }
    #endregion

    private bool Button(string title,int width = 80,int height = 25)
    {

        bool result = false;
        if (GUILayout.Button(title,GUILayout.Width(width), GUILayout.Height(height)))
        {
            result = true;
        }
        return result;
    }


    private GUIStyle GetGUIStyle_Label(Color color, int font_Size = 12, FontStyle font_Style = FontStyle.Normal)
    {

        return new GUIStyle(GUI.skin.label)
        {
            normal = { textColor = color },
            fontSize = font_Size,
            fontStyle = font_Style,
            hover = { textColor = color },
        };

    }

    private void DrawObejctField(int index,GameObject parts)
    {
        EditorGUILayout.ObjectField(
           $"{index}. Parts",
           parts,
           typeof(GameObject),
           true
           );
       
    }
}
