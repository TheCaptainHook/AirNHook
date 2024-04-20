using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Text.RegularExpressions;

public class MapEditorControllerUI : MonoBehaviour
{
    [Header("Components")]
    PlaceMentSystem placeMentSystem;

    [Header("Controller State")]
    bool onHide;

    [Header("Btn Color")]
    Color activeColor = new Color(0.47f,0.47f, 0.47f);


    [Header("Map ID Container")]
    string mapIdRegex = @"^[a-zA-Z0-9_\s]{1,20}$";
    [SerializeField] TMP_InputField mapIdInputField;
    [SerializeField] Button saveBtn;

    [Header("Map Size")]
    [SerializeField] TMP_InputField widthInputField;
    [SerializeField] TMP_InputField heightInputField;
    [SerializeField] Button initBtn;
    //[SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button onOffBtn;

    [Header("Mode")]
    [SerializeField] Button tileMode;
    [SerializeField] Button objectMode;
    [SerializeField] Button tileUndoBtn;

    [Header("Tile Draw Tool Btn")]
    [SerializeField] GameObject tileMode_BtnContainer;
    [SerializeField] Button tileBtn;
    [SerializeField] Button eraserBtn;
    [SerializeField] Button drawBoxBtn;
    [SerializeField] Button clearBoxBtn;

    [Header("Object Tool")]
    [SerializeField] GameObject objectSpaceUi;
    [SerializeField] GameObject objectMode_BtnContainer;
    [SerializeField] Button moveBtn;
    [SerializeField] Button rotationBtn;
    [SerializeField] Button scaleBtn;
    [SerializeField] Button clearBtn;

    public Button[] tileDrawBtns;
    public Button[] objectDrawBtns;


    [SerializeField] GameObject wrongMessage;


    private void Awake()//todo
    {
        placeMentSystem = MapEditor.Instance.placeMentSystem;
        initBtn.onClick.AddListener(MapSizeInit);
        onOffBtn.onClick.AddListener(HideController);
        //Map Id Container
        saveBtn.onClick.AddListener(() => { SaveUserMap(); });

        //mode
        tileUndoBtn.onClick.AddListener(placeMentSystem.invoker.Undo);
        tileMode.onClick.AddListener(TileMode);
        objectMode.onClick.AddListener(ObjectMode);
        //mode
        //Tile Mode Btn
        tileBtn.onClick.AddListener(() => { ChangeTileMode(tileBtn, ModeState.Tile_Draw); });
        eraserBtn.onClick.AddListener(() => { ChangeTileMode(eraserBtn, ModeState.Tile_Clear); });
        drawBoxBtn.onClick.AddListener(() => { ChangeTileMode(drawBoxBtn, ModeState.Tile_BoxDraw); });
        clearBoxBtn.onClick.AddListener(() => { ChangeTileMode(clearBoxBtn, ModeState.Tile_ClearBox); });
        tileDrawBtns = new Button[] { tileBtn, eraserBtn, drawBoxBtn, clearBoxBtn };
        //Tile Mode Btn
        //Obejct Mode Btn
        moveBtn.onClick.AddListener(() => { ChangeObjectMode(moveBtn); placeMentSystem.CreateIndicator(ModeState.Obj_Move); });
        rotationBtn.onClick.AddListener(() => { 
            if(placeMentSystem.CurbuildObject != null && placeMentSystem.CurbuildObject.GetComponent<BuildObj>().onRotateable)
            {
                ChangeObjectMode(rotationBtn);
                placeMentSystem.CreateIndicator(ModeState.Obj_Rotation);
            }});
        scaleBtn.onClick.AddListener(() => { 
            if(placeMentSystem.CurbuildObject != null && placeMentSystem.CurbuildObject.GetComponent<BuildObj>().onScaleable)
            {
                ChangeObjectMode(scaleBtn); OpenScaleUI();
            }});
        clearBtn.onClick.AddListener(() => { ObjectDrawModeBtn_Reset(); placeMentSystem.objectModeClient.Clear(); });
        objectDrawBtns = new Button[] { moveBtn, rotationBtn, scaleBtn, clearBtn };
    }


