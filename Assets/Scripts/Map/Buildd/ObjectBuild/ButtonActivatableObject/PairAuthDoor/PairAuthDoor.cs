using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PairAuthDoor : MonoBehaviour
{
    public AirSM _air;
    public HookSM _hook;

    public PairAuthDoor_Eye _leftEye;
    public PairAuthDoor_Eye _rightEye;
    /**
    1. Start Detection(Server) -> Rpc_StartDetection(Eye Detect Effect)
    2.   1). AuthComplete 
         2). AuthFail
         3). AuthCancel
    
    **/

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(AuthCoroutine());
        }
    }
    public void AuthPlayer(AirSM air, HookSM hook)
    {
        _air = air;
        _hook = hook;
    }


    private IEnumerator AuthCoroutine()
    {
        StartCoroutine(_leftEye.DetectCoroutine());
        StartCoroutine(_rightEye.DetectCoroutine());
        yield return new WaitForSeconds(1.5f);

        Debug.Log($"air : {_air}, hook : {_hook}");

        if (_air != null && _hook != null)
        {
            Debug.Log("Auth Complete");
            //Open Door
        }
        else
        {
            Debug.Log("Auth Fail");
            //Fail Effect
        }
        _leftEye.MeshClear();
        _rightEye.MeshClear();
    }

}
