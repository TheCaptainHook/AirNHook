using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileModeClient
{
    Invoker invoker;
    TileDrawModeCommand tileDrawModeCommand;

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

    public void ClearTile() { }
    public void DrawBundleTile() { }
  



}
