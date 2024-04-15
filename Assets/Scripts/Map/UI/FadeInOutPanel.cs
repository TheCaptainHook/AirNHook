using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class FadeInOutPanel : MonoBehaviour
{
    Image image;
    Color orgColor;


    public event Action OnNextStage;

    private void Awake()
    {
        image = GetComponent<Image>();
        orgColor = image.color;
    }

    public void MoveNextStage(string mapId,MapType mapType)
    {
        StartCoroutine(FadeInOut(mapId, mapType));
    }


    IEnumerator FadeInOut(string mapId, MapType mapType)
    {
        image.enabled = true;
        float percent = 0;
        Color fadeOutcolor = new Color(orgColor.r, orgColor.g, orgColor.b, 1);
        while(percent < 1)
        {
            percent += Time.deltaTime;

            image.color = Color.Lerp(image.color, fadeOutcolor, percent);
            yield return null;
        }
        //
        MapEditor.Instance.LoadMap(mapId, mapType);
        //열쇠
        yield return new WaitForSeconds(0.5f);
        Managers.Game.Player.GetComponent<Player>().Respawning();
        //
        while (percent > 0)
        {
            percent -= Time.deltaTime;
            image.color = Color.Lerp(image.color, orgColor, percent);
            yield return null;
        }

        
        image.enabled = false;
    }
}
