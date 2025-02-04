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

    //bool onPrograss;
    [ReadOnly]
    public bool onActivable;
    //private Coroutine portalCoroutine;
    private Util util;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] GameObject _TpEffect;
    
    private Portal_Net Portal_Net => GetComponent<Portal_Net>();
  
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
            
         Portal_Net.SetTargetPortal(targetPosition);
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
                    //targetPortal = component;
                    Portal_Net.SetTargetPortal(component.gameObject);
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
    
    #if UNITY_EDITOR
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
    #endif
    #endregion


    #endregion

    #region Network
    public void Net_ChangeOnPrograss()
    {
        Portal_Net.Server_ChangePrograss();
    }
    #endregion
    #region Portal Logic
    private void FixedUpdate(){
        if(Portal_Net.onActive){
            ActiveOnRay();
        }
    }

    protected override void Activation()
    {
        //_animator.SetBool(IsActive, true);
        //_TpEffect.SetActive(true);
        //onActivable = true;
        Portal_Net.Cmd_CallSetOnActive(true);
    }

    protected override void Deactivated()
    {
        //_animator.SetBool(IsActive, false);
        //_TpEffect.SetActive(false);
        //onActivable= false;
        Portal_Net.Cmd_CallSetOnActive(false);
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
            
                if(!Portal_Net.onPrograss){
                //StartCoroutine(CoPortal(hit.collider.gameObject));
                    //hit.collider.GetComponent<PlayerSM>().UsePortCamerEffect();
                    Portal_Net.Cmd_UsePortal(hit.collider.gameObject);
                } 
        }
       
    }
   private void OnDrawGizmos(){
    Gizmos.color = Color.red;
    Gizmos.DrawRay(transform.position,transform.up*.5f);
   }

    //IEnumerator CoPortal(GameObject targetObj)
    //{
    //    onPrograss = true;
    //    //GameObject player = Managers.Game.Player;
    //    GameObject player = targetObj;
    //    Rigidbody2D rg = player.GetComponent<Rigidbody2D>();

    //    if (rg == null) yield break;

    //    //TODO Take Care logic : Cant Move Player 
    //    //rg.simulated = false;
    //    //Portal_Net.Cmd_HoldPlayer(player);

    //    //targetPortal.onPrograss = true;

    //    // FadeOut

    //    //Portal_Net.Cmd_CameraEffect();
    //    //Camera.main.GetComponent<PlayerCameraView>()._CameraGlobalVolumeController.PortalSpace_TimeTransitionEffect();

    //    //player.transform.position = Portal_Net.targetPortalPosition + Vector2.up;
    //    Portal_Net.Cmd_Portal(player);

    //    //Finish

    //    yield return new WaitForSeconds(1f);
    //    //TODO 1206, AcData Update
    //    //Managers.AcManager.CallUsePortal();
        
    //    Portal_Net.Cmd_CallUsePortal();

    //    //rg.simulated = true;
    //    Portal_Net.Cmd_RecoverPlayer(player);
    //    yield return new WaitForSeconds(1f);
    //    //portalCoroutine = null;
    //    onPrograss = false;
    //    //targetPortal.onPrograss = false;

    //    //TODO Take Care logic : Can Move Player 
        
    //}

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

