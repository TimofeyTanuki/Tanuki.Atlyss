namespace Tanuki.Atlyss.Core.Managers;

public sealed class Chat
{
    private readonly Commands Commands;
    private readonly Data.Commands.Callers.Player CommandCaller = new();

    internal Chat(Commands Commands) => this.Commands = Commands;

    public void OnPlayerChatted(string message, ref bool runOriginal)
    {
        if (CommandCaller.player != Player._mainPlayer)
            CommandCaller.player = Player._mainPlayer;

        if (Commands.ProcessCommand(CommandCaller, message))
        {
            ChatBehaviour._current._chatAssets._chatInput.text = string.Empty;
            ChatBehaviour._current._chatAssets._chatInput.DeactivateInputField();

            runOriginal = false;
        }
    }

    public void SendClientMessage(string message) =>
        Player._mainPlayer._chatBehaviour.New_ChatMessage(message);

    public void SendServerMessage(Player player, string message) =>
        player._chatBehaviour.Target_RecieveMessage(message);
}