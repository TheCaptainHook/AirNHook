using System.Collections;
using UnityEngine;
using Mirror;
using System;
public class Portal_Net : ActivatableObject_Net_Entity
{
    [SerializeField] GameObject _TpEffect;
    [SyncVar] public uint targetId;
    protected override void Active()
    {
        Animator.SetBool(IsActive, true);
        _TpEffect.SetActive(true);
    }
    protected override void Deactive()
    {
         Animator.SetBool(IsActive, false);
        _TpEffect.SetActive(false);
    }
    

    //------------------------------------------------------------------Refactoring0303
    public Vector2 orgPosition => Portal.ButtonActivatedObjectStruct.position;
    public GameObject targetPortal;
    // public bool onSync;
    [SyncVar] public Vector2 targetPortalPosition;

    public bool onPrograss;

    protected override void SetData(ButtonActivatableObjectStruct data)
    {
        base.SetData(data);
        if(isServer)
        StartCoroutine(FindTarget(data.talPot));
    }

    IEnumerator  FindTarget(Vector2 target)
    {
        yield return new WaitForSeconds(0.1f);
        foreach (Transform tr in MapEditor.Instance.buttonActivatableObjectTransform)
        {
            if (tr.TryGetComponent(out Portal component))
            {
                if (target == (Vector2)component.transform.position)
                {
                    targetId = component.TryGetComponent(out NetworkIdentity identity) ? identity.netId : 9999;
                    targetPortalPosition = component.transform.position;
                    yield break;
                }
            }
        }
    }

    [Server]
    public override void Server_PlayUniqueEffect(uint id)
    {
        var item = NetworkClient.spawned.TryGetValue(id, out var identity) ? identity : null;
        
        if (item != null)
        {
            TRpc_PlayUniqueEffect(item.connectionToClient, identity.gameObject);
        }
    }
  
    [TargetRpc]
    private void TRpc_PlayUniqueEffect(NetworkConnection conn, GameObject obj)
    {
        if (targetPortal == null)
        {
            var targetItem = NetworkClient.spawned.TryGetValue(targetId, out var identity) ? identity.gameObject : null;
            if (targetItem != null)
            {
                targetPortal = targetItem;
                UsePortal(obj);
            }
        }
        else
        {
            UsePortal(obj);
        }

       
    }

    private void UsePortal(GameObject obj)
    {
        if(!onPrograss) StartCoroutine(UsePortal_Co(obj));
    }
  


    //------------------------------------------------------------------Refactoring
    #region StringCache
    private static readonly int IsActive = Animator.StringToHash("IsActive");
    #endregion

    private Portal Portal => GetComponent<Portal>();
    private Animator Animator => GetComponent<Animator>();


    IEnumerator UsePortal_Co(GameObject obj)
    {
        OnPrograss(true);

        //Player Hold
        if (obj.TryGetComponent(out Rigidbody2D component))
        {
            component.simulated = false;
        }
        //Player Hold
        Sound(true);
        //Camera Effect
        if (Camera.main != null)
        {
            Camera.main.GetComponent<PlayerCameraView>()?._CameraGlobalVolumeController?
                .PortalSpace_TimeTransitionEffect();
        }
        //Camera Effect
        //Player Position
        obj.transform.position = targetPortalPosition + Vector2.up;
        //Player Position

        yield return new WaitForSeconds(1);
        Sound(false);
        //Player Recover
        component.simulated = true;
        //Player Recover

        yield return new WaitForSeconds(2);
 
        OnPrograss(false);

    }
    private void Sound(bool inOut)
    {
        if (inOut) Managers.Sound.PlaySound(GlobalText.PORTAL_IN);
        else Managers.Sound.PlaySound(GlobalText.PORTAL_OUT);
    }
    public void Animation_Active(bool active)
    {
        Animator.SetBool(IsActive, active);
        _TpEffect.SetActive(active);
    }

  
    private void OnPrograss(bool onOff)
    {
        onPrograss = onOff;

        var target = targetPortal.GetComponent<Portal_Net>();
        target.onPrograss = onOff;

        Animation_Active(!onOff);
        target.Animation_Active(!onOff);
    }
    

}
