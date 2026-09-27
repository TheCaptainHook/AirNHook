using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : InteractableObjectEntity
{
    private Key_Net Net => GetComponent<Key_Net>();



    public bool _is_Looby_Key = false;

    public void Lobby_UseKey()
    {
        _is_Looby_Key = true;
    }

    public override void Clean()
    {
        // base.Clean();
        _is_Looby_Key = false;
    }
}
