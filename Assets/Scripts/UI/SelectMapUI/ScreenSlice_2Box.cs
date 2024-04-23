using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScreenSlice_2Box : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI mapInfoText;




    public void SetData(PlayData data) // LoadData.playData[mapId]
    {
        mapInfoText.text = $"Map ID :   {data.stageID}  \nstageClear :  {data.stageClear}   \nclearTime : {data.clearTime}  \ntotalDeath :  {data.totalDeath}   \ndeathCount :  {data.deathCount}   \nskip : {data.skip}    \nstageLevel :  {data.stageLevel}\nstageHiddenObject :  {data.stageHiddenObject}(준비중)";
    }


    public void Reset()
    {
        mapInfoText.text = "";
    }

}
