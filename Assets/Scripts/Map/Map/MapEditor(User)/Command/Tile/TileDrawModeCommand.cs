using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDrawModeCommand : ICommand
{
    TileDraw tileDraw;

    public TileDrawModeCommand(TileDraw tileDraw)
    {
        this.tileDraw = tileDraw;
    }


    public void Execute()
    {
        tileDraw.DrawTile();
    }
    public void Undo()
    {
        tileDraw.UndoTile();
    }
}
