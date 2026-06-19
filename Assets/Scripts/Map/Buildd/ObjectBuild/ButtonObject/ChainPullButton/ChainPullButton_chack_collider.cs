using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ChainPullButton_chack_collider :  NetworkBehaviour, IInteractable, IInhalable
{
    private ChainPullButton_Net _net;
    private ChainPullButton_Net Net {get{_net ??= transform.GetComponentInParent<ChainPullButton_Net>(); return _net; }}
    private Rigidbody2D _rb;
    private Rigidbody2D Rb { get { _rb ??= GetComponent<Rigidbody2D>(); return _rb; } }


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
            Net.Cmd_Start_Hailing_Track_L(accessor.root.gameObject.GetComponent<NetworkIdentity>().netId);
        }else
        {

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
#region IInteractable
    [field: SerializeField] protected ObjectTypeEnum _objectType = ObjectTypeEnum.Grab;

    public void Interaction(Transform accessor)
    {
        
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
        
    }

    public void HideEButton()
    {
        
    }
 #endregion
}
