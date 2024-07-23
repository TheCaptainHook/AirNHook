using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveData
{
    private string filePath = Path.Combine(Application.persistentDataPath, "savefile.json");
    public SaveFileData _SaveFileData;


    

    public void Create_NewSaveDataFile()
    {
        SerializableSaveMapDataDictionary<string, MapSaveData> _SSMDD = new();

        foreach(var map in Managers.Data.mapData.mapAllDictionary.Keys)
        {
            
        }
    }



    public void Save_SaveFile()
    {
        string json = JsonUtility.ToJson(_SaveFileData);
        File.WriteAllText(filePath, json);
        Debug.Log("Data Saved to " + filePath);
    }

    public void Load_SaveFile()
    {
        string json = File.ReadAllText(filePath);
        _SaveFileData = JsonUtility.FromJson<SaveFileData>(json);
    }

}


[Serializable]
public class SaveFileData
{
    public SerializableSaveMapDataDictionary<string, MapSaveData> SerializableSaveMapDataDictionary;
    public PlayerSaveData _PlayerSaveData;

}

[Serializable]
public class MapSaveData
{
    public string mapName;
    public bool clear;
    public float clearTime;
    
    public MapSaveData(string mapName,bool clear,float clearTime)
    {
        
    }
    
}

[Serializable]
public class PlayerSaveData
{
    int totalDeath;
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

