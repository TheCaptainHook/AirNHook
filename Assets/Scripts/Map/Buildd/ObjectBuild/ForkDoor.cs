using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;

using System.Threading.Tasks;

public class ForkDoor : BuildObj
{
    public enum EnterAndExit
    {
        Enter,
        Exit
    }

    public EnterAndExit _EE;

    [SerializeField] AbsencePanel absencePanel;
    private int curPlayerInDoor;

    private Vector3Int _CreateOffset = new Vector3Int(1000,1000);

    #region Components
    private PlaceMentSystem placeMentSystem;
    #endregion

    //saveData
    private Vector3 _EnterPot;
    private Vector3 _ExitPot;
    public string linkMapId;

    private void Start()
    {
        placeMentSystem = MapEditor.Instance.placeMentSystem;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            absencePanel.Enter(collision.gameObject);
            curPlayerInDoor++;
            if(curPlayerInDoor >= 2)
            {
                //Move Link Map Position _ExitPot move
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            absencePanel.Exit(collision.gameObject);
            curPlayerInDoor--;
        }
    }


    #region Get,Set
    public ForkDoorData GetData()
    {
        ForkDoorData data = new ForkDoorData(id,transform.position,linkMapId);
        return data;
    }

    public void SetData(ForkDoorData data)
    {
        transform.position = data.position;
        _EnterPot = data.position;
        id = data.id;
        linkMapId = data.linkMapId;

        //Create ForkMap
    }

    public void SetExitPot(Vector2 pot)
    {
        _ExitPot = pot;
    }


    #endregion



    async void CreateForkMap(string linkMapId)
    {

        TextAsset textAsset = Resources.Load<TextAsset>($"MapDat/Fork/{linkMapId}");
        Map map = JsonUtility.FromJson<Map>(textAsset.text);

        await Task.Run(() => { CreateTile(map); });
        await Task.Run(() => { CreateObj(map); });
    }

 



    

    private void CreateTile(Map curMap)
    {
        foreach (TileData data in curMap.mapTileDataList)
        {
            MapDataStruct mapDataStruct = Managers.Data.mapData.mapTileDataDictionary[data.id];
            placeMentSystem.floorTileMap.SetTile(data.position + _CreateOffset, Resources.Load<TileBase>(mapDataStruct.path));
            placeMentSystem.tileDic[data.position] = data.id;
        }

        foreach (TileData data in curMap.mapHalfTileDataList)
        {
            MapDataStruct mapDataStruct = Managers.Data.mapData.mapTileDataDictionary[data.id];
            placeMentSystem.halfTileMap.SetTile(data.position + _CreateOffset, Resources.Load<TileBase>(mapDataStruct.path));
            placeMentSystem.tileDic[data.position] = data.id;
        }
        foreach (TileData data in curMap.mapBackgroundTileDataList)
        {
            MapDataStruct mapDataStruct = Managers.Data.mapData.mapTileDataDictionary[data.id];
            placeMentSystem.backgroundTileMap.SetTile(data.position + _CreateOffset, Resources.Load<TileBase>(mapDataStruct.path));
            placeMentSystem.tileDic[data.position] = data.id;
        }

    }

    private void CreateObj(Map curMap)
    {
        foreach (ObjectData data in curMap.mapObjectDataList)
        {
            if (Managers.Data.mapData.mapSceneDataDictionary.ContainsKey(data.id))
            {
                MapDataStruct mapDataStruct = Managers.Data.mapData.mapSceneDataDictionary[data.id];
                if (Managers.Game.CurrentState != GameState.Editor && (data.id == 1001 || data.id == 1002))
                {
                    Managers.Stage.CmdBatchObject(mapDataStruct.name, data);
                }
             

            }
            else if (Managers.Data.mapData.mapBackgroundDataDictionary.ContainsKey(data.id))
            {
                MapDataStruct mapDataStruct = Managers.Data.mapData.mapBackgroundDataDictionary[data.id];

                Create(transform, mapDataStruct, data);

            }
            else if (Managers.Data.mapData.mapOtherDataDictionary.ContainsKey(data.id))
            {
                MapDataStruct mapDataStruct = Managers.Data.mapData.mapOtherDataDictionary[data.id];

                Create(transform, mapDataStruct, data);

            }
            else
            {
                MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
                if (Managers.Game.CurrentState != GameState.Editor &&
                    (
                    data.id == 307 ||
                    data.id == 300 ||
                    data.id == 311 ||
                    data.id == 313 ||
                    data.id == 315 ||
                    data.id == 317 ||
                    data.id == 318 ||
                    data.id == 319
                    ))
                {
                    Managers.Stage.CmdBatchObject(mapDataStruct.name, data);
                }
              
            }

        }
    }
    private void CreateBtnActiveDoor(Map map)
    {

    }
    private void CreateBtnActiveObj(Map map)
    {

    }

    private void CreateDialouge(Map map)
    {

    }




    void Create(Transform transform, MapDataStruct mapDataStruct, ObjectData data)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
        obj.GetComponent<BuildObj>().SetData(data);

        obj.transform.SetParent(transform);
    }
}
