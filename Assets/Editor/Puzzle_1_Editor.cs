using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor(typeof(Puzzle_1_Helper))]
public class Puzzle_1_Editor : Editor
{
    Puzzle_1 puzzle_1;
    Puzzle_1_Helper helper;


    Texture2D checkBtnTexture;
    string checkBtnPath = "Assets/Resources/Arts/Icons/check-mark.png";

    #region Style
    private GUIStyle buttonStyle;
    #endregion


    private void OnEnable(){
        checkBtnTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(checkBtnPath);
    }
   


    public override void OnInspectorGUI()
    {   
     
    }


}
