using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScreenSlice_2Box : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI mapInfoText;




    //public void SetData(PlayData data) // LoadData.playData[mapId]
    //{
    //    mapInfoText.text = $"Map ID :   {data.stageID}  \nstageClear :  {data.stageClear}   \nclearTime : {data.clearTime}  \ntotalDeath :  {data.totalDeath}   \ndeathCount :  {data.deathCount}   \nskip : {data.skip}    \nstageLevel :  {data.stageLevel}\nstageHiddenObject :  {data.stageHiddenObject}(준비중)";
    //}

    //Todo 0726(Data)
    public void SetData(string mapId) // LoadData.playData[mapId]
    {
        MapSaveData mapSaveData = Managers.Data.saveData.dic[mapId];
        
        //mapInfoText.text = $"Map ID :   {data.stageID}  \nstageClear :  {data.stageClear}   \nclearTime : {data.clearTime}  \ntotalDeath :  {data.totalDeath}   \ndeathCount :  {data.deathCount}   \nskip : {data.skip}    \nstageLevel :  {data.stageLevel}\nstageHiddenObject :  {data.stageHiddenObject}(준비중)";

        mapInfoText.text = $"Map ID\t:\t{mapSaveData.mapName}\t\nShortest Clear Time\t:\t{mapSaveData.shortestClearTime}\t\nRecently Clear Time\t:\t{mapSaveData.recentlyClearTime}\t\nDeath Count\t:\t{mapSaveData.deathCount}\t\n";

    }
    //Todo 0726
    public void Reset()
    {
        mapInfoText.text = "";
    }

}
