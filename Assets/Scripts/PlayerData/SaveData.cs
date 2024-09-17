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
            // DICCHECK_TESTCODE();
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
            _SSMDD.Add(map.mapID, new MapSaveData(map.mapID,false,false,0,map.dialogueDataList));
        }

        _SaveFileData = new SaveFileData(_SSMDD, new PlayerSaveData());
        dic = _SaveFileData.SerializableSaveMapDataDictionary.ToDictionary();

        await Save_SaveFile();
    }
    //TEST 0917
   
    //TEST 0917
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
    // private void DICCHECK_TESTCODE()
    // {
    //     foreach(var data in dic)
    //     {
    //         Debug.Log(dic[data.Key].mapName);
    //     }
    // }
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
    public bool openStage;
    public bool clear;
    public float shortestClearTime;
    public float recentlyClearTime;
    public int deathCount; //해당맵에 몇번 죽었나 
    public List<DialogueData> _DialogueDataList;//해당 맵에 존재하는 다이얼로그 트리거 오브젝트

    public MapSaveData(string mapName, bool clear, bool openStage,float clearTime, List<DialogueData> _DialogueDataList) //초기화
    {
        this.mapName = mapName;
        this.clear = clear;
        this.openStage = openStage;
        this.shortestClearTime = 0;
        this.recentlyClearTime = 0;
        this.deathCount = 0;

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
//0917 Validata must take care
    private void ValidateDialougeData(List<DialogueData> dataList){
        foreach(DialogueData data in dataList){
            foreach(DialogueData sD in _DialogueDataList){
                if(sD.dialogueId == data.dialogueId){

                    break;
                }
            }
        }
    }
    private DialogueData ValidateDialougeDataPosition(DialogueData newData , DialogueData oldData){
        DialogueData data;
        if(newData.position != oldData.position){
            data = newData;
            newData.excuted = oldData.excuted;
        }else{
            data = oldData;
        }
        return data;
    }
//0917 Validata must take care

    private void ModifyClearTime(float time)
    {
       if(shortestClearTime == 0)
        {
            shortestClearTime = time;
        }else if(shortestClearTime > time)
        {
            shortestClearTime = time;
        }

        recentlyClearTime = time;
    }


    public void ClearMapDataUpdate(float clearTime,int deathCount)
    {
        clear = true;
        ModifyClearTime(clearTime);
        this.deathCount = deathCount;
    }

    
}

[Serializable]
public class PlayerSaveData
{
    public int totalDeath;
    public int TotalDeath { get { return totalDeath; } }
    public List<string> clearMapId;
    //클리어한 맵들

    //State
    public bool _IstutorialClear;

    public PlayerSaveData()
    {
        this.totalDeath = 0;
        this.clearMapId = new();

        //State
        _IstutorialClear = false;
    }


    public void AddClearMapId(string mapId)
    {
        if (mapId == "Tutorial_3") _IstutorialClear = true;


        if (!clearMapId.Contains(mapId))
        {
            clearMapId.Add(mapId);
        }
    }

    public void AddTotalDeath(int death)
    {
        totalDeath += death;
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

    public int GetKeyIndex(TKey key)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if(key.Equals(keys[i]))
            {
                return i;
            }
        }

        return 9999;
    }
}
#endregion

