using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectModeClient
{
    Invoker invoker;
    Object_CreateModeCommand object_CreateModeCommand;

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

    }
    public void Rotaion()
    {

    }
    public void Clear()
    {

    }


}
