using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileModeClient
{
    TileInvoker tileInvoker;
    TileDrawModeCommand tileDrawModeCommand;

    public TileModeClient()
    {
        tileInvoker = new TileInvoker();
    }

    public void DrawTile()
    {
        tileDrawModeCommand = new TileDrawModeCommand(new TileDraw());
        tileInvoker.AddCommand(tileDrawModeCommand);
        tileInvoker.Execute();
    }

    public void ClearTile() { }

    public void UndoTile()
    {
        tileInvoker.Undo();
    }




}
