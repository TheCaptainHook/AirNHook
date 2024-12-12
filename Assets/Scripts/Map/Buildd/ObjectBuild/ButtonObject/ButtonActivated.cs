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

    private void Update()
    {
        //if (!NetworkServer.active || !NetworkClient.isConnected) return; //24.05.20        

        if (isPressed && !onActive)
        {
            onActive = true;
            Activation();
        }
    }

    
    private void FixedUpdate()
    {
        if (!NetworkServer.active || !NetworkClient.isConnected) return;
        
        RaycastHit2D hit = Physics2D.Raycast(buttonTransform.position, Vector2.up, 1, mask);
        if(hit.collider is not null && !turnOff && !onPrograss)
        {
            isPressed = true;
        }
        else if (isPressed && onActive && hit.collider is null && !onPrograss)
        {
            Deactivated();
        }
    }

    public override void EditorMode_Destroy()
    {   
            base.EditorMode_Destroy();
    }

    protected override void Activation()
    {
         if (onPrograss) return;

        StartCoroutine(Co_Activation());
    }

    protected override void Deactivated()
    {
        
        if (onPrograss) return;
        if (!onActive) return;
        StartCoroutine(Co_Deactivated());
    }


    // override IEnumerator Co_Activation()
    // {
    //     onPrograss = true;

    //     _animator.SetBool(IsActivated, true);

    //     PrograssButtonActivatedObject(true);

    //     yield return new WaitForSeconds(0.5f);
    //     onPrograss = false;
    // }
    protected override IEnumerator Co_Activation()
    {
        onPrograss = true;
        _animator.SetBool(IsActivated, true);

        PrograssButtonActivatedObject(true);

        yield return new WaitForSeconds(0.5f);
        onPrograss = false;
    }
    protected override IEnumerator Co_Deactivated()
    {
         onPrograss = true;

        isPressed = false;
        onActive = false;
        
        _animator.SetBool(IsActivated, false);

        PrograssButtonActivatedObject(false);

        yield return new WaitForSeconds(0.5f);
        onPrograss = false;
    }
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
