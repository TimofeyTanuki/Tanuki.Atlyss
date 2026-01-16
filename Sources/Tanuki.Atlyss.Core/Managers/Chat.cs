namespace Tanuki.Atlyss.Core.Managers;

public sealed class Chat
{
    private readonly Routers.Commands commandRouter;

    internal Chat(Routers.Commands commandRouter) => this.commandRouter = commandRouter;

    public void OnPlayerChatted(string message, ref bool runOriginal)
    {
        if (runOriginal == false)
            return;

        if (commandRouter.RouteCommand(message))
        {
            Player._mainPlayer._chatBehaviour._chatAssets._chatInput.text = string.Empty;
            Player._mainPlayer._chatBehaviour._chatAssets._chatInput.DeactivateInputField();

            runOriginal = false;
        }
    }

    public void SendClientMessage(string message) =>
        Player._mainPlayer._chatBehaviour.New_ChatMessage(message);

    public void SendServerMessage(Player player, string message) =>
        player._chatBehaviour.Target_RecieveMessage(message);
}