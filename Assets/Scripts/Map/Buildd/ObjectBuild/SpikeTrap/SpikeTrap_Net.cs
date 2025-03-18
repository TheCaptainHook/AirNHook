using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap_Net : NetworkBehaviour  
{
    private SpikeTrap spikeTrap;
    private SpikeTrap Main { get { if(spikeTrap == null)spikeTrap = GetComponent<SpikeTrap>(); return spikeTrap;} }

    public float attackCoolTime;
    [ReadOnly]
    public float curAttackCooltime;
    public float attackStartTime;

    #region Init Sync
    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        attackStartTime = Main.ObjectData.attackStartTime;
        attackCoolTime = Main.ObjectData.attackCooldown;
        Rpc_InitSync(Main.ObjectData);
    }
    [ClientRpc]
    private void Rpc_InitSync(ObjectData data)
    {
        if (onSync) return;
        Main.ObjectData = data;

        transform.position = data.position;
        transform.rotation = data.quaternion;
        transform.localScale = data.scale;

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
        if (!onSync) Cmd_InitSync();
    }
    #endregion


    private void Update()
    {
        if (!isServer || !onSync) return;
        if (attackStartTime > 0) { attackStartTime -= Time.deltaTime; return; }

        if(curAttackCooltime <= 0)
        {
            curAttackCooltime = attackCoolTime;
            Attack();
        }
        else
        {
            curAttackCooltime -= Time.deltaTime;
        }
    }

    #region Main
    [ClientRpc]
    private void Attack()
    {
        Main.Attack();
    }
    #endregion
}
