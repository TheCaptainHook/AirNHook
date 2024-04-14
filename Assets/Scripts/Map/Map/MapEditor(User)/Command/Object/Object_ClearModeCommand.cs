using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_ClearModeCommand : ICommand
{

    Object_Clear object_Clear;

    public Object_ClearModeCommand(Object_Clear object_Clear)
    {
        this.object_Clear = object_Clear;
    }

    public void Execute()
    {
        object_Clear.Execute();
    }
    public void Undo()
    {
        object_Clear.Undo();
    }



}
