using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_Base : MonoBehaviour
{
    public bool IsEnabled { get; private set; } = true;

    public abstract void OnEnable();

    protected virtual void OpenUI()
    {
        IsEnabled = true;
        gameObject.SetActive(true);
    }

    protected virtual void CloseUI()
    {
        gameObject.SetActive(false);
        IsEnabled = false;
    }
}
