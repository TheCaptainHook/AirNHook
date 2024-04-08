using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MousePointerEntity
{
    [SerializeField] protected GameObject curLinkObj;
    protected WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
    public bool isClicking;
    public Vector3 mousePosition;



    protected IEnumerator Co_CheckClicking()
    {
        while (isClicking)
        {
            if (Input.GetMouseButton(0))
            {
                isClicking = true;
            }
            else
            {
                isClicking = false;
                break;
            }
            yield return waitForSeconds;
        }
    }

    public virtual void SetLinkObj(GameObject obj)
    {
        curLinkObj = obj;
    }
}
