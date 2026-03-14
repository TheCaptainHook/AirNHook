using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TUTORIAL_CODE
{
    EX_1 = 1,
    EX_2 = 2,
    EX_3 = 3,
}
public class Tutorial_Drone : BuildObj, IInteractable
{
    public TUTORIAL_CODE tutorialCode;
    private ObjectTypeEnum type = ObjectTypeEnum.Interaction;


    public Vector3 btnOffset;
    private UI_Base _E_Btn;
    #region Get,Set
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ObjectData))
        {
            return (T)(object)new ObjectData(id, transform.position, transform.rotation, transform.localScale, (int)tutorialCode);
        }

        return default(T);
    }
    public override void SetData<T>(T data)
    {
        base.SetData(data);
    }
    public override void SetData(ObjectData data)
    {
        base.SetData(data);
        tutorialCode = (TUTORIAL_CODE)data.tutorialCode;
    }

    #endregion

    private void Open_Tutorial(TUTORIAL_CODE code)
    {
        // var ui = Managers.UI.GetUI<UI_Tutorial>().TryGetComponent(out UI_Tutorial component) ? component : null;
        var ui = Managers.UI.ShowUI<UI_Tutorial>().TryGetComponent(out UI_Tutorial component) ? component : null;
        if (ui == null) return;

        if (!ui.onActive)
        {
            Debug.Log($"[1] Open UI_Tutorial, {code}");
            ui.Setting(code);     
        }
    }


    #region  Interactable
    public void Interaction(Transform accessor)
    {
        HideEButton();
        Open_Tutorial(tutorialCode);
    }

    public bool CanInteract()
    {
        return true;
    }

    public bool Interacting(bool value, GameObject player)
    {
        return true;
    }

    public ObjectTypeEnum GetObjectType()
    {
        return type;
    }

    public void ShowEButton()
    {
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position = transform.position + btnOffset;
    }

    public void HideEButton()
    {
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }
    #endregion

}
