using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Tutorial : UI_Base
{
    private PlayerInputAction.PlayerActions playerActions => Managers.Game.playerInput.playerActions;
    private PlayerInputAction.UIActions uiActions => Managers.Game.playerInput.uiActions;

    [ReadOnly]
    public bool onActive;
    [ReadOnly]
    public TUTORIAL_CODE code;



    [SerializeField] Button closeBtn;
    //Test
    [SerializeField] TextMeshProUGUI text;


    void Awake()
    {
        closeBtn.onClick.AddListener(CloseUI);
    }
    public override void OnEnable()
    {
        playerActions.Disable();
        uiActions.Disable();

        FreezePlayerState(true);

    }

    private void FreezePlayerState(bool onOff)
    {
        var player = Managers.Game.Player.TryGetComponent(out PlayerSM sm) ? sm : null;
        if (player == null) return;

        sm.FreezePlayerState(onOff);
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && onActive)
        {
            CloseUI();
        }
    }
    protected override void OpenUI()
    {
        // gameObject.SetActive(true);
        onActive = true;
        Debug.Log($"[3] Open UI_Tutorial");
    }


    protected override void CloseUI()
    {
        if (openCoroutine != null)
        {
            StopCoroutine(openCoroutine);
            openCoroutine = null;
        }

        StartCoroutine(CloseCoroutine());
        
    }

    IEnumerator CloseCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        FreezePlayerState(false);
        
        playerActions.Enable();
        uiActions.Enable();
        onActive = false;
        gameObject.SetActive(false);
    }

    private Coroutine openCoroutine;
    public void Setting(TUTORIAL_CODE code)
    {
        if (openCoroutine != null) StopCoroutine(openCoroutine);

        openCoroutine = StartCoroutine(Set_TutorialCode(code));
    }

    IEnumerator Set_TutorialCode(TUTORIAL_CODE code)
    {
        this.code = code;
        //test
        text.text = code.ToString();
        Debug.Log($"[2] Open UI_Tutorial, {code}");
        //test
        yield return new WaitForSeconds(1);
        openCoroutine = null;
        OpenUI();

    }
}
