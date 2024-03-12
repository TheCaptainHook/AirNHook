using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class BuildBase : MonoBehaviour
{

    [SerializeField] bool isActive;
    [SerializeField] bool placeable;
    [SerializeField] bool isCollision;
    [SerializeField] LayerMask mask;

    [SerializeField] SpriteRenderer mainSprite;
    Color orgColor;

    RaycastHit2D hit;


    //check

    protected virtual void BuildCheck()
    {
        if (isActive)
        {
            hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, mask);

            if (hit.collider != null)
            {
                placeable = true;
                mainSprite.color = orgColor;
                Debug.Log(hit.collider.gameObject.GetComponent<BuildObj>().tileType);
            }
            else
            {
                placeable = false;
                mainSprite.color = Color.red;
            }
        }
    }

    bool CheckArea()
    {
        float x = mainSprite.transform.localScale.x / 2;

        if (transform.position.x - x >= 0 && transform.position.x + x < MapEditor.Instance.width)
        {
            return true;
        }
        else { return false; }

    }
}

