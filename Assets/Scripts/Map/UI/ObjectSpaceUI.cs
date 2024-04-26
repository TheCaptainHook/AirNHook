using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class ObjectSpaceUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Transform content;
    RectTransform rTransform;

    [Header("State")]
    bool onHide;

    [Header("Info")]
    private Vector2 originAnchoredPosition;
    private string path = "Prefabs/MapEditor/Object";
    private string objPreviewSpritePath = "Arts/Sprites/PreviewSprites";
    private GameObject[] objects;
    private Sprite[] objPreviewSprites;
    [SerializeField] GameObject objectSpaceUIItem;
    //[SerializeField] Button toggleBtn;

    //Select space
    [SerializeField] Button downBtn;
    [SerializeField] Button upBtn;

    private void Awake()
    {
        downBtn.onClick.AddListener(() => { ShowAndHide(); });
        upBtn.onClick.AddListener(() => { ShowAndHide(); });


        rTransform = transform as RectTransform;
        //toggleBtn.onClick.AddListener(ShowAndHide);
        originAnchoredPosition = rTransform.anchoredPosition;
        objects = Resources.LoadAll<GameObject>(path);
        objPreviewSprites = Resources.LoadAll<Sprite>(objPreviewSpritePath);
        LoadAllObject();
    }

    private void OnEnable()
    {
        onHide = false;
        rTransform.anchoredPosition = originAnchoredPosition;
        content.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    void LoadAllObject()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            GameObject obj = Instantiate(objectSpaceUIItem, content);
            for (int j = 0; j < objPreviewSprites.Length; j++)
            {
                if (objects[i].name == objPreviewSprites[j].name)
                {
                    obj.GetComponent<Interaction_BuildItem>().Init(objects[i], objPreviewSprites[j]);

                    obj.AddComponent<UI_ShowToolTip>();
                    obj.GetComponent<UI_ShowToolTip>().SetText(objects[i].name);

                }
            }

           


        }
        
    }


    void ShowAndHide()
    {
        StartCoroutine(Co_ShowAndHide());
    }


    IEnumerator Co_ShowAndHide()
    {
        if (upBtn.gameObject.activeSelf)
        {
            upBtn.gameObject.SetActive(false);
            downBtn.gameObject.SetActive(true);
            downBtn.interactable = false;
        }
        else
        {
            downBtn.gameObject.SetActive(false);
            upBtn.gameObject.SetActive(true);
        }


        RectTransform rectTransform = GetComponent<RectTransform>();
        float percent = 0;
        float num = 0;
        //toggleBtn.enabled = false;
        if (onHide)
        {
            onHide = false;
            num = originAnchoredPosition.y;

        }
        else
        {
            onHide = true;
            num = -rectTransform.rect.height;
        }
        while (percent < 1)
        {
            percent += Time.deltaTime + 0.08f;
            Vector2 ar = new Vector2(rTransform.anchoredPosition.x, num);
            rTransform.anchoredPosition = Vector4.Lerp(rTransform.anchoredPosition, ar, percent);
            yield return null;
        }
        //toggleBtn.enabled = true;

        if (upBtn.gameObject.activeSelf)
        {
            upBtn.interactable = true;
        }
        else
        {
            downBtn.interactable = true;
        }


    }

}
