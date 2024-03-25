using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnTest : NetworkBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _text;

    private void Start()
    {
        _button.onClick.AddListener(clicke);
    }

    // Command를 통해 서버에 명령.
    [Command(requiresAuthority = false)]
    public void clicke()
    {
        RpcTextChange();
    }
    
    // Command를 통해 불러진 ClientRpc, 모든 클라이언트들에게 Call.
    [ClientRpc]
    public void RpcTextChange()
    {
        _text.text = "test";
    }
}
