using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GoogleSheetsToUnity;
using GoogleSheetsToUnity.ThirdPary;
public class GoogleSheetsUtil : MonoBehaviour
{
    public string associatedSheet;
    public string associatedWorksheet;

    Dictionary<string, List<GSTU_Cell>> dataDir = new Dictionary<string, List<GSTU_Cell>>();

    public List<string> keyList = new List<string>();

    private void Awake()
    {
        associatedSheet = "1Ttdyeb0uOoBoFHTAK5T5PX0ArpqnMfn0vnEC_XA1ywc";
        associatedWorksheet = "Sheet1";

    }

    private void Start()
    {
        LoadTileData(GetData);
        
    }


    public void LoadTileData(UnityAction<GstuSpreadSheet> callBack, bool mergedCells = false) //병합된 셀 고려
    {
        SpreadsheetManager.Read(new GSTU_Search(associatedSheet, associatedWorksheet), callBack, mergedCells);
    }


    void GetData(GstuSpreadSheet gs)
    {
        keyList = GetKeys(gs);

        for (int i = 0; i < keyList.Count; i++)
        {
            List<GSTU_Cell> list = gs.rows[keyList[i]];
            dataDir.Add(keyList[i], list);
        }

        GetTileData();

    }

    void GetTileData()
    {
        foreach (KeyValuePair<string, List<GSTU_Cell>> kv in dataDir)
        {
            MapEditor.Instance.mapTileDataList.Add(GetData(kv.Value));

        }
    }


    TileData GetData(List<GSTU_Cell> list)
    {
        TileType tileType = GetTileType(list[1].value);
        Vector2 coord = GetCoord(list[2].value);
        string path = list[3].value;
        return new TileData(tileType, 0, coord,path);

    }

    TileType GetTileType(string type)
    {
        switch (type)
        {
            case "TileType.Floor":
                return TileType.Floor;
        }

        return TileType.None;
    }

    Vector2 GetCoord(string coord)
    {
        string co = coord.Replace(",", "");
        int x = int.Parse(co[0].ToString());
        int y = int.Parse(co[1].ToString());

        return new Vector2(x, y);

    }

    List<string> GetKeys(GstuSpreadSheet gs)
    {
        List<GSTU_Cell> cells = gs.columns["A"];

        List<string> list = new List<string>();

        for (int i = 1; i < cells.Count; i++)
        {
            list.Add(cells[i].value);
        }

        return list;
    }


}

