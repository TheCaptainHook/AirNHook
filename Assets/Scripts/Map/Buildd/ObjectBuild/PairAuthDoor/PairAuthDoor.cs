using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class PairAuthDoor : BuildObj, IInteractable
{
    [CustomHeader("PairAuthDoor")]
    [SerializeField] Transform _authTr;
    public PairAuthDoor_Eye _leftEye;
    public PairAuthDoor_Eye _rightEye;

    private PairAuthDoor_Net _net;
    private PairAuthDoor_Net Net { get { _net ??= GetComponent<PairAuthDoor_Net>(); return _net; } }

    [ReadOnly]
    public bool _onProgress;

    [Header("Line")]
    [SerializeField] Transform _lineContainerTr;

    [Header("Panel")]
    [SerializeField] PairAuthDoor_Panel _panel;
    [Header("Auth Scan Shader")]
    [SerializeField] Material _scanMat;
    [SerializeField] Sprite _hook_Sprite;
    [SerializeField] Sprite _air_Sprite;

    private WaitForSeconds _zeroDotOne = new WaitForSeconds(0.1f);
    void Awake()
    {
        _lrPool.Enqueue(CreateLine());
        _lrPool.Enqueue(CreateLine());
    }
   
    public IEnumerator AuthCoroutine()
    {
        _onProgress = true;
        StartCoroutine(_leftEye.ScaningCoroutine());
        StartCoroutine(_rightEye.ScaningCoroutine());
        yield return new WaitForSeconds(1f);
        //Check Effect
        yield return Auth_DrawLineCo();
        //Check Effect
        yield return new WaitForSeconds(1f);
        Destory_Dummy_ScanShader();
        ClearAllLine();
        //Panel
        Auth_Panel();
        //Panel
        yield return new WaitForSeconds(1f);

        //------------ Auth Check(Server)
        if (NetworkServer.active)
        {
            if (_panel.AuthCheck())
            {
                Debug.Log("Auth Complete");
                Net._authSuccess = true;
            }
            else
            {
                Debug.Log("Auth Fail");       
            }
        }
        //------------ Auth Check(Server)

        _panel.PanelReset();
        PlayerDic_Clear();
        _onProgress = false;

        if(NetworkServer.active) Net._onProgress = false;
    }

    
    private Dictionary<uint, LineRenderer> _playerLinesDic = new Dictionary<uint, LineRenderer>(2);

    #region Line
    Queue<LineRenderer> _lrPool = new Queue<LineRenderer>();
    private LineRenderer GetLine()
    {
        if (_lrPool.Count > 0)
        {
            var lr = _lrPool.Dequeue();
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
        lr.startColor = Color.red;
        lr.endColor = Color.red;

        go.transform.SetParent(_lineContainerTr);
        lr.sortingLayerName = "ForeGround";
        lr.sortingOrder = 6;
        return lr;
    }
    
    private void SetLine(LineRenderer lr, Vector3 endPos)
    {
        lr.positionCount = 2;
        lr.SetPosition(0, _authTr.position);
        lr.SetPosition(1, endPos);
    }
    private IEnumerator Auth_DrawLineCo()
    {
        foreach (var key in _playerLinesDic.Keys)
        {
            if (NetworkClient.spawned.TryGetValue(key, out var netIdentity))
            {
                if (netIdentity.TryGetComponent(out PlayerSM player))
                {
                    SetLine(_playerLinesDic[key], player.transform.position);
                    yield return _zeroDotOne;
                    Create_Dummy_ScanShader(player);
                }
            }
            yield return _zeroDotOne;
        }
    }

    private List<GameObject> _dummyScanShaderList = new List<GameObject>();
    private void Create_Dummy_ScanShader(PlayerSM player)
    {
        var go = new GameObject("PairAuthDoor_ScanShader");
        var sr = go.AddComponent<SpriteRenderer>();
        go.transform.SetParent(player.transform);
        go.transform.localPosition = Vector2.zero;

        if (player.characterType == CharacterType.Air)
        {
            go.transform.localScale = Vector2.one * 0.5f;
            sr.sprite = _air_Sprite;
        }
        else if (player.characterType == CharacterType.Hook)
        {
            go.transform.localScale = Vector2.one * 0.2f;
            sr.sprite = _hook_Sprite;
        }

        sr.material = _scanMat;
        sr.sortingLayerName = "PlayerBack";
        sr.sortingOrder = 8;

        _dummyScanShaderList.Add(go);
    }
    private void Destory_Dummy_ScanShader()
    {
        foreach(var item in _dummyScanShaderList)
        {
            Destroy(item);
        }
        _dummyScanShaderList.Clear();
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
    private void PlayerDic_Clear()
    {
        foreach (var id in _playerLinesDic.Keys)
        {
            if (NetworkClient.spawned.TryGetValue(id, out var netIdentity))
            {
                if (netIdentity.TryGetComponent(out PlayerSM player))
                {
                    player.FreezePlayerState(false);
                }
            }
        }
        _playerLinesDic.Clear();
    }
    #endregion


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
                    player.FreezePlayerState(true);
                }
            }
        }
    }

    private void Auth_Panel()
    {
        foreach(var key in _playerLinesDic.Keys)
        {
            if (NetworkClient.spawned.TryGetValue(key, out var netIdentity))
            {
                if (netIdentity.TryGetComponent(out PlayerSM player))
                {
                    _panel.SetPanel(player);
                }
            }
        }
    }


    #region Interactable
    public ObjectTypeEnum _objectType = ObjectTypeEnum.Interaction;
    private UI_Base _E_Btn;
    [SerializeField] float _BtnOffset;

    public void Interaction(Transform accessor = null)
    {
        if(!Net._authSuccess && !_onProgress)
        {
            HideEButton();
            Net.Cmd_Auth();
        }
    }
    public bool CanInteract()
    {
        return true;
    }
    public bool Interacting(bool value, GameObject player)
    {
        return true;
    }
    public ObjectTypeEnum GetObjectType()
    {
        return _objectType;
    }
    public void ShowEButton()
    {
        if (!Net._authSuccess && !_onProgress)
        {
            _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
            _E_Btn.transform.position = transform.position + (transform.up * _BtnOffset);
        }
    }
    public void HideEButton()
    {
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }
        #endregion
    }

