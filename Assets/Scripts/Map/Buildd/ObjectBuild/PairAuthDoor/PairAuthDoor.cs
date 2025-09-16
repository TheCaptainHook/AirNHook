using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class PairAuthDoor : MonoBehaviour
{
    // public AirSM _air;
    // public HookSM _hook;

    [SerializeField] Transform _authTr;
    public PairAuthDoor_Eye _leftEye;
    public PairAuthDoor_Eye _rightEye;
    [ReadOnly]
    public bool _onProgress;
    /**
    1. Start Detection(Server) -> Rpc_StartDetection(Eye Detect Effect)
    2.   1). AuthComplete 
         2). AuthFail
         3). AuthCancel
    
    **/

    [Header("Line")]
    [SerializeField] Transform _lineContainerTr;

    [Header("Panel")]
    [SerializeField] PairAuthDoor_Panel _panel;
    void Awake()
    {
        _lrPool.Enqueue(CreateLine());
        _lrPool.Enqueue(CreateLine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !_onProgress)
        {
            StartCoroutine(AuthCoroutine());
        }
    }

    private IEnumerator AuthCoroutine()
    {
        _onProgress = true;
        StartCoroutine(_leftEye.DetectCoroutine());
        StartCoroutine(_rightEye.DetectCoroutine());
        yield return new WaitForSeconds(2f);
       
        //------------ Clear
        _leftEye.MeshClear();
        _rightEye.MeshClear();
        ClearAllLine();
        //------------ Clear

        //------------ Auth Check(Server)
        if (NetworkServer.active)
        {
            if (_panel.AuthCheck())
            {
                Debug.Log("Auth Complete");
            }
            else
            {
                Debug.Log("Auth Fail");
            }
        }
        //------------ Auth Check
        PlayerDic_Clear();
        _onProgress = false;
    }

    
    private Dictionary<uint, LineRenderer> _playerLinesDic = new Dictionary<uint, LineRenderer>(2);

    #region Line
    Queue<LineRenderer> _lrPool = new Queue<LineRenderer>();
    private LineRenderer GetLine()
    {
        if (_lrPool.Count > 0)
        {
            var lr = _lrPool.Dequeue();
            // lr.gameObject.SetActive(true);
            return lr;
        }
        else return null;
    }
    private LineRenderer CreateLine()
    {
        var go = new GameObject("PairAuthDoor_Line");
        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 0;
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        go.transform.SetParent(_lineContainerTr);
        lr.sortingLayerName = "ForeGround";
        lr.sortingOrder = 6;
        // lr.material = Resources.Load<Material>("Material/Line_Mat");
        return lr;
    }
    
    private void SetLine(LineRenderer lr, Vector3 endPos)
    {
        lr.positionCount = 2;
        lr.SetPosition(0, _authTr.position);
        lr.SetPosition(1, endPos);
    }
    #endregion

    #region  Clear
    private void ClearAllLine()
    {
        foreach (var lr in _playerLinesDic.Values)
        {
            ClearLine(lr);
        }
    }
    private void ClearLine(LineRenderer lr)
    {
        lr.positionCount = 0;
        _lrPool.Enqueue(lr);
    }

    #endregion
    private void PlayerDic_Clear()
    {
        foreach (var id in _playerLinesDic.Keys)
        {
            if (NetworkClient.spawned.TryGetValue(id, out var netIdentity))
            {
                if (netIdentity.TryGetComponent(out PlayerSM player))
                {
                    player.FreezePlayerState(false);
                    _panel.SetPanel(player, false);
                }
            }
        }
        _playerLinesDic.Clear();
    }
    public void DetectPlayer(Collider2D col)
    {
        var id = col.TryGetComponent(out NetworkIdentity netId) ? netId.netId : 9999;
        if (id != 9999)
        {
            if (!_playerLinesDic.ContainsKey(id))
            {
                var lr = GetLine();
                if (lr != null)
                {
                    _playerLinesDic.Add(id, lr);
                }

                if (col.TryGetComponent(out PlayerSM player))
                {
                    Debug.Log("asdasd 11");
                    player.FreezePlayerState(true);
                    //Set Panel 
                    _panel.SetPanel(player);
                    //Set Panel 

                }
            }

            SetLine(_playerLinesDic[id], col.transform.position);
        }
    }

}
    


    // (netId,LineRenderer)


