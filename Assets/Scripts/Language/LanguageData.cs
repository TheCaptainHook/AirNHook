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

    //testCode Dialogue System 0707
    public Dictionary<int, List<Dialogue>> map;
    
    //testCode Dialogue System 0707


    public void Setup()
    {
        // 저장된 데이터 확인
        if (!PlayerPrefs.HasKey("Language"))
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

        //testCode Dialogue System 0707
        map = GetDialogueData("English");
        //testCode Dialogue System 0707

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

        //testCode Dialogue System 0707
        map.Clear();
        map = GetDialogueData(language);
        //testCode Dialogue System 0707

        // 언어 세팅 설정
        Managers.UI.SettingLanguage();
    }

    

    public string GetSentence(int id)
    {
        return dict.GetValueOrDefault(id, " ");
    }

    #region Dialogue System Test Code 0710

    public Dictionary<int, List<Dialogue>> GetDialogueData(string language)
    {
        List<Dictionary<string, object>> list = CSVReader.Read($"DialogueDB_{language}");

        Dictionary<int, List<Dialogue>> map = new();

        foreach (var entry in list)
        {
            int id = (int)entry["id"];
            int index = (int)entry["index"];
            string name = entry["name"].ToString();
            string emotion = entry["emotion"].ToString();
            string spritePosition = entry["spritePosition"].ToString();
            string textBoxPosition = entry["textBoxPosition"].ToString();
            string sentence = entry["sentence"].ToString();
            
            Dialogue dialogue = new Dialogue(index, name, emotion, textBoxPosition, spritePosition, sentence);

            if (!map.ContainsKey(id))
            {
                map[id] = new List<Dialogue>();
            }
            map[id].Add(dialogue);
        }

        //foreach (var aa in map.Keys)
        //{
        //    List<Dialogue> aaaa = map[aa];
        //    foreach (Dialogue di in aaaa)
        //    {
        //        Debug.Log($"id:{aa}\nindex:{di.index}\nemotion:{di.emotion}\nsp : {di.spritePosition}\ntp : {di.textBoxPivot}\nsentence:{di.sentence}");
        //    }

        //}

        return map;
    }

   
    #endregion
}

public class Dialogue
{
    public int index;
    public string name;
    public string emotion;
    public TextBoxPivot textBoxPivot;
    public SpritePosition spritePosition;
    public string sentence;

    public Dialogue(int index, string name, string emotion,string textBoxPivotStr,string spritePositionStr, string sentence)
    {
        this.index = index;
        this.name = name;
        this.emotion = emotion;
        this.sentence = sentence;


        if (Enum.TryParse(textBoxPivotStr, true, out TextBoxPivot textBoxPivot))
        {
            this.textBoxPivot = textBoxPivot;
        }
       

        if (Enum.TryParse(spritePositionStr, true, out SpritePosition spritePosition))
        {
            this.spritePosition = spritePosition;
        }
      
    }
}





