using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InteractionLeverInfo : UI_Base
{
    GameObject curObject;
    LeverBody lb;
    [SerializeField] Button confirmBtn;
    [SerializeField] Button closeBtn;
    [SerializeField] TMP_Dropdown dropdown;

    public bool firstOption;


    // private void Awake()
    // {
    //     confirmBtn.onClick.AddListener(Confirm);
    //     closeBtn.onClick.AddListener(CloseUI);
    // }

    public override void OnEnable()
    {
        OpenUI();
    }
    // public void SetDateInfo()
    // {
    //     //interactionDoorList = new();

    //     List<TMP_Dropdown.OptionData> options = new();
    //     List<int> check = new();
    //     foreach (Transform tr in MapEditor.Instance.interactionObjectTransform)
    //     {
    //         TMP_Dropdown.OptionData tdod = new TMP_Dropdown.OptionData($"{tr.GetComponent<ButtonActivatedDoor>().linkId}");
    //         int id = tr.GetComponent<ButtonActivatedDoor>().linkId;
    //         if (!check.Contains(id))
    //         {
    //             options.Add(tdod);
    //             check.Add(id);
    //         }

    //     }
    //     dropdown.options = options;
    //     dropdown.RefreshShownValue();


    //     for (int i = 0; i < dropdown.options.Count; i++)
    //     {
    //         TMP_Dropdown.OptionData option = dropdown.options[i];
    //         if (int.Parse(option.text) == lb.linkId)
    //         {
    //             dropdown.value = i;
    //             break;
    //         }
    //     }


    // }

    // private void Confirm()
    // {
    //     MapEditor.Instance.placeMentSystem.onInteraction = true;
    //     if (CheckButtonInteractionDoor())
    //     {
    //         if (firstOption)
    //         {
    //             GameObject obj = Instantiate(curObject);
    //             MapEditor.Instance.placeMentSystem.first_holdingObj = obj;
    //             obj.GetComponent<BuildObj>().TurnOff();

    //             obj.GetComponent<LeverBody>().linkId = int.Parse(dropdown.options[dropdown.value].text);
    //             Destroy(gameObject);
    //         }
    //         else
    //         {
    
    //             if (lb.linkId != int.Parse(dropdown.options[dropdown.value].text))
    //             {
    //                 lb.linkId = int.Parse(dropdown.options[dropdown.value].text);

    //             }

    //             Destroy(gameObject);
    //         }
    //     }


    // }

    // private bool CheckButtonInteractionDoor()
    // {
    //     foreach (Transform tr in MapEditor.Instance.interactionObjectTransform)
    //     {
    //         if (tr.GetComponent<ButtonActivatedDoor>()) { return true; }

    //     }

    //     return false;
    // }
    protected override void CloseUI()
    {
        MapEditor.Instance.placeMentSystem.onInteraction = true;
        if (firstOption)
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // public override void SetCurObject(GameObject obj)
    // {
    //     base.SetCurObject(obj);

    //     curObject = obj;
    //     lb = curObject.GetComponent<LeverBody>();
    //     SetDateInfo();
    // }
}
