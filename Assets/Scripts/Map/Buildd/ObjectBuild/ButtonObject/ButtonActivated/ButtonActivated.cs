using Mirror;
using UnityEngine;

public class ButtonActivated : ButtonEntity
{
    [CustomHeader("ButtonActivated")]
    public LayerMask mask;
    public bool isPressed = false;

    public Transform buttonTransform;


    //-------------------------------------------------------------------------------------------------------Refeac 250213
    #region Get,Set

    public override void SetData<T>(T data)
    {
        base.SetData(data);

        //    B_Net.onSync = true;
        //    B_Net.Server_SetPosition();
        if (Application.isPlaying) Net.Server_SetInit();
    }


    #endregion
    #region  Clean
    public override void Clean()
    {
        Net.Clean();
    }
    #endregion

    ButtonActivated_Net b_Net;
    ButtonActivated_Net B_Net
    {
        get 
        {
            if(b_Net == null) b_Net = GetComponent<ButtonActivated_Net>();
            return b_Net;
        }
    }

    private void Press()//server
    {
        if (B_Net.rate >= 1) return;
        if(NetworkClient.isConnected && NetworkClient.ready)
        B_Net.Server_SetRate(Time.fixedDeltaTime);
    }
    private void Release() //server
    {
        if (!MapEditor.Instance._onMapTransition_Complete) return;

        if (B_Net.rate <= 0) return;
        if (NetworkClient.isConnected && NetworkClient.ready)
            B_Net.Server_SetRate(-Time.fixedDeltaTime);

            // Debug.Log("TTTTTTTTTTTT");
    }

    private void FixedUpdate()
    {
        // if (!NetworkServer.active || !NetworkClient.isConnected) return;
        if(!NetworkServer.active) return;

        RaycastHit2D hit = Physics2D.Raycast(buttonTransform.position,transform.up, 0.8f, mask);
        // Debug.DrawRay(buttonTransform.position,transform.up*0.8f,Color.red);
        if (hit.collider != null)
        {
            //isPressed = true;
            Press();
        }
        else
        {
            //Deactivated();
            Release();
        }
    }

    //-------------------------------------------------------------------------------------------------------Refeac

   
    public override void EditorMode_Destroy()
    {   
            base.EditorMode_Destroy();
    }


    // public void Net_Actvie()
    // {
    //     Activation();
    // }
    // public void Net_Deactivated()
    // {
    //     Deactivated();
    // }
    public override void Activation()
    {
        PrograssButtonActivatedObject(true);
        Managers.Sound.PlaySound3D(GlobalText.BUTTON_PRESS_SOUND_2, transform.position, 0.35f);
    }

    public override void Deactivated()
    {
        PrograssButtonActivatedObject(false);
        Managers.Sound.PlaySound3D(GlobalText.BUTTON_RELEASE_SOUND_1, transform.position, 0.35f);
    }

    // public override void TurnOff()
    // {
    //     base.TurnOff();
    //     turnOff = true;

    //     if (isPressed && onActive)
    //     {
    //         Deactivated();
    //     }

    // }

    // public override void TurnOn()
    // {
    //     base.TurnOn();
    //     turnOff = false;
    // }

}
