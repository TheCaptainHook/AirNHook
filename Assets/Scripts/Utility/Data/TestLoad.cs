using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheet;
using UGS;
using GoogleSheet.Core.Type;
using GoogleSheet.Type;

public class TestLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DataManager.Instance.MapDataLoad();
        //foreach (var value in MapData.Data.DataList)
        //{
        //    MapEditor.Instance.mapTileDataList.Add(new TileData(value.Type, value.ID, value.Position, value.Path));
        //}
    }
}

