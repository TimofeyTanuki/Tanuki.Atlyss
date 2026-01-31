namespace Tanuki.Atlyss.Core.Managers;

public sealed class Chat
{
    private readonly Routers.Commands commandRouter;

    internal Chat(Routers.Commands commandRouter) => this.commandRouter = commandRouter;

    public void Enable()
    {
        Game.Patches.Player.OnStartAuthority.OnPostfix += CheckServerRuntime;
        Game.Patches.AtlyssNetworkManager.OnStopClient.OnPostfix += DisableServerRuntime;
        Game.Patches.ChatBehaviour.Send_ChatMessage.OnPrefix += HandleSendChatMessage;

        if (Player._mainPlayer)
            CheckServerRuntime(null);
    }

    private void CheckServerRuntime(Player? _)
    {
        System.Console.WriteLine($"START CLIENT {Player._mainPlayer._isHostPlayer}");

        if (!Player._mainPlayer._isHostPlayer)
            return;

        Game.Patches.ChatBehaviour.UserCode_Cmd_SendChatMessage__String__ChatChannel.OnPrefix += HandleSendChatMessageCommand;
    }

    private void DisableServerRuntime()
    {
        System.Console.WriteLine($"STOP CLIENT");
        Game.Patches.ChatBehaviour.UserCode_Cmd_SendChatMessage__String__ChatChannel.OnPrefix -= HandleSendChatMessageCommand;
    }

    public void Disable()
    {
        Game.Patches.ChatBehaviour.Send_ChatMessage.OnPrefix -= HandleSendChatMessage;
        Game.Patches.AtlyssNetworkManager.OnStopClient.OnPostfix -= DisableServerRuntime;

        DisableServerRuntime();
    }

    private void HandleSendChatMessage(string message, ref bool runOriginal)
    {
        if (runOriginal == false)
            return;

        if (commandRouter.HandleCommandClient(message))
        {
            Player._mainPlayer._chatBehaviour._chatAssets._chatInput.text = string.Empty;
            Player._mainPlayer._chatBehaviour._chatAssets._chatInput.DeactivateInputField();

            runOriginal = false;
        }
    }

    private void HandleSendChatMessageCommand(ChatBehaviour chatBehaviour, string message, ref bool runOriginal)
    {
        Player player = Game.Accessors.ChatBehaviour._player(chatBehaviour);

        if (player.isLocalPlayer)
            return;

        if (commandRouter.HandleCommandServer(player, message))
            runOriginal = false;

        System.Console.WriteLine($"XD {player._nickname} - {message}");
    }

    public void SendClientMessage(string message) =>
        Player._mainPlayer._chatBehaviour.New_ChatMessage(message);

    public void SendServerMessage(Player player, string message) =>
        player._chatBehaviour.Target_RecieveMessage(message);
}