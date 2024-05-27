using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class ObjectSpaceUI : MousePointerEntity
{
  
    RectTransform rTransform;
    Color activeColor = new Color(0.686f, 0.913f, 0.713f);

    [Header("State")]
    bool onHide;

    [Header("Info")]
    private Vector2 originAnchoredPosition;
    private string path = "Prefabs/MapEditor/Object";
    private string objPreviewSpritePath = "Arts/Sprites/PreviewSprites/Object"; //todo 0427

    private string backgroundObjPath = "Prefabs/MapEditor/Background";
    private string backgroundPreviewSpritePath = "Arts/Sprites/PreviewSprites/Background"; //todo 0427

    private string otherObjPath = "Prefabs/MapEditor/Other";
    private string otherPreviewSpritePath = "Arts/Sprites/PreviewSprites/other"; //todo 0427

    private GameObject[] objects;
    private GameObject[] otherObjects;
    private GameObject[] backgroundObjects;
    private Sprite[] objPreviewSprites;
    private Sprite[] otherObjPreviewSprites;
    private Sprite[] backgroundObjPreviewSprites;
    [SerializeField] GameObject objectSpaceUIItem;
    //[SerializeField] Button toggleBtn;

    //Select space
    [Header("Button")]
    [SerializeField] Button downBtn;
    [SerializeField] Button upBtn;

    [SerializeField] Button objectSectionBtn;
    [SerializeField] Button backgroundSectionBtn;
    [SerializeField] Button otherSectionBtn;

    [Header("Current")]
    Button currentBtn;
    GameObject currentSpace;

    [Header("Container")]
    [SerializeField] GameObject objectSpace;
    [SerializeField] GameObject backgroundSpace;
    [SerializeField] GameObject otherSpace;

    [Header("Contents")]
    [SerializeField] Transform objectContent;
    [SerializeField] Transform backgroundContent;
    [SerializeField] Transform otherContent;

    private void Awake()
    {
        downBtn.onClick.AddListener(() => { ShowAndHide(); });
        upBtn.onClick.AddListener(() => { ShowAndHide(); });

        objectSectionBtn.onClick.AddListener(() => { SelectChangeModeBtn(objectSectionBtn,objectSpace);});
        backgroundSectionBtn.onClick.AddListener(() => { SelectChangeModeBtn(backgroundSectionBtn, backgroundSpace); });
        otherSectionBtn.onClick.AddListener(() => { SelectChangeModeBtn(otherSectionBtn, otherSpace); });

        rTransform = transform as RectTransform;
        //toggleBtn.onClick.AddListener(ShowAndHide);
        originAnchoredPosition = rTransform.anchoredPosition;

        Init();
       
    }

    private void Init()
    {
        objects = Resources.LoadAll<GameObject>(path);
        otherObjects = Resources.LoadAll<GameObject>(otherObjPath);
        backgroundObjects = Resources.LoadAll<GameObject>(backgroundObjPath);

        objPreviewSprites = Resources.LoadAll<Sprite>(objPreviewSpritePath);
        otherObjPreviewSprites = Resources.LoadAll<Sprite>(otherPreviewSpritePath);
        backgroundObjPreviewSprites = Resources.LoadAll<Sprite>(backgroundPreviewSpritePath);

        LoadAllObject();
    }

    private void OnEnable()
    {
        onHide = false;
        rTransform.anchoredPosition = originAnchoredPosition;
        objectContent.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        if (upBtn.gameObject.activeSelf)
        {
            upBtn.gameObject.SetActive(false);
            downBtn.gameObject.SetActive(true);
        }

    }

    void LoadAllObject()
    {
        //for (int i = 0; i < objects.Length; i++)
        //{
        //    GameObject obj = Instantiate(objectSpaceUIItem, objectContent);
        //    for (int j = 0; j < objPreviewSprites.Length; j++)
        //    {
        //        if (objects[i].name == objPreviewSprites[j].name)
        //        {
        //            obj.GetComponent<Interaction_BuildItem>().Init(objects[i], objPreviewSprites[j]);

        //            obj.AddComponent<UI_ShowToolTip>();
        //            obj.GetComponent<UI_ShowToolTip>().SetText(objects[i].name);

        //        }
        //    }

        //}

        CreateObjContents(objectSpace,objects, objPreviewSprites, objectContent);
        CreateObjContents(otherSpace, otherObjects, otherObjPreviewSprites, otherContent);
        CreateObjContents(backgroundSpace, backgroundObjects, backgroundObjPreviewSprites, backgroundContent);


    }

    void CreateObjContents(GameObject container,GameObject[] objects, Sprite[] sprites,Transform content)
    {
        if (!container.activeSelf) container.SetActive(true);
        for (int i = 0; i < objects.Length; i++)
        {
            GameObject obj = Instantiate(objectSpaceUIItem, content);
            for (int j = 0; j < sprites.Length; j++)
            {
                if (objects[i].name == sprites[j].name)
                {
                    obj.GetComponent<Interaction_BuildItem>().Init(objects[i], sprites[j]);

                    obj.AddComponent<UI_ShowToolTip>();
                    obj.GetComponent<UI_ShowToolTip>().SetText(objects[i].name);

                }
            }

        }
        container.SetActive(false);

    }


    IEnumerator Co_LoadAllObject()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            GameObject obj = Instantiate(objectSpaceUIItem, objectContent);
            for (int j = 0; j < objPreviewSprites.Length; j++)
            {
                if (objects[i].name == objPreviewSprites[j].name)
                {
                    obj.GetComponent<Interaction_BuildItem>().Init(objects[i], objPreviewSprites[j]);

                    obj.AddComponent<UI_ShowToolTip>();
                    obj.GetComponent<UI_ShowToolTip>().SetText(objects[i].name);

                }
                yield return null;
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
            num = -rectTransform.rect.height - 85;
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



    private void SelectChangeModeBtn(Button btn,GameObject space)
    {
        if(currentBtn != null)
        {
            currentSpace.SetActive(false);

            ColorBlock currentColorBlock = currentBtn.colors;
            currentColorBlock.normalColor = Color.white;
            currentBtn.colors = currentColorBlock;
        }

        currentSpace = space;
        currentSpace.SetActive(true);
        currentBtn = btn;

        ColorBlock colorBlock = btn.colors;
        colorBlock.selectedColor = activeColor;
        colorBlock.normalColor = activeColor;
        btn.colors = colorBlock;
    }



    public override void OnPointerEnter(PointerEventData data)
    {
        MapEditor.Instance.placeMentSystem.onEnterMapEditorUi = true;
        Debug.Log("inter UI");
    }

    public override void OnPointerExit(PointerEventData data)
    {
        MapEditor.Instance.placeMentSystem.onEnterMapEditorUi = false;
        Debug.Log("Exit UI");
    }

}
