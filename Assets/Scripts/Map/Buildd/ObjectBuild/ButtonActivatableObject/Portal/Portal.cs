using Mirror;
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

    //private Portal_Net Portal_Net => GetComponent<Portal_Net>();

    private Portal_Net net;
    private Portal_Net Portal_Net
    {
        get
        {
            net ??= GetComponent<Portal_Net>();
            return net;
        }
    }

  
    #region Get,Set

    private void Awake()
    {
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
        try
        {
            if (typeof(T) == typeof(ButtonActivatableObjectStruct))
            {
                ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
                ButtonActivatedObjectStruct = objData;
                targetPosition = objData.talPot;

                
            }
        }
        catch
        {
            Debug.Log($"ERROR,{typeof(T)}");
        }

        if (Application.isPlaying)
        {
            Portal_Net.SetTargetPortal(targetPosition);
            await util.Delay(() => { CheckActiveRequirAmount(); });
        }


    }

    public override void CheckActiveRequirAmount()
    {
        base.CheckActiveRequirAmount();

        foreach(Transform tr in MapEditor.Instance.buttonActivatableObjectTransform){
            if(tr.TryGetComponent(out Portal component)){
                if(targetPosition == (Vector2)component.transform.position){
                    //targetPortal = component;
                    Portal_Net.Server_SetTargetPortal(component.gameObject);
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
    public void Net_ChangeOnPrograss(bool val)
    {
        Portal_Net.onPrograss = val;
        _animator.SetBool("IsActive", !val);
        _TpEffect.SetActive(!val);
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
        Portal_Net.Cmd_CallSetOnActive(true);
    }

    protected override void Deactivated()
    {
        Portal_Net.Cmd_CallSetOnActive(false);
    }


    #endregion

    #region Interactable

    //todo 0913 RayCast
    private void ActiveOnRay()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, .5f, layer);
        if (hit.collider != null)
        {

            Portal_Net.Cmd_UsePortal(hit.collider.gameObject);

        }

    }
    private void OnDrawGizmos(){
    Gizmos.color = Color.red;
    Gizmos.DrawRay(transform.position,transform.up*.5f);
   }

    #endregion

}

