using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class BuildBase : BuildObj
{

    bool editorMod_isActive;
    bool placeable;
    bool isCollision;
    LayerMask mask;

    protected Color orgColor;

    RaycastHit2D hit;


    //check

    //protected virtual void BuildCheck()
    //{
    //    if (editorMod_isActive)
    //    {
    //        hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, mask);

    //        if (hit.collider != null)
    //        {
    //            placeable = true;
    //            mainSprite.color = orgColor;
    //        }
    //        else
    //        {
    //            placeable = false;
    //            mainSprite.color = Color.red;
    //        }
    //    }
    //}

}

