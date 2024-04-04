using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class ObjectSpaceUI : MonoBehaviour
{
    [SerializeField] Transform content;
    RectTransform rTransform;
    public GameObject objectSpaceUIItem;
    public Button toggleBtn;
    bool onHide;
    private Vector2 originAnchoredPosition;
    GameObject[] objects;

   public bool itemCheckComplete;

    public List<GameObject> items = new();

    string path = "Prefabs/MapEditor/Object";

    private void Awake()
    {
        rTransform = transform as RectTransform;
        toggleBtn.onClick.AddListener(ShowAndHide);
        originAnchoredPosition = rTransform.anchoredPosition;
        objects = Resources.LoadAll<GameObject>(path);
        LoadAllObject();
    }

    private void OnEnable()
    {
        onHide = false;
        rTransform.anchoredPosition = originAnchoredPosition;

        if (!itemCheckComplete)
        {
            CheckItemTexture();
        }
    }

    void LoadAllObject()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            GameObject obj = Instantiate(objectSpaceUIItem,content);
            obj.GetComponent<Interaction_BuildItem>().Init(objects[i]);
            items.Add(obj);
        }

    }

    void CheckItemTexture()
    {
        if (items[objects.Length-1].GetComponent<Interaction_BuildItem>().image.sprite == null)
        {
            foreach(GameObject obj in items)
            {
                obj.GetComponent<Interaction_BuildItem>().Init();
            }
        }
        else
        {
            itemCheckComplete = true;
        }
    }

    void ShowAndHide()
    {
        StartCoroutine(Co_ShowAndHide());
    }


    IEnumerator Co_ShowAndHide()
    {
        float percent = 0;
        float num = 0;
        toggleBtn.enabled = false;
        if (onHide)
        {
            onHide = false;
            num = originAnchoredPosition.y;
        }
        else
        {
            onHide = true;
            num = -385;
        }
        while (percent < 1)
        {
            percent += Time.deltaTime + 0.08f;
            Vector2 ar = new Vector2(rTransform.anchoredPosition.x, num);
            rTransform.anchoredPosition = Vector2.Lerp(rTransform.anchoredPosition, ar, percent);
            yield return null;
        }
        toggleBtn.enabled = true;
    }

}
