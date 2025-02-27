using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ButtonActivated_Net : NetworkBehaviour
{
    private float max = -0.1f;
    private float min = -0.3f;
    [SerializeField] Transform plate;
    [SerializeField] GameObject _light;

    [SerializeField] Sprite greenSprite;
    [SerializeField] Sprite redSprite;

    private ButtonActivated button;
    private ButtonActivated Button
    {
        get
        {
            if(button == null) button = GetComponent<ButtonActivated>();
            return button;
        }
    }

    [SyncVar] public bool onActive;
    [SyncVar(hook =nameof(OnChageRate))] 
    public float rate;


    private float pressSpeed = 3;
    [Server]
    public void Server_SetRate(float rate)
    {
        this.rate += rate*pressSpeed;
        this.rate = Mathf.Clamp01(this.rate);

        if(this.rate >= 1 && !onActive)
        {
            onActive = true;
            Button.Net_Actvie();
        }
        else if(this.rate  < 1 && onActive)
        {
            onActive = false;
            Button.Net_Deactivated();
        }

    }
    [Command(requiresAuthority = false)]
    public void Cmd_SetRate(float rate)
    {
        Server_SetRate(rate);
    }


    private void OnChageRate(float old,float newVal)
    {
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
