using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingBox_Net : NetworkBehaviour
{
    private CrumblingBox Main => GetComponent<CrumblingBox>();
    [SyncVar(hook =nameof(Hook_OnChangeCrumbring_Index))]public int crumbling_Index;

    WaitForSeconds waitForSeconds;
    private void Awake()
    {
        waitForSeconds = new WaitForSeconds(3);
    }


    private void Hook_OnChangeCrumbring_Index(int old,int newVal)
    {
        //crumbling
        Main.Crumbling(newVal);  
    }



    #region Init Sync
    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(Main.ObjectData);
    }
    [ClientRpc]
    private void Rpc_InitSync(ObjectData data)
    {
        if (onSync) return;
        transform.position = data.position;
        onSync = true;
    }

    #endregion



    [Server]
    public void Server_SetCrumbringIndex()
    {
        this.crumbling_Index += 1;
        if (recoverCoroutine != null) StopCoroutine(recoverCoroutine);
        recoverCoroutine = StartCoroutine(Recover());


        if (crumbling_Index >= 3) StartCoroutine(Reset());

    }


    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(3);
        crumbling_Index = 0;
    }

    Coroutine recoverCoroutine;
    private IEnumerator Recover()
    {
        while(1 <= crumbling_Index && crumbling_Index <=2)
        {
            yield return waitForSeconds;
            crumbling_Index -= 1;
        }
    }
}
