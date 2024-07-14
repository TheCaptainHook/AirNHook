using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;

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
    [Header("Text Box Anchor Position")]
    public float _TopPosition;
    public float _BottomPosition;

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
    private Util Util = new Util();
    private Image _LeftImage;
    private Image _RightImage;



    [Header("Main Logic")]
    private int dialogueId;
    private int dialogueIndex;
    private List<Dialogue> list;
    private bool onPrograss;
    

    [Header("Color")]
    Color _Alpha_1 = new Color(255, 255, 255, 255);
    Color _Alpha_0 = new Color(255, 255, 255, 0);

    [Header("WaitforSecond")]
    WaitForSeconds delay = new WaitForSeconds(0.1f);


    private CancellationTokenSource _cancellationTokenSource;


    public override void OnEnable()
    {
        
    }

    protected override void Start()
    {
        _LeftImage = _SpriteLeftRT.GetComponent<Image>();
        _RightImage = _SpriteRightRT.GetComponent<Image>();



        //TestCode

        SetData(0);

    }

    //public override void SetLanguage()
    //{
    //    list = Managers.Data.language.map[dialogueId];
    //}


    /// <summary>
    /// SetData -> StartDialogue(Co) - > Dialogue(Co)
    /// </summary>
    /// <param name="id"></param>

    public void SetData(int id)
    {
        //Init
        dialogueIndex = 0;

        dialogueId = id;
        
        list = Managers.Data.language.map[id];

        StartCoroutine(StartDialogue());

    }


    IEnumerator StartDialogue()
    {
        // Player 못 움직이게 설정

        //

        for (int i = 0; i < list.Count; i++)
        {
            yield return Dialogue(list[i]);
        }



        //Dialogue ShutDown


        DialogueReset(); //todo test

        //Dialogue ShutDown

    }

    public string[] Split_DialogueName(string str)
    {
       return str.Split(",", System.StringSplitOptions.None);

    }

    IEnumerator Dialogue(Dialogue dialogue)
    {
        Sprite _LeftImage;
        Sprite _RightImage;

        //Sprite Alpha,Pivot Setting
        // dialogue.spritePositon == left 면 이미지 플립시켜주기
        switch (dialogue.spritePosition)
        {
            case SpritePosition.Left:
                break;
            case SpritePosition.Right:
                break;
            case SpritePosition.Both:
                break;
            case SpritePosition.None:
                break;
            default:
                break;
        }

        //Sprite Alpha,Pivot Setting


        //TextBox Alpha,Pivot Setting
        switch (dialogue.textBoxPivot)
        {
            case TextBoxPivot.Top:
                SetAnchor(_TextBoxRT, AnchorPresets.TopCenter);
                SetAnchor(_SpriteLeftRT, AnchorPresets.TopLeft);
                SetAnchor(_SpriteRightRT, AnchorPresets.TopRight);

                _TextBoxRT.anchoredPosition = new Vector2(_TextBoxRT.anchoredPosition.x, _TopPosition);
                _SpriteLeftRT.anchoredPosition = new Vector2(_SpriteLeftRT.anchoredPosition.x, _SpriteTopPosition);
                _SpriteRightRT.anchoredPosition = new Vector2(_SpriteRightRT.anchoredPosition.x, _SpriteTopPosition);

                break;
            case TextBoxPivot.Bottom:
                SetAnchor(_TextBoxRT, AnchorPresets.BottomCenter);
                SetAnchor(_SpriteLeftRT, AnchorPresets.BottomLeft);
                SetAnchor(_SpriteRightRT, AnchorPresets.BottomRight);
                _TextBoxRT.anchoredPosition = new Vector2(_TextBoxRT.anchoredPosition.x, _BottomPosition);
                _SpriteLeftRT.anchoredPosition = new Vector2(_SpriteLeftRT.anchoredPosition.x, _SpriteBottomPosition);
                _SpriteRightRT.anchoredPosition = new Vector2(_SpriteRightRT.anchoredPosition.x, _SpriteBottomPosition);

                break;
        }
        //TextBox Alpha, Pivot Setting
        //TextName Text
        _TextNameText.text = dialogue.name;
        //TextName Text


        _cancellationTokenSource = new CancellationTokenSource();

        Task task = Util.TypingEffectTask(_TextBoxText, dialogue.sentence, Color.black, 36, 1f, _cancellationTokenSource);

        while (!task.IsCompleted)
        {
            if (Input.anyKeyDown)
            {
                _cancellationTokenSource.Cancel();

            }
            yield return null;
        }
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


    }


    private void DialogueReset()
    {
        _LeftImage.enabled = false;
        _RightImage.enabled = false;
        _TextBoxRT.gameObject.SetActive(false);
    }


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
}
