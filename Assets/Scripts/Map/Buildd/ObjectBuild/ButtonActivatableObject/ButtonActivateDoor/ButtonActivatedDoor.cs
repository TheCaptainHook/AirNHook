using Mirror;
using System.Collections;
using UnityEngine;

public class ButtonActivatedDoor : ActivatableObjectEntity
{
    public override void Activation()
    {
        Net.Server_ChangeOnActive(true);
        Managers.Sound.PlaySound3D(GlobalText.DOOR_SOUND_4, transform.position, 0.45f);
    }

    public override void Deactivated()
    {
        Net.Server_ChangeOnActive(false);
        Managers.Sound.PlaySound3D(GlobalText.DOOR_SOUND_3, transform.position, 0.45f);
    }
}






