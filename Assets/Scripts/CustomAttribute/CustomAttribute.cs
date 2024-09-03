using UnityEngine;
using UnityEditor;


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
        EditorGUI.DrawRect(new Rect(0,0,position.width,3),Color.cyan);
        GUIStyle textStyle = new GUIStyle()
        {
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold,
            fontSize = 15,
            normal = new GUIStyleState() { textColor = CustomHeader.headerColor }
        };

        EditorGUI.LabelField(new Rect(0,0,position.width,GetHeight()),CustomHeader.header,textStyle);
        GUILayout.Space(10);
    }

}

#endregion

