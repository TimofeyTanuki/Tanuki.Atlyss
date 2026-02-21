/*
using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Commands;

[CommandMetadata(EExecutionSide.Server, typeof(Policies.Commands.Caller.Player))]
public sealed class Test : ICommand
{
    public void Execute(IContext context)
    {
        Data.Commands.Callers.Player caller = (Data.Commands.Callers.Player)context.Caller;
        string message = $"[{caller.player.netId}] {caller.player._nickname} executed test, args (x{context.Arguments.Count}) [{string.Join(",", context.Arguments)}]";

        if (message.Length > 96)
            message = $"{message[..96].TrimEnd(']')}...]";

        HostConsole._current.Init_ServerMessage(message);
    }
}
*/
