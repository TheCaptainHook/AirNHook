using UnityEngine;
using TMPro;
using UnityEngine.Audio;

public class UI_ScreenSaver : UI_Base
{
    #region SerializeFields
    [Header("Animations")]
    [SerializeField] private AnimationCurve _curve;
    
    [Header("Frames")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _titleImg;

    [Header("Texts")]
    [SerializeField] private TMP_Text _pressText;

    private AudioMixer _audioMixer;
    private bool _keyPressed = false;
    #endregion
    
    public override void OnEnable()
    {
        OpenUI();
        Show();
        _keyPressed = false;
        Managers.Sound.PlayBGM(AudioType.Title, AudioMixerGroupType.BGM, true);
    }

    private void Show()
    {
        //StartCoroutine(Fade(true, _canvasGroup));
        StartCoroutine(BounceRoutine(_titleImg,Vector3.one * 0.65f, Vector3.one * 0.58f, _curve));
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
        Managers.Sound.PlaySound(AudioType.UI_Click, AudioMixerGroupType.Effects, false, 0.35f, 0f);
    }

    public override void SetLanguage()
    {
        SetSentence(_pressText, 2012);
    }
}
