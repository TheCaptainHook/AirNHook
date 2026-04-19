using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;

public class BlinkingButton_var2_Net : NetworkBehaviour
{
    [SerializeField] public Transform _blueHandel;
    [SerializeField] public Transform _redHandel;
    [SerializeField] private LineRenderer _L_lineRenderer;
    [SerializeField] private LineRenderer _R_lineRenderer;
    private void Active()
    {
        // Animator.SetBool(On, _onOff);
        MapEditor.Instance.CallBlinkingBoxEvent_Red();
        MapEditor.Instance.CallBlinkingBoxEvent_Blue();
    }

    private float _curCooltime = 0f;
    //private bool _server_bool = true; // server activation permission flag
    [SerializeField] private float _maxCooltime;

    public void Init()
    {
          //==L
        _L_lineRenderer.positionCount = 2;
        _L_lineRenderer.SetPosition(0, transform.position);
        _L_lineRenderer.SetPosition(1, _blueHandel.position);
        //==R
        _R_lineRenderer.positionCount = 2;
    }

    [ServerCallback]
    private void Update()
    {
        // if (_curCooltime > 0f && !_server_bool)
        // {
        //     _curCooltime -= Time.deltaTime;
        //     if(_curCooltime < 0f)
        //     {
        //         _server_bool = true;
        //     }
                
        // }
    }

     #region  Air
    [Command(requiresAuthority = false)]
    public void Cmd_Air_Active(uint id,bool rL)
    {
        // if(!_server_bool) return;
   
        _curCooltime = _maxCooltime;
        // _server_bool = false;
        if (id == 99999) return;

        Rpc_Air_Active(id,rL);
    }
    /// <summary>
    /// rL == true : Right -> Change Red
    /// rL == false : Left -> Change Blue
    /// </summary>
    /// <param name="rL"></param>
    [ClientRpc]
    public void Rpc_Air_Active(uint id,bool rL)
    {
        if(rL)
        Debug.Log("Right Air Active");
        else
        {
             Debug.Log("Left Air Active");
            // MoveInDirection(Get_Accessor_Transform(id),rL);
            if(_R_Co != null) StopCoroutine(_R_Co);
            if(_L_Co != null) StopCoroutine(_L_Co);
            _L_Co = StartCoroutine(MoveInDirection_L_Co(Get_Accessor_Transform(id)));
        }
       
        //Active();
    }
    private Coroutine _L_Co;
    private Coroutine _R_Co;
    private IEnumerator MoveInDirection_L_Co(Transform target)
    {
        while(true)
        {
            Vector2 dir = (target.position -_blueHandel.position ).normalized;
            _blueHandel.position += (Vector3)(dir * Time.deltaTime); 
            _blueHandel.right = dir;
            LineRenderer_Update(_L_lineRenderer);
            yield return null;
        }
    }
     
    #endregion
    //LineRenderer Update
    private void LineRenderer_Update(LineRenderer lineRenderer)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, _blueHandel.position);
    }
    private Transform Get_Accessor_Transform(uint id)
    {
        NetworkIdentity identity = NetworkClient.spawned.TryGetValue(id, out NetworkIdentity foundIdentity) ? foundIdentity : null;
        if (identity != null)
        {
            return identity.transform;
        }
        else
        {
            Debug.LogError($"No NetworkIdentity found for ID {id}");
            return null;
        }
    }


    [Command(requiresAuthority = false)]
    public void Cmd_Clean()
    {
        Rpc_Clean();
    }
    [ClientRpc]
    private void Rpc_Clean()
    {
        Clean();
    }

    public void Clean()
    {
        _curCooltime = 0f;
        if(_R_Co != null) StopCoroutine(_R_Co);
        if(_L_Co != null) StopCoroutine(_L_Co);


        // _server_bool = true;
    }
}
