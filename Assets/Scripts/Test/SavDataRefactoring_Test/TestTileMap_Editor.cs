using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestTileMap))]
public class TestTileMap_Editor : Editor
{
    private TestTileMap testTileMap;
    private void OnEnable()
    {
        testTileMap = (TestTileMap)target;
    }

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

       if(GUILayout.Button("Save"))
        {
            testTileMap.Save();
        }

        if (GUILayout.Button("미리보기"))
        {
            testTileMap.Preview();
        }
        if (GUILayout.Button("Clear"))
        {
            testTileMap.Clear();
        }
        if (GUILayout.Button("Draw Tile"))
        {
            testTileMap.DrawTile();
        }
    }
}
