using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;


//TODO Develop Code Line (Async): 21

public class SaveData
{
    private string filePath;
    public SaveFileData _SaveFileData;
    public Dictionary<string, MapSaveData> dic = new();



    public void SetUp()
    {
        filePath = Path.Combine(Application.persistentDataPath, "savefile.json");

        //TODO TESTCODE 0725
        SearchSaveFile();
        //DeletFile();
        //TODO TESTCODE 0725
    }

    private void SearchSaveFile()
    {
        if (File.Exists(filePath))
        {
            Load_SaveFile();
            DICCHECK_TESTCODE();
        }
        else
        {
            Create_NewSaveDataFile();
        }
    }

    // TODO 0724 Async 작업중
    private async void Create_NewSaveDataFile()
    {
        SerializableSaveMapDataDictionary<string, MapSaveData> _SSMDD = new();

        foreach (var key in Managers.Data.mapData.mapAllDictionary.Keys)
        {
            Map map = Managers.Data.mapData.mapAllDictionary[key];
            _SSMDD.Add(map.mapID, new MapSaveData(map.mapID,false,0,map.dialogueDataList));
        }

        _SaveFileData = new SaveFileData(_SSMDD, new PlayerSaveData());
        dic = _SaveFileData.SerializableSaveMapDataDictionary.ToDictionary();

        await Save_SaveFile();
    }

    public async Task Save_SaveFile()
    {
        _SaveFileData.SerializableSaveMapDataDictionary.FromDictionary(dic);
       
        string json = JsonUtility.ToJson(_SaveFileData);
        //File.WriteAllText(filePath, json);
        await WriteTextAsync(filePath, json);
        Debug.Log("Data Saved to " + filePath);
    }

    private void Load_SaveFile()
    {
        string json = File.ReadAllText(filePath);
        _SaveFileData = JsonUtility.FromJson<SaveFileData>(json);

        dic = _SaveFileData.SerializableSaveMapDataDictionary.ToDictionary();
        Debug.Log("Data Load");
    }


    private async Task WriteTextAsync(string path, string content)
    {
        byte[] encodedText = Encoding.UTF8.GetBytes(content);

        using (FileStream sourceStream = new FileStream(path,
            FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true))
        {
            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        };
    }


    private void DeletFile()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("File deleted: " + filePath);
        }
    }

    #region TEST CODE
    private void DICCHECK_TESTCODE()
    {
        foreach(var data in dic)
        {
            Debug.Log(dic[data.Key].mapName);
        }
    }
    #endregion

}

[Serializable]
public class SaveFileData
{
    public SerializableSaveMapDataDictionary<string, MapSaveData> SerializableSaveMapDataDictionary;
    public PlayerSaveData _PlayerSaveData;

    public SaveFileData(SerializableSaveMapDataDictionary<string, MapSaveData> SerializableSaveMapDataDictionary, PlayerSaveData _PlayerSaveData)
    {
        this.SerializableSaveMapDataDictionary = SerializableSaveMapDataDictionary;
        this._PlayerSaveData = _PlayerSaveData;
    }
}

[Serializable]
public class MapSaveData
{
    public string mapName;
    public bool clear;
    public float clearTime;
    public List<DialogueData> _DialogueDataList;

    public MapSaveData(string mapName, bool clear, float clearTime, List<DialogueData> _DialogueDataList)
    {
        this.mapName = mapName;
        this.clear = clear;
        this.clearTime = clearTime;
        this._DialogueDataList = _DialogueDataList;
    }

    public void ModifyDialogueData(int id)
    {
        for (int i = 0; i < _DialogueDataList.Count; i++)
        {
            if (_DialogueDataList[i].dialogueId == id)
            {
                DialogueData data = _DialogueDataList[i];
                DialogueData modifyData = new DialogueData(data.id, data.dialogueId, true, data.position, data.quaternion, data.scale);
                Debug.Log($"ID:{modifyData.dialogueId} Modify : {modifyData.excuted}");
                _DialogueDataList[i] = modifyData;
                return;
            }
        }
    }
}

[Serializable]
public class PlayerSaveData
{
   public int totalDeath;

   public PlayerSaveData()
    {
        this.totalDeath = 0;
    }
}



#region Serializable

[Serializable]
public class SerializableSaveMapDataDictionary<TKey, TValue>
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();
    [SerializeField]
    private List<TValue> values = new List<TValue>();

    public void Add(TKey key, TValue value)
    {
        if (keys.Contains(key))
        {
            Debug.LogWarning($"Key {key} already exists. Value will not be added.");
            return;
        }

        keys.Add(key);
        values.Add(value);
    }

    public Dictionary<TKey, TValue> ToDictionary()
    {
        Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        for (int i = 0; i < keys.Count; i++)
        {
            if (dictionary.ContainsKey(keys[i]))
            {
                Debug.LogWarning($"Duplicate key {keys[i]} found during ToDictionary conversion. Skipping.");
                continue;
            }

            dictionary[keys[i]] = values[i];
        }

        return dictionary;
    }

    public void FromDictionary(Dictionary<TKey, TValue> dictionary)
    {
        keys.Clear();
        values.Clear();

        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

}
#endregion

