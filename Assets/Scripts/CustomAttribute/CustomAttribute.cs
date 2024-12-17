#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


#region  HEADER

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class CustomHeaderAttribute : PropertyAttribute
{
    
    public float spaceHeight;
    public float lineHeight;

    public readonly string header;
    public readonly Color headerColor;

    public CustomHeaderAttribute(string header, float r = 31f/255f, float g=222f/255f, float b=38f/255f)
    {
        this.header = header;
        this.headerColor = new Color(r, g, b);
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CustomHeaderAttribute))]
public class CustomHeaderDrawer : DecoratorDrawer
{
     private CustomHeaderAttribute CustomHeader => (CustomHeaderAttribute)attribute;

    public override float GetHeight()
    {
        return base.GetHeight() + 20;
    }

    public override void OnGUI(Rect position)
    {

        Rect lineRect = new Rect(position.x, position.y, position.width, 3);
        EditorGUI.DrawRect(lineRect, Color.cyan);

        // EditorGUI.DrawRect(new Rect(0,0,position.width,3),Color.cyan);
        GUIStyle textStyle = new GUIStyle()
        {
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold,
            fontSize = 15,
            normal = new GUIStyleState() { textColor = CustomHeader.headerColor }
        };
        Rect textRect = new Rect(position.x, position.y + 5, position.width, position.height);
        EditorGUI.LabelField(textRect, CustomHeader.header, textStyle);
        // EditorGUI.LabelField(new Rect(0,0,position.width,GetHeight()),CustomHeader.header,textStyle);
        // GUILayout.Space(10);
    }

}
#endif
#endregion

#region  ReadOnly
// ReadOnly 어트리뷰트 정의
public class ReadOnlyAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;  // 인스펙터에서 편집을 불가능하게 만듦
        EditorGUI.PropertyField(position, property, label);
        // EditorGUILayout.Space(10);
        GUI.enabled = true;  // 다시 편집 가능하게 복원
    }
}
#endif
#endregion

