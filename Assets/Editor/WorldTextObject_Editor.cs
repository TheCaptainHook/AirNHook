using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldTextObject))]
public class WorldTextObject_Editor : Editor
{
    WorldTextObject _WorldTextObject;
    SerializedProperty _SerializedProperty_Size;
    SerializedProperty _SerializedProperty_Text;

    private Vector2 previousSize;
    private string previousText;

    private void OnEnable()
    {
        _WorldTextObject = target as WorldTextObject;
        _SerializedProperty_Size = new SerializedObject(_WorldTextObject).FindProperty("size");
        previousSize = _SerializedProperty_Size.vector2Value;

        _SerializedProperty_Text = new SerializedObject(_WorldTextObject).FindProperty("mainText");
        previousText = _SerializedProperty_Text.stringValue;

    }


    public override void OnInspectorGUI()
    {

        // Update the serialized properties first
        _SerializedProperty_Size.serializedObject.Update();
        _SerializedProperty_Text.serializedObject.Update();

        GUILayout.BeginVertical();

        // Draw fields for size and text
        _WorldTextObject.size = EditorGUILayout.Vector2Field("Width and Height", _WorldTextObject.size);
        _WorldTextObject.mainText = EditorGUILayout.TextField("Text", _WorldTextObject.mainText);

        GUILayout.EndVertical();

        // Apply any changes made in the inspector
        if (GUI.changed)
        {
            _SerializedProperty_Size.vector2Value = _WorldTextObject.size;
            _SerializedProperty_Text.stringValue = _WorldTextObject.mainText;
        }

        // Apply changes to the serialized properties
        _SerializedProperty_Size.serializedObject.ApplyModifiedProperties();
        _SerializedProperty_Text.serializedObject.ApplyModifiedProperties();

        // Call the update method to handle size and text updates
        OnEditorUpdate();
    }

    private void OnEditorUpdate()
    {
        _SerializedProperty_Size.serializedObject.Update();
        _SerializedProperty_Text.serializedObject.Update();

        Vector2 curSize = _SerializedProperty_Size.vector2Value;
        string curText = _SerializedProperty_Text.stringValue;

        if(curSize != previousSize)
        {
           _WorldTextObject.SetSize(curSize);
            previousSize = curSize;
        }
        if(curText != previousText)
        {
            _WorldTextObject.SetText(previousText);
            previousText = curText;
        }
    }

}
