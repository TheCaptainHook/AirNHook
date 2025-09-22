using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

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
    #region  uni


    WaitForSeconds _wait = new WaitForSeconds(0.1f);

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
