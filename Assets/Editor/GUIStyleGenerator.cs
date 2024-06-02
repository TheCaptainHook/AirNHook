using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIStyleGenerator
{
    GUIStyle style;






    public GUIStyle Generator(int fontSize, Color fontColor, TextAnchor textAnchor, float fixedHeight = 0, float fixedWidth = 0)
    {
        style = new GUIStyle();
        style.fontSize = fontSize;
        style.fixedHeight = fixedHeight;
        style.fixedWidth = fixedWidth;
        style.normal.textColor = fontColor;
        style.alignment = textAnchor;

        return style;
    }
}
