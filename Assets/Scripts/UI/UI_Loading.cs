using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Loading : UI_Base
{
    [Header("■ Animations")]
    [SerializeField] private AnimationCurve _curve;
    
    [Header("■ CanvasGroup")]
    [SerializeField] private CanvasGroup _canvasGroup; //로딩화면 캔버스
    
    [Header("■ Image")]
    [SerializeField] private Image _loadingImg;
    [SerializeField] private GameObject _loadingImgFrame;
    [SerializeField] private GameObject _titleImg;
    
    [Header("■ Text")]
    [SerializeField] private TMP_Text _loadingText;

    [SerializeField] private Slider _slider;
    private Sprite[] _loadingSprites; // 랜덤한 로딩 스프라이트 배열
    private string _loadSceneName; // 로드할 씬의 이름

    public bool _isLoading;

    public override void OnEnable()
    {
        OpenUI();
        StartCoroutine(BounceRoutine(_titleImg, Vector3.one * 0.25f, Vector3.one * 0.22f, _curve));
        StartCoroutine(BounceRoutine(_loadingImgFrame, Vector3.one * 0.95f, Vector3.one * 0.90f, _curve));
        _loadSceneName = Managers.UI.sceneName;
        LoadSpritesFromResources();
        SetRandomBackground();

        _isLoading = true;
        LoadScene();

    }

    protected override void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 완료 시 이벤트 처리
        StartCoroutine(Co_LoadSceneProcess());
    }

    private IEnumerator Co_LoadSceneProcess()
    {
        _slider.value = 0f;
        yield return StartCoroutine(Fade(true, _canvasGroup));

        AsyncOperation op = SceneManager.LoadSceneAsync(_loadSceneName);
        Managers.Network.LoadingSceneAsync = op;
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                // 씬 로드 진행률에 따라 프로그레스 바 갱신
                _slider.value = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime * 0.5f;
                // 로드가 거의 완료된 경우 프로그레스 바를 가득 채움
                _slider.value = Mathf.Lerp(0.9f, 1f, timer);
                if (_slider.value >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
        
        _isLoading = false;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StartCoroutine(Fade(false, _canvasGroup)); // 페이드 아웃 애니메이션
    }

    private void SetRandomBackground()
    {
        if (_loadingSprites.Length > 0)
        {
            // 배열에서 랜덤하게 인덱스 선택
            int randomIndex = Random.Range(0, _loadingSprites.Length);

            // 선택된 인덱스에 해당하는 스프라이트를 스프라이트 렌더러에 할당
            _loadingImg.sprite = _loadingSprites[randomIndex];
        }
        else
        {
            Debug.Log("배경 스프라이트가 배열에 없습니다.");
        }
    }
    
    private void LoadSpritesFromResources()
    {
        // Resources 폴더에서 스프라이트들을 로드하여 배열에 추가
        object[] loadedSprites = Resources.LoadAll("Arts/Sprites/LoadingSprites", typeof(Sprite));

        // 로드된 스프라이트를 배열에 추가
        _loadingSprites = new Sprite[loadedSprites.Length];
        for (int i = 0; i < loadedSprites.Length; i++)
        {
            _loadingSprites[i] = (Sprite)loadedSprites[i];
        }
        
        Debug.Log("BGSpriteLoad");
    }
    public override void SetLanguage()
    {
        SetSentence(_loadingText, 2011);
    }
}
