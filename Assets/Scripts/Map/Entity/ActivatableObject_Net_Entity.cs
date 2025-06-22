using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using System;

// [RequireComponent(typeof(NetworkIdentity))]
public class ActivatableObject_Net_Entity : NetworkBehaviour
{
    private Collider2D col;
    protected Collider2D Col
    {
        get
        {
            col ??= GetComponent<Collider2D>();
            return col;
        }
    }
    private Rigidbody2D rb;
    protected Rigidbody2D Rb
    {
        get
        {
            rb ??= GetComponent<Rigidbody2D>();
            return rb;
        }
    }
    
    private ActivatableObjectEntity entity;
    protected ActivatableObjectEntity Main
    {
        get
        {
            entity ??= GetComponent<ActivatableObjectEntity>();
            return entity;
        }
    }
    protected ButtonActivatableObjectStruct data;

    #region Init Sync
    [ReadOnly]
    public bool onSync;
    [ReadOnly]
    [SyncVar] public bool onActive;
    [Server]
    public virtual void Server_InitSync()
    {
        StartCoroutine(AllClientReadyChecker_Co(() => { Rpc_InitSync(Main.ButtonActivatedObjectStruct); }));
    }
    [ClientRpc]
    protected virtual void Rpc_InitSync(ButtonActivatableObjectStruct data)
    {
        if (onSync) return;

        SetData(data);

        if (onActive) Active();

        onSync = true;

    }
    protected virtual void SetData(ButtonActivatableObjectStruct data)
    {
        transform.position = data.position;
        transform.rotation = data.quaternion;
        transform.localScale = data.scale;
        this.data = data;

        if (data.indicator == INDICATOR.TEXT)
            Main.Create_Indicator_var_1();
        else if (data.indicator == INDICATOR.MARK)
            Main.Create_Indicator_var_2();

    }

    [Command(requiresAuthority = false)]
    public void Cmd_InitSync()
    {
        Server_InitSync();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!onSync) Cmd_InitSync();
    }




    IEnumerator AllClientReadyChecker_Co(Action action)
    {
        while (true)
        {
            int connectionClinetAmount = NetworkServer.connections.Count;
            int num = 0;
            foreach (var conn in NetworkServer.connections.Values)
            {
                if (conn.isReady) num++;
            }

            if (connectionClinetAmount == num) break;

            yield return null;
        }
        action?.Invoke();

    }
    #endregion

    #region Server onActive Change
    [Server]
    public virtual void Server_ChangeOnActive(bool onOff)
    {
        onActive = onOff;
        Rpc_ChangeOnActive(onActive);
    }
    [ClientRpc]
    protected virtual void Rpc_ChangeOnActive(bool onOff)
    {
        if (onOff)
        {
            Active();
        }
        else
        {
            Deactive();
        }
    }
    #endregion

    #region  Active,Deactive
    protected virtual void Active()
    {

    }
    protected virtual void Deactive()
    {

    }
    #endregion



    #region  Play Unique Effect
    [Server]
    public virtual void Server_PlayUniqueEffect(uint id)
    {

    }

    #endregion
}

///
/// 1. Main의 Activation, Deactivated 는 서버에서만 호출됨
/// 2. 서버에서 Server_ChangeOnActive 호출 하면 syncvar onActive를 업데이트하고
///     각 클라이언트에 Active 또는 Deactie 실행
/// Ex) BridgeBox
