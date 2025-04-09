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


    public event Action preMapLoadEvent;


    private PlayerCameraView playerCameraView;

    private void Awake()
    {
        image = GetComponent<Image>();
        playerCameraView = Camera.main.GetComponent<PlayerCameraView>();
        orgColor = new Color(0, 0, 0, 0);
    }

    public void MoveNextStage(string mapId)
    {
        try
        {
            StartCoroutine(FadeInOut(mapId));
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        
    }


    IEnumerator FadeInOut(string mapId)
    {
        preMapLoadEvent?.Invoke(); //Event to be executed before map transition
        Managers.Stage.stageName = mapId;
        
        image.enabled = true;
        float percent = 0;
        Color fadeOutcolor = new Color(orgColor.r, orgColor.g, orgColor.b, 1);
        
        while (percent < 1)
        {
            percent += Time.deltaTime;

            image.color = Color.Lerp(image.color, fadeOutcolor, percent);
            yield return null;
        }
        image.color = fadeOutcolor;
        percent = 1;

  
        Managers.Network.startPos.Clear();
        MapEditor.Instance.LoadMap(mapId);

        yield return new WaitForSeconds(1f);

        var player = Managers.Game.Player;
        var sm = player ? player.TryGetComponent(out PlayerSM playerSm) ? playerSm : null : null;

        if (!player)
        {
            while (!player)
            {
                Debug.Log("Lost Player");
                player = Managers.Game.Player;
                yield return null;
            }
            sm = player ? player.TryGetComponent(out PlayerSM playerSm1) ? playerSm1 : null : null;
        }
           sm.Respawning();
       
        Camera.main.GetComponent<ParallaxCamera>().enabled = true;
        Camera.main.GetComponent<PlayerCameraView>()._CameraGlobalVolumeController.Volume_1();

        yield return new WaitUntil(()=>playerCameraView.isCameraCenter);

        while (percent > 0)
        {
            percent -= Time.deltaTime;
            image.color = Color.Lerp(orgColor,fadeOutcolor, percent);
            yield return null;
        }
        image.color = orgColor;

            Managers.Game.StageStart(mapId);
        
        
        
        image.enabled = false;
   
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



    #region Default Fade In, Out


   public IEnumerator FadeOut()
    {
        float percent = 0;
        Color fadeOutcolor = new Color(orgColor.r, orgColor.g, orgColor.b, 0);
        while (percent < 1)
        {
            percent += Time.deltaTime;
            image.color = Color.Lerp(fadeOutcolor, orgColor, percent);
            yield return null;
        }

        image.color = new Color(orgColor.r, orgColor.g, orgColor.b, 0);
        image.enabled = false;

    }
   public IEnumerator FadeIn()
    {
        image.enabled = true;
        float percent = 0;
        Color fadeIncolor = new Color(orgColor.r, orgColor.g, orgColor.b, 1);

        while (percent < 1)
        {
            percent += Time.deltaTime;
            image.color = Color.Lerp(orgColor, fadeIncolor, percent);
            yield return null;
        }

    }
    #endregion

}
