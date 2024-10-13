using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Threading;

public enum TextBoxPivot
{
    Top,
    Bottom
}
public enum SpritePosition
{
    Left,
    Right,
    Both,
    None
}
public enum AnchorPresets
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    MiddleCenter,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight,
    StretchHorizontalTop,
    StretchHorizontalMiddle,
    StretchHorizontalBottom,
    StretchVerticalLeft,
    StretchVerticalCenter,
    StretchVerticalRight,
    StretchAll
}

public class UI_Dialogue : UI_Base
{
    [Header("For Anims")] 
    [SerializeField] private GameObject _mainFrame;
    [SerializeField] private GameObject _leftSprite;
    [SerializeField] private GameObject _rightSprite;
    [SerializeField] private AnimationCurve _curve;
    
    [Header("Text Box Anchor Position")]
    public float _TopTextBoxPosition;
    public float _BottomTextBoxPosition;
    public float _LeftTextBoxPosition;
    public float _RightTextBoxPosition;

    [Header("Sprite Anchor Positon")]
    public float _SpriteTopPosition;
    public float _SpriteBottomPosition;


    [Header("Rect Transform")]
    [SerializeField] RectTransform container;
    [SerializeField] RectTransform _TextBoxRT;
    [SerializeField] RectTransform _SpriteLeftRT;
    [SerializeField] RectTransform _SpriteRightRT;

    [Header("Components")]
    [SerializeField] TextMeshProUGUI _TextBoxText;
    [SerializeField] TextMeshProUGUI _TextNameText;
    [SerializeField] Image _Panel;
    private Util Util = new Util();
    
    public Image _LeftImage;
    public Image _RightImage;
    


    [Header("Main Logic")]
    private int dialogueId;
    private int nextDialogueIndex;
    private List<Dialogue> list;
    private bool onPrograss;
    private string path = "Arts/Sprites/DialogueSprites";
    
    private string _PreviousDialogueName;//

    [Header("Color")]
    Color _Alpha_1 = new Color(1, 1, 1, 1);
    Color _Alpha_0 = new Color(1, 1, 1, 0);
    Color _Alpha_translucent = new Color(0, 0, 0, 180f/255f);

    [Header("WaitforSecond")]
    WaitForSeconds delay = new WaitForSeconds(0.1f);


    private CancellationTokenSource _cancellationTokenSource;


    public override void OnEnable()
    {
        StartCoroutine(BounceRoutine(_leftSprite, Vector3.one, Vector3.one * 0.935f, _curve));
        StartCoroutine(BounceRoutine(_rightSprite, Vector3.one, Vector3.one * 0.935f, _curve));
    }

    // public void OnDisable()
    // {
    //     //켜질 때 다시 커지는 애니메이션이 나오도록
    //     _mainFrame.transform.localScale = Vector3.one * 0.1f;
    // }

    private void Awake()
    {
        _LeftImage = _SpriteLeftRT.GetComponent<Image>();
        _RightImage = _SpriteRightRT.GetComponent<Image>();
    }

   

    public void SetData(int id)
    {
        //Init
        nextDialogueIndex = 1;
        dialogueId = id;
        
        list = Managers.Data.language.dialogueMap[id];

        StartCoroutine(StartDialogue());

    }


    IEnumerator StartDialogue()
    {
        _Panel.color = _Alpha_translucent;
        // Player 못 움직이게 설정
        GameObject player = Managers.Game.Player;
        player.GetComponent<PlayerMovement>().canControl = false;
        Managers.Game.Player.GetComponent<Rigidbody2D>().velocity  = Vector2.zero;
        //
        //TextBox SetActive
        if (!_TextBoxRT.gameObject.activeSelf)
        {
            _TextBoxRT.gameObject.SetActive(true);
        }
        //TextBox SetActive

        for (int i = 0; i < list.Count; i++)
        {
            yield return Dialogue(list[i]);
            nextDialogueIndex++;
        }



        //Dialogue ShutDown
        Managers.Game.PlayerAndMapSavaDataUpdate();
        DialogueReset(); //todo test

        _Panel.color = _Alpha_0;
        //Dialogue ShutDown

        // Player 다시 움직이게 설정
        player.GetComponent<PlayerMovement>().canControl = true;
        //
        Managers.UI.HideUI<UI_Dialogue>();


    }



