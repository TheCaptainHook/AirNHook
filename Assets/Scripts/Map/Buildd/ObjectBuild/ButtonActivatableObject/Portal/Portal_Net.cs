using System.Collections;
using UnityEngine;
using Mirror;

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
#region  Clean
    public override void Clean_Value()
    {
        onPrograss = false;
    }
#endregion
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
        yield return new WaitForSeconds(1f);
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
            // TRpc_PlayUniqueEffect(item.connectionToClient, identity.gameObject);
            Rpc_PlayUniqueEffect(id);
        }
    }
  
    // [TargetRpc]
    // private void TRpc_PlayUniqueEffect(NetworkConnection conn, GameObject obj)
    // {
    //     if (targetPortal == null)
    //     {
    //         var targetItem = NetworkClient.spawned.TryGetValue(targetId, out var identity) ? identity.gameObject : null;
    //         if (targetItem != null)
    //         {
    //             targetPortal = targetItem;
    //             UsePortal(obj);
    //         }
    //     }
    //     else
    //     {
    //         UsePortal(obj);
    //     }
    // }

    //----------------
    [ClientRpc]
    private void Rpc_PlayUniqueEffect(uint id)
    {
        var item = NetworkClient.spawned.TryGetValue(id, out var identity) ? identity.gameObject : null;
        if (item != null)
        {
             var targetItem = NetworkClient.spawned.TryGetValue(targetId, out var targetPortal) ? targetPortal.gameObject : null;
            if (targetItem != null)
            {
                this.targetPortal = targetItem;
                UsePortal(item);
            }
            // UsePortal(item);
        }
    }
//----------------

    private void UsePortal(GameObject obj)
    {
        if (!onPrograss) StartCoroutine(UsePortal_Co(obj));
    }
  


    //------------------------------------------------------------------Refactoring
    #region StringCache
    private static readonly int IsActive = Animator.StringToHash("IsActive");
    #endregion

    private Portal Portal => GetComponent<Portal>();
    private Animator Animator => GetComponent<Animator>();


    IEnumerator UsePortal_Co(GameObject obj)
    {
        var identity = obj.TryGetComponent(out NetworkIdentity netIdentity) ? netIdentity : null;
        var sm =obj.TryGetComponent(out PlayerSM playerSm)  ? playerSm : null;

        if (identity.isLocalPlayer)
        {
            //Player Hold
            
            if(sm != null) sm.FreezePlayerState(true);
            
            if (obj.TryGetComponent(out Rigidbody2D component))
            {
                component.simulated = false;
            }
            //Player Hold

            OnPrograss(true);

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
        }
        else
        {
            Managers.Sound.PlaySound3D(GlobalText.PORTAL_IN, obj.transform);
        }

        yield return new WaitForSeconds(1);
        if (identity.isLocalPlayer)
        {
            Sound(false);
            if(sm != null) sm.FreezePlayerState(false);
            //Player Recover
            if (obj.TryGetComponent(out Rigidbody2D component))
            {
                component.simulated = true;
            }

            //Player Recover
            yield return new WaitForSeconds(2);
            OnPrograss(false);
        }
        else
        {
            Managers.Sound.PlaySound3D(GlobalText.PORTAL_OUT, targetPortal.transform);
        }

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
