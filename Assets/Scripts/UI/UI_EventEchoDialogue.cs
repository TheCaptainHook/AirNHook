using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UI_EventEchoDialogue : UI_Base
{
    [SerializeField] Image mainSprite;
    [SerializeField] TextMeshProUGUI text;


    #region  Components
    private RectTransform textMeshRectTransform;
    #endregion
    string testSentence = "Lorem Ipsum is simply dummy {n} text of the printing {}";
    private string stringToIntPattern = @"\{\}";

    //TEST
    private void Awake(){
        textMeshRectTransform = text.transform as RectTransform;
    }

    protected override void Start()
    {
        text.text = Replace_PlaceholdersToInt(testSentence,5);
        UpdateTextMeshRectTransform();
    }
    public override void OnEnable()
    {
    }

    protected override void OpenUI()
    {
        base.OpenUI();
    }
    protected override void CloseUI()
    {
        base.CloseUI();
    }

    #region  Util
    private string GetDialogue(int id){
        return Managers.Data.language.dict[id];
    }

    private string Replace_PlaceholdersToInt(string sentence,int num){
        return Regex.Replace(sentence,stringToIntPattern,m=>num.ToString());
    }

    private void UpdateTextMeshRectTransform(){
        text.ForceMeshUpdate();

        Vector2 preferredValues = text.GetPreferredValues(text.text);

        RectTransform rectTransform = text.GetComponent<RectTransform>();
        float containerHeight = rectTransform.rect.height;

        float remainingSpace = containerHeight - preferredValues.y;
        if (remainingSpace < 0) remainingSpace = 0;
        float marginTop = remainingSpace / 2f;
        float marginBottom = remainingSpace / 2f;

        Vector4 currentMargin = text.margin;
        text.margin = new Vector4(currentMargin.x, marginTop, currentMargin.z, marginBottom);
    }
    #endregion
}
