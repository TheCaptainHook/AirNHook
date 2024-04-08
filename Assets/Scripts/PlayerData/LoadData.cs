using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadData : MonoBehaviour
{
    public Dictionary<int, PlayerData> playerData = new Dictionary<int, PlayerData>();

    private void Start()
    {
        var path = Path.Combine(Application.dataPath, "Resources/PlayerData/PlayerData.json");

        var list = Managers.Data.ReadJson<PlayerData>(path);

        foreach(var sentence in list)
        {
            playerData.Add(sentence.StageID, sentence);
        }
        
        foreach(var key in playerData.Keys)
        {
            Debug.Log(playerData[key].StageID);
        }
    }
}
