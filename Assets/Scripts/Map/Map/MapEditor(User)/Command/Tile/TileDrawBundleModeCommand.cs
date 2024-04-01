using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDrawBundleModeCommand : ICommand
{
    TileDrawBundle tileDrawBundle;

    public TileDrawBundleModeCommand(TileDrawBundle tileDrawBundle)
    {
        this.tileDrawBundle = tileDrawBundle;
    }


    public void Execute()
    {
        tileDrawBundle.DrawTile();
    }
    public void Undo()
    {
        tileDrawBundle.UndoTile();
    }
}
