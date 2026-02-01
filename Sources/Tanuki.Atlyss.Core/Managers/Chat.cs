namespace Tanuki.Atlyss.Core.Managers;

public sealed class Chat
{
    private readonly Routers.Commands commandRouter;

    internal Chat(Routers.Commands commandRouter)
    {
        this.commandRouter = commandRouter;

        Game.Patches.Player.OnStartAuthority.OnPostfix += OnStartAuthority;
        Game.Patches.AtlyssNetworkManager.OnStopClient.OnPrefix += OnStopClient_OnPrefix;
        Game.Patches.ChatBehaviour.Send_ChatMessage.OnPrefix += HandleSendChatMessage;
    }

    private void OnStopClient_OnPrefix() =>
        Game.Patches.ChatBehaviour.UserCode_Cmd_SendChatMessage__String__ChatChannel.OnPrefix -= HandleSendChatMessageCommand;

    private void OnStartAuthority(Player player)
    {
        if (!player._isHostPlayer)
            return;

        Game.Patches.ChatBehaviour.UserCode_Cmd_SendChatMessage__String__ChatChannel.OnPrefix += HandleSendChatMessageCommand;
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

    private void HandleSendChatMessageCommand(ChatBehaviour chatBehaviour, string message, ChatBehaviour.ChatChannel chatChannel, ref bool runOriginal)
    {
        Player player = Game.Accessors.ChatBehaviour._player(chatBehaviour);

        if (player.isLocalPlayer)
            return;

        if (commandRouter.HandleCommandServer(player, message))
            runOriginal = false;
    }

    public void SendClientMessage(string message) =>
        Player._mainPlayer._chatBehaviour.New_ChatMessage(message);

    public void SendServerMessage(Player player, string message) =>
        player._chatBehaviour.Target_RecieveMessage(message);
}