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

    public void clicke()
    {
        RpcTextChange();
    }
    
    [ClientRpc(includeOwner = true)]
    public void RpcTextChange()
    {
        _text.text = "test";
    }
}
