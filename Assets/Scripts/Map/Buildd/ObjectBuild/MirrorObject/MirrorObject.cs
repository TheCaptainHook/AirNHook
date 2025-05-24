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
    public bool isActive;
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
    //----------------------------------before 0523
    // private void Update(){
    //     if(isActive)
    //     {
    //         if(Input.GetKey(KeyCode.A)){
    //             MirrorRotate(true);
    //         }
    //         if(Input.GetKey(KeyCode.D)){
    //             MirrorRotate(false);
    //         }

    //     }
    // }

    // #region  main
    // float serverRotRate = 0.005f;
    // float clientRotRate = 0.01f;
    // private void MirrorRotate(bool pm){


    //     if(pm){
    //         Quaternion curRot = _Mirror.transform.rotation;
    //         //curRot.z +=.005f;
    //         curRot.z += (NetworkServer.active) ? serverRotRate : clientRotRate;
    //         // _Mirror.transform.rotation = curRot;
    //         M_Net.Cmd_SetRot_z(curRot.z);
    //     }else{
    //         Quaternion curRot = _Mirror.transform.rotation;
    //         //curRot.z -=.005f;
    //         curRot.z -= (NetworkServer.active) ? serverRotRate : clientRotRate;
    //         M_Net.Cmd_SetRot_z(curRot.z);
    //         // _Mirror.transform.rotation = curRot;
    //     }
    // }
    // #endregion
    //----------------------------------before 0523

    //--------------------------- Refectoring 0523
    private float cendMessageRate = 0.1f;
    private float curCendMessageRate = 0;
    private float curRot = 0;
    float rotRate = 0.5f;
    private void Update()
    {

        if (isActive)
        {
            if (Input.GetKey(KeyCode.A))
            {
                curCendMessageRate += Time.deltaTime;
                curRot += rotRate;
                
                if (curCendMessageRate >= cendMessageRate)
                {
                    MirrorRotate(curRot, true); //1
                    curRot = 0;
                    curCendMessageRate = 0;
                }

            }
            if (Input.GetKey(KeyCode.D))
            {
                curCendMessageRate += Time.deltaTime;
                curRot -= rotRate;
                if (curCendMessageRate >= cendMessageRate)
                {
                    MirrorRotate(curRot,false);
                    curRot = 0;
                    curCendMessageRate = 0;
                }
            }

        }
    }
    #region  main
    

    // float clientRotRate = 0.01f;
    // private void MirrorRotate(float z)
    // {

    //     if (pm)
    //     {
    //         Quaternion curRot = _Mirror.transform.rotation;
    //         //curRot.z +=.005f;
    //         // curRot.z += (NetworkServer.active) ? serverRotRate : clientRotRate;
    //         // _Mirror.transform.rotation = curRot;
    //         M_Net.Cmd_SetRot_z(curRot.z);
    //     }
    //     else
    //     {
    //         Quaternion curRot = _Mirror.transform.rotation;
    //         //curRot.z -=.005f;
    //         // curRot.z -= (NetworkServer.active) ? serverRotRate : clientRotRate;
    //         M_Net.Cmd_SetRot_z(curRot.z);
    //         // _Mirror.transform.rotation = curRot;
    //     }
    // }
     private void MirrorRotate(float z,bool lr)
    {     
        M_Net.Cmd_SetRot_z(z,lr);
    }
 

#endregion
    //--------------------------- Refectoring 0523



    private bool IsInnerPlayer => M_Net.InnerPlayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsInnerPlayer) return;

        if (other)
        {
            if (other.TryGetComponent(out PlayerSM player))
            {
                var playerIdentity = player.gameObject.TryGetComponent(out NetworkIdentity identity) ? identity : null;
                if(playerIdentity != null)
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


  

#region  Interacte


    public void Interaction(Transform accessor = null)
    {
        if (!M_Net.InnerPlayer || M_Net.InnerPlayer != accessor.gameObject) return;

        if (isActive)
        {
            //Dis Connect
            M_Net.Recover();

        }
        else
        {
            //Connect
            M_Net.Holding(accessor.gameObject);
        }


    }
    public bool CanInteract(){
        return true;
    }

    public bool Interacting(bool value, GameObject player)
    {
        return true;
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
