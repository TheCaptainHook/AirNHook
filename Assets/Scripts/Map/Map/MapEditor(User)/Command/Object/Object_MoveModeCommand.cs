using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_MoveModeCommand : ICommand
{
    Object_Move object_Move;

    public Object_MoveModeCommand(Object_Move object_Move)
    {
        this.object_Move = object_Move;
    }

    public void Execute()
    {
        object_Move.Move();
    }
    public void Undo()
    {
        object_Move.Undo();
    }

}
