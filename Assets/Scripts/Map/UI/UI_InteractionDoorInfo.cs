using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UI_InteractionDoorInfo : UI_Base
{
    GameObject curObject;
    ButtonActivatedDoor bAD;

    [SerializeField] Button confirmBtn;
    [SerializeField] Button closeBtn;

    [SerializeField] TMP_InputField idInputField;
    [SerializeField] TMP_Dropdown linked_Dropdown;
    [SerializeField] TMP_InputField conditionInputField;


    [SerializeField] List<GameObject> linkBtnsList;

    private void Awake()
    {
        confirmBtn.onClick.AddListener(Confirm);
        closeBtn.onClick.AddListener(CloseUI);
    }

    public override void OnEnable()
    {
        OpenUI();
    }

    public override void SetCurObject(GameObject obj)
    {
        base.SetCurObject(obj);

        curObject = obj;
        bAD = curObject.GetComponent<ButtonActivatedDoor>();
        SetDataInfo();
    }




    private void SetDataInfo()
    {
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        idInputField.text = bAD.linkId.ToString();
        linkBtnsList = GetLinkBtns();
        for (int i = 0; i < bAD.buttonActivatedBtnList.Count; i++)
        {
            options.Add(new TMP_Dropdown.OptionData($"{i}.Position :[{bAD.buttonActivatedBtnList[i].x},{bAD.buttonActivatedBtnList[i].y}"));
        }
        linked_Dropdown.options = options;
        linked_Dropdown.RefreshShownValue();

        conditionInputField.text = bAD.activeRequirAmount.ToString();
    }

    public void ConditionInputFieldClamping()
    {
        int max = bAD.buttonActivatedBtnList.Count;
        int conditionInputFiledText = int.Parse(conditionInputField.text);
        conditionInputFiledText = Mathf.Clamp(conditionInputFiledText, 0, max);
        conditionInputField.text = conditionInputFiledText.ToString();
    }


    private List<GameObject> GetLinkBtns()
    {
        List<GameObject> list = new();

        foreach(Transform tr in MapEditor.Instance.dontSaveObjectTransform)
        {
            ButtonActivated btn = tr.GetComponent<ButtonActivated>();
            if(btn != null)
            {
                if(btn.linkDoor == bAD)
                {
                    list.Add(btn.gameObject);
                }
            }
        }
        return list;

    }


    public void ChangeLinkId()
    {
        bAD.buttonActivatedBtnList.Clear();
        linkBtnsList.Clear();
        foreach(Transform tf in MapEditor.Instance.dontSaveObjectTransform)
        {
            ButtonActivated ba = tf.gameObject.GetComponent<ButtonActivated>();
            if(ba != null)
            {
                if(ba.linkId == int.Parse(idInputField.text))
                {
                    bAD.buttonActivatedBtnList.Add(ba.curPosition);
                    linkBtnsList.Add(ba.gameObject);
                    //버튼 라인렌더러 함수 재실행,
                }
                else
                {
                    //버튼 라인렌더러 함수 재실행
                }
               
            }
        }
       
    }

    private void Confirm()
    {
        MapEditor.Instance.placeMentSystem.onInteraction = true;
        curObject.GetComponent<ButtonActivatedDoor>().linkId = int.Parse(idInputField.text);
        curObject.GetComponent<ButtonActivatedDoor>().activeRequirAmount = int.Parse(conditionInputField.text);
        CloseUI();
    }


    protected override void CloseUI()
    {
        MapEditor.Instance.placeMentSystem.onInteraction = true;
        base.CloseUI();
    }
}
