using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class StageButton : MonoBehaviour
{
    [SerializeField] GameObject _stageSelect;
    //생성할 위치
    public Transform stageButtonCreate;
    public GameObject stageButton;
    public TextMeshProUGUI stageNameTxt;
    public TextMeshProUGUI clearTimeTxt;
    public TextMeshProUGUI clearDeathTxt;

    private string _selectStage;
    private float _clearTime;
    private int _clearDeath;
    private bool _skip;

    private int _min;
    private int _sec;

    private bool _timeCheck;
    private bool _stageSelectShow;

    //private LoadData _loadData;

    private void Update()
    {
        if (_timeCheck)
        {
            _clearTime += Time.deltaTime;
        }
    }

    //TODO 스테이지 진입했을때 실시간으로 변동되고, 스테이지를 클리어하면 그순간 변동을 멈추고 저장되도록해야함.
    public void StageSelect(PlayData playData)
    {
        stageNameTxt.text = playData.stageID;

        


        if (playData.clearTime >= 60)
        {
            _min = (int)playData.clearTime / 60;
            _sec = (int)playData.clearTime % 60;
        }
        else
        {
            _sec = (int)playData.clearTime % 60;
        }
        clearTimeTxt.text = string.Format("{0:D2}:{1:D2}", _min, _sec);
    }

    //시간,죽음횟수,스킵여부 전부 0으로 초기화
    public void StageStart()
    {
        _clearTime = 0.0f;
        _clearDeath = 0;
        _skip = false;
    }

    //스테이지 시작되는순간 실행
    public void TimeCheck()
    {
        StageStart();
        _selectStage = stageNameTxt.text;
        _timeCheck = true;
    }

    //캐릭터 사망시 데스카운트추가
    public void DeathCheck()
    {
        _clearDeath++;
    }

    //스킵버튼클릭시 활성화
    public void SkipCheck()
    {
        _skip = true;
    }

   

    //public void StageFalse()
    //{
    //    _timeCheck = false;
    //    Managers.Data.loadData.Save();
    //}

    //public void StageSkip()
    //{
    //    Managers.Data.loadData.playData[_selectStage].skip = _skip;
    //    Managers.Data.loadData.Save();
    //}

    //public void TimeCompare()
    //{
    //    if (Managers.Data.loadData.playData[_selectStage].clearTime == 0 || _clearTime < Managers.Data.loadData.playData[_selectStage].clearTime)
    //        Managers.Data.loadData.playData[_selectStage].clearTime = _clearTime;
    //}

    

    //TODO 현재 테스트코드에선 생성할때 1번만불려져서 최신화가 안되고있는상황임
    //실제로 적용할땐 실시간 업데이트가 가능하도록 해야한다.
    //or ResourceManager.Destroy사용해서 전부삭제했다 재생성하던가... <-하다 실패
    public void CreateButton()
    {
        _stageSelect.SetActive(true);

        //Json파일 읽어오는 코드
        var playDataPath = Path.Combine(Application.streamingAssetsPath, "PlayDatas/PlayDatas.json");
        var playDataList = Managers.Data.ReadJson<PlayData>(playDataPath);

        if (!_stageSelectShow)
        {
            foreach (var key in playDataList)
            {
                //list 개수만큼 버튼이 생성
                var slot = ResourceManager.Instantiate(stageButton, stageButtonCreate).GetComponent<StageButton>();

                slot.StageSelect(key);
                //slot.LoadData(GetComponent<LoadData>());
            }
        }
        _stageSelectShow = true;
    }

    public void StageClearLevelPlus()
    {
        //_loadData.stageClearLevelData.Add();
    }
}