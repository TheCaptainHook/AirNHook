using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class DataManager
{
    public Languages language;

    public DataManager()
    {
        language = new Languages();
    }

    public void Setup()
    {
        language.Setup();
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
