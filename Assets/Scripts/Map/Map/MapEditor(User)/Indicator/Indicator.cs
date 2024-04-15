using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Indicator : MousePointerEntity
{
    [SerializeField] protected GameObject curLinkObj;
    protected WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
    public bool isClicking;
    public Vector3 mousePosition;
    public bool linked;
    private bool onEnterPointer;

    protected Color orgColor;
    protected SpriteRenderer[] spriteRenderers;

    public virtual void SetLinkObj(GameObject obj)
    {
        curLinkObj = obj;
        linked = true;
    }

    protected void SpriteAlphaChange(int num)
    {
        if (num == 0) // point down
        {
            Color alphaCol = new Color(orgColor.r, orgColor.g, orgColor.b, 0.2f);
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = alphaCol;
            }
        }
        else //point up
        {

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = orgColor;
            }
        }


    }

}
