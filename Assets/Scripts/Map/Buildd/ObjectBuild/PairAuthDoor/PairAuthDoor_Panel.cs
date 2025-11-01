using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class PairAuthDoor_Panel : MonoBehaviour
{
    private AirSM airSM;
    private HookSM hookSM;

    TypingEffect typingEffect;
    #region  Air
    [SerializeField] TextMeshPro _air_Text;
    #endregion
    #region  Hook
    [SerializeField] TextMeshPro _hook_Text;
    #endregion


    [SerializeField] GameObject _correctImg;
    [SerializeField] GameObject _failImg;
    #region  uni


    WaitForSeconds _wait = new WaitForSeconds(0.05f);

    #endregion
    public void SetPanel(PlayerSM playerSm, bool onOff = true)
    {
        switch (playerSm.characterType)
        {
            case CharacterType.Air:
                airSM = playerSm as AirSM;
                StartCoroutine(Typing("Air", CharacterType.Air));
                // _airIcon.SetActive(onOff);
                break;
            case CharacterType.Hook:
                hookSM = playerSm as HookSM;
                StartCoroutine(Typing("Hook", CharacterType.Hook));
                // _hookIcon.SetActive(onOff);
                break;
        }
    }

    public void PanelReset()
    {
        airSM = null;
        hookSM = null;
        _air_Text.text = "";
        _hook_Text.text = "";

        // _correctImg.SetActive(false);
        // _failImg.SetActive(false);
    }
    public void Clean()
    {
        StopAllCoroutines();
        PanelReset();
        _failCo = null;
        _correctImg.SetActive(false);
        _failImg.SetActive(false);

    }
    public void Correct()
    {
        //Sound
        Managers.Sound.PlaySound(GlobalText.PUZZLE_HINT_CORRECT);
        //Sound
        _correctImg.SetActive(true);
    }
    Coroutine _failCo;
    public void Fail()
    {
        //Sound
        Managers.Sound.PlaySound(GlobalText.PUZZLE_HINT_WRONG);
        //Sound

        if (_failCo != null) StopCoroutine(_failCo);
        _failCo = StartCoroutine(FailCo());
    }
    private IEnumerator FailCo()
    {
        _failImg.SetActive(true);
        yield return new WaitForSeconds(1f);
        _failImg.SetActive(false);
        _failCo = null;
    }

    public bool AuthCheck()
    {
        return airSM && hookSM;
    }


    private IEnumerator Typing(string text,CharacterType type)
    {
        StringBuilder sb = new();
        var tmp = type == CharacterType.Air ? _air_Text : _hook_Text;

        for (int i = 0; i < text.Length; i++)
        {
            sb.Append(text[i]);
            tmp.text = sb.ToString();
            yield return _wait;
        }
    }
}
