using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UI_EventEchoDialogue))]
public class UI_EventEchoDialogue_Editor : Editor
{
    UI_EventEchoDialogue ui_EventEchoDialogue;
    // Coroutine coroutine;
    // private void OnEnable()
    // {
    //     ui_EventEchoDialogue = (UI_EventEchoDialogue)target;
    //     coroutine = ui_EventEchoDialogue.mark_1_EffectCoroutine;
    // }
    // public override void OnInspectorGUI()
    // {
    //     base.OnInspectorGUI();
    //     if (GUILayout.Button("TEST"))
    //     {
    //         ui_EventEchoDialogue.SetDialogue("Lorem [Ipsum] is <simply> dummy /1");
    //     }

    //     if (GUILayout.Button("Reset"))
    //     {
    //         ui_EventEchoDialogue.Reset();
    //     }
    // }

}
