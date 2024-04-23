using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class FadeInOutPanel : MonoBehaviour
{
    Image image;
    Color orgColor;
    float fadeTime = 1f;

    private void Awake()
    {
        image = GetComponent<Image>();
        orgColor = image.color;
    }

    public void MoveNextStage(string mapId)
    {
        StartCoroutine(FadeInOut(mapId));
    }


    IEnumerator FadeInOut(string mapId)
    {
        image.enabled = true;
        float percent = 0;
        Color fadeOutcolor = new Color(orgColor.r, orgColor.g, orgColor.b, 1);
        Managers.Stage.stageName = mapId;

        while (percent < 1)
        {
            percent += Time.deltaTime;

            image.color = Color.Lerp(image.color, fadeOutcolor, percent);
            yield return null;
        }

        Managers.Network.startPos.Clear();
        MapEditor.Instance.LoadMap(mapId);

        yield return new WaitForSeconds(1f);
        //while (!CheckNetworkStartPos())
        //{
        //    Debug.Log("Loading");
        //    yield return null;
        //}

        //Debug.Log($"startPos[0] : {(Vector2)Managers.Network.startPos[0].position}, MapEditor start pot: {MapEditor.Instance.startPosition}");
        //Debug.Log($"{(Vector2)Managers.Network.startPos[0].position == MapEditor.Instance.startPosition}");

        Managers.Game.Player.GetComponent<Player>().Respawning();
        //Managers.Game.OtherPlayer.GetComponent<Player>().Respawning();

        Camera.main.GetComponent<ParallaxCamera>().enabled = true;

        while (percent > 0)
        {
            percent -= Time.deltaTime;
            image.color = Color.Lerp(image.color, orgColor, percent);
            yield return null;
        }

        Managers.Game.StageStart(mapId);
        image.enabled = false;
        //StartCoroutine(FadeInOut(mapId));
        StartCoroutine(Fadein(mapId));
    }

    public IEnumerator Fadein(string mapId)
    {
        MapEditor.Instance.stageText.text = mapId;
        Color tempColor = MapEditor.Instance.stageText.color;
        tempColor.a = 0f;
        while (tempColor.a < 1f)
        {
            tempColor.a += Time.deltaTime / fadeTime;
            MapEditor.Instance.stageText.color = tempColor;

            if (tempColor.a >= 1f)
            {
                tempColor.a = 1f;
            }
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        while (tempColor.a > 0f)
        {
            tempColor.a -= Time.deltaTime / fadeTime;
            MapEditor.Instance.stageText.color = tempColor;

            if (tempColor.a <= 0f)
            {
                tempColor.a = 0f;
            }
            yield return null;
        }
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
