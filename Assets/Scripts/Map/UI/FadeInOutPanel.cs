using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Mirror;

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
        MapEditor.Instance._onMapTransition_Complete = false;
        var uiOption = Managers.UI.GetUI<UI_Option>().GetComponent<UI_Option>();
        if (NetworkServer.active) 
        {
            Managers.Command.Server_UpdateCurClientConnectionCount();
            uiOption.HoldAndReleaseLobby_StageRestartBtn(true);
        } 
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

        //------------------------Pooling
        while (MapEditor.Instance._n_activePoolingObject.Count > 0)
        {
            var obj = MapEditor.Instance._n_activePoolingObject.Dequeue();
            obj.Clean();

            if (NetworkServer.active)
                Managers.Pooling.N_ReleaseToPool(obj.gameObject);

        }
        
        
        while (MapEditor.Instance._d_activePoolingObject.Count > 0)
        {
            var obj = MapEditor.Instance._d_activePoolingObject.Dequeue();
            obj.Clean();
            Managers.Pooling.D_ReleaseToPool(obj.gameObject);
        }

        //------------------------Pooling

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
        sm.canControl = false;
        sm.canMovable = false;

        yield return new WaitForSeconds(1f);

        var playerCol = sm.GetComponent<Collider2D>();
        var playerRb = sm.GetComponent<Rigidbody2D>();
        var playerGravity = playerRb.gravityScale;

        playerRb.gravityScale = 0;
        playerRb.velocity = Vector2.zero;   
        playerCol.enabled = false;

        //------------------------Player Ignore Damage

        //------------------------Create Next Stage
        Managers.Network.startPos.Clear();
        // MapEditor.Instance.LoadMap(mapId);
        yield return StartCoroutine(MapEditor.Instance.LoadMapCo(mapId));
        //------------------------Create Next Stage

        //------------------------Player, Camera Setting
        sm.Respawning();
        Camera.main.GetComponent<ParallaxCamera>().enabled = true;
        // Camera.main.GetComponent<PlayerCameraView>()._CameraGlobalVolumeController.Volume_1();
        playerCameraView._CameraGlobalVolumeController.Volume_1();
        yield return new WaitForSeconds(.5f);

        //yield return new WaitUntil(() => playerCameraView.isCameraCenter);
        yield return WaitUntilOrTimeout(() => playerCameraView.isCameraCenter, 10, () => { Debug.Log("[1] TimeOut Camera"); });
        //------------------------Player, Camera Setting

        Managers.Command.Cmd_IsCompleteMoveStage();
        //var num = Managers.Command.currentClientConnectionCount;
        //yield return new WaitUntil(() => Managers.Command.isCompleteMoveStageCount == num);
        yield return WaitUntilOrTimeout(() => Managers.Command.AllReadyClient(), 10, () => { Debug.Log("[2] TimeOut"); });

        //------------------------UI_MapOpenClosePanel Prograss 2
        yield return StartCoroutine(UI_MapOpenClosePanel.Prograss_2());
        //------------------------UI_MapOpenClosePanel Prograss 2

        //--------------------------------Player recover
        if (playerCol) playerCol.enabled = true;
        if (playerRb) playerRb.gravityScale = playerGravity;
        //--------------------------------Player recover

        yield return new WaitForSeconds(1f);
        sm.canMovable = true;
        sm.canControl = true;
        //==========Map Transition Complete
        MapEditor.Instance._onMapTransition_Complete = true;
        //==========Map Transition Complete

        //------------------------UI_MapOpenClosePanel Prograss 3
        yield return StartCoroutine(UI_MapOpenClosePanel.Prograss_3());
        Managers.UI.HideUI<UI_MapOpenClosePanel>();
        //------------------------UI_MapOpenClosePanel Prograss 3

        moveNextStageCoroutine = null;
        if (NetworkServer.active) 
        {
            uiOption.HoldAndReleaseLobby_StageRestartBtn(false);

        } 
        //Managers.Command.Cmd_IsCompleteMoveStage();
        Managers.Game.StageStart(mapId);
    }


    private IEnumerator WaitUntilOrTimeout(Func<bool> cond, float timeoutSec, Action onTimeout = null)
    {
        float end = Time.unscaledTime + timeoutSec;
        while (!cond())
        {
            if (Time.unscaledTime >= end)
            {
                onTimeout?.Invoke();
                yield break;
            }
            yield return null;
        }
    }
}
