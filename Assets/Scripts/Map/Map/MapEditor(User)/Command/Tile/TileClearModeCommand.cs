using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClearModeCommand : ICommand
{
    TileClear tileClear;

    public TileClearModeCommand(TileClear tileClear)
    {
        this.tileClear = tileClear;
    }


    public void Execute()
    {
        tileClear.ClearTile();
    }
    public void Undo()
    {
        tileClear.UndoTile();
    }
}
