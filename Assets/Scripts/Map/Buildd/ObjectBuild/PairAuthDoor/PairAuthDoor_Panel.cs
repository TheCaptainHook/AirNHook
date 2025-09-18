using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PairAuthDoor_Panel : MonoBehaviour
{
    private AirSM airSM;
    private HookSM hookSM;

    //test
    [SerializeField] GameObject _airIcon;
    [SerializeField] GameObject _hookIcon;
    //test

    public void SetPanel(PlayerSM playerSm,bool onOff = true)
    {
        switch (playerSm.characterType)
        {
            case CharacterType.Air:
                airSM = playerSm as AirSM;
                _airIcon.SetActive(onOff);
                break;
            case CharacterType.Hook:
                hookSM = playerSm as HookSM;
                _hookIcon.SetActive(onOff);
                break;
        }
    }

    public void PanelReset()
    {
        airSM = null;
        hookSM = null;
        _airIcon.SetActive(false);
        _hookIcon.SetActive(false);
    }
    public bool AuthCheck()
    {
        return airSM && hookSM;
    }
}
