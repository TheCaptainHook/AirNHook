using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Indicator : Indicator
{

    private Vector3 offset;

    //float height = 0;
    private void Update()
    {
        if (curLinkObj != null)
        {
   
            transform.position = curLinkObj.transform.position + offset;
        }
    }



    public override void SetLinkObj(GameObject obj)
    {
        base.SetLinkObj(obj);
        offset = curLinkObj.GetComponent<BuildObj>().offset;
        transform.position = obj.transform.position + offset;

    
    }


}
