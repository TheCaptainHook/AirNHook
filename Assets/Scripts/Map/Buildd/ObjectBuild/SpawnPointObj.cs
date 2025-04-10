using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SpawnPointObj : BuildBase
{
    public bool onSpawn;
    NetworkStartPosition networkStartPosition;


    private void Awake()
    {
        onSpawn = true;
        networkStartPosition = GetComponent<NetworkStartPosition>();
    }

    public void EnableNetWorkStartPosition()
    {
        Debug.Log(Managers.Network.startPos.Count);
        networkStartPosition.enabled = false;
        onSpawn = false;
    }

}
