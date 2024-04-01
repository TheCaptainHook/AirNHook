using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invoker 
{
    ICommand command;
    Stack<ICommand> commandStack = new Stack<ICommand>();

    public void AddCommand(ICommand command)
    {
        this.command = command;
        commandStack.Push(command);
    }

    public void Execute()
    {
        command.Execute();
    }

    public void Undo()
    {
        if(commandStack.Count > 0)
        {
            ICommand command = commandStack.Pop();
            command.Undo();
        }

    }
}
