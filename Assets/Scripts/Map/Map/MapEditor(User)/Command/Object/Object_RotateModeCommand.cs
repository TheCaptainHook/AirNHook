using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_RotateModeCommand : ICommand
{
    Object_Rotate object_Rotate;

    public Object_RotateModeCommand(Object_Rotate object_Rotate)
    {
        this.object_Rotate = object_Rotate;
    }

    public void Execute()
    {
        object_Rotate.Rotate();
    }
    public void Undo()
    {
        object_Rotate.Undo();
    }
}
