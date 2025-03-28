using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingBox_Net : NetworkBehaviour
{
    private CrumblingBox Main => GetComponent<CrumblingBox>();

    [SyncVar(hook =nameof(Hook_OnChangeCrumbring_Index))]public int crumbling_Index;

    private void Hook_OnChangeCrumbring_Index(int old,int newVal)
    {
        //crumbling
        Main.Crumbling(newVal);
       
    }



    [Server]
    public void Server_SetCrumbringIndex()
    {
        this.crumbling_Index += 1;
        if (crumbling_Index >= 3) StartCoroutine(Recover());

    }



    private IEnumerator Recover()
    {
        yield return new WaitForSeconds(3);
        crumbling_Index = 0;
    }
}
