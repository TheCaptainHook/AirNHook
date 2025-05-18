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


    public override void SetData<T>(T data)
    {
       if(typeof(T) == typeof(ObjectData)){
            ObjectData = (ObjectData)(object)data;
            if(Application.isPlaying)
            M_Net.Server_InitSync();
            else SetData(ObjectData);
            // SetData(objData);
        }
    }

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

    


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(IsInnerPlayer) return;

        if(other)
        {
            if(other.TryGetComponent(out PlayerSM PS))
            {
                // M_Net.Cmd_SetInnerPlayer(hook.gameObject);
             
                _ConnectPlayer = PS.gameObject;
                M_Net.Cmd_ShowE(PS.gameObject, true);
            }
        }
     
    }

    private void OnTriggerExit2D(Collider2D other)
    {
       if(IsInnerPlayer) return;
        if(other)
        {
            if(other.TryGetComponent(out PlayerSM PS))
            {
                // M_Net.Cmd_SetInnerPlayer(hook.gameObject);
                M_Net.Cmd_ShowE(PS.gameObject, false);
                _ConnectPlayer = null;
            }
        }
    }


#region  main
    private void MirrorRotate(bool pm){
        if(pm){
            Quaternion curRot = _Mirror.transform.rotation;
            curRot.z +=.005f;
            // _Mirror.transform.rotation = curRot;
            M_Net.Cmd_SetRot_z(curRot.z);
        }else{
            Quaternion curRot = _Mirror.transform.rotation;
            curRot.z -=.005f;
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


    public void Interaction(Transform accessor = null)
    {
        if (_ConnectPlayer != null)
        {
            HideE();
            if (IsActive)
            {
                if (_ConnectPlayer) _ConnectPlayer = null; //test
                M_Net.Cmd_SetInnerPlayer(null);
            }
            else
            {
                M_Net.Cmd_SetInnerPlayer(_ConnectPlayer);
            }

        }
        else
        {
            if (IsActive)
            {
                M_Net.Cmd_SetInnerPlayer(null);
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
 
    public void HideEButton(){
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
    public void HideE()
    {
        if (_E_Btn == null) return;
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }
    #endregion





}
