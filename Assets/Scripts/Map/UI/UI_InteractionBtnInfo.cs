using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_InteractionBtnInfo : UI_Base
{
    GameObject curObject;
    ButtonActivated bA;
    [SerializeField] Button confirmBtn;
    [SerializeField] Button closeBtn;
    [SerializeField] TMP_Dropdown dropdown;


    public bool firstOption;

    List<GameObject> interactionDoorList;


    private void Awake()
    {
        confirmBtn.onClick.AddListener(Confirm);
        closeBtn.onClick.AddListener(CloseUI);
    }

    public override void OnEnable(){ OpenUI(); }



    public void SetDateInfo()
    {
        interactionDoorList = new();

        List<TMP_Dropdown.OptionData> options = new();
        foreach(Transform tr in MapEditor.Instance.interactionObjectTransform)
        {
            interactionDoorList.Add(tr.gameObject);
            options.Add(new TMP_Dropdown.OptionData($"{tr.GetComponent<ButtonActivatedDoor>().linkId}"));
        }
        dropdown.options = options;
        dropdown.RefreshShownValue();
    }


    private void Confirm()
    {
        MapEditor.Instance.placeMentSystem.onInteraction = true;
        if (firstOption)
        {
            Destroy(gameObject);
        }
        else
        {

        }
       
        
    }

    protected override void CloseUI()
    {
        MapEditor.Instance.placeMentSystem.onInteraction = true;
        if (firstOption)
        {
            Destroy(gameObject);
        }
        else
        {
            base.CloseUI();
        }
        
    }

    public void SetCurObject(GameObject obj)
    {
        curObject = obj;
        bA = curObject.GetComponent<ButtonActivated>();
        SetDateInfo();
    }








}
