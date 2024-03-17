using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class DemoMap : MonoBehaviour
{
    public static DemoMap instance;

    [Header("Map Info")]
    public string mapId;
    public GameObject playerSpawnPoint;
    public GameObject exitPoint;

    public GameObject[] buttonActivatedDoors;

    [Header("Condition")]
    public int stageClearCondition_keyAmount;
    public int curKeyAmount;


    public event Action OnGetKey;


    private void Awake()
    {
        instance = this;

        OnGetKey += Addkey;
    }

    public void CallOnGetKey()
    {
        OnGetKey?.Invoke();
    }


    void Addkey()
    {
        curKeyAmount++;

        if(curKeyAmount == stageClearCondition_keyAmount)
        {
            Debug.Log("Clear Stage");
        }
    }




}
