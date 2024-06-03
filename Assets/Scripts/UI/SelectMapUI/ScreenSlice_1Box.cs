using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScreenSlice_1Box : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI mapIdText;
    [SerializeField] Image mapIdImage;

    public void SetData(string text,byte[] bytes)
    {
        mapIdText.text = text;
        StartCoroutine(CreateSprtie(bytes));
    }
    public void Reset()
    {
        mapIdText.text = "";
        mapIdImage.sprite = null;
    }



    //public Sprite LoadImage(int width, int height)
    //{
    //    Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
    //    texture.LoadImage(bytesImage);
    //    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);

    //    return sprite;
    //}

    IEnumerator CreateSprtie(byte[] bytes)
    {

        Texture2D texture = new Texture2D(1920 , 1080, TextureFormat.ARGB32, false);
        texture.LoadImage(bytes);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 1920, 1080), new Vector2(0.5f, 0.5f), 100f);

        yield return new WaitForSeconds(0.5f);

        //while(sprite == null)
        //{
        //    Debug.Log("Loading");
        //    yield return null;
        //}

        if(Managers.Game.CurrentState == GameState.Title) { StopCoroutine(CreateSprtie(bytes)); }

        mapIdImage.sprite = sprite;
    }


}
