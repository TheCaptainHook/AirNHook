using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mirror;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : ActivatableObjectEntity,IInteractable
{
    [CustomHeader("Portal")]
    public ObjectTypeEnum objectType = ObjectTypeEnum.Interaction;
    public Vector2 btnOffset;

    public Portal targetPortal;
    
    [ReadOnly]
    public Vector2 targetPosition;

    bool onPrograss;
    bool onActivable;
    private Coroutine portalCoroutine;
    
    [Header("Animation")]
    [SerializeField] private Animator _animator;
    
    #region StringCache
    private static readonly int IsActive = Animator.StringToHash("IsActive");
    #endregion

    #region Get,Set

    public override T GetData<T>()
    {
         if(typeof(T) == typeof(ButtonActivatableObjectStruct)){
            return (T)(object)new ButtonActivatableObjectStruct(id,activeRequirAmount,transform.position,transform.rotation,transform.localScale,targetPortal.transform.position);
        }

        return default(T);
    }

    public override async void SetData<T>(T data)
    {
         try{
            if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
         ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
         ButtonActivatedObjectStruct = objData;
         targetPosition = objData.talPot;

        }
        }catch{
                Debug.Log($"ERROR,{typeof(T)}");
        }
        
        if(Application.isPlaying){
              Util util  = new Util();
              await util.Delay(()=>{CheckActiveRequirAmount();});
        }
       
        
    }

    public override void CheckActiveRequirAmount()
    {
        base.CheckActiveRequirAmount();

        foreach(Transform tr in MapEditor.Instance.buttonActivatableObjectTransform){
            if(tr.TryGetComponent(out Portal component)){
                if(targetPosition == (Vector2)component.transform.position){
                    targetPortal = component;
                    return;
                }
            }
        }
    }


    #region Editor
    public async void Editor_SetTarget(MapEditor editor){
        Util util = new Util();
            await util.Delay(()=>{
                try{
                    foreach(Transform tr in editor.buttonActivatableObjectTransform){
                        if(tr.TryGetComponent(out Portal component)){
                            if(targetPosition == (Vector2)component.transform.position){
                                targetPortal = component;
                                return;
                            }
                        }       
                    }
                }catch{
                    Debug.Log("Can't find Transform");
                    return;
                }

                
        });
       
    }

    #endregion

    public override void ApplyActive(int num)
    {
        CurActiveBtn = num;
    }

    public void FindTargetPortal()
    {
        if (targetPortal != null) return;

        foreach(Transform tr in MapEditor.Instance.buttonActivatableObjectTransform)
        {
            Portal portal = tr.GetComponent<Portal>();

            if(portal != null)
            {
                if (portal.ObjectData.position == targetPosition)
                {
                    targetPortal = portal;
                    return;
                }
            }
        }
    }


    #endregion


    #region Portal Logic
    //TODO : 포탈 껏다 켜짐 옵션으로 애니메이터 조절
    //_animator.SetBool(IsActive, true);
    //_animator.SetBool(IsActive, false);

    protected override void Activation()
    {
        _animator.SetBool(IsActive, true);
        onActivable = true;
    }

    protected override void Deactivated()
    {
        _animator.SetBool(IsActive, false);
        onActivable= false;
    }


    #endregion

    #region Interactable
    public void Interaction(Transform accessor = null)
    {
        if (!NetworkServer.active || !NetworkClient.isConnected)
            return;

        if (!onPrograss && onActivable)
        {
            if(portalCoroutine != null)
            {
                StopCoroutine(CoPortal());
            }

            portalCoroutine = StartCoroutine(CoPortal());
        }
    }

    IEnumerator CoPortal()
    {
        onPrograss = true;
        GameObject player = Managers.Game.Player;
        Rigidbody2D rg = player.GetComponent<Rigidbody2D>();
        //TODO Take Care logic : Cant Move Player 
        rg.simulated = false;

        FindTargetPortal();
        targetPortal.onPrograss = true;

        // FadeOut
        yield return MapEditor.Instance.fadeInOutPanel.FadeIn();
        player.transform.position = targetPosition;


        //Finish
        yield return MapEditor.Instance.fadeInOutPanel.FadeOut();
        portalCoroutine = null;
        onPrograss = false;
        targetPortal.onPrograss = false;

        //TODO Take Care logic : Can Move Player 
        rg.simulated = true;
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interacting(bool value)
    {
        return;
    }

    public ObjectTypeEnum GetObjectType()
    {
        return objectType;
    }

    public void ShowEButton()
    {
        var eButtonUI = Managers.UI.ShowUI<UI_ShowEButton>();
        eButtonUI.transform.position = transform.position + (Vector3)btnOffset;
    }

    public void HideEButton()
    {
        Managers.UI.HideUI<UI_ShowEButton>();
    }

    #endregion

}

