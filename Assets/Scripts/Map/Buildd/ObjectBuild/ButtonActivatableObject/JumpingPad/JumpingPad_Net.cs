using Mirror;
using UnityEngine;

public class JumpingPad_Net : ActivatableObject_Net_Entity
{
    public int jumpingPower;

    #region  Animation
    Animator Animator => GetComponent<Animator>();
    readonly int Activated = Animator.StringToHash("Activated");
    #endregion

    #region  Init Sync
   
    protected override void SetData(ButtonActivatableObjectStruct data)
    {
        base.SetData(data);
        jumpingPower = data.jumpingPower;
    }

    #endregion


    protected override void Active()
    {
        Animator.SetBool(Activated, true);
    }
    protected override void Deactive()
    {
        Animator.SetBool(Activated, false);
    }



    [Server]
    public override void Server_PlayUniqueEffect(uint id)
    {
        var item = NetworkClient.spawned.TryGetValue(id,out NetworkIdentity identity) ? identity : null;
        if (!item ) return;

        TRpc_PlayUniqueEffect(identity.connectionToClient, identity.gameObject);

    }

    [TargetRpc]
    private void TRpc_PlayUniqueEffect(NetworkConnection con, GameObject obj)
    {
      if(obj.TryGetComponent(out Rigidbody2D component))
        {
            Jumping(component);
        }
           
    }

    public void Jumping(Rigidbody2D rb)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(transform.up * jumpingPower, ForceMode2D.Impulse);
    }
}
