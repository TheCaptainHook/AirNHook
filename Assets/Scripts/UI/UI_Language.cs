using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Language : UI_Base
{
    [SerializeField] private TMP_Dropdown _languageDropDown;
    private Dictionary<string, string> _dict;
    private List<string> _fileNames;
    private List<string> _languageNames;

    protected override void Start()
    {
        base.Start();
        
        GetLanguageList();
    }

    public override void OnEnable()
    {
        
    }

    private void GetLanguageList()
    {
        // TODO Data Manager에서 처리
        var path = Path.Combine(Application.streamingAssetsPath, "Localization/");

        _fileNames = Managers.Data.GetJsonNames(path, "*.json");
        
        _languageNames = _fileNames.Select(fileName =>
                JsonConvert.DeserializeObject<Sentence[]>(File.ReadAllText(path + fileName + ".json")))
            .Select(list => list[0].text).ToList();

        _dict = new Dictionary<string, string>();
        for (var i = 0; i < _fileNames.Count; i++)
        {
            _dict.Add(_languageNames[i], _fileNames[i]);
        }
        
        _languageDropDown.ClearOptions();
        _languageDropDown.AddOptions(_languageNames);
    }

    public void SetLocalization(int index)
    {
        Managers.Data.language.SetLanguage(_dict[_languageNames[index]]);
    }
}
