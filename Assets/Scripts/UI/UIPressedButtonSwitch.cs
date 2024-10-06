using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPressedButtonSwitch : MonoBehaviour
{
    [SerializeField]
    private Button[] buttons;

    [SerializeField] 
    private Button disabledButton;

    public void SetAllButtonsInteractable()
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }

    public void OnButtonClicked(Button clickedButton)
    {
        int buttonIndex = System.Array.IndexOf(buttons, clickedButton);

        if (buttonIndex == -1)
            return;

        SetAllButtonsInteractable();

        clickedButton.interactable = false;
    }
    
    public void StartedAsDisabled()
    {
        disabledButton.interactable = false;
    }
    
    private void OnDisable()
    {
        SetAllButtonsInteractable();
        StartedAsDisabled();
    }

    
}
