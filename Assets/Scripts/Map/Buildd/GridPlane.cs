using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlane : MonoBehaviour
{
    int width;
    int height;
    
    Material material;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    public void SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;

        transform.localScale=new Vector3(width*2, height*2);
        material.SetVector("_Tilling",new Vector2(width,height));

    }
}
