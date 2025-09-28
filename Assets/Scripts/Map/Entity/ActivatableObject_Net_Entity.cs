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
    [HideInInspector]
    public ButtonActivatableObjectStruct data;

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

        if (data.indicatorStruct.indicator == INDICATOR.TEXT)
            Create_Indicator_var_1();
        else if (data.indicatorStruct.indicator == INDICATOR.MARK)
            Create_Indicator_var_2();
        else if (data.indicatorStruct.indicator == INDICATOR.BOTH)
        {
            Create_Indicator_var_1();
            Create_Indicator_var_2();
        }

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
        Debug.Log("Debug Active change");
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
    /// <summary>
    /// ref) Portal, JumpingPad
    /// </summary>
    /// <param name="id"></param>
    [Server]
    public virtual void Server_PlayUniqueEffect(uint id)
    {

    }

    #endregion


    #region  Indicator
    [ClientRpc]
    public virtual void ApplyActive_Sync_var1(int curActiveAmount)
    {
        indicator_var1.SetApplyActive(curActiveAmount);
    }
    /// </summary>
    /// <param name="id">Network ID</param>
    /// <param name="inc">[-1] : deactive, [1] : active </param>
    [ClientRpc]
    public virtual void ApplyActive_Sync_var2(uint id, int curActiveBtn, int inc)
    {
        indicator_var2.SetApplyActive(id, curActiveBtn, inc);
    }

    private ActivatableObject_Indicator_var1 indicator_var1;
    private void Create_Indicator_var_1()
    {
        //var indicator = Resources.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_1_Path);
        var indicator = ResourceManager.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_1_Path);
        indicator_var1 = Instantiate(indicator).GetComponent<ActivatableObject_Indicator_var1>();
        indicator_var1.Setting(Main, this);

    }
    private ActivatableObject_Indicator_var2 indicator_var2;
    private void Create_Indicator_var_2()
    {
        var indicator = ResourceManager.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_2_Path);
        //var indicator = Resources.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_2_Path);
        indicator_var2 = Instantiate(indicator).GetComponent<ActivatableObject_Indicator_var2>();
        indicator_var2.Setting(Main, this);

    }

    [Server]
    public void Server_Indicator_var_2_PathChacking(uint targetID)
    {
        StartCoroutine(AllClientReadyChecker_Co(()=>Rpc_Indicator_var_2_PathChacking(targetID)));

    }
    [ClientRpc]
    private void Rpc_Indicator_var_2_PathChacking(uint targetID)
    {
        Debug.Log($"[3] RPC INdicator_var_2 PathChaking, netid : {targetID}");
        indicator_var2.PathChacking(targetID);
    }

    

    #endregion
}

///
/// 1. Main의 Activation, Deactivated 는 서버에서만 호출됨
/// 2. 서버에서 Server_ChangeOnActive 호출 하면 syncvar onActive를 업데이트하고
///     각 클라이언트에 Active 또는 Deactie 실행
/// Ex) BridgeBox
