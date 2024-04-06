using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_CreateModeCommand :ICommand
{
    Object_Create object_Create;

    public Object_CreateModeCommand(Object_Create object_Create)
    {
        this.object_Create = object_Create;
    }

    public void Execute()
    {
        object_Create.Create();
    }
    public void Undo()
    {
        object_Create.Undo();
    }
}
