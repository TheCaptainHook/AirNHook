using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_EditorTItle : UI_Base
{
    [SerializeField] Button newBtn;
    [SerializeField] Button loadBtn;


    private void Awake()
    {
        newBtn.onClick.AddListener(() => { NewCreate(); });
    }

    public override void OnEnable()
    {

    }

    private void NewCreate()
    {
        Instantiate(ResourceManager.Instantiate("Prefabs/MapEditor/MapEditor"));
        MapEditor.Instance.EditorMode_Init();
        MapEditor.Instance.mapEditorType = MapEditorType.New;

        CloseUI();

    }
    private void OpenLoadUI()
    {
        //스트리밍폴더에 유서맵데이터 긁어와서 보여주기, Ui 만들어서 보여주기.
        //선택후 불러오기 누르면 맵에디터 생성,맵데이터 변환후 로드 


        //맵 에디터 컨트롤러 유아이 로드전용 인잇 실행
        MapEditor.Instance.editorUIController.LoadUserMapEditorInit();
    }
}
