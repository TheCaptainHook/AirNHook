using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectModeClient
{
    Invoker invoker;
    Object_CreateModeCommand object_CreateModeCommand;
    Object_MoveModeCommand object_MoveModeCommand;
    Object_RotateModeCommand object_RotateModeCommand;
    Object_ScaleModeCommand object_ScaleModeCommand;
    Object_ClearModeCommand object_ClearModeCommand;

    public ObjectModeClient()
    {
        invoker = MapEditor.Instance.placeMentSystem.invoker;
    }

    public void Create()
    {
        object_CreateModeCommand = new Object_CreateModeCommand(new Object_Create());
        invoker.AddCommand(object_CreateModeCommand);
        invoker.Execute();
    }
    public void Move()
    {
        object_MoveModeCommand = new Object_MoveModeCommand(new Object_Move());
        invoker.AddCommand(object_MoveModeCommand);
        invoker.Execute();
    }
    public void Rotaion()
    {
        object_RotateModeCommand = new Object_RotateModeCommand(new Object_Rotate());
        invoker.AddCommand(object_RotateModeCommand);
        invoker.Execute();
    }
    public void Scale()
    {
        object_ScaleModeCommand = new Object_ScaleModeCommand(new Object_Scale());
        invoker.AddCommand(object_ScaleModeCommand);
        invoker.Execute();
    }
    public void Clear()
    {
        object_ClearModeCommand = new Object_ClearModeCommand(new Object_Clear());
        invoker.AddCommand(object_ClearModeCommand);
        invoker.Execute();
    }


}
