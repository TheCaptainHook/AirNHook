using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class ButtonActivated : ButtonEntity
{
    [CustomHeader("ButtonActivated")]
    public LayerMask mask;
    public bool isPressed = false;

    public Transform buttonTransform;
    
    Color orgColor;

    bool isRunningCoroutine;

    [Header("Components")]
    private SpriteRenderer spriteRenderer;
    private Animator _animator;
    
    #region StringCache
    private static readonly int IsActivated = Animator.StringToHash("IsActivated");
    #endregion


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        orgColor = spriteRenderer.material.color;
    }

    //-------------------------------------------------------------------------------------------------------Refeac 250213
    ButtonActivated_Net b_Net;
    ButtonActivated_Net B_Net
    {
        get 
        {
            if(b_Net == null) b_Net = GetComponent<ButtonActivated_Net>();
            return b_Net;
        }
    }

    private void Press()
    {
        if (B_Net.rate >= 1) return;
       B_Net.Cmd_SetRate(Time.fixedDeltaTime);
    }
    private void Release()
    {
        if (B_Net.rate <= 0) return;
        B_Net.Cmd_SetRate(-Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        // if (!NetworkServer.active || !NetworkClient.isConnected) return;
        if(!NetworkServer.active) return;

        RaycastHit2D hit = Physics2D.Raycast(buttonTransform.position,transform.up, 0.8f, mask);
        Debug.DrawRay(buttonTransform.position,transform.up*0.8f,Color.red);
        if (hit.collider is not null)
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

    //private void FixedUpdate()
    //{
    //    if (!NetworkServer.active || !NetworkClient.isConnected) return;

    //    RaycastHit2D hit = Physics2D.Raycast(buttonTransform.position, Vector2.up, 1, mask);
    //    if(hit.collider is not null && !turnOff && !onPrograss)
    //    {
    //        isPressed = true;
    //    }
    //    else if (isPressed && onActive && hit.collider is null && !onPrograss)
    //    {
    //        Deactivated();
    //    }
    //}

    public override void EditorMode_Destroy()
    {   
            base.EditorMode_Destroy();
    }


    //protected override void Activation()
    //{
    //     if (onPrograss) return;

    //    StartCoroutine(Co_Activation());
    //}

    //protected override void Deactivated()
    //{

    //    if (onPrograss) return;
    //    if (!onActive) return;
    //    StartCoroutine(Co_Deactivated());
    //}


    // override IEnumerator Co_Activation()
    // {
    //     onPrograss = true;

    //     _animator.SetBool(IsActivated, true);

    //     PrograssButtonActivatedObject(true);

    //     yield return new WaitForSeconds(0.5f);
    //     onPrograss = false;
    // }
    public void Net_Actvie()
    {
        Activation();
    }
    public void Net_Deactivated()
    {
        Deactivated();
    }
    protected override void Activation()
    {
        PrograssButtonActivatedObject(true);
    }

    protected override void Deactivated()
    {
        PrograssButtonActivatedObject(false);
    }
    //protected override IEnumerator Co_Activation()
    //{
    //    onPrograss = true;
    //    _animator.SetBool(IsActivated, true);

    //    PrograssButtonActivatedObject(true);

    //    yield return new WaitForSeconds(0.5f);
    //    onPrograss = false;
    //}
    //protected override IEnumerator Co_Deactivated()
    //{
    //     onPrograss = true;

    //    isPressed = false;
    //    onActive = false;
        
    //    _animator.SetBool(IsActivated, false);

    //    PrograssButtonActivatedObject(false);

    //    yield return new WaitForSeconds(0.5f);
    //    onPrograss = false;
    //}
    // IEnumerator Co_Deactivated()
    // {
    //     onPrograss = true;

    //     isPressed = false;
    //     onActive = false;

    //     _animator.SetBool(IsActivated, false);

    //     PrograssButtonActivatedObject(false);

    //     yield return new WaitForSeconds(0.5f);
    //     onPrograss = false;
    // }

    public override void TurnOff()
    {
        base.TurnOff();
        turnOff = true;

        if (isPressed && onActive)
        {
            Deactivated();
        }

    }

    public override void TurnOn()
    {
        base.TurnOn();
        turnOff = false;
    }

}
