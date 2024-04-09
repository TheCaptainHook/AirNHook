
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
        GameObject obj = Instantiate(buildObj);
        MapEditor.Instance.placeMentSystem.first_holdingObj = obj;
        obj.GetComponent<BuildObj>().TurnOff();
    }


    public void Init(GameObject obj, Sprite sprite)
    {
        image = GetComponent<Image>();
        image.sprite = sprite;
        buildObj = obj;
        
    }


}
