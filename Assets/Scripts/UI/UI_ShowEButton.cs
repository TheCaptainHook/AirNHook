using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ShowEButton : UI_Base
{
    public override void OnEnable()
    {
        OpenUI();
    }

    private void OnDisable()
    {
        CloseUI();
    }
}

