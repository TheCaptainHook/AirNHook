using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Interaction_BuildItem : MonoBehaviour
{
    public GameObject buildObj;
    Image image;
    Button button;


    private void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(ChoiceItem);
       
    }


    void ChoiceItem()
    {
        MapEditor.Instance.curBuildObj = buildObj;
    }


    public void Init(GameObject tile)
    {
        buildObj = tile;
        SetImage();
    }
    void SetImage()
    {
        image.sprite = buildObj.GetComponent<SpriteRenderer>().sprite;
        image.color = buildObj.GetComponent<SpriteRenderer>().color;
    }

}
