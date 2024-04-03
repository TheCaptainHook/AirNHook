using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileModeClient
{
    Invoker invoker;
    TileDrawModeCommand tileDrawModeCommand;
    TileClearModeCommand tileClearModeCommand;
    TileDrawBoxModeCommand tileDrawBundleModeCommand;
    TileClearBoxModeMommand tileClearBoxModeCommand;

    public TileModeClient()
    {
        invoker = MapEditor.Instance.placeMentSystem.invoker;
    }

    public void DrawTile()
    {
        tileDrawModeCommand = new TileDrawModeCommand(new TileDraw());
        invoker.AddCommand(tileDrawModeCommand);
        invoker.Execute();
    }

    public void ClearTile()
    {
        tileClearModeCommand = new TileClearModeCommand(new TileClear());
        invoker.AddCommand(tileClearModeCommand);
        invoker.Execute();
    }
    public void DrawBoxTile()
    {
        tileDrawBundleModeCommand = new TileDrawBoxModeCommand(new TileDrawBox());
        invoker.AddCommand(tileDrawBundleModeCommand);
        invoker.Execute();
    }

    public void ClearBoxTile()
    {
        tileClearBoxModeCommand = new TileClearBoxModeMommand(new TileClearBox());
        invoker.AddCommand(tileClearBoxModeCommand);
        invoker.Execute();
    }

}
