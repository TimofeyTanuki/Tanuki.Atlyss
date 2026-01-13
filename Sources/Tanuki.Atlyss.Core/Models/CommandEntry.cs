using System;
using Tanuki.Atlyss.API.Commands;

namespace Tanuki.Atlyss.Core.Models;

public class CommandEntry(ICommand Command)
{
    public readonly ICommand Command = Command;

    public CommandConfigurationItem Configuration = null!;

    public void Dispose()
    {
        if (Command is not IDisposable Disposable)
            return;

        Disposable.Dispose();
    }
}