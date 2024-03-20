using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Test_UI_Input : MonoBehaviour
{
    public static Test_UI_Input Instance;
    
    private bool _emoteOnCoolDown;
    private void Start()
    {
        Instance = this;
        _emoteOnCoolDown = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapeKeyPressed();
        }

        if (Input.GetKeyDown((KeyCode.Tab)))
        {
            OnTabKeyPressed();
        }
    }
    
    private void OnEscapeKeyPressed()
    {
        ShowOptionUI();
        Debug.Log("ESC 키");
    }

    private void OnTabKeyPressed()
    {
        ShowEmoteWheel();
        Debug.Log("탭키");
    }

    private void ShowOptionUI()
    {
        if(!Managers.UI.IsActive<UI_Option>())
        {
            Managers.UI.ShowUI<UI_Option>();
        }
        else
        {
            Managers.UI.HideUI<UI_Option>();
        }
    }

    private void ShowEmoteWheel()
    {
        if (_emoteOnCoolDown == false)
        {
            if(!Managers.UI.IsActive<UI_EmoteWheel>())
            {
                Managers.UI.ShowUI<UI_EmoteWheel>();
            }
            else
            {
                Managers.UI.HideUI<UI_EmoteWheel>();
            }
        }
    }

    public void UsingEmote()
    {
        _emoteOnCoolDown = true;
        StartCoroutine(C0_EmoteCoolDown());
    }
    
    private IEnumerator C0_EmoteCoolDown()
    {
        yield return new WaitForSeconds(3.5f);
        _emoteOnCoolDown = false;
        Debug.Log("EmoteCOEnds");
    }
}
