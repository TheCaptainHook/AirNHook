using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : ActivatableObjectEntity
{
    [CustomHeader("Portal")]
    // public ObjectTypeEnum objectType = ObjectTypeEnum.Interaction;
    // public Vector2 btnOffset;
    public Portal targetPortal;
    [SerializeField] LayerMask layer;
    [Space(20)]
    [ReadOnly]
    public Vector2 targetPosition;

    bool onPrograss;
    [ReadOnly]
    public bool onActivable;
    //private Coroutine portalCoroutine;
    private Util util;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] GameObject _TpEffect;
    
    #region StringCache
    private static readonly int IsActive = Animator.StringToHash("IsActive");
    #endregion

    #region Get,Set

    private void Awake(){
        util = new Util();
    }
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
    // public async override void Editor_Setting(Transform transform)
    // {
    //      Util util = new Util();
    //         await util.Delay(()=>{
    //             try{
    //                 foreach(Transform tr in transform){
    //                     if(tr.TryGetComponent(out Portal component)){
    //                         if(targetPosition == (Vector2)component.transform.position){
    //                             targetPortal = component;
    //                             return;
    //                         }
    //                     }       
    //                 }
    //             }catch{
    //                 Debug.Log("Can't find Transform");
    //                 return;
    //             }


    //     });
    // }
    public async override void Editor_Setting(MapEditor mapEditor)
    {
        Util util = new Util();
            await util.Delay(()=>{
                try{
                    foreach(Transform tr in mapEditor.buttonActivatableObjectTransform){
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

    // public void FindTargetPortal()
    // {
    //     if (targetPortal != null) return;

    //     foreach(Transform tr in MapEditor.Instance.buttonActivatableObjectTransform)
    //     {
    //         Portal portal = tr.GetComponent<Portal>();

    //         if(portal != null)
    //         {
    //             if (portal.ObjectData.position == targetPosition)
    //             {
    //                 targetPortal = portal;
    //                 return;
    //             }
    //         }
    //     }
    // }


    #endregion


    #region Portal Logic
    private void FixedUpdate(){
        if(onActivable){
            ActiveOnRay();
        }
    }

    protected override void Activation()
    {
        _animator.SetBool(IsActive, true);
        _TpEffect.SetActive(true);
        onActivable = true;
    }

    protected override void Deactivated()
    {
        _animator.SetBool(IsActive, false);
        _TpEffect.SetActive(false);
        onActivable= false;
    }


    #endregion

    #region Interactable
    // public void Interaction(Transform accessor = null)
    // {
    //     if (!NetworkServer.active || !NetworkClient.isConnected)
    //         return;

    //     if (!onPrograss && onActivable)
    //     {
    //         if(portalCoroutine != null)
    //         {
    //             StopCoroutine(CoPortal());
    //         }

    //         portalCoroutine = StartCoroutine(CoPortal());
    //     }
    // }

    //todo 0913 RayCast
    private void ActiveOnRay(){
        RaycastHit2D hit = Physics2D.Raycast(transform.position,transform.up,.5f,layer);
        if(hit.collider != null){
            
                if(!onPrograss){
                    StartCoroutine(CoPortal(hit.collider.gameObject));
                } 
        }
       
    }
   private void OnDrawGizmos(){
    Gizmos.color = Color.red;
    Gizmos.DrawRay(transform.position,transform.up*.5f);
   }

    IEnumerator CoPortal(GameObject targetObj)
    {
        onPrograss = true;
        //GameObject player = Managers.Game.Player;
        GameObject player = targetObj;
        Rigidbody2D rg = player.GetComponent<Rigidbody2D>();

        if (rg == null) yield break;

        //TODO Take Care logic : Cant Move Player 
        rg.simulated = false;

        // FindTargetPortal();
        targetPortal.onPrograss = true;

        // FadeOut
        //yield return MapEditor.Instance.fadeInOutPanel.FadeIn();
        Camera.main.GetComponent<PlayerCameraView>()._CameraGlobalVolumeController.PortalSpace_TimeTransitionEffect();
        player.transform.position = targetPosition + Vector2.up;

        //Finish
        //yield return MapEditor.Instance.fadeInOutPanel.FadeOut();
       
        yield return new WaitForSeconds(1f);
        //TODO 1206, AcData Update
        Managers.AcManager.CallUsePortal();
        rg.simulated = true;
        yield return new WaitForSeconds(1f);
        //portalCoroutine = null;
        onPrograss = false;
        targetPortal.onPrograss = false;

        //TODO Take Care logic : Can Move Player 
        
    }

    // public bool CanInteract()
    // {
    //     return true;
    // }

    // public void Interacting(bool value)
    // {
    //     return;
    // }

    // public ObjectTypeEnum GetObjectType()
    // {
    //     return objectType;
    // }

    // public void ShowEButton()
    // {
    //     var eButtonUI = Managers.UI.ShowUI<UI_ShowEButton>();
    //     eButtonUI.transform.position = transform.position + (Vector3)btnOffset;
    // }

    // public void HideEButton()
    // {
    //     Managers.UI.HideUI<UI_ShowEButton>();
    // }

    #endregion

}

