
using UnityEngine;
using Mirror;

public class ButtonActivated_Net : ButtonEntity_Net
{
    private float max = -0.2f;
    private float min = -0.4f;
    [SerializeField] Transform plate;
    [SerializeField] GameObject _light;

    [SerializeField] Sprite greenSprite;
    [SerializeField] Sprite redSprite;

    // private ButtonActivated button;
    // private ButtonActivated Button
    // {
    //     get
    //     {
    //         if(button == null) button = GetComponent<ButtonActivated>();
    //         return button;
    //     }
    // }

    // public bool onSync;
    public bool onActive;


    #region  Init
    // [Server]
    // public void Server_SetPosition()
    // {
    //     Rpc_SetPosition(Button.ButtonObjectData);
    // }
    // [ClientRpc]
    // private void Rpc_SetPosition(ButtonObjectStruct data)
    // {
    //     if(onSync) return;
    //     transform.position = data.position;
    //     transform.rotation = data.quaternion;
    //     transform.localScale = data.scale;
    //     onSync = true;
    // }
    // [Command(requiresAuthority = false)]
    // public void Cmd_SetPosition()
    // {
    //     Server_SetPosition();
    // }
    protected override void Set_Value(ButtonObjectStruct data)
    {
        base.Set_Value(data);
        transform.localScale = data.scale;
    }
    // public override void OnStartClient()
    // {
    //     base.OnStartClient();
    //     if(!onSync)Cmd_SetPosition();    
    // }

    #endregion
    #region Clean
    public override void Clean()
    {
        _onSync = false;
        var plat = plate.localPosition;
        plat.y = max;
        plate.localPosition = plat;
        onActive = false;
        
        rate = 0;
        DeactiveEffect();
        
    }
    #endregion


    // [SyncVar(hook =nameof(OnChageRate))] 
    public float rate;
    private float pressSpeed = 3;
    [Server]
    public void Server_SetRate(float rate)
    {
        this.rate += rate * pressSpeed;
        this.rate = Mathf.Clamp01(this.rate);

        Rpc_SetRate(this.rate);

        if (this.rate >= 1 && !onActive)
        {
            onActive = true;
            Main.Activation();
        }
        else if (this.rate < 1 && onActive)
        {
            onActive = false;
            Main.Deactivated();
        }

    }
    [ClientRpc]
    private void Rpc_SetRate(float rate)
    {
        // this.rate = rate;
        float val = Mathf.Lerp(max,min, rate);
        Vector3 vec = plate.localPosition;
        vec.y = val;

        plate.localPosition = vec;

        if (rate >= 1)
        {
            ActiveEffect();
        }
        else
        {
            DeactiveEffect();
        }
        
    }
    // [Command(requiresAuthority = false)]
    // public void Cmd_SetRate(float rate)
    // {
    //     Server_SetRate(rate);
    // }


    private void OnChageRate(float old,float newVal)
    {
        if (!MapEditor.Instance._onMapTransition_Complete) return;

       float val = Mathf.Lerp(max,min, newVal);
        Vector3 vec = plate.localPosition;
        vec.y = val;

        plate.localPosition = vec;


        if(newVal >= 1)
        {
            ActiveEffect();
        }
        else
        {
            DeactiveEffect();
        }
    }




    private void ActiveEffect()
    {
        _light.GetComponent<SpriteRenderer>().sprite = greenSprite;

    }
    private void DeactiveEffect()
    {
        _light.GetComponent<SpriteRenderer>().sprite = redSprite;
    }

}
