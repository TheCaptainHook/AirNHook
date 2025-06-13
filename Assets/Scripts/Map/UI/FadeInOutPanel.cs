using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class FadeInOutPanel : MonoBehaviour
{
    public event Action preMapLoadEvent;

    #region Animation

    #endregion

    private PlayerCameraView playerCameraView;

    [SerializeField] TextMeshProUGUI text;
    private void Awake()
    {
        playerCameraView = Camera.main.GetComponent<PlayerCameraView>();
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

        //------------------------UI_MapOpenClosePanel Prograss 1
        var UI_MapOpenClosePanel = Managers.UI.ShowUI<UI_MapOpenClosePanel>().GetComponent<UI_MapOpenClosePanel>();
        // Managers.UI.ShowUI<UI_MapOpenClosePanel>();

        yield return StartCoroutine(UI_MapOpenClosePanel.Prograss_1());
        //------------------------UI_MapOpenClosePanel Prograss 1

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
        sm.CallPlayerDeathEvent();
        sm.canMovable = false;

        yield return new WaitForSeconds(1f);

        var playerCol = sm.GetComponent<Collider2D>();
        var playerRb = sm.GetComponent<Rigidbody2D>();
        playerRb.gravityScale = 0;
        playerRb.velocity = Vector2.zero;   
        playerCol.enabled = false;

        //------------------------Player Ignore Damage

        //------------------------Create Next Stage
        Managers.Network.startPos.Clear();
        MapEditor.Instance.LoadMap(mapId);
        //------------------------Create Next Stage

        yield return new WaitForSeconds(1f);
        //------------------------Player, Camera Setting

        sm.Respawning();
       
        Camera.main.GetComponent<ParallaxCamera>().enabled = true;
        Camera.main.GetComponent<PlayerCameraView>()._CameraGlobalVolumeController.Volume_1();

        yield return new WaitUntil(()=>playerCameraView.isCameraCenter);
        //------------------------Player, Camera Setting

        //------------------------UI_MapOpenClosePanel Prograss 2
        yield return StartCoroutine(UI_MapOpenClosePanel.Prograss_2());
        //------------------------UI_MapOpenClosePanel Prograss 2

        //--------------------------------Player recover
        if (playerCol) playerCol.enabled = true;
        if(playerRb) playerRb.gravityScale = 3;
        //--------------------------------Player recover

        yield return new WaitForSeconds(1f);
        sm.canMovable = true;
        //------------------------UI_MapOpenClosePanel Prograss 3
        yield return StartCoroutine(UI_MapOpenClosePanel.Prograss_3());
        //------------------------UI_MapOpenClosePanel Prograss 3

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
