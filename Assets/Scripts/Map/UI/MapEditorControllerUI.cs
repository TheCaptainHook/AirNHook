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


    [Header("Btn Color")]
    Color activeColor = new Color(0.47f,0.47f, 0.47f);

    [Header("Map Size")]
    [SerializeField] TMP_InputField widthInputField;
    [SerializeField] TMP_InputField heightInputField;
    [SerializeField] Button initBtn;
    //[SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button onOffBtn;

    [Header("Mode")]
    [SerializeField] Button tileMode;

    [SerializeField] Button tileUndoBtn;

    [Header("Tile Draw Tool Btn")]
    [SerializeField] GameObject tileMode_BtnContainer;
    [SerializeField] Button tileBtn;
    [SerializeField] Button eraserBtn;
    [SerializeField] Button drawBoxBtn;
    [SerializeField] Button clearBoxBtn;

    public Button[] tileDrawBtns;
    
    private void Awake()//todo
    {
        placeMentSystem = MapEditor.Instance.placeMentSystem;
        initBtn.onClick.AddListener(MapSizeInit);
        onOffBtn.onClick.AddListener(HideController);
        //test
        tileMode.onClick.AddListener(TileMode);
        tileUndoBtn.onClick.AddListener(placeMentSystem.invoker.Undo);

        tileBtn.onClick.AddListener(() => { ChangeTileMode(tileBtn, TileModeState.Tile); });
        eraserBtn.onClick.AddListener(() => { ChangeTileMode(eraserBtn, TileModeState.Clear); });
        drawBoxBtn.onClick.AddListener(() => { ChangeTileMode(drawBoxBtn, TileModeState.TileBox); });
        clearBoxBtn.onClick.AddListener(() => { ChangeTileMode(clearBoxBtn, TileModeState.ClearBox); });

        tileDrawBtns = new Button[] { tileBtn, eraserBtn, drawBoxBtn, clearBoxBtn };
    }

    
    
    void TileMode() //타일모드로 진입할때, //todo
    {
        ModeBtn_Reset();
        if(MapEditor.Instance.mapEditorState == MapEditorState.Tile)
        {
            Deactive_BtnChangeColor(tileMode);
            TileDrawModeBtn_Reset();

            tileMode_BtnContainer.SetActive(false);
            MapEditor.Instance.mapEditorState = MapEditorState.Editor;
            MapEditor.Instance.placeMentSystem.tileBase = null;
        }
        else
        {
            Active_BtnChangeColor(tileMode);
            MapEditor.Instance.mapEditorState = MapEditorState.Tile;
            MapEditor.Instance.placeMentSystem.tileBase = Resources.Load<TileBase>("Arts/Tiles/1");//todo
            tileMode_BtnContainer.SetActive(true);
        }
       
    }
    
    
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
    private void ModeBtn_Reset() //todo
    {
        TileDrawModeBtn_Reset();
    }
    private void TileDrawModeBtn_Reset()
    {
        placeMentSystem.ResetPreviewTileMap();
        placeMentSystem.tileModeState = TileModeState.None;
        for (int i = 0; i < tileDrawBtns.Length; i++)
        {
            Deactive_BtnChangeColor(tileDrawBtns[i]);
        }
    }
    #region Tile Draw Mode
    private void ChangeTileMode(Button btn ,TileModeState tileModeState)
    {
        TileDrawModeBtn_Reset();
        Active_BtnChangeColor(btn);
        placeMentSystem.tileModeState = tileModeState;
    }
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


    #region Util
    private void Active_BtnChangeColor(Button btn)
    {
        ColorBlock colorBlock = btn.colors;
        colorBlock.normalColor = activeColor;
        btn.colors = colorBlock;
    }
    private void Deactive_BtnChangeColor(Button btn)
    {
        ColorBlock colorBlock = btn.colors;
        colorBlock.normalColor = Color.white;
        btn.colors = colorBlock;
    }
    #endregion
}
