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
    Coroutine coroutine;
    private void Awake()
    {
        
        button = GetComponent<Button>();
        //button.onClick.AddListener(ChoiceItem);
       
    }


    //void ChoiceItem()
    //{
      
    //}


    public void Init(GameObject obj)
    {
        image = GetComponent<Image>();
        buildObj = obj;
        coroutine = StartCoroutine(CreateSprite());
    }

    public void Init()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(CreateSprite());
    }

    IEnumerator CreateSprite()
    {
        Texture2D texture = AssetPreview.GetAssetPreview(buildObj);
        while (texture == null)
        {
            yield return null;
        }

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        while (sprite == null)
        {
            yield return null;
        }

        image.sprite = sprite;
    }
}
