using Mirror;
using UnityEngine;

public class ToggleButton_Net : ButtonEntity_Net
{
    [SerializeField] private GameObject energyIcon;
    protected override void Hook_ChargeRequired(bool old, bool newVal)
    {
        if (newVal)
        {
            energyIcon.SetActive(true);
        }
        else
        {
            energyIcon.SetActive(false);
        }
    }

    #region Has Power
    // [Server]
    // private void Server_SetHasPower(bool hasPower)
    // {
    //     if (hasPower) this.hasPower++;
    //     else
    //     {
    //         this.hasPower--;
    //         if (this.hasPower < 0) this.hasPower = 0;
    //     }
    //     //this.hasPower = hasPower;  //this.hasPower++;

    //     Server_SetChargeRequired(this.hasPower > 0 ? false : true);
    // }

    // [Command(requiresAuthority = false)]
    // public void Cmd_SetHasPower(bool hasPower)
    // {
    //     Server_SetHasPower(hasPower);
    // }
    #endregion




}
