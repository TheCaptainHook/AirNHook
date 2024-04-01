using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileModeClient
{
    Invoker invoker;
    TileDrawModeCommand tileDrawModeCommand;
    TileClearModeCommand tileClearModeCommand;
    TileDrawBundleModeCommand tileDrawBundleModeCommand;

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
    public void DrawBundleTile()
    {
        tileDrawBundleModeCommand = new TileDrawBundleModeCommand(new TileDrawBundle());
        invoker.AddCommand(tileDrawBundleModeCommand);
        invoker.Execute();
    }
  



}
