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
    

    
    private void Update(){
        if(onActive){
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
        if(onActive) return;

        if(_ConnectPlayer != null){
            return;
        }
        // ShowEButton();
       _ConnectPlayer = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other){
        // HideEButton();
        if (onActive) onActive = false;
        _ConnectPlayer = null;
    }
  

#region  main
    private void MirrorRotate(bool pm){
        if(pm){
            Quaternion curRot = _Mirror.transform.rotation;
            curRot.z +=.01f;
            _Mirror.transform.rotation = curRot;
        }else{
            Quaternion curRot = _Mirror.transform.rotation;
            curRot.z -=.01f;
            _Mirror.transform.rotation = curRot;
        }
    }
    private void OnActiveMirrorRotate(){
        if(_ConnectPlayer == null) return;
        //_ConnectPlayer.GetComponent<PlayerSM>().canControl = false;
        //Managers.Game.Player.GetComponent<Rigidbody2D>().simulated = false;
        Managers.Game.Player.GetComponent<Rigidbody2D>().velocity  = Vector2.zero;
        onActive = true;
        
    }
    private void OnDeactiveMirrorRotate(){
        //_ConnectPlayer.GetComponent<PlayerSM>().canControl = true;
        //Managers.Game.Player.GetComponent<Rigidbody2D>().simulated = true;
        onActive  = false;
    }
    

#endregion

#region  Interacte
    public void Interaction(Transform accessor = null){
         if (!NetworkServer.active || !NetworkClient.isConnected)
            return;
        if (_ConnectPlayer != null){
            if (onActive){
                OnDeactiveMirrorRotate();
            }else{
                OnActiveMirrorRotate();
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
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position =  transform.position + (transform.up * _BtnOffset);
    }
    
    public void HideEButton(){
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }
#endregion





}
