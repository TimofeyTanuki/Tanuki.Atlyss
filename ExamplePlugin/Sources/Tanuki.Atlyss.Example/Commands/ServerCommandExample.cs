using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Example.Commands;

/// <summary>
/// This command can be executed by any player, but is performed exclusively on the server side.
/// </summary>
[CommandMetadata(EExecutionSide.Server, typeof(Core.Policies.Commands.Caller.Player))]
internal class ServerCommandExample : ICommand
{
    private readonly Main main;
    private readonly Core.Managers.Chat chatManager;

    public ServerCommandExample()
    {
        main = Main.Instance;
        chatManager = Core.Tanuki.Instance.Managers.Chat;
    }

    public void Execute(IContext context)
    {
        Core.Data.Commands.Callers.Player caller = (Core.Data.Commands.Callers.Player)context.Caller;

        chatManager.SendServerMessage(caller.player, main.Translate("Commands.ServerCommandExample", caller.player == Player._mainPlayer));
    }
}
