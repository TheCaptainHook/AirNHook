using Mirror;
using UnityEngine;

public class MirrorObject : BuildObj,IInteractable
{
    [CustomHeader("Mirror Object")]
    [SerializeField] GameObject _Mirror;
    [ReadOnly]
    //[SerializeField] GameObject _ConnectPlayer;
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

   
    //private bool IsActive => M_Net.onActive;
    private bool isActive;
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
        if(isActive)
        {
            if(Input.GetKey(KeyCode.A)){
                MirrorRotate(true);
            }
            if(Input.GetKey(KeyCode.D)){
                MirrorRotate(false);
            }
            
        }
    }


    private bool IsInnerPlayer => M_Net.InnerPlayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsInnerPlayer) return;

        if (other)
        {
            if (other.TryGetComponent(out PlayerSM player))
            {
                var playerIdentity = player.gameObject.TryGetComponent(out NetworkIdentity identity) ? identity : null;
                if(playerIdentity)
                {
                    if (playerIdentity.isLocalPlayer) ShowE();
                    M_Net.Cmd_InnerPlayer(playerIdentity.netId);
                }

            }
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other)
        {
            if (other.TryGetComponent(out PlayerSM PS))
            {
                if(PS.gameObject == M_Net.InnerPlayer)
                {
                    HideE();
                    M_Net.Cmd_InnerPlayer(9999);
                }
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
 

#endregion

#region  Interacte


    public void Interaction(Transform accessor = null)
    {
        if (!M_Net.InnerPlayer || M_Net.InnerPlayer != accessor.gameObject) return;

        if (isActive)
        {
            //Dis Connect
            M_Net.Recover(accessor.gameObject); 
            isActive = false;
        }
        else
        {
            //Connect
            M_Net.Holding(accessor.gameObject);
            isActive = true;
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
