using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TileSpaceUI : MonoBehaviour
{
    [SerializeField] Transform content;

    public GameObject tileSpaceUIItem;
    public Button cancel;


    private void Awake()
    {
        cancel.onClick.AddListener(Cancel);
    }
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

    void Cancel()
    {
        MapEditor.Instance.CurBuildObj = null;
    }
}
