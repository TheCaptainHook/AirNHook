using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public struct Sentence
{
    public int id;
    public string text;
}

public class LanguageData
{
    public Dictionary<int, string> dict;
    
    private string _currentLanguage;

    public void Setup()
    {
        // 저장된 데이터 확인
        if(!PlayerPrefs.HasKey("Language"))
            PlayerPrefs.SetString("Language", "English");

        _currentLanguage = PlayerPrefs.GetString("Language");
        
        // 주소 불러오기
        var path = Path.Combine(Application.streamingAssetsPath, "Localization/" + _currentLanguage + ".json");

        var list = Managers.Data.ReadJson<Sentence>(path);
        
        // Dictionary에 추가.
        dict = new Dictionary<int, string>();
        foreach (var sentence in list)
        {
            dict.Add(sentence.id, sentence.text);
        }
        
        // 언어 세팅 설정
        Managers.UI.SettingLanguage();
    }

    /// <summary> 언어 변경시 호출. </summary>
    public void SetLanguage(string language)
    {
        _currentLanguage = language;
        PlayerPrefs.SetString("Language", _currentLanguage);
        
        // 주소 불러오기
        var path = Path.Combine(Application.streamingAssetsPath, "Localization/" + _currentLanguage + ".json");

        var list = Managers.Data.ReadJson<Sentence>(path);
        
        // Dictionary에 추가.
        dict.Clear();
        foreach (var sentence in list)
        {
            dict.Add(sentence.id, sentence.text);
        }
        
        // 언어 세팅 설정
        Managers.UI.SettingLanguage();
    }

    public string GetSentence(int id)
    {
        return dict.GetValueOrDefault(id, " ");
    }
}
