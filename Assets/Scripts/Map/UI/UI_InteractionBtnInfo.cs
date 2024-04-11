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


        for (int i = 0; i < dropdown.options.Count; i++)
        {
            TMP_Dropdown.OptionData option = dropdown.options[i];
            if (int.Parse(option.text) == bA.linkId)
            {
                dropdown.value = i;
                break;
            }
        }


    }


    private void Confirm()
    {
        MapEditor.Instance.placeMentSystem.onInteraction = true;
        if (firstOption)
        {
            GameObject obj = Instantiate(curObject);
            MapEditor.Instance.placeMentSystem.first_holdingObj = obj;
            obj.GetComponent<BuildObj>().TurnOff();

            obj.GetComponent<ButtonActivated>().linkId = int.Parse(dropdown.options[dropdown.value].text);
            Destroy(gameObject);
        }
        else
        {
            if(bA.linkId != int.Parse(dropdown.options[dropdown.value].text)){
                bA.linkDoor.buttonActivatedBtnList.Remove(bA.curPosition);
                bA.linkDoor = null;
                bA.linkId = int.Parse(dropdown.options[dropdown.value].text);
                bA.LinkDoor();
            }
           
            CloseUI();
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

    public override void SetCurObject(GameObject obj)
    {
        base.SetCurObject(obj);

        curObject = obj;
        Debug.Log(obj.name);
        bA = curObject.GetComponent<ButtonActivated>();
        SetDateInfo();
    }








}
