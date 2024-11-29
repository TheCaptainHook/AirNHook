
using UnityEngine;

public class TESTBOX : MonoBehaviour
{

    UI_Dialogue uI_Dialogue ;
   
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I)){
            if(uI_Dialogue == null || !uI_Dialogue.gameObject.activeSelf){
                uI_Dialogue = Managers.UI.ShowUI<UI_Dialogue>().GetComponent<UI_Dialogue>();
            }
            StartCoroutine(uI_Dialogue.TutorialClearDialogue());
            

        }
        if(Input.GetKeyDown(KeyCode.O)){
           if(uI_Dialogue == null || !uI_Dialogue.gameObject.activeSelf){
                uI_Dialogue = Managers.UI.ShowUI<UI_Dialogue>().GetComponent<UI_Dialogue>();
            }
            uI_Dialogue.StartDialogue(105);
        }
        if(Input.GetKeyDown(KeyCode.P)){
           if(uI_Dialogue == null || !uI_Dialogue.gameObject.activeSelf){
                uI_Dialogue = Managers.UI.ShowUI<UI_Dialogue>().GetComponent<UI_Dialogue>();
            }
            uI_Dialogue.StartDialogue(104);
        }
    }
}
