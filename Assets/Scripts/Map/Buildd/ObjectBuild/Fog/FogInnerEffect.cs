using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogInnerEffect : MonoBehaviour,IPooling
{
   public void D_ReleaseToPool(){
    StartCoroutine(ReleaseCoroutine());
   }

    

   IEnumerator ReleaseCoroutine(){
    yield return new WaitForSeconds(2);
    try{
        Managers.Pooling.D_ReleaseToPool(this.gameObject);
    }catch(Exception ex){
        Debug.Log(ex);
    }
    
   }

    
    public void N_ReleaseToPool(){}
}
