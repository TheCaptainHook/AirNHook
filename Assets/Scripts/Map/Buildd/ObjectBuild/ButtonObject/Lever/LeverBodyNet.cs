using Mirror;
using System.Collections;
using UnityEngine;

public class LeverBodyNet : ButtonEntity_Net
{
    [SerializeField] Transform attachedLeverHead;
    Animator _ani;
    Animator Animator {get{ _ani ??= GetComponent<Animator>(); return _ani; } }
    private static readonly int OnActive = Animator.StringToHash("OnActive");
    private static readonly int OnCompletion = Animator.StringToHash("OnCompletion");
    private static readonly int OnIdel = Animator.StringToHash("OnIdel");


    // private LeverBody body;
    // private LeverBody Body { get { if (body == null) body = GetComponent<LeverBody>(); return body; } }

    [SyncVar] public bool onCompletionParts;
    // [SyncVar] public bool onActive; //_isActive
    [SyncVar] public bool onOperation;


    [Server]
    public void Server_SetLeverHead(LeverHead head) // Server
    {
        head.AttachToLevelBody(); //head 스프라이트 제거 후 오브젝트 제거.

        onCompletionParts = true;
        Rpc_SetLeverHead(onCompletionParts);

    }


    [Command(requiresAuthority = false)]
    public void Cmd_SetLeverHead(uint id)
    {
        if (isServer)
        {
            var item = NetworkClient.spawned.TryGetValue(id, out var body) ? body : null;
            if (item == null) return;

            Server_SetLeverHead(item.GetComponent<LeverHead>());
        }
    }

    [ClientRpc]
    private void Rpc_SetLeverHead(bool onCompletionParts)
    {
        attachedLeverHead.gameObject.SetActive(onCompletionParts);
        Animator.SetTrigger(OnCompletion);
    }


    [Server]
    public void Server_Active()
    {
        // onActive = !onActive;

        // StartCoroutine(Co_Operation());
        Rpc_Active();
    }
    [ClientRpc]
    private void Rpc_Active()
    {
        _isActive = !_isActive;
        StartCoroutine(Co_Operation());
    }

    [Command(requiresAuthority = false)]
    public void Cmd_Active()
    {
        if (onCompletionParts && !onOperation)
        {
            Server_Active();
        }
    }


    IEnumerator Co_Operation()
    {
        onOperation = true;
        if (_isActive)
        {
            Animator.SetBool(OnActive, true);
            AnimatorStateInfo animationState = Animator.GetCurrentAnimatorStateInfo(0);
            Main.Activation();
            yield return new WaitForSeconds(animationState.length + 0.5f);
        }
        else
        {
            Animator.SetBool(OnActive, false);
            AnimatorStateInfo animationState = Animator.GetCurrentAnimatorStateInfo(0);
            Main.Deactivated();
            yield return new WaitForSeconds(animationState.length + 0.5f);

        }
        onOperation = false;

    }

    public override void Server_Clean()
    {
        onCompletionParts = false;
        base.Server_Clean();
    }
    protected override void Rpc_Clean()
    {
        StopAllCoroutines();
        Animator.SetTrigger(OnIdel);
        
        attachedLeverHead.gameObject.SetActive(false);

        onOperation = false;
        _isActive = false;
        _onSync = false;
    }
}
