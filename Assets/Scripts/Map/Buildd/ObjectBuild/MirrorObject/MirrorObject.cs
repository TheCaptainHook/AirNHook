using Mirror;
using System.Collections;
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
    public ObjectTypeEnum _objectType = ObjectTypeEnum.Control;
    private Vector3 _offset = new Vector2(0, 2.8f);
    private UI_Base _E_Btn;
    private UI_ControlADE _AorD_Btn;

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

    //--------------------------- Refectoring 0523
    private float cendMessageRate = 0.1f;
    private float curCendMessageRate = 0;
    private float curRot = 0;
    float rotRate = 1f;

    private void Update()
    {
        if (isActive)
        {
            if (Input.GetKey(KeyCode.A))
            {
                _AorD_Btn.AButtonPress(true);
                curCendMessageRate += Time.deltaTime;
                curRot += rotRate;

                if (curCendMessageRate >= cendMessageRate)
                {
                    MirrorRotate(curRot); //1
                    curRot = 0;
                    curCendMessageRate = 0;
                }

            }
            if (Input.GetKey(KeyCode.D))
            {
                _AorD_Btn.DButtonPress(true);
                curCendMessageRate += Time.deltaTime;
                curRot -= rotRate;
                if (curCendMessageRate >= cendMessageRate)
                {
                    MirrorRotate(curRot);
                    curRot = 0;
                    curCendMessageRate = 0;
                }
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                _AorD_Btn.AButtonPress(false);
                MirrorRotate(curRot);
                curRot = 0;
                curCendMessageRate = 0;
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                _AorD_Btn.DButtonPress(false);
                MirrorRotate(curRot);
                curRot = 0;
                curCendMessageRate = 0;
            }
        }
    }



    #region  main

    private void MirrorRotate(float z)
    {
        M_Net.Cmd_SetRot_z(z);
    }


#endregion
    //--------------------------- Refectoring 0523

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (IsInnerPlayer) return;
    //
    //    if (other)
    //    {
    //        if (other.TryGetComponent(out PlayerSM player))
    //        {
    //            var playerIdentity = player.gameObject.TryGetComponent(out NetworkIdentity identity) ? identity : null;
    //            if(playerIdentity != null)
    //            {
    //                if (playerIdentity.isLocalPlayer) ShowE();
    //                M_Net.Cmd_InnerPlayer(playerIdentity.netId);
    //            }
    //
    //        }
    //    }
    //
    //}
    //
    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other)
    //    {
    //        if (other.TryGetComponent(out PlayerSM PS))
    //        {
    //            if(PS.gameObject == M_Net.InnerPlayer)
    //            {
    //                HideE();
    //                M_Net.Cmd_InnerPlayer(9999);
    //            }
    //        }
    //    }
    //}


  

#region  Interacte
    public void Interaction(Transform accessor = null)
    {
        //if (!M_Net.InnerPlayer || M_Net.InnerPlayer != accessor.gameObject) return;

        if (isActive && M_Net.InnerPlayer != null && M_Net.InnerPlayer == accessor.gameObject)
        {
            //Dis Connect
            ChangeEbutton(false);
            M_Net.Cmd_InnerPlayer(9999);
            M_Net.Recover();
        }
        else
        {
            //Connect
            ChangeEbutton(true);
            M_Net.Cmd_InnerPlayer(accessor.GetComponent<NetworkIdentity>().netId);
            M_Net.Holding(accessor.gameObject);
        }
    }

    public bool CanInteract()
    {
        return true;
    }

    public bool Interacting(bool value, GameObject player)
    {
        return true;
    }

    public ObjectTypeEnum GetObjectType()
    {
        return _objectType;
    }

    public void ShowEButton()
    {
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position = transform.position + _offset;
    }

    public void HideEButton()
    {
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }

    public void ShowADEButton()
    {
        _AorD_Btn = (UI_ControlADE)Managers.UI.ShowUI<UI_ControlADE>();
        _AorD_Btn.transform.position = transform.position;
    }

    public void HideADEButton()
    {
        _AorD_Btn = null;
        Managers.UI.HideUI<UI_ControlADE>();
    }

    private void ChangeEbutton(bool isInteracting)
    {
        if (isInteracting)
        {
            HideEButton();
            ShowADEButton();
        }
        else
        {
            ShowEButton();
            HideADEButton();
        }
    }

    //public void ShowE()
    //{
    //    if (!IsInnerPlayer)
    //    {
    //        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
    //        _E_Btn.transform.position = transform.position + (transform.up * _BtnOffset);
    //    }
    //}
    //public void HideE()
    //{
    //    if (_E_Btn == null) return;
    //    _E_Btn = null;
    //    Managers.UI.HideUI<UI_ShowEButton>();
    //}\
    #endregion
}
