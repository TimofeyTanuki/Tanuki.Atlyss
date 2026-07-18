using System;

namespace Tanuki.Atlyss.Core.Managers;

public sealed class Chat
{
    private readonly Routers.Commands commandRouter;

    internal Chat(Routers.Commands commandRouter)
    {
        this.commandRouter = commandRouter;

        Game.Patches.Player.OnStartAuthority.OnPostfix += OnPlayerStartAuthority;
        Game.Patches.AtlyssNetworkManager.OnStopClient.OnPrefix += OnAtlyssNetworkManagerStop;
        Game.Patches.ChatBehaviour.Cmd_SendChatMessage.OnPrefix += OnPlayerChatted;
    }

    private void OnAtlyssNetworkManagerStop() =>
        Game.Patches.ChatBehaviour.Rpc_RecieveChatMessage.OnPrefix -= OnPlayerMessageReceived;

    private void OnPlayerStartAuthority(Player player)
    {
        if (!player._isHostPlayer)
            return;

        Game.Patches.ChatBehaviour.Rpc_RecieveChatMessage.OnPrefix += OnPlayerMessageReceived;
    }

    private void OnPlayerChatted(string message, ChatBehaviour.ChatChannel _chatChannel, ref bool runOriginal)
    {
        if (runOriginal == false)
            return;

        if (commandRouter.HandleCommandClient(message))
            runOriginal = false;
    }

    private void OnPlayerMessageReceived(ChatBehaviour instance, string message, bool isEmoteMessage, ChatBehaviour.ChatChannel chatChannel, ref bool runOriginal)
    {
        if (isEmoteMessage)
            return;

        Player player = Game.Accessors.ChatBehaviour._player(instance);

        if (player.isLocalPlayer)
            return;

        if (commandRouter.HandleCommandServer(player, message))
            runOriginal = false;
    }

    public void AddMessage(string message) =>
        Player._mainPlayer._chatBehaviour.New_ChatMessage(message);

    public void SendMessage(Player player, string message) =>
        player._chatBehaviour.Target_RecieveMessage(message);
}