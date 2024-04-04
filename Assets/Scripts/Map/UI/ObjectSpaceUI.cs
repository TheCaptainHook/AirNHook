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

    string path = "Prefabs/MapEditor/Object";

    private void Awake()
    {
        rTransform = transform as RectTransform;
        toggleBtn.onClick.AddListener(ShowAndHide);
        originAnchoredPosition = rTransform.anchoredPosition;
    }

    private void Start()
    {
        LoadAllObject();
    }

    private void OnEnable()
    {
        onHide = false;
        rTransform.anchoredPosition = originAnchoredPosition;
    }

    void LoadAllObject()
    {
        GameObject[] objects = Resources.LoadAll<GameObject>(path);

        for (int i = 0; i < objects.Length; i++)
        {
            GameObject obj = Instantiate(objectSpaceUIItem,content);
            obj.GetComponent<Interaction_BuildItem>().Init(objects[i]);
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
