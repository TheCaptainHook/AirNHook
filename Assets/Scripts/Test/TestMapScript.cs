
using System.Collections;
using UnityEngine;

public class TestMapScript : MonoBehaviour
{
    public GameObject testPrefab;

    private GameObject curObj;

    private void Update(){
        if(Input.GetKeyDown(KeyCode.P)){
           curObj = Managers.Pooling.D_GetItem(testPrefab);
           curObj.SetActive(true);
           StartCoroutine(TEST());
        }
    }



    IEnumerator TEST(){
        yield return new WaitForSeconds(3);
        if(curObj == null) yield break;
        Managers.Pooling.D_ReleaseToPool(curObj);
    }
    
}
