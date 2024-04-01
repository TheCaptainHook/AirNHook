using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class DemoMap : MonoBehaviour
{
    public static DemoMap instance;

    public GameObject[] buttonActivatedDoors;


    [Header("Save Data")]
    public string mapId;
    public GameObject playerSpawnPoint;
    public GameObject exitPoint;

    private void Awake()
    {
        instance = this;

    }



}
