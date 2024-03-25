using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class DataManager
{
    public LanguageData language;
    public MapData mapData;
    public DataManager()
    {
        language = new LanguageData();
        mapData = new MapData();
    }

    public void Setup()
    {
        language.Setup();
        mapData.SetUp();
    }
    
    public T[] ReadJson<T>(string path)
    {
        // json 읽기
        var content = File.ReadAllText(path);
        
        // Json으로 변환
        var list = JsonConvert.DeserializeObject<T[]>(content);
        
        return list;
    }

    public List<string> GetJsonNames(string path, string searchPattern)
    {
        var filenames = Directory.GetFiles(path, searchPattern)
            .Select(Path.GetFileNameWithoutExtension).ToList();
        
        return filenames;
    }
}
