using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayData
{
    //맵 클리어 시간, 최소 데스 수, 스킵 횟수(아 이맵하기싫어 넘겨), 퍼펙트 클리어
    public string stageID;
    public bool stageClear;
    public float clearTime;
    public int totalDeath;
    public int deathCount;
    public bool skip;
    public int stageLevel;
    public int stageHiddenObject;
}