    private void Start()
    {
        

    }

    void TileMode() //타일모드로 진입할때, //todo
    {
        if (MapEditor.Instance.gridPlane.activeSelf && placeMentSystem.onInteraction)
        {
            ModeBtn_Reset();
            if (MapEditor.Instance.mapEditorState == MapEditorState.Tile)
            {
                tileMode_BtnContainer.SetActive(false);
                MapEditor.Instance.mapEditorState = MapEditorState.Editor;
                placeMentSystem.tileBase = null;
                placeMentSystem.curPlacedObjTurnOn();
            }
            else
            {
                Active_BtnChangeColor(tileMode);
                MapEditor.Instance.mapEditorState = MapEditorState.Tile;
                placeMentSystem.tileBase = Resources.Load<TileBase>("Prefabs/MapEditor/Tile/1");//todo
                tileMode_BtnContainer.SetActive(true);
                placeMentSystem.curPlacedObjTurnOff();
            }
        }
       
    }
    void ObjectMode()
    {
        if (MapEditor.Instance.gridPlane.activeSelf && placeMentSystem.onInteraction)
        {
            ModeBtn_Reset();
            if (MapEditor.Instance.mapEditorState == MapEditorState.Object)
            {
                objectMode_BtnContainer.SetActive(false);
                objectSpaceUi.SetActive(false);
                MapEditor.Instance.mapEditorState = MapEditorState.Editor;
                placeMentSystem.curPlacedObjTurnOn();
                placeMentSystem.ObjectMode_Reset();//remove curindicator,first_holdingObj,curBuildobj =null;
            }
            else
            {
                objectMode_BtnContainer.SetActive(true);
                Active_BtnChangeColor(objectMode);
                MapEditor.Instance.mapEditorState = MapEditorState.Object;
                placeMentSystem.curPlacedObjTurnOff();
                objectSpaceUi.SetActive(true);
            }
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
    private void ModeBtn_Reset() //Btn All Reset
    {
        placeMentSystem.modeState = ModeState.None;
        placeMentSystem.ObjectMode_Reset();
        if (tileMode_BtnContainer.activeSelf) { tileMode_BtnContainer.SetActive(false);}
        if (objectMode_BtnContainer.activeSelf) { objectMode_BtnContainer.SetActive(false); }
        if(objectSpaceUi.activeSelf) { objectSpaceUi.SetActive(false); }
        TileDrawModeBtn_Reset();
        ObjectDrawModeBtn_Reset();
        Deactive_BtnChangeColor(tileMode);
        Deactive_BtnChangeColor(objectMode);
    }
    private void TileDrawModeBtn_Reset()
    {
        placeMentSystem.ResetPreviewTileMap();
        for (int i = 0; i < tileDrawBtns.Length; i++)
        {
            Deactive_BtnChangeColor(tileDrawBtns[i]);
        }
    }

    private void ObjectDrawModeBtn_Reset()
    {
        for (int i = 0; i < objectDrawBtns.Length; i++)
        {
            Deactive_BtnChangeColor(objectDrawBtns[i]);
        }
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
        float num = 0;
        onOffBtn.enabled = false;
        if (onHide)
        {
            onHide = false;
            num = 0;
            onOffBtn.transform.GetChild(0).gameObject.SetActive(true);
            onOffBtn.transform.GetChild(1).gameObject.SetActive(false);

        }
        else
        {
            onHide = true;
            num = rectTransform.rect.width;
            onOffBtn.transform.GetChild(0).gameObject.SetActive(false);
            onOffBtn.transform.GetChild(1).gameObject.SetActive(true);
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

    #region Map ID Container
    

    #endregion

    #region Save
    private void SaveUserMap()
    {

        if(!MapEditor.Instance.gridPlane.activeSelf)
        {
            StartCoroutine(Co_WrongMaeeage("맵 사이즈 설정을 해주세요"));
            return;
        }


        if (!CheckSaveCondition())
        {
            StartCoroutine(Co_WrongMaeeage("스폰 포인트, 탈출 문, 열쇠가 하나 이상 \n있어야 저장이 가합니다."));
            return;
        }



        bool isValid = Regex.IsMatch(mapIdInputField.text, mapIdRegex);
        if (isValid)
        {
            int width = int.Parse(widthInputField.text);
            int height = int.Parse(heightInputField.text);

            MapEditor.Instance.width = width;
            MapEditor.Instance.height = height;
            MapEditor.Instance.mapID = mapIdInputField.text;


            Debug.Log($"{width},{height},{mapIdInputField.text}");
            MapEditor.Instance.SaveMapData();
            //데이터 저장 완료 유아이.

        }
        else
        {
            StartCoroutine(Co_WrongMapId());
        }

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
    private void ChangeTileMode(Button btn, ModeState tileModeState)
    {
        TileDrawModeBtn_Reset();
        Active_BtnChangeColor(btn);
        placeMentSystem.modeState = tileModeState;
    }
    private void ChangeObjectMode(Button btn)
    {
        ObjectDrawModeBtn_Reset();
        Active_BtnChangeColor(btn);
    }

    private void OpenScaleUI()
    {
        if(placeMentSystem.CurbuildObject != null)
        {
            GameObject ui = Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI_ScaleInfo"));
            ui.transform.position = placeMentSystem.CurbuildObject.transform.position + new Vector3(3, 0);
            ui.GetComponent<UI_ScaleInfo>().SetCurObject(placeMentSystem.CurbuildObject);
            placeMentSystem.objectModeClient.Scale();
            //COmmand

        }
    }


    bool  CheckSaveCondition()
    {
        //스폰포인트,탈출구, 열쇠 1개 이상,
        bool spawnPoint = false;
        bool exitPoint = false;
        bool key = false;

        if (MapEditor.Instance.FindObj(MapEditor.Instance.dontSaveObjectTransform, 302))
        {
            spawnPoint = true;
        }
        if (MapEditor.Instance.FindObj(MapEditor.Instance.exitDoorObjectTransform, 301))
        {
            exitPoint = true;
        }
        if (MapEditor.Instance.FindObj(MapEditor.Instance.objectTransform, 307))
        {
            key = true;
        }


        if(spawnPoint && exitPoint && key)
        {
            return true;
        }

        return false;

    }


    IEnumerator Co_WrongMapId()
    {
        float percent = 0;
        mapIdInputField.interactable = false;
        mapIdInputField.image.color = Color.red;
        while (percent < 1)
        {
            percent += Time.deltaTime;
            mapIdInputField.image.color = Color.Lerp(mapIdInputField.image.color, Color.white, percent);
            yield return null;
        }
        mapIdInputField.interactable = true;
    }

    IEnumerator Co_WrongMaeeage(string message)
    {
        float percent = 0;
        saveBtn.interactable = false;
        wrongMessage.SetActive(true);
        wrongMessage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        while(percent < 1)
        {
            percent += Time.deltaTime;
            yield return null;
        }
        wrongMessage.SetActive(false);
        saveBtn.interactable = true;
    }

    //todo 0420
    public void LoadUserMapEditorInit(Map map)
    {
        MapEditor.Instance.EditorMode_Init();
        MapEditor.Instance.LoadMap(map);
        mapIdInputField.text = MapEditor.Instance.mapID;

        widthInputField.text = MapEditor.Instance.CurMap.mapSize.x.ToString();
        heightInputField.text = MapEditor.Instance.CurMap.mapSize.y.ToString();

        MapSizeInit();

        
        
    }
    #endregion
}