    IEnumerator Dialogue(Dialogue dialogue)
    {

        //Sprite image, Pivot Setting and FadeIn
        DialogueSpriteSetting(dialogue);
        //Sprite image, Pivot Setting and FadeIn

        //TextBox and Pivot Setting
        DialoguePositionSetting(dialogue);
        //TextBox and Pivot Setting
        
        _PreviousDialogueName = dialogue.name;

        //Typing Effect
        yield return ScaleOverTime(new Vector3(0.7f, 0.7f), new Vector3(1.2f, 1.2f),0.2f);
        yield return ScaleOverTime(new Vector3(1.2f, 1.2f), new Vector3(1f, 1f), 0.3f);

        _cancellationTokenSource = new CancellationTokenSource();
        Task task = Util.TypingEffectTask(_TextBoxText, Managers.Data.language.dict[dialogue.sentenceID], Color.black, 42, .05f, _cancellationTokenSource,true);
        //Typing Effect

        //TypingEffectTask Cancel
        while (!task.IsCompleted)
        {
            if (Input.anyKeyDown)
            {
                _cancellationTokenSource.Cancel();

            }
            yield return null;
        }
        //TypingEffectTask Cancel

        //Start Next Dialogue
        bool onAnyKey = false;
        while (!onAnyKey)
        {
            if (Input.anyKeyDown)
            {
                onAnyKey = true;
            }
            Debug.Log("Delay");
            yield return null;
        }

        _TextBoxText.text = "";
        //Start Next Dialogue

        ///
        /// 다음 실행할 다이얼로그가 없거나,다음 실행할 다이얼로그가 같은 캐릭턱 아닌 경우
        ///
        if (nextDialogueIndex>=list.Count || !Check_PrivousCharacterNameMatch(list[nextDialogueIndex].name))
        {
            _TextNameText.text = "";
            if (_LeftImage.enabled)
            {
                StartCoroutine(SpriteFadeOut(_LeftImage));
            }

            if (_RightImage.enabled)
            {
                StartCoroutine(SpriteFadeOut(_RightImage));
                
            }

            yield return new WaitForSeconds(.5f);

        }
    

    }


    //todo 0718
   // 텍스트박스 아래위치일때 스프라이트 대각선 위쪽으로
   //위쪽위치할때는 그대로.ㄴ

    private void DialogueSpriteSetting(Dialogue dialogue)
    {
        string path = $"{this.path}/{dialogue.name}/{dialogue.emotion}";

        switch (dialogue.spritePosition)
        {
            case SpritePosition.Left:
               
                _RightImage.enabled = false;
                _LeftImage.sprite = Resources.Load<Sprite>(path);
                if (!_LeftImage.enabled)
                {
                    StartCoroutine(SpriteFadeIn(_LeftImage));
                }
                _TextNameText.text = dialogue.name;
                break;
            case SpritePosition.Right:
                _LeftImage.enabled = false;
                _RightImage.sprite = Resources.Load<Sprite>(path);

                if (!_RightImage.enabled)
                {
                    //Fade in
                    StartCoroutine(SpriteFadeIn(_RightImage));
                }
                _TextNameText.text = dialogue.name;
                break;
            case SpritePosition.Both:
                Sprite_SettingBoth(dialogue);
                _TextNameText.text = "";
                break;
            case SpritePosition.None:
                break;
            default:
                break;
        }
    }
    private void DialoguePositionSetting(Dialogue dialogue)
    {
        switch (dialogue.textBoxPivot)
        {
            case TextBoxPivot.Top:
                if(dialogue.spritePosition == SpritePosition.Left)
                {
                    SetAnchor(_TextBoxRT, AnchorPresets.TopRight);
                 
                    _TextBoxRT.anchoredPosition = new Vector2(_RightTextBoxPosition, _TopTextBoxPosition);
                }
                else if(dialogue.spritePosition == SpritePosition.Right)
                {
                    SetAnchor(_TextBoxRT, AnchorPresets.TopLeft);
                    _TextBoxRT.anchoredPosition = new Vector2(_LeftTextBoxPosition, _TopTextBoxPosition);

                }
                else
                {
                    SetAnchor(_TextBoxRT, AnchorPresets.TopCenter);
                    _TextBoxRT.anchoredPosition = new Vector2(0, _TopTextBoxPosition);
                }

                SetAnchor(_SpriteLeftRT, AnchorPresets.TopLeft);
                SetAnchor(_SpriteRightRT, AnchorPresets.TopRight);

               
                _SpriteLeftRT.anchoredPosition = new Vector2(_SpriteLeftRT.anchoredPosition.x, _SpriteTopPosition);
                _SpriteRightRT.anchoredPosition = new Vector2(_SpriteRightRT.anchoredPosition.x, _SpriteTopPosition);

                break;
            case TextBoxPivot.Bottom:
                //if (dialogue.spritePosition == SpritePosition.Left)
                //{
                //    SetAnchor(_TextBoxRT, AnchorPresets.BottomRight);
                //    _TextBoxRT.anchoredPosition = new Vector2(_RightTextBoxPosition, _BottomTextBoxPosition);
                //}
                //else if (dialogue.spritePosition == SpritePosition.Right)
                //{
                //    SetAnchor(_TextBoxRT, AnchorPresets.BottomLeft);
                //    _TextBoxRT.anchoredPosition = new Vector2(_LeftTextBoxPosition, _BottomTextBoxPosition);
                //}
                //else
                //{
                //    SetAnchor(_TextBoxRT, AnchorPresets.BottomCenter);
                //    _TextBoxRT.anchoredPosition = new Vector2(0, _BottomTextBoxPosition);
                //}
                SetAnchor(_TextBoxRT, AnchorPresets.BottomCenter);
                SetAnchor(_SpriteLeftRT, AnchorPresets.BottomLeft);
                SetAnchor(_SpriteRightRT, AnchorPresets.BottomRight);

                _TextBoxRT.anchoredPosition = new Vector2(0, _BottomTextBoxPosition);
                _SpriteLeftRT.anchoredPosition = new Vector2(_SpriteLeftRT.anchoredPosition.x, _SpriteBottomPosition);
                _SpriteRightRT.anchoredPosition = new Vector2(_SpriteRightRT.anchoredPosition.x, _SpriteBottomPosition);

                break;
        }
    }




