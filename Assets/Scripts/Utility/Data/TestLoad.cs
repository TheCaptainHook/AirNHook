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
        

    }


    void Test()
    {
        var newData = MapData.Data.DataMap["TestMap"];
        newData.PlayerSpawnPot = new Vector2(5, 5);

        
    }
    
    
    

}

