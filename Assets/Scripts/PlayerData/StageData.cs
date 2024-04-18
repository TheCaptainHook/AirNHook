using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    //스테이지를 클리어하면 올라가는 스테이지 클리어레벨
    //맵 이름, 맵 클리어 여부, 스테이지 클리어 레벨, 스테이지별 히든피스개수
    public string stageID;
    public bool stageClear;
    public int stageClearLevel;
    public int stageHiddenObject;
}