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
        _text.text = SteamUser.GetSteamID().ToString();
    }
}
