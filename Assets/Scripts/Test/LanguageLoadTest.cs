using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class LanguageLoadTest : MonoBehaviour
{
    private Dictionary<int, string> _dict = new Dictionary<int, string>();
    
    private void Start()
    {
        var path = Path.Combine(Application.streamingAssetsPath, "Localization/");

        var dirInfo = new DirectoryInfo(path);
        var fileInfo = dirInfo.GetFiles("*.json");
        var list = new List<Sentence>();
        
        foreach (var file in fileInfo)
        {
            Debug.Log(file.Name);
            var cont = File.ReadAllText(path + file.Name);
            list = JsonConvert.DeserializeObject<List<Sentence>>(cont);
        }

        foreach (var value in list)
        {
            _dict.Add(value.id, value.text);
        }

        foreach (var key in _dict.Keys)
        {
            Debug.Log(key + ", " + _dict[key]);
        }
    }
}
