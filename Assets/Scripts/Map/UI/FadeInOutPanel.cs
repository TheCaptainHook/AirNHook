using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Net.Mail;
using Mirror;

public class FadeInOutPanel : MonoBehaviour
{
    Image image;
    Color orgColor;
    float fadeTime = 1f;


    public event Action preMapLoadEvent;


    private PlayerCameraView playerCameraView;
    [SerializeField] TextMeshProUGUI text;
    private void Awake()
    {
        image = GetComponent<Image>();
        playerCameraView = Camera.main.GetComponent<PlayerCameraView>();
        orgColor = new Color(0, 0, 0, 0);
    }
    private Coroutine moveNextStageCoroutine;
    public void MoveNextStage(string mapId)
    {
        if(moveNextStageCoroutine == null)
        moveNextStageCoroutine = StartCoroutine(FadeInOut(mapId));
    }

    private string GetMapSubName()
    {
        var name = MapEditor.Instance.CurMap.subMapName;
        return name != null ? name : MapEditor.Instance.CurMap.mapID;
    }
    IEnumerator FadeInOut(string mapId)
    {
        //Event to be executed before map transition
        preMapLoadEvent?.Invoke(); 
        Managers.Sound.CollectAmbientSoundSource();
        Camera.main.GetComponent<ParallaxCamera>().enabled = false;
        //Event to be executed before map transition

        //------------------------Next Stage
        Managers.Stage.stageName = mapId;
        //------------------------Next Stage

        //------------------------Fade Out
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
        //------------------------Fade Out

        //------------------------Player Ignore Damage
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
            sm = player.GetComponent<PlayerSM>();

        }
        var playerCol = sm.GetComponent<Collider2D>();
        var playerRb = sm.GetComponent<Rigidbody2D>();
        playerRb.gravityScale = 0;
        playerCol.enabled = false;

        //------------------------Player Ignore Damage


        //------------------------Create Next Stage
        Managers.Network.startPos.Clear();
        MapEditor.Instance.LoadMap(mapId);
        //------------------------Create Next Stage
        
        //------------------------Map Name  UI
        text.enabled = true;
        text.text = GetMapSubName();
        //------------------------Map Name UI
        yield return new WaitForSeconds(1f);
        //------------------------Player, Camera Setting
        //var player = Managers.Game.Player;
        //var sm = player ? player.TryGetComponent(out PlayerSM playerSm) ? playerSm : null : null;

        //if (!player)
        //{
        //    while (!player)
        //    {
        //        Debug.Log("Lost Player");
        //        player = Managers.Game.Player;
        //        yield return null;
        //    }
        //    // sm = player ? player.TryGetComponent(out PlayerSM playerSm1) ? playerSm1 : null : null;
        //    sm = player.GetComponent<PlayerSM>();
         
        //}
      

        sm.Respawning();
       
        Camera.main.GetComponent<ParallaxCamera>().enabled = true;
        Camera.main.GetComponent<PlayerCameraView>()._CameraGlobalVolumeController.Volume_1();

        yield return new WaitUntil(()=>playerCameraView.isCameraCenter);
        //------------------------Player, Camera Setting

        //--------------------------------Player recover
        playerCol.enabled = true;
        playerRb.gravityScale = 3;
        //--------------------------------Player recover

        //Map Name  UI
        text.enabled = false;
        //Map Name UI

        //------------------------Fade Out
        while (percent > 0)
        {
            percent -= Time.deltaTime;
            image.color = Color.Lerp(orgColor,fadeOutcolor, percent);
            yield return null;
        }
        image.color = orgColor;
        //------------------------Fade Out
        
        image.enabled = false;
        moveNextStageCoroutine = null;

        try
        {
            Managers.Command.Cmd_IsCompleteMoveStage();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
       

        //Managers.Command.Cmd_IsCompleteMoveStage();
        Managers.Game.StageStart(mapId);
    }



}
