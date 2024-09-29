
using UnityEngine;



public class GUIStyleGenerator
{
    GUIStyle style;
    enum Skin{
        box,
        button,
        label,
        window
    }

    public GUIStyle Generator(string skin,int fontSize, Color fontColor , 
            TextAnchor textAnchor = TextAnchor.UpperLeft,
            FontStyle fontStyle = FontStyle.Normal,
             float fixedHeight = 0, 
             float fixedWidth = 0
             ){
        style = GetStyle(skin);
        style.fontSize = fontSize;
        style.fixedHeight = fixedHeight;
        style.fixedWidth = fixedWidth;
        style.normal.textColor = fontColor;
        style.fontStyle = fontStyle;
        style.alignment = textAnchor;

        return style;
    }


    private GUIStyle GetStyle(string skin){
        switch(skin){
            case "box":
            return new GUIStyle(GUI.skin.box);
            case "button":
            return new GUIStyle(GUI.skin.button);
             case "label":
            return new GUIStyle(GUI.skin.label);
             case "window":
            return new GUIStyle(GUI.skin.window);
            default:
            return new GUIStyle(GUI.skin.box);
        }
    }

 
}


