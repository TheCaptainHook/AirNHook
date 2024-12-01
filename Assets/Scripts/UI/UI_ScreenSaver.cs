using UnityEngine;
using TMPro;
using UnityEngine.Audio;

public class UI_ScreenSaver : UI_Base
{
    #region SerializeFields
    [Header("Animations")]
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private AnimationCurve _curve2;
    
    [Header("Frames")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _titleImg;

    [Header("Texts")]
    [SerializeField] private TMP_Text _pressText;

    [SerializeField] private GameObject _textToBounce;

    private AudioMixer _audioMixer;
    private bool _keyPressed = false;
    #endregion
    
    public override void OnEnable()
    {
        OpenUI();
        Show();
        _keyPressed = false;
        Managers.Sound.PlayBGM(GlobalText.TITLE_SOUND);
    }

    private void Show()
    {
        //StartCoroutine(Fade(true, _canvasGroup));
        StartCoroutine(BounceRoutine(_titleImg,Vector3.one * 0.5f, Vector3.one * 0.475f, _curve));
        StartCoroutine(BounceRoutine(_textToBounce,Vector3.one * 1.05f, Vector3.one * 0.975f, _curve2));
    }

    protected override void Start()
    {
        base.Start();
        _audioMixer = Managers.Sound.audioMixer;
        _audioMixer.SetFloat("MasterParam",GetAudioMixVolume(PlayerPrefs.GetFloat("MasterVolume", 1f)));
        _audioMixer.SetFloat("BGMParam", GetAudioMixVolume(PlayerPrefs.GetFloat("BGMVolume", 1f)));
        _audioMixer.SetFloat("EffectsParam", GetAudioMixVolume(PlayerPrefs.GetFloat("EffectsVolume", 1f)));
    }

    private void Update()
    {
        if (Input.anyKey && !_keyPressed)
        {
            OnClick();
            _keyPressed = true;
        }

        if (!_keyPressed) return;
        Managers.UI.ShowUI<UI_Title>();
        Managers.UI.HideUI<UI_ScreenSaver>();
    }

    private float GetAudioMixVolume(float volume)
    {
        return Mathf.Log10(volume) * 20;
    }

    private void OnClick()
    {
        Managers.Sound.PlaySound(GlobalText.UI_CLICK_SOUND, 0.35f);
    }

    public override void SetLanguage()
    {
        SetSentence(_pressText, 2012);
    }
}
