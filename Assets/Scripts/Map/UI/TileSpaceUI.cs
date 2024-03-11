using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpaceUI : MonoBehaviour
{
    [SerializeField] Transform content;

    public GameObject tileSpaceUIItem;




    private void Start()
    {
        LoadAllTile();
    }



    void LoadAllTile()
    {
        GameObject[] tileObj = Resources.LoadAll<GameObject>("Prefabs/Map/Tile");
        for(int i = 0; i< tileObj.Length; i++)
        {
            GameObject item = Instantiate(tileSpaceUIItem);
            item.transform.SetParent(content);
            item.GetComponent<Interaction_BuildItem>().Init(tileObj[i]);
         
        }
    }

}
