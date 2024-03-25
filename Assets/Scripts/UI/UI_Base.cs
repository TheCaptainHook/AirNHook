using TMPro;
using UnityEngine;

public abstract class UI_Base : MonoBehaviour
{
    public bool IsEnabled { get; private set; } = true;

    protected virtual void Start()
    {
        SetLanguage();
    }

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

    public virtual void SetLanguage() { }
    
    public void SetSentence(TMP_Text target, int id)
    {
        var text = Managers.Data.language.GetSentence(id);
        if (text != " ")
            target.text = text;
    }
}
