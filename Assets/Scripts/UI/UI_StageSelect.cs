using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageSelect : UI_Base
{
    public Transform layout;
    
    public override void OnEnable()
    {
        OpenUI();
    }

    protected override void Start()
    {
        var maps = Managers.Data.mapData.mapDictionary.Keys;
        
        foreach (var map in maps)
        {
            var button = ResourceManager.Instantiate("Prefabs/UI/Button", layout).GetComponent<Button>();
            button.GetComponentInChildren<TMP_Text>().text = map;
            button.onClick.AddListener(() => StageSet(map));
        }

        //TEST 맵 시작 테스트 코드
        var endButton = ResourceManager.Instantiate("Prefabs/UI/Button", layout).GetComponent<Button>();
        endButton.onClick.AddListener(StartGame);
    }

    private void StageSet(string mapName)
    {
        Managers.Game.Player.GetComponent<Player>().CmdChangeStage(mapName);
    }
    
    //TEST 맵 시작 테스트 코드
    private void StartGame()
    {
        Managers.Network.ServerChangeScene("MainScene");
    }
}
