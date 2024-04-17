using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class StageButton : MonoBehaviour
{
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

    private LoadData _loadData;

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

        if (!playData.perfectClear)
        {
            clearDeathTxt.text = playData.deathCount.ToString();
        }
        else
        {
            clearDeathTxt.text = playData.perfect.ToString();
        }


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

    //스테이지 시작됬을땐 시간,죽음횟수,스킵여부 전부 0으로 초기화
    public void StageStart()
    {
        _clearTime = 0.0f;
        _clearDeath = 0;
        _skip = false;
    }

    public void TimeCheck()
    {
        StageStart();
        _selectStage = stageNameTxt.text;
        _timeCheck = true;
    }


    public void DeathCheck()
    {
        _clearDeath++;
    }

    public void SkipCheck()
    {
        _skip = true;
    }

    public void StageClear()
    {
        _timeCheck = false;
        _loadData.stageData[_selectStage].stageClear = true;

        TimeCompare();
        DeathCompare();

        _loadData.Save();
    }

    public void StageFalse()
    {
        _timeCheck = false;
        _loadData.Save();
    }

    public void StageSkip()
    {
        _loadData.playData[_selectStage].skip = _skip;
        _loadData.Save();
    }

    public void TimeCompare()
    {
        if (_loadData.playData[_selectStage].clearTime == 0 || _clearTime < _loadData.playData[_selectStage].clearTime)
            _loadData.playData[_selectStage].clearTime = _clearTime;
    }

    public void DeathCompare()
    {
        if (!_loadData.playData[_selectStage].perfectClear)
        {
            if (_loadData.playData[_selectStage].deathCount == 0 || _clearDeath < _loadData.playData[_selectStage].deathCount)
                _loadData.playData[_selectStage].deathCount = _clearDeath;
            if(_clearDeath == 0 && _loadData.stageData[_selectStage].stageClear == true)
            {
                _loadData.playData[_selectStage].perfectClear = true;
                _loadData.playData[_selectStage].perfect = "PerfectClear!";
            }
        }
        Debug.Log(_clearDeath);
    }

    public void LoadData(LoadData loadData)
    {
        _loadData = loadData;
    }

    public void StageClearLevelPlus()
    {
        //_loadData.stageClearLevelData.Add();
    }
}