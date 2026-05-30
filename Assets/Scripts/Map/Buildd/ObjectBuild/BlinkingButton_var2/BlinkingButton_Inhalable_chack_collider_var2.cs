using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class BlinkingButton_Inhalable_chack_collider_var2 : NetworkBehaviour, IInteractable, IInhalable
{
    private BlinkingButton_var2_Net _net;
    private BlinkingButton_var2_Net Net { get { _net ??= transform.GetComponentInParent<BlinkingButton_var2_Net>(); return _net; } }

    public bool rl; //true : r, false : l

//===================================================0524
    /**
        during inhaling, l,r chain parts colider disable.
    **/
//===================================================0524

    public void OnInhaling(bool onoff)
    {
        if(onoff)
        {
            transform.GetComponent<Collider2D>().enabled = false;
            transform.GetComponent<Rigidbody2D>().gravityScale = 0f;
        }else
        {
            transform.GetComponent<Collider2D>().enabled = true;
            transform.GetComponent<Rigidbody2D>().gravityScale = 3f;
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
        Debug.Log($"Start InHal,Blink, / {accessor.root.name}");
        if(accessor == null) return;


        if (!rl)
        { 
            Cmd_OnInhaling(true);
            Net.Cmd_Start_Track_L(accessor.root.gameObject.GetComponent<NetworkIdentity>().netId);
        }
    }
   

    public void StopInhale(GameObject accessor)
    {
        Debug.Log("Stop InHal,Blink");
        Cmd_OnInhaling(false);
        Net.Cmd_Stop_Track_L();
        //Start Recovery Chain
    }

    public void Fixed(bool value)
    {
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
