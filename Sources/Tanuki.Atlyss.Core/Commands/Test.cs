using System;
using System.Collections.Generic;
using System.Text;
using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Commands;

[CommandMetadata(EExecutionSide.Server)]
internal class Test : ICommand
{
    public void Execute(IContext context)
    {
        ChatBehaviour._current.New_ChatMessage($"{((Data.Commands.Callers.Player)context.Caller).player._nickname} executed test, args: [{string.Join(" ", context.Arguments)}]");
    }
}
