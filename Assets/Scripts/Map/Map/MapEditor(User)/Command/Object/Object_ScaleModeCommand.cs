using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_ScaleModeCommand : ICommand
{
  Object_Scale object_Scale;

    public Object_ScaleModeCommand(Object_Scale object_Scale)
    {
        this.object_Scale = object_Scale;
    }

    public void Execute()
    {
        object_Scale.Execute();
    }
    public void Undo()
    {
        object_Scale.Undo();
    }
}
