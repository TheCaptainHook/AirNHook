using Mirror;
using UnityEngine;

public class MirrorObject : BuildObj,IInteractable
{
    [CustomHeader("Mirror Object")]
    [SerializeField] GameObject _Mirror;
    [ReadOnly]
    [SerializeField] GameObject _ConnectPlayer;
    public bool onActive;

    [Header("Interacte")]
    [SerializeField] float _BtnOffset;
    public ObjectTypeEnum _objectType = ObjectTypeEnum.Interaction;
    private UI_Base _E_Btn;
    private UI_Base _AorD_Btn;
    

    #region Network
    private MirrorObject_Net m_Net;
    private MirrorObject_Net M_Net
    {
        get
        {
            if(m_Net==null) m_Net = GetComponent<MirrorObject_Net>();
            return m_Net;
        }
    }

    private bool IsInnerPlayer => M_Net.InnerPlayer;
    private bool IsActive => M_Net.onActive;
    #endregion



    private void Update(){
        if(IsActive){
            if(Input.GetKey(KeyCode.A)){
                MirrorRotate(true);
            }
            if(Input.GetKey(KeyCode.D)){
                MirrorRotate(false);
            }
            
        }
    }

    

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if(onActive) return;

    //     if(_ConnectPlayer != null){
    //         return;
    //     }
    //     // ShowEButton();
    //    _ConnectPlayer = other.gameObject;
    // }

    // private void OnTriggerExit2D(Collider2D other){
    //     // HideEButton();
    //     if (onActive) onActive = false;
    //     _ConnectPlayer = null;
    // }
  

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(IsInnerPlayer) return;

        if(other)
        {
            if(other.TryGetComponent(out HookSM hook))
            {
                // M_Net.Cmd_SetInnerPlayer(hook.gameObject);
             
                _ConnectPlayer = hook.gameObject;
                ShowE();
            }
        }
     
    }

    private void OnTriggerExit2D(Collider2D other)
    {
       if(IsInnerPlayer) return;
        if(other)
        {
            if(other.TryGetComponent(out HookSM hook))
            {
                // M_Net.Cmd_SetInnerPlayer(hook.gameObject);
                HideE();
                _ConnectPlayer = null;
            }
        }
    }


#region  main
    private void MirrorRotate(bool pm){
        if(pm){
            Quaternion curRot = _Mirror.transform.rotation;
            curRot.z +=.001f;
            // _Mirror.transform.rotation = curRot;
            M_Net.Cmd_SetRot_z(curRot.z);
        }else{
            Quaternion curRot = _Mirror.transform.rotation;
            curRot.z -=.001f;
            M_Net.Cmd_SetRot_z(curRot.z);
            // _Mirror.transform.rotation = curRot;
        }
    }
    private void OnActiveMirrorRotate(){
        if(_ConnectPlayer == null) return;
        //_ConnectPlayer.GetComponent<PlayerSM>().canControl = false;
        //Managers.Game.Player.GetComponent<Rigidbody2D>().simulated = false;
        //Managers.Game.Player.GetComponent<Rigidbody2D>().velocity  = Vector2.zero;
        onActive = true;
        
    }
    private void OnDeactiveMirrorRotate(){
        //_ConnectPlayer.GetComponent<PlayerSM>().canControl = true;
        //Managers.Game.Player.GetComponent<Rigidbody2D>().simulated = true;
        onActive  = false;
    }
    

#endregion

#region  Interacte
    // public void Interaction(Transform accessor = null){
    //      if (!NetworkServer.active || !NetworkClient.isConnected)
    //         return;
    //     if (_ConnectPlayer != null){
    //         if (onActive){
    //             OnDeactiveMirrorRotate();
    //         }else{
    //             OnActiveMirrorRotate();
    //         }
    //     }

      
    // }

    public void Interaction(Transform accessor = null)
    {
        if(_ConnectPlayer != null)
        {
            HideE() ;
            if(IsActive)
            {
                M_Net.Cmd_SetInnerPlayer(null);
            }
            else
            {
                M_Net.Cmd_SetInnerPlayer(_ConnectPlayer);
            }
            
        }
    }
    public bool CanInteract(){
        return true;
    }

    public void Interacting(bool value)
    {
        return;
    }

    public ObjectTypeEnum GetObjectType(){
        return _objectType;
    }

    public void ShowEButton(){
        return;
        
    }
    public void ShowE()
    {
        if (!IsInnerPlayer)
        {
            _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
            _E_Btn.transform.position = transform.position + (transform.up * _BtnOffset);
        }
    }
    private void HideE()
    {
        if (_E_Btn == null) return;
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }
    
    public void HideEButton(){
        return;
    }
#endregion





}
