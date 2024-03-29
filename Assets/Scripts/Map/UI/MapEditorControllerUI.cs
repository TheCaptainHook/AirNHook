using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
public class MapEditorControllerUI : MonoBehaviour
{
    [Header("Controller State")]
    bool onHide;


    [Header("Map Size")]
    [SerializeField] TMP_InputField widthInputField;
    [SerializeField] TMP_InputField heightInputField;
    [SerializeField] Button initBtn;
    //[SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button onOffBtn;
    [SerializeField] Button testBTN;



    private void Awake()
    {
        initBtn.onClick.AddListener(MapSizeInit);
        onOffBtn.onClick.AddListener(HideController);
        testBTN.onClick.AddListener(TestTileCLIKC);
    }

    
    //test
    void TestTileCLIKC()
    {   if(MapEditor.Instance.mapEditorState == MapEditorState.Tile)
        {
            MapEditor.Instance.mapEditorState = MapEditorState.Editor;
            MapEditor.Instance.placeMentSystem.tileBase = null;
        }
        else
        {
            MapEditor.Instance.mapEditorState = MapEditorState.Tile;
            MapEditor.Instance.placeMentSystem.tileBase = Resources.Load<TileBase>("Arts/Tiles/1");
        }
       
    }
    
    //test
    #region Map Size UI
    void MapSizeInit()
    {
        if (!MapEditor.Instance.gridPlane.activeSelf) { MapEditor.Instance.gridPlane.SetActive(true); }
        int width = int.Parse(widthInputField.text);
        int height = int.Parse(heightInputField.text);
        Material material = MapEditor.Instance.gridPlane.GetComponent<SpriteRenderer>().material;
        material.SetVector("_Tilling", new Vector2(width, height));
        MapEditor.Instance.gridPlane.transform.localScale = new Vector2(width, height);
    }

    #endregion

    #region Controller
    private void HideController()
    {
        StartCoroutine(Co_HideController());
    }
    IEnumerator Co_HideController()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        float percent = 0;
        int num = 0;
        if (onHide)
        { onHide = false; num = 300; }
        else
        {
            onHide = true;
            num = -300;
        }
        while (percent < 1)
        {
            percent += Time.deltaTime + 0.08f;
            Vector2 ar = new Vector2(num, rectTransform.anchoredPosition.y);
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, ar, percent);
            yield return null;
        }
    }
    #endregion

}
