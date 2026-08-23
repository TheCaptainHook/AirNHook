
using UnityEngine;
using UnityEngine.Animations;
using Mirror;

public class ChainPullButton_chack_collider :  NetworkBehaviour, IInteractable, IInhalable
{
    private ChainPullButton_Net _net;
    private ChainPullButton_Net Net {get{_net ??= transform.GetComponentInParent<ChainPullButton_Net>(); return _net; }}
    private Rigidbody2D _rb;
    private Rigidbody2D Rb { get { _rb ??= GetComponent<Rigidbody2D>(); return _rb; } }


    private ParentConstraint _parentConstraint;
    private ParentConstraint ParentConstraint { get { _parentConstraint ??= GetComponent<ParentConstraint>(); return _parentConstraint; } }

    public bool rl; //true : r, false : l

    public void OnInhaling(bool onoff)
    {
        if(onoff)
        {
            // transform.GetComponent<Collider2D>().enabled = false;
            transform.GetComponent<Rigidbody2D>().gravityScale = 0f;
        }else
        {
            // transform.GetComponent<Collider2D>().enabled = true;
            transform.GetComponent<Rigidbody2D>().gravityScale = 10f;
        }
    }
    public void RemoveVelocity()
    {
        Rb.velocity = Vector2.zero;
    }

    [Command(requiresAuthority = false)]
    private void Cmd_OnInhaling(bool onOff)
    {
        Rpc_OnInHailing(onOff);
    }

    [ClientRpc]
    private void Rpc_OnInHailing(bool onOff)
    {
        OnInhaling(onOff);
    }
    
    #region IInhalable
    
    public void Inhalation(Transform accessor)
    {

        Cmd_OnInhaling(true);
        if (!rl) //false(l) : blue, true(r) : red
        { 
            Debug.Log("left");
            Net.Cmd_Start_Track_L(accessor.root.gameObject.GetComponent<NetworkIdentity>().netId);
        }else
        {
            Debug.Log("right");
            Net.Cmd_Start_Track_R(accessor.root.gameObject.GetComponent<NetworkIdentity>().netId);
        }
    }
   

    public void StopInhale(GameObject accessor)
    {
        Cmd_OnInhaling(false); //After Sync

        if(!rl)
        {
            Net.Cmd_Stop_Track_L();
            
        }else
        {
            Net.Cmd_Stop_Track_R();
        }

        //Start Recovery Chain
    }

    [Command(requiresAuthority = false)]
    private void Cmd_Fixed(bool value)
    {
        if(!rl)
        Net._Left_isAirGun_Attached = value;
        else Net._Right_isAirGun_Attached = value;
    }

    public void Fixed(bool value)
    {
        Cmd_Fixed(value);
        return;
    }
    
    public bool Inhaling(bool value, GameObject player)
    {

        return true;
    }

    public void Shooting(Vector2 force)
    {
        return;
    }

    public bool CanInhale()
    {
        return true;
    }
#endregion
#region  Sound
public void Hook_PickUpSound()
{
    
}
public void Air_PickUpSound()
{
    
}

#endregion
#region IInteractable
    [field: SerializeField] protected ObjectTypeEnum _objectType = ObjectTypeEnum.Grab;

    public void Interaction(Transform accessor)
    {
        NetworkIdentity sm = accessor.root.gameObject.GetComponentInParent<NetworkIdentity>();
        if(sm == null) return;

        if(rl) //false(l) : blue, true(r) : red
        {
            if(Net._Right_isAirGun_Attached) return;
            if(Net._Right_isGrapping)
            {
                Cmd_OnInhaling(false);
                Net.Cmd_Stop_Track_R();
            }else
            {
                Cmd_OnInhaling(true);
                Net.Cmd_Start_Track_R(sm.netId);
            }
            Debug.Log("Right");
            
        }
        else
        {
            if(Net._Left_isAirGun_Attached) return;
            if(Net._Left_isGrapping)
            {
                Cmd_OnInhaling(false);
                Net.Cmd_Stop_Track_L();
            }else
            {
                Cmd_OnInhaling(true);
                Net.Cmd_Start_Track_L(sm.netId);
            }
            Debug.Log("Left");
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
    private UI_Base _E_Btn;
    [SerializeField] Vector2 _BtnOffset;
    public void ShowEButton()
    {
        if(Net._Left_isGrapping || Net._Right_isGrapping) return;

        if(rl) //false(l) : blue, true(r) : red
        {
            if(!Net._Right_isAirGun_Attached)
            {
                _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
                _E_Btn.transform.position =  (Vector2)transform.position + _BtnOffset;
            }
        }
        else
        {
            if(!Net._Left_isAirGun_Attached)
            {
                _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
                _E_Btn.transform.position =  (Vector2)transform.position + _BtnOffset;
            }
        }

       
    }

    public void HideEButton()
    {
        if(_E_Btn != null) _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }

    public void Constranint_Reset()
    {
        if (ParentConstraint.sourceCount > 0)
        {
            ParentConstraint.RemoveSource(0);
        }
        
    }
 #endregion
}
