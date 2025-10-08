using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using System.Threading;


public class SaveData
{
    private string filePath;
    private string achievmentDataPath;

    public SaveFileData _SaveFileData;
    public AchievementData _AchievementData; // TODO 1206
    public Dictionary<string, MapSaveData> dic = new();

    public string savePath => filePath;
    public void SetUp()
    {
        filePath = Path.Combine(Application.persistentDataPath, "savefile.json");

        achievmentDataPath = Path.Combine(Application.persistentDataPath,"acData.json");

        Debug.Log(filePath);
        SearchSaveFile();
        
        //TODO 1206
        SearchAcData();
    }

    public void DeleteSaveFile()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Save file deleted: " + filePath);
        }
        else
        {
            Debug.Log("Save file does not exist at: " + filePath);
        }

        if (File.Exists(achievmentDataPath))
        {
            File.Delete(achievmentDataPath);
            Debug.Log("Save file deleted: " + achievmentDataPath);
        }
        else
        {
            Debug.Log("Save file does not exist at: " + achievmentDataPath);
        }
    }

    private void SearchSaveFile()
    {
         UI_SaveAndLoad uI_SaveAndLoad =  GetUI_SaveAndLoad();
        if (File.Exists(filePath))
        {
            Load();
        }
        else
        {
            uI_SaveAndLoad.LoadData(Create_NewSaveDataFile());
        }
    }

    //TODO 1101
    public void Save()
    {
        GetUI_SaveAndLoad().SaveData(Save_SaveFile());
    }
    public void Load()
    {
        GetUI_SaveAndLoad().LoadData(Load_SaveFile());
    }
   
    #region Achievement
    private CancellationTokenSource _cts = new CancellationTokenSource();
    private Task _lastAcSaveTask = Task.CompletedTask;
    private readonly object _lock = new object();
    private bool _saveScheduled = false;
    public async void SearchAcData(){
        if(File.Exists(achievmentDataPath)){
            await Ac_Load();
            return;    
        }

        Ac_CreateNewData();
        
    }
    public async Task Ac_Load(){
        string json = await File.ReadAllTextAsync(achievmentDataPath);
        _AchievementData = JsonUtility.FromJson<AchievementData>(json);
    }
    public async Task Ac_Save()
    {
        lock (_lock)
        { 
            if (_saveScheduled)
            {
                _cts.Cancel();
                _cts = new CancellationTokenSource();
            }
            _saveScheduled = true;
        }

        try
        {
            await Task.Delay(1000, _cts.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        lock (_lock)
        {
            _saveScheduled = false;
            _lastAcSaveTask = PerformAc_Save();
        }

        await _lastAcSaveTask;

    }

    private async Task PerformAc_Save(){
        string json = JsonUtility.ToJson(_AchievementData);
        await WriteTextAsync(achievmentDataPath,json);
        // Debug.Log("Ac data Save");
    }
    public async void Ac_CreateNewData(){
        _AchievementData = new AchievementData();
        await Ac_Save();
    }
    #endregion

     #region  Intergrity Check
    //1101
    private async Task IntergrityCheck(){
       await Task.Run(() => {
        int updatedVariableCount = 0;
        
        var keysToRemove = dic.Keys.Where(key => !Managers.Data.mapData.mapAllDictionary.ContainsKey(key)).ToList();
        //----------------------------------------------------Map ID Check, Update and Remove
        foreach (var key in keysToRemove) { 
            dic.Remove(key);
            Debug.Log($"Delete: {key}");
            updatedVariableCount++;
        }
        foreach (var source in Managers.Data.mapData.mapAllDictionary) {
            if (!dic.ContainsKey(source.Key)) {
                AddDic_NewMapSource(source.Value);
                Debug.Log($"Add: {source.Key}");
                updatedVariableCount++;
            }else if(!IntergrityCheck_Detail(source.Value,dic[source.Key])){
                updatedVariableCount++;
            }
        }
        //----------------------------------------------------Map ID Check, Update and Remove



            return updatedVariableCount > 0;
        }).ContinueWith(task => {
            if (task.Result) Save();
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private bool IntergrityCheck_Detail(Map value,MapSaveData data){
     return data.IntergrityCheck(value);
    }
   

    private void AddDic_NewMapSource(Map map)
    {
        MapSaveData data = new MapSaveData(map.mapID,map.subMapName, map.nextMapId,false,false,map.stageDifficulty,map.dialogueDataList,map.collectableObjectStructList);
        dic[map.mapID] = data;
    }
    #endregion

    public void ClearMap(string key,bool stageLevelUp = false)
    {
        if (dic.ContainsKey(key))
        {
            var data = Managers.Game.GetClearData();
            // _SaveFileData._PlayerSaveData.AddTotalDeath(data.deathCount);
            _SaveFileData._PlayerSaveData.UpdateClearData(key);
            dic[key].ClearMapDataUpdate(data.clearTime,data.deathCount);

            if (stageLevelUp && Managers.Game.stageLevel > _SaveFileData._PlayerSaveData.curStageLevel)
            {
                _SaveFileData.StageLevelUp();
            }

            Save();

        }
        else
        {
            Debug.Log("Can't find key");
        }
    }

   
    
    private async Task Create_NewSaveDataFile()
    {
        SerializableSaveMapDataDictionary<string, MapSaveData> _SSMDD = new();

        foreach (var key in Managers.Data.mapData.mapAllDictionary.Keys)
        {
            Map map = Managers.Data.mapData.mapAllDictionary[key];
            
            _SSMDD.Add(map.mapID, new MapSaveData(
                map.mapID,  //map Name
                map.subMapName, //map SubName
                map.nextMapId, //next Map
                false, //clear
                false, //open stage
                map.stageDifficulty,
                new List<DialogueData>(map.dialogueDataList), // map.dialogueDataList,                                                     
                new List<CollectableObjectStruct>(map.collectableObjectStructList)  // map.collectableObjectStructList));
                ));

                foreach(var item in map.dialogueDataList)
                {
                    Debug.Log($"mapId:{map.mapID} ,dialogue iD: {item.dialogueId},excute :{item.excuted}");
                }

        }

        _SaveFileData = new SaveFileData(_SSMDD, new PlayerSaveData());
        dic = _SaveFileData.SerializableSaveMapDataDictionary.ToDictionary();

        await Save_SaveFile();
    }



    #region Save
    public async Task Save_SaveFile()
    {
        _SaveFileData.SSDD_Update(dic);
        string json = JsonUtility.ToJson(_SaveFileData, true);
        await WriteTextAsync(filePath, json);
        Debug.Log("Data Saved to " + filePath);
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
    #endregion

    #region Load
    private async Task Load_SaveFile()
    {
        string json = await File.ReadAllTextAsync(filePath);
        _SaveFileData = JsonUtility.FromJson<SaveFileData>(json);
        dic = _SaveFileData.SSDD_LoadDictionary();
 
        await IntergrityCheck();
    }

    #endregion
    //intergrity Check

    #region Util
    private UI_SaveAndLoad GetUI_SaveAndLoad()
    {
        Managers.UI.ShowUI<UI_SaveAndLoad>();
        return Managers.UI.GetUI<UI_SaveAndLoad>().GetComponent<UI_SaveAndLoad>();
    }
    #endregion


    #region  Player Data Update

    public void AddCollectable(string mapId,Vector2 position){
        dic[mapId].ModifyCollectable(position);
        _SaveFileData._PlayerSaveData.AddCollectable();
        Save();
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


    public void SSDD_Update(Dictionary<string,MapSaveData> dic)
    {
        SerializableSaveMapDataDictionary.FromDictionary(dic);
    }
    public Dictionary<string,MapSaveData> SSDD_LoadDictionary()
    {
        return SerializableSaveMapDataDictionary.ToDictionary();
    }

    public void StageLevelUp()
    {
        int curStage = ++_PlayerSaveData.curStageLevel;
        string firstStageMapName = Managers.Data.mapData.GetMainMapStageArray(curStage)[0].mapID;
        try{
            Managers.Data.saveData.dic[firstStageMapName].openStage = true;
        }catch{
            Debug.Log("Can't Find key");
        }
        
    }


}

[Serializable]
public class MapSaveData
{
    public string mapName;
    public string mapSubName;
    public string nextMapId;
    public bool openStage;
    public bool clear;
    public float shortestClearTime;
    public float recentlyClearTime;
    public int deathCount; //해당맵에 몇번 죽었나 
    public int stageDifficulty;
    public List<DialogueData> _DialogueDataList;//해당 맵에 존재하는 다이얼로그 트리거 오브젝트
    public List<CollectableObjectStruct> _CollectableObjectStructList;

    public MapSaveData(string mapName, string mapSubName,string nextMapId,bool clear, bool openStage, int stageDifficulty,
                        List<DialogueData> _DialogueDataList,List<CollectableObjectStruct> _CollectableObjectStructList) //초기화
    {
        this.mapName = mapName;
        this.mapSubName = mapSubName;
        this.nextMapId = nextMapId;
        this.clear = clear;
        this.openStage = openStage;
        shortestClearTime = 0;
        recentlyClearTime = 0;
        deathCount = 0;

        this.stageDifficulty = stageDifficulty;

        this._DialogueDataList = _DialogueDataList;
        this._CollectableObjectStructList = _CollectableObjectStructList;
    }

    public void ModifyDialogueData(int id)
    {
        for (int i = 0; i < _DialogueDataList.Count; i++)
        {
            if (_DialogueDataList[i].dialogueId == id)
            {
                DialogueData data = _DialogueDataList[i];
                DialogueData modifyData = new DialogueData(data.id, data.dialogueId, true, data.position, data.quaternion, data.scale);
                _DialogueDataList[i] = modifyData;
                return;
            }
        }
    }
    #region  Clear
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
        openStage = true;
        ModifyClearTime(clearTime);
        this.deathCount += deathCount;
        if(!string.IsNullOrEmpty(nextMapId)){
            try{
                Managers.Data.saveData.dic[nextMapId].openStage = true;
            }catch{
                Debug.Log("Can't Find key");
            }
            
        }
        
    }
    #endregion
    #region  Intergrity Check
    public bool IntergrityCheck(Map map){
        if(!DialougeCheck(map)){
            return false;
        }
        if (this.mapSubName != map.subMapName)
        {
            this.mapSubName = map.subMapName;
            Debug.Log($"Intergrity : {mapSubName}");
            return false;
        } 

        return true;
    }
    
    private bool DialougeCheck(Map map){
         if(map.dialogueDataList.Count != _DialogueDataList.Count){
            _DialogueDataList.Clear();
            Debug.Log("diff dialogueDataList Count");
            _DialogueDataList = new List<DialogueData>(map.dialogueDataList);
            return false;
        }
        
        //Dialogue Id Check
        //

        return true;
    }
    #endregion
    #region  Collectable
    public bool CheckIsFoundCollectableObject(Vector2 position){
        foreach(CollectableObjectStruct data in _CollectableObjectStructList){
            if(Mathf.Approximately(data.position.x,position.x) && Mathf.Approximately(data.position.y,position.y)){
                return data.isFound;
            }
        }
        return false;
    }    
    public void ModifyCollectable(Vector2 position){
        for(int i =0;i<_CollectableObjectStructList.Count;i++){
            Vector2 a = _CollectableObjectStructList[i].position;
            if(Mathf.Approximately(a.x,position.x) && Mathf.Approximately(a.y,position.y)){
                CollectableObjectStruct data = _CollectableObjectStructList[i];
                CollectableObjectStruct modifyData = new CollectableObjectStruct(data.id,data.position,data.quaternion,true);
                _CollectableObjectStructList[i] = modifyData;
                return;
            }
            
        }
    }
    #endregion
    
}

[Serializable]
public class PlayerSaveData
{
    public List<string> clearMapId;
    public int curStageLevel;
    //클리어한 맵들

    //State
    public bool _IstutorialClear;
    public int collectableAmount;

    //CutScene
    public bool _cutScene_Page_1;

    public PlayerSaveData()
    {
        //this.totalDeath = 0;
        this.clearMapId = new();
        curStageLevel = 0;
        //State
        _IstutorialClear = false;

        collectableAmount = 0;
        _cutScene_Page_1 = false;
    }

    public void UpdateClearData(string clearMapId)
    {
        if (!this.clearMapId.Contains(clearMapId))
            this.clearMapId.Add(clearMapId);
    }

    public void AddCollectable()
    {
        collectableAmount++;
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

#region Achievement
public class AchievementData{
    //Object
    public int use_Portal;
     //btn
    //Player
    public int player_Jumping;
    public int player_Death;
     //Death type
    //Map 1220

        
    public AchievementData(){
        use_Portal = 0;
        player_Jumping = 0;
        player_Death = 0;
    }

    public void Update_Player_Death()
    {
        player_Death++;

    }
}
#endregion
