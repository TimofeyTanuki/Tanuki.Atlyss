using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Example.Commands;

/// <summary>
/// This command can only be executed locally on behalf of the player.
/// </summary>
[CommandMetadata(EExecutionSide.Client, typeof(Core.Policies.Commands.Caller.Player))]
internal class ClientCommandExample : ICommand
{
    private readonly Main main;
    private readonly Core.Managers.Chat chatManager;

    public ClientCommandExample()
    {
        main = Main.Instance;
        chatManager = Core.Tanuki.Instance.Managers.Chat;
    }

    public void Execute(IContext context)
    {
        chatManager.SendClientMessage(
            main.Translate(
                "Commands.ClientCommandExample",
                context.Arguments.Count,
                context.Arguments.Count > 0 ?
                    main.Translate("Commands.ClientCommandExample.Arguments", string.Join(", ", context.Arguments))
                    :
                    string.Empty
            )
        );
    }
}
