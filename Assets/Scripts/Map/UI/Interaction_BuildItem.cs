
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class Interaction_BuildItem : MonoBehaviour
{
    public GameObject buildObj;
    public Image image;
    Button button;
    private void Awake()
    {        
        button = GetComponent<Button>();
        button.onClick.AddListener(ChoiceItem);

    }


    void ChoiceItem()
    {
        if (MapEditor.Instance.placeMentSystem.onInteraction)
        {
            if(MapEditor.Instance.placeMentSystem.first_holdingObj != null)
            {
                Destroy(MapEditor.Instance.placeMentSystem.first_holdingObj);  
            }

            if (buildObj.GetComponent<BuildObj>().id == 306)
            {
                MapEditor.Instance.placeMentSystem.onInteraction = false;
                GameObject ui = ResourceManager.Instantiate("Prefabs/UI/UI_InteractionInfo");
                ui.GetComponent<UI_InteractionBtnInfo>().firstOption = true;
                ui.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 5, 2);
                ui.GetComponent<UI_InteractionBtnInfo>().SetCurObject(buildObj);
            }else if (buildObj.GetComponent<BuildObj>().id == 312) 
            {
                MapEditor.Instance.placeMentSystem.onInteraction = false;
                GameObject ui = ResourceManager.Instantiate("Prefabs/UI/UI_InteractionLeverInfo");
                ui.GetComponent<UI_InteractionLeverInfo>().firstOption = true;
                ui.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 5, 2);
                ui.GetComponent<UI_InteractionLeverInfo>().SetCurObject(buildObj);
            }
            else
            {
                GameObject obj = Instantiate(buildObj);
                MapEditor.Instance.placeMentSystem.first_holdingObj = obj;
                obj.GetComponent<BuildObj>().TurnOff();
            }
        }
       
    }


    public void Init(GameObject obj, Sprite sprite)
    {
        image = GetComponent<Image>();
        image.sprite = sprite;
        buildObj = obj;
        
    }


}
