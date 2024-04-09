using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Indicator : Indicator
{
    float height = 0;
    private void Update()
    {
        if(curLinkObj != null)
        {
            transform.position = curLinkObj.transform.position + new Vector3(0,height*0.5f,0);
        }
    }


    private void Tracking()
    {
        Collider2D[] cols = curLinkObj.GetComponentsInChildren<Collider2D>();
       
        foreach (Collider2D co in cols)
        {
            height = Mathf.Max(height, co.bounds.size.y);
        }
    }

    public override void SetLinkObj(GameObject obj)
    {
        base.SetLinkObj(obj);
        Tracking();
    }
}
