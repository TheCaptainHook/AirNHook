using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheet;
using UGS;



public class TestLoad :MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityGoogleSheet.Load<DefaultTable.Data>();

        foreach(var value in DefaultTable.Data.DataList)
        {
            Debug.Log(value.intValue);
        }

    }






}