    #region Utility

    private void Sprite_SettingBoth(Dialogue dialogue)
    {
        string[] names = Split_DialogueName(dialogue.name);
        string[] emotions = Split_DialogueName(dialogue.emotion);

        string path1 = $"{this.path}/{names[0]}/{emotions[0]}";
        string path2 = $"{this.path}/{names[1]}/{emotions[1]}";

        _LeftImage.sprite = Resources.Load<Sprite>(path1);
        _RightImage.sprite = Resources.Load<Sprite>(path2);

        if (!_LeftImage.enabled)
        {
            StartCoroutine(SpriteFadeIn(_LeftImage));
        }
        if (!_RightImage.enabled)
        {
            StartCoroutine(SpriteFadeIn(_RightImage));
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="nextName">this parameter is next Dialogue name</param>
    /// <returns></returns>
    private bool Check_PrivousCharacterNameMatch(string nextName)
    {
        if (string.Empty == nextName) { Debug.Log("false"); return false; }

        if (_PreviousDialogueName.Contains(nextName)) { Debug.Log("next dialogue name contains previouse dialogue name,true"); return true; }
        if (nextName.Contains(_PreviousDialogueName)) { Debug.Log("previouse Dialogue name contain next dialogue name,true"); return true; }
        if (string.Compare(_PreviousDialogueName, nextName) == 0) { Debug.Log("Compare =0,ture"); return true; }

        Debug.Log("else, false");
        return false;

    }

    public string[] Split_DialogueName(string str)
    {
        return str.Split(",", System.StringSplitOptions.None);
    }



    private void DialogueReset()
    {
        _PreviousDialogueName = "";
        _LeftImage.enabled = false;
        _RightImage.enabled = false;
        _TextBoxRT.gameObject.SetActive(false);


    }
  
    IEnumerator SpriteFadeOut(Image image)
    {

        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime;
            image.color = Color.Lerp(image.color, _Alpha_0, percent);
            yield return null;
        }

        image.color = _Alpha_0;
        image.enabled = false;
        _mainFrame.transform.localScale = Vector3.one * 0.1f;
    }

    IEnumerator SpriteFadeIn(Image image)
    {
        image.enabled = true;
        //AppendAnim(_mainFrame, 1.15f, 0.2f, 1f, 0.1f);
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime;
            image.color = Color.Lerp(image.color, _Alpha_1, percent);
            yield return null;
        }
        image.color = _Alpha_1;
    }

    //TextBoxRT
    private IEnumerator ScaleOverTime(Vector3 fromScale, Vector3 toScale, float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            _TextBoxRT.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = toScale;
    }

    #endregion

    #region Anchor

    public void SetAnchor(RectTransform rectTransform, AnchorPresets anchorPreset)
    {
        switch (anchorPreset)
        {
            case AnchorPresets.TopLeft:
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                break;
            case AnchorPresets.TopCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 1);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                break;
            case AnchorPresets.TopRight:
                rectTransform.anchorMin = new Vector2(1, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                break;
            case AnchorPresets.MiddleLeft:
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(0, 0.5f);
                break;
            case AnchorPresets.MiddleCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                break;
            case AnchorPresets.MiddleRight:
                rectTransform.anchorMin = new Vector2(1, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                break;
            case AnchorPresets.BottomLeft:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                break;
            case AnchorPresets.BottomCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 0);
                break;
            case AnchorPresets.BottomRight:
                rectTransform.anchorMin = new Vector2(1, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                break;
            case AnchorPresets.StretchHorizontalTop:
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                break;
            case AnchorPresets.StretchHorizontalMiddle:
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                break;
            case AnchorPresets.StretchHorizontalBottom:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                break;
            case AnchorPresets.StretchVerticalLeft:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 1);
                break;
            case AnchorPresets.StretchVerticalCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                break;
            case AnchorPresets.StretchVerticalRight:
                rectTransform.anchorMin = new Vector2(1, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                break;
            case AnchorPresets.StretchAll:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                break;
        } 

    }




    #endregion




    #region Dialogue
    public IEnumerator TutorialClearDialogue()
    {
        nextDialogueIndex = 1;
        dialogueId = 105;
        list = Managers.Data.language.dialogueMap[105];

        if (!gameObject.activeSelf) gameObject.SetActive(true);

        yield return StartDialogue();
        
    }


    #endregion
}
