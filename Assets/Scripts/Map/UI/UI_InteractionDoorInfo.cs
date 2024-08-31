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

    // private void Awake()
    // {
    //     confirmBtn.onClick.AddListener(Confirm);
    //     closeBtn.onClick.AddListener(CloseUI);
    // }

    public override void OnEnable()
    {
        OpenUI();
    }

    // public override void SetCurObject(GameObject obj)
    // {
    //     base.SetCurObject(obj);

    //     curObject = obj;
    //     bAD = curObject.GetComponent<ButtonActivatedDoor>();
    //     // SetDataInfo();
    // }




    // private void SetDataInfo()
    // {
    //     List<TMP_Dropdown.OptionData> btnOptions = new List<TMP_Dropdown.OptionData>();
    //     List<TMP_Dropdown.OptionData> leverOptions = new List<TMP_Dropdown.OptionData>();
      
    //     idInputField.text = bAD.linkId.ToString();

    //     linkBtnList = GetLinkBtns();
    //     linkLeverList = GetLinkLever();

    //     int i = 0;
    //     foreach(Transform tr in MapEditor.Instance.buttonActivatedObjectTransform)
    //     {
    //         ButtonActivated ba = tr.GetComponent<ButtonActivated>();
    //         if(ba != null && bAD.linkId == ba.linkId)
    //         {
    //             i++;
    //             btnOptions.Add(new TMP_Dropdown.OptionData($"{i}.Btn :[{ba.transform.position.x},{ba.transform.position.y}]"));
    //         }
    //     }
    //     linkedBtn_Dropdown.options = btnOptions;
    //     linkedBtn_Dropdown.RefreshShownValue();

    //     i = 0;

    //     foreach (Transform tr in MapEditor.Instance.interactionObjectTransform)
    //     {
    //         LeverBody lb = tr.GetComponent<LeverBody>();
    //         if (lb != null && bAD.linkId == lb.linkId)
    //         {
    //             i++;
    //             leverOptions.Add(new TMP_Dropdown.OptionData($"{i}.Btn :[{lb.transform.position.x},{lb.transform.position.y}]"));
    //         }
    //     }
    //     linkedLever_Dropdown.options = leverOptions;
    //     linkedLever_Dropdown.RefreshShownValue();

    //     conditionInputField.text = bAD.activeRequirAmount.ToString();
    // }

    // public void ConditionInputFieldClamping()
    // {
    //     int max = 0;

    //     foreach(Transform tr in MapEditor.Instance.interactionObjectTransform)
    //     {
    //         if(tr.GetComponent<ButtonActivated>() || tr.GetComponent<LeverBody>())
    //         {
    //             if (tr.GetComponent<ButtonActivated>().linkId == bAD.linkId || tr.GetComponent<LeverBody>().linkId == bAD.linkId)
    //             {
    //                 max++;
    //             }
    //         }

    //     }

    //     int conditionInputFiledText = int.Parse(conditionInputField.text);
    //     conditionInputFiledText = Mathf.Clamp(conditionInputFiledText, 0, max);
    //     conditionInputField.text = conditionInputFiledText.ToString();
    // }


    // private List<GameObject> GetLinkBtns()
    // {
    //     List<GameObject> list = new();

    //     foreach(Transform tr in MapEditor.Instance.interactionObjectTransform)
    //     {
    //         ButtonActivated btn = tr.GetComponent<ButtonActivated>();
    //         if(btn != null)
    //         {
    //             if (btn.linkId == bAD.linkId)
    //             {
    //                 list.Add(btn.gameObject);
    //             }
    //         }
    //     }
    //     return list;

    // }

    // private List<GameObject> GetLinkLever()
    // {
    //     List<GameObject> list = new();

    //     foreach (Transform tr in MapEditor.Instance.dontSaveObjectTransform)
    //     {
    //         LeverBody lever = tr.GetComponent<LeverBody>();
    //         if (lever != null)
    //         {
    //             if (lever.linkId == bAD.linkId)
    //             {
    //                 list.Add(lever.gameObject);
    //             }
    //         }
    //     }
    //     return list;

    // }
    //public void ChangeLinkId()
    //{

    //    foreach(Transform tf in MapEditor.Instance.interactionObjectTransform)
    //    {
    //        ButtonActivated ba = tf.gameObject.GetComponent<ButtonActivated>();
    //        LeverBody lb = tf.GetComponent<LeverBody>();

    //        if(ba != null)
    //        {
    //            if(ba.linkId == int.Parse(idInputField.text))
    //            {
    //                bAD.buttonActivatedBtnList.Add(ba.curPosition);
    //                linkBtnList.Add(ba.gameObject);
    //                //버튼 라인렌더러 함수 재실행,
    //            }
    //            else
    //            {
    //                //버튼 라인렌더러 함수 재실행
    //            }
               
    //        }else if(lb != null)
    //        {
    //            if (lb.linkId == int.Parse(idInputField.text))
    //            {
    //                bAD.leverBodyPotiionList.Add(lb.curPosition);
    //                linkLeverList.Add(lb.gameObject);
    //                //버튼 라인렌더러 함수 재실행,
    //            }
    //            else
    //            {
    //                //버튼 라인렌더러 함수 재실행
    //            }
    //        }



    //    }
       
    //}

    // private void Confirm()
    // {
    //     //if(curObject.GetComponent<ButtonActivatedDoor>().linkId != int.Parse(idInputField.text))
    //     //{

    //     //} // todo 0602 if change link id, all check interaction transfrom obj and change same linkid activate obj.


    //     MapEditor.Instance.placeMentSystem.onInteraction = true;
    //     curObject.GetComponent<ButtonActivatedDoor>().linkId = int.Parse(idInputField.text);
    //     curObject.GetComponent<ButtonActivatedDoor>().activeRequirAmount = int.Parse(conditionInputField.text);
    //     Destroy(gameObject);
    // }


    // protected override void CloseUI()
    // {
    //     MapEditor.Instance.placeMentSystem.onInteraction = true;
    //     Destroy(gameObject);
    // }
}

