using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDrawBoxModeCommand : ICommand
{
    TileDrawBox tileDrawBox;

    public TileDrawBoxModeCommand(TileDrawBox tileDrawBox)
    {
        this.tileDrawBox = tileDrawBox;
    }


    public void Execute()
    {
        tileDrawBox.DrawTile();
    }
    public void Undo()
    {
        tileDrawBox.UndoTile();
    }
}
