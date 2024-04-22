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
    [SerializeField] TMP_Dropdown linkedBtn_Dropdown;
    [SerializeField] TMP_Dropdown linkedLever_Dropdown;
    [SerializeField] TMP_InputField conditionInputField;


    [SerializeField] List<GameObject> linkBtnList;
    [SerializeField] List<GameObject> linkLeverList;

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
        List<TMP_Dropdown.OptionData> btnOptions = new List<TMP_Dropdown.OptionData>();
        List<TMP_Dropdown.OptionData> leverOptions = new List<TMP_Dropdown.OptionData>();
      
        idInputField.text = bAD.linkId.ToString();

        linkBtnList = GetLinkBtns();
        linkLeverList = GetLinkLever();

        for (int i = 0; i < bAD.buttonActivatedBtnList.Count; i++)
        {
            btnOptions.Add(new TMP_Dropdown.OptionData($"{i}.Position :[{bAD.buttonActivatedBtnList[i].x},{bAD.buttonActivatedBtnList[i].y}"));
        }
        linkedBtn_Dropdown.options = btnOptions;
        linkedBtn_Dropdown.RefreshShownValue();


        for (int i = 0; i < bAD.leverBodyPotiionList.Count; i++)
        {
            leverOptions.Add(new TMP_Dropdown.OptionData($"{i}.Position :[{bAD.leverBodyPotiionList[i].x},{bAD.leverBodyPotiionList[i].y}"));
        }
        linkedLever_Dropdown.options = leverOptions;
        linkedLever_Dropdown.RefreshShownValue();

        conditionInputField.text = bAD.activeRequirAmount.ToString();
    }

    public void ConditionInputFieldClamping()
    {
        int max = bAD.buttonActivatedBtnList.Count + bAD.leverBodyPotiionList.Count;
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
                if (btn.linkDoorList[0].linkId == bAD.linkId)
                {
                    list.Add(btn.gameObject);
                }
            }
        }
        return list;

    }

    private List<GameObject> GetLinkLever()
    {
        List<GameObject> list = new();

        foreach (Transform tr in MapEditor.Instance.dontSaveObjectTransform)
        {
            LeverBody lever = tr.GetComponent<LeverBody>();
            if (lever != null)
            {
                if (lever.linkId == bAD.linkId)
                {
                    list.Add(lever.gameObject);
                }
            }
        }
        return list;

    }
    public void ChangeLinkId()
    {
        bAD.buttonActivatedBtnList.Clear();
        bAD.leverBodyPotiionList.Clear();
        linkBtnList.Clear();
        linkLeverList.Clear();

        foreach(Transform tf in MapEditor.Instance.dontSaveObjectTransform)
        {
            ButtonActivated ba = tf.gameObject.GetComponent<ButtonActivated>();
            LeverBody lb = tf.GetComponent<LeverBody>();

            if(ba != null)
            {
                if(ba.linkId == int.Parse(idInputField.text))
                {
                    bAD.buttonActivatedBtnList.Add(ba.curPosition);
                    linkBtnList.Add(ba.gameObject);
                    //버튼 라인렌더러 함수 재실행,
                }
                else
                {
                    //버튼 라인렌더러 함수 재실행
                }
               
            }else if(lb != null)
            {
                if (lb.linkId == int.Parse(idInputField.text))
                {
                    bAD.leverBodyPotiionList.Add(lb.curPosition);
                    linkLeverList.Add(lb.gameObject);
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
