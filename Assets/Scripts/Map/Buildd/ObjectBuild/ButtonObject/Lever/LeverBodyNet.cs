using Mirror;
using System.Collections;
using UnityEngine;

public class LeverBodyNet : NetworkBehaviour
{
    [SerializeField] Transform attachedLeverHead;
    Animator Animator => GetComponent<Animator>();
    private static readonly int OnActive = Animator.StringToHash("OnActive");
    private static readonly int OnCompletion = Animator.StringToHash("OnCompletion");


    private LeverBody body;
    private LeverBody Body { get { if (body == null) body = GetComponent<LeverBody>(); return body; } }

    [SyncVar] public bool onCompletionParts;
    [SyncVar] public bool onActive;
    [SyncVar] public bool onOperation;


#region  InitSync
    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(Body.ButtonObjectData);
    }
    [ClientRpc]
    private void Rpc_InitSync(ButtonObjectStruct data)
    {
        if(onSync) return;
        transform.position = data.position;
        onSync = true;

    }
    [Command]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!onSync)Cmd_InitSync();
    }
#endregion


    [Server]
    public void Server_SetLeverHead(LeverHead head)
    {
        head.AttachToLevelBody();

        StartCoroutine(Destroy_Head(head));

        onCompletionParts = true;

        Rpc_SetLeverHead();
        
    }


    [Command(requiresAuthority = false)]
    public void Cmd_SetLeverHead(uint id)
    {
        if(isServer)
        {
            var item = NetworkClient.spawned.TryGetValue(id, out var body) ? body : null;
            if (item == null) return;

            Server_SetLeverHead(item.GetComponent<LeverHead>());
        }
    }

    IEnumerator Destroy_Head(LeverHead head)
    {
        //head.transform.GetChild(0).gameObject.SetActive(false);
        head.Net_Att();
        yield return new WaitForSeconds(1f);
        NetworkServer.Destroy(head.gameObject);
    }

    [ClientRpc]
    private void Rpc_SetLeverHead()
    {
        attachedLeverHead.gameObject.SetActive(true);
        Animator.SetTrigger(OnCompletion);
    }


    [Server]
    public void Server_Active()
    {
        onActive = !onActive;

        StartCoroutine(Co_Operation());
    }

    [Command(requiresAuthority = false)]
    public void Cmd_Active()
    {
        if(onCompletionParts && !onOperation)
        {
            Server_Active();
        }
    }

    //[ClientRpc]
    //public void Rpc_Active()
    //{

    //}





    IEnumerator Co_Operation()
    {
        onOperation = true;
        if (onActive)
        {
            Animator.SetBool(OnActive, true);
            AnimatorStateInfo animationState = Animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log(animationState.length);
            //PrograssButtonActivatedObject(true);
            Body.Net_Act();
            yield return new WaitForSeconds(animationState.length + 0.5f);
        }
        else
        {
            Animator.SetBool(OnActive, false);
            AnimatorStateInfo animationState = Animator.GetCurrentAnimatorStateInfo(0);
            //PrograssButtonActivatedObject(false);
            Body.Net_Deac();
            yield return new WaitForSeconds(animationState.length + 0.5f);

        }

        onOperation = false;

    }

}
