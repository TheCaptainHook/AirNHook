using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class FadeInOutPanel : MonoBehaviour
{
    Image image;
    Color orgColor;

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
        Managers.Network.startPos.Clear();
        MapEditor.Instance.LoadMap(mapId, mapType);

        while (!CheckNetworkStartPos())
        {
            Debug.Log("Loading");
            yield return null;
        }

        //Debug.Log($"startPos[0] : {(Vector2)Managers.Network.startPos[0].position}, MapEditor start pot: {MapEditor.Instance.startPosition}");
        //Debug.Log($"{(Vector2)Managers.Network.startPos[0].position == MapEditor.Instance.startPosition}");


        Debug.Log(Managers.Network.startPos.Count);
        Managers.Game.Player.GetComponent<Player>().Respawning();
        ///
        while (percent > 0)
        {
            percent -= Time.deltaTime;
            image.color = Color.Lerp(image.color, orgColor, percent);
            yield return null;
        }

        
        image.enabled = false;
    }

    
    private bool CheckNetworkStartPos()
    {
        try
        {
                if (MapEditor.Instance.startPosition == (Vector2)Managers.Network.startPos[0].position)
                {
                    return true;
                }
           
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
            return false;
        }

        return false;


    }


}
