using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;

public class TestRoomCode : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    
    void Start()
    {
        _text.text = Base62Converter.ToBase62(Managers.Network.steamLobby.currentLobbyID.m_SteamID);
        //_text.text = SteamUser.GetSteamID().ToString();
    }
}
