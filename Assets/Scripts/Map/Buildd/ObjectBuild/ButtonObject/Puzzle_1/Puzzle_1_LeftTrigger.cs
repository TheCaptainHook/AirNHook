using Mirror;
using UnityEngine;

public class Puzzle_1_LeftTrigger : MonoBehaviour
{
    [SerializeField] Puzzle_1 puzzle_1;
    [SerializeField] Puzzle_1_Button button;
    [ReadOnly]
    public AirSM air;
    [ReadOnly]
    public Transform airWeaponPivot;

    UI_Base eBtn;







    private void Update()
    {
        // if(Input.GetMouseButton(1) && air && !button.onRecover)
        // {
        //     if (GetReadyToCharge(GetAirDir()) &&!air.airGun._inhaling && !button.onProgress)
        //     {
        //         //Charging;
        //         //puzzle_1.Charging();
        //         if(NetworkClient.localPlayer)
        //         {
        //             if(eBtn == null){
        //                 eBtn = Managers.UI.ShowUI<UI_ShowEButton>();
        //             }
        //         }
        //         puzzle_1.Net_Charging();
        //     }
        // }else{
        //     if(eBtn != null || eBtn.gameObject.activeSelf)
        //     {
        //         eBtn = null;
        //         Managers.UI.HideUI<UI_ShowEButton>();

        //     }
        // }
        //if(GetReadyToCharge(GetAirDir())&& !air.airGun._inhaling && !button.onProgress)
        //{
        //    //SHow UI
        //        if(NetworkClient.localPlayer)
        //        {
        //            if(eBtn == null){
        //                eBtn = Managers.UI.ShowUI<UI_ShowEButton>();
        //                eBtn.transform.position = transform.position + new Vector3(0,1,0);
        //            }
        //        }
        //    //SHow UI
        //    if(Input.GetMouseButton(1) && !button.onRecover)
        //    {
        //        //Charging
        //         puzzle_1.Net_Charging();
        //        //Charging

        //    }

        //}else{
        //    if(NetworkClient.localPlayer && eBtn != null)
        //    {
        //        eBtn = null;
        //        Managers.UI.HideUI<UI_ShowEButton>();

        //    }
        //}
    }

    //Refectoring 0324

    public Vector3 offset;
    [SerializeField] Puzzle_1_Net net;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.TryGetComponent(out AirSM air))
            {
                if(NetworkClient.localPlayer)
                {
                    ShowE(true);
                }
                //net.Cmd_ShowE(collision.gameObject, true, true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.TryGetComponent(out AirSM air))
            {
                if (NetworkClient.localPlayer)
                {
                    ShowE(false);
                }
                //net.Cmd_ShowE(collision.gameObject, true, false);
            }
        }
    }
    //Refectoring 0324


    #region UI
    public void ShowE(bool onOff)
    {
        if (onOff)
        {
            var ui = Managers.UI.ShowUI<UI_ShowEButton>();
            ui.transform.position = transform.position + offset;
           
        }
        else
        {
            Managers.UI.HideUI<UI_ShowEButton>();
        }
    }
    #endregion


    #region before
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision != null)
    //    {
    //        if (collision.TryGetComponent(out AirSM component))
    //        {
    //            air = component;
    //            //dir
    //        }
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision != null)
    //    {
    //        if (collision.TryGetComponent(out AirSM component))
    //        {
    //            air = null;
    //        }
    //    }
    //}

    //private Vector3 GetAirDir()
    //{
    //    if (air == null) return Vector3.zero;

    //    if(airWeaponPivot == null)
    //    {
    //        foreach (Transform tr in air.transform)
    //        {
    //            if (tr.name == "WeaponPivot")
    //            {
    //                airWeaponPivot = tr;
    //            }
    //        }
    //    }

    //    return airWeaponPivot.rotation.eulerAngles;

    //}

    //private bool GetReadyToCharge(Vector3 rot)
    //{
    //    float z = rot.z - 360;
    //    if(rot.y ==0 && (z >=-10 && z <= 0))
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
    #endregion
}
