using Mirror;
using System.Collections;
using UnityEngine;


public class MirrorObject : BuildObj, IInteractable
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
    private float _minRotationSpeed = 2f;
    private float _maxRotationSpeed = 45f;
    public float accelerateDuration = 0.05f;
    private bool isKeyHeld = false;
    private float holdTime = 0f;
    private KeyCode heldKey;

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

    #region  Clean
    public override void Clean()
    {
        _Mirror.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
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

    private void Update()
    {
        if (!isActive) return;

        bool aHeld = Input.GetKey(KeyCode.A);
        bool dHeld = Input.GetKey(KeyCode.D);

        if (aHeld && dHeld) return;

        if (aHeld || dHeld)
        {
            KeyCode key = aHeld ? KeyCode.A : KeyCode.D;
            float dir = (key == KeyCode.A) ? 1f : -1f;

            if (!isKeyHeld || key != heldKey)
            {
                isKeyHeld = true;
                heldKey = key;
                holdTime = 0f;
            }

            float currentSpeed = _minRotationSpeed;

            if (holdTime > accelerateDuration)
            {
                float t = Mathf.Clamp01((holdTime - accelerateDuration) / accelerateDuration);
                currentSpeed = Mathf.Lerp(_minRotationSpeed, _maxRotationSpeed, t);
            }
            holdTime += Time.deltaTime;
            Debug.Log(holdTime);

            _Mirror.transform.Rotate(0f, 0f, currentSpeed * Time.deltaTime * dir);
        }
        else
        {
            // 키를 떼면 즉시 멈춤
            isKeyHeld = false;
            holdTime = 0f;
        }
    }

#region  Interacte
    public void Interaction(Transform accessor = null)
    {
        if (isActive && M_Net.InnerPlayer != null && M_Net.InnerPlayer == accessor.gameObject)
        {
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

    public void ChangeEbutton(bool isInteracting)
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
    #endregion
}
