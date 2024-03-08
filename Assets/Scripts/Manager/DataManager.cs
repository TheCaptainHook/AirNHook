using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheet;
using UGS;
using GoogleSheet.Core.Type;
using GoogleSheet.Type;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private void Awake()
    {
        if(Instance != null) { Destroy(gameObject); }
        else { Instance = this; }

    }

    public void MapDataLoad()
    {
        UnityGoogleSheet.LoadFromGoogle<int, MapData.Data>((list, map) =>
        {
            Debug.Log("Load");
        }, true);
    }

}
