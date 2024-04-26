using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class UI_EditorTItle : UI_Base
{
    [SerializeField] Button newBtn;
    [SerializeField] Button loadBtn;

    string path = "";

    [Header("LoadUI")]
    [SerializeField] Button loadUiCloseBtn;
    [SerializeField] GameObject loadContainer;
    [SerializeField] GameObject loadUserMapDataBoxItem;
    [SerializeField] Transform contents;
    [SerializeField] GameObject loadingPanel;

    List<string> userMapDataJsonList;
    List<UserMapData> userMapDataList;
    List<UserMapDataBoxItem> userMapDataBoxItemList;

    private void Awake()
    {
        path = Path.Combine(Application.dataPath, "UserMapData");
        //Instantiate(ResourceManager.Instantiate("Prefabs/MapEditor/MapEditor"));
        newBtn.onClick.AddListener(() => { NewCreate(); });
        loadBtn.onClick.AddListener(OpenLoadUI);
        loadUiCloseBtn.onClick.AddListener(() => { loadContainer.SetActive(false); });
        userMapDataJsonList = new();
        userMapDataList = new();
        userMapDataBoxItemList = new();
    }

    public override void OnEnable()
    {

    }

    private void NewCreate()
    {
        MapEditor.Instance.EditorMode_Init();
        MapEditor.Instance.mapEditorType = MapEditorType.New;

        CloseUI();

    }
    private void OpenLoadUI()
    {
        loadContainer.SetActive(true);
        string[] filePaths = Directory.GetFiles(path, "*.json");

        foreach (string filePath in filePaths)
        {
            if (!userMapDataJsonList.Contains(filePath))
            {
                userMapDataJsonList.Add(filePath);
                string jsonString = File.ReadAllText(filePath);
                UserMapData data = JsonUtility.FromJson<UserMapData>(jsonString);

                userMapDataList.Add(data);
                CreateUserMapDataBoxItem(data);
            }
        }

    }


    public void Reset()
    {
        loadContainer.SetActive(false);        
    }

    //스트리밍폴더에 유서맵데이터 긁어와서 보여주기, Ui 만들어서 보여주기.
    //선택후 불러오기 누르면 맵에디터 생성,맵데이터 변환후 로드


    //1. 맵 에디터 인잇
    //2. 에디터타입.로드
    //3. 에디터모드 그리트플렛 크기설정후 인잇
    //3. UserMapDataBoxUi 클릭하면 에디.LoadMap(Map map)


    //맵 에디터 컨트롤러 유아이 로드전용 인잇 실행
    //MapEditor.Instance.editorUIController.LoadUserMapEditorInit();



    #region Util
    public void FadeInLoadingPanel()
    {
        StartCoroutine(Co_FadeInLoadingPanel());
    }
    IEnumerator Co_FadeInLoadingPanel()
    {
        loadingPanel.SetActive(true);
        Image image = loadingPanel.GetComponent<Image>();
        Color newColor = new Color(1, 1, 1, 1);
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime;
            image.color = Color.Lerp(image.color, newColor, percent);
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        loadingPanel.SetActive(false);
        CloseUI();
    }


    private void CreateUserMapDataBoxItem(UserMapData data)
    {
        GameObject ui = Instantiate(loadUserMapDataBoxItem, contents);
        UserMapDataBoxItem umdb = ui.GetComponent<UserMapDataBoxItem>();
        umdb.SetData(data);
        umdb.OnLoadUserMap += FadeInLoadingPanel;
        umdb.OnLoadUserMap += Reset;



    }

    #endregion

}
