using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Test_UI_Input : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapeKeyPressed();
        }
    }
    
    private void OnEscapeKeyPressed()
    {
        ShowOptionUI();
        Debug.Log("ESC 키");
    }

    private void ShowOptionUI()
    {
        if(!Managers.UI.IsActive<UI_Option>())
        {
            Managers.UI.ShowUI<UI_Option>();
        }
        else
        {
            Debug.Log("HideUI");
            Managers.UI.HideUI<UI_Option>();
        }
    }
    
}
