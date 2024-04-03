using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System;

public class MapEditorControllerUI : MonoBehaviour
{
    PlaceMentSystem placeMentSystem;

    [Header("Controller State")]
    bool onHide;


    [Header("Map Size")]
    [SerializeField] TMP_InputField widthInputField;
    [SerializeField] TMP_InputField heightInputField;
    [SerializeField] Button initBtn;
    //[SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button onOffBtn;

    [Header("TEST")]
    [SerializeField] Button tileMode_Test;
    [SerializeField] Button tileUndo_Test;

    [SerializeField] Button tileBtn_Test;
    [SerializeField] Button eraserBtn_Test;
    [SerializeField] Button drawBoxBtn_Test;
    [SerializeField] Button clearBoxBtn_Test;

    [SerializeField] GameObject tileMode_BtnContainer;
    
    private void Awake()//todo
    {
        placeMentSystem = MapEditor.Instance.placeMentSystem;
        initBtn.onClick.AddListener(MapSizeInit);
        onOffBtn.onClick.AddListener(HideController);
        //test
        tileMode_Test.onClick.AddListener(TestTileCLIKC);
        tileUndo_Test.onClick.AddListener(placeMentSystem.invoker.Undo);
        tileBtn_Test.onClick.AddListener(() => { placeMentSystem.tileModeState = TileModeState.Tile; placeMentSystem.ResetPreviewTileMap(); });
        eraserBtn_Test.onClick.AddListener(() => { placeMentSystem.tileModeState = TileModeState.Clear; placeMentSystem.ResetPreviewTileMap(); });
        drawBoxBtn_Test.onClick.AddListener(() => { placeMentSystem.tileModeState = TileModeState.TileBox; placeMentSystem.ResetPreviewTileMap(); });
        clearBoxBtn_Test.onClick.AddListener(() => { placeMentSystem.tileModeState = TileModeState.ClearBox; placeMentSystem.ResetPreviewTileMap(); });
    }

    
    //test
    void TestTileCLIKC() //타일모드로 진입할때, //todo
    {   if(MapEditor.Instance.mapEditorState == MapEditorState.Tile)
        {
            tileMode_BtnContainer.SetActive(false);
            MapEditor.Instance.mapEditorState = MapEditorState.Editor;

            MapEditor.Instance.placeMentSystem.tileBase = null;
        }
        else
        {
            MapEditor.Instance.mapEditorState = MapEditorState.Tile;
            MapEditor.Instance.placeMentSystem.tileBase = Resources.Load<TileBase>("Arts/Tiles/1");
            tileMode_BtnContainer.SetActive(true);
        }
       
    }
    
    //test
    #region Map Size UI
    void MapSizeInit()
    {
        if (!MapEditor.Instance.gridPlane.activeSelf) { MapEditor.Instance.gridPlane.SetActive(true); }
        int width = int.Parse(widthInputField.text);
        if (width % 2 != 0) width++;
        width = Mathf.Clamp(width, 10, 100);
        int height = int.Parse(heightInputField.text);
        if (height % 2 != 0) height++;
        height = Mathf.Clamp(height, 10, 100);

        Material material = MapEditor.Instance.gridPlane.GetComponent<SpriteRenderer>().material;
        material.SetVector("_Tilling", new Vector2(width, height));
        MapEditor.Instance.gridPlane.transform.localScale = new Vector2(width, height);


        widthInputField.text = width.ToString();
        heightInputField.text = height.ToString();

    }

    #endregion

    #region   Button
    private void ModeBtnBtn_Reset() { }//todo
    private void TileDrawModeBtnBtn_Reset() { }//todo
    #region Tile Draw Mode
    private void ChangeTileMode(TileModeState tileModeState) { } //button active color, origin color//todo
    #endregion
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
        onOffBtn.enabled = false;
        if (onHide)
        {
            onHide = false;
            num = 300;
            onOffBtn.transform.GetChild(0).gameObject.SetActive(false);
            onOffBtn.transform.GetChild(1).gameObject.SetActive(true);

        }
        else
        {
            onHide = true;
            num = -300;
            onOffBtn.transform.GetChild(0).gameObject.SetActive(true);
            onOffBtn.transform.GetChild(1).gameObject.SetActive(false);
        }
        while (percent < 1)
        {
            percent += Time.deltaTime + 0.08f;
            Vector2 ar = new Vector2(num, rectTransform.anchoredPosition.y);
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, ar, percent);
            yield return null;
        }
        onOffBtn.enabled = true;
    }

    

    #endregion

}
