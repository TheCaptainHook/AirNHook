using Org.BouncyCastle.Crypto.Digests;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class Interaction_BuildItem : MonoBehaviour
{
    public GameObject buildObj;
    public Image image;
    Button button;
    private void Awake()
    {
        
        button = GetComponent<Button>();
        //button.onClick.AddListener(ChoiceItem);
       
    }


    //Test
    public Texture2D ttttt;
    public Sprite ssssss;
    //Tet


    //void ChoiceItem()
    //{
      
    //}


    public void Init(GameObject obj, Sprite sprite)
    {
        image = GetComponent<Image>();
        image.sprite = sprite;
        buildObj = obj;
        
    }


}
