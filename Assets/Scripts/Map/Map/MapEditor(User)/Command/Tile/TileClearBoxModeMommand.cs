using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClearBoxModeMommand : ICommand
{
    TileClearBox tileClearBox;

    public TileClearBoxModeMommand(TileClearBox tileClearBox)
    {
        this.tileClearBox = tileClearBox;
    }

    public void Execute()
    {
        tileClearBox.ClearTile();
    }
    public void Undo()
    {
        tileClearBox.UndoTile();
    }
}
