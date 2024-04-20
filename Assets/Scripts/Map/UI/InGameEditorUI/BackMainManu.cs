using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class BackMainManu : UI_Base
{
    [SerializeField] Button popUpUiOpen;
    [SerializeField] GameObject container;
    [SerializeField] Button yesBtn;
    [SerializeField] Button noBtn;

    private void Awake()
    {
        popUpUiOpen.onClick.AddListener(OpenUi);
        yesBtn.onClick.AddListener(YesBtn);
        noBtn.onClick.AddListener(NoBtn);
    }


    public override void OnEnable()
    {
    }

    private void OpenUi()
    {
        container.SetActive(true);
    }


    private void YesBtn()
    {
        Managers.Game.CurrentState = GameState.Title;
        SceneManager.LoadScene("StartScene");
        Managers.UI.ShowUI<UI_Loading>();

    }
    private void NoBtn()
    {
        container.SetActive(false);
    }
}
