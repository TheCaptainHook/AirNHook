using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class DemoMap : MonoBehaviour
{
    public static DemoMap instance;

    public GameObject[] buttonActivatedDoors;


    [Header("Save Data")]
    public int width = 31;
    public int height = 20;
    //cellSize
    public string mapId;
    public GameObject playerSpawnPoint;
    public GameObject exitPoint;
    public int stageClearCondition_keyAmount;
    public int curKeyAmount;

    List<ObjectData> mapTileDataList = new List<ObjectData>(); 
    List<ButtonActivatedDoor> buttonActivate = new List<ButtonActivatedDoor>(); //구조체로 데이터화
    
    
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
