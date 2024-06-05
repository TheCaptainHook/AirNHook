using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectScrollView : MonoBehaviour
{
    
    [SerializeField]Transform content;
    [SerializeField] GameObject stageSelectItem;



    List<MainMapItem> mainMapItems;

    MainMapItem curMainMapItem;


    public void SetData(Map[] maps)
    {
  
        for (int i = 0; i < maps.Length; i++)
        {
            MainMapItem item = Instantiate(stageSelectItem, content).GetComponent<MainMapItem>();
            mainMapItems.Add(item);
            item.SetData(maps[i],i,this);
        }
    }



    public void Reset()
    {
        
    }
}
