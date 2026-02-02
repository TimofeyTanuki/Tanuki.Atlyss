using Steamworks;

namespace Tanuki.Atlyss.Core.Services;

public sealed class TanukiServer
{
    private readonly Network.Tanuki network;
    private readonly Routers.Commands commandRouter;
    private readonly Providers.Settings settingProvider;
    private readonly Network.Routers.Packets packetRouter;

    private readonly Network.Providers.SteamLobby steamLobbyProvider;

    private readonly Packets.TanukiServerInfo tanukiServerInfo = new()
    {
        Version = PluginInfo.VERSION
    };

    internal TanukiServer(
        Network.Tanuki network,
        Routers.Commands commandRouter,
        Providers.Settings settingProvider,
        Network.Routers.Packets packetRouter)
    {
        this.network = network;
        this.commandRouter = commandRouter;
        steamLobbyProvider = network.Providers.SteamLobby;
        this.settingProvider = settingProvider;
        this.packetRouter = packetRouter;

        network.Registers.Packets.Register<Packets.TanukiServerInfo>();
        network.Managers.Packets.AddHandler<Packets.TanukiServerInfo>(TanukiServerInfoReceived);

        Game.Patches.Player.OnStartAuthority.OnPostfix += OnStartAuthority;
        Game.Patches.AtlyssNetworkManager.OnStopClient.OnPostfix += OnStopClient;
    }

    private void TanukiServerInfoReceived(CSteamID sender, Packets.TanukiServerInfo packet)
    {
        if (!steamLobbyProvider.OwnerSteamId.Equals(sender))
            return;

        if (packet.Version != PluginInfo.VERSION)
            return;

        if (!string.IsNullOrEmpty(packet.ServerCommandPrefix))
            if (packet.ServerCommandPrefix.Length > 0 && packet.ServerCommandPrefix.Length < Data.Settings.Commands.PREFIX_MAX_LENGTH)
                commandRouter.ServerPrefix = packet.ServerCommandPrefix;
    }

    public void Refresh()
    {
        tanukiServerInfo.ServerCommandPrefix = settingProvider.CommandSection.serverPrefix;

        if (Player._mainPlayer && Player._mainPlayer._isHostPlayer)
            network.Routers.Packet.SendPacketToLobbyChat(tanukiServerInfo);
    }

    private void OnStartAuthority(Player player)
    {
        bool isHost = player._isHostPlayer;

        if (isHost)
            Game.Providers.Player.OnPlayerLoaded += SendTanukiServerInfo;

        commandRouter.ServerPrefix = settingProvider.CommandSection.serverPrefix;

        network.Managers.Packets.ChangeMuteState<Packets.TanukiServerInfo>(isHost);
    }

    private void SendTanukiServerInfo(Player player)
    {
        if (player.isLocalPlayer)
            return;

        CSteamID target = Game.Providers.Player.Instance.Players[player.netId].SteamId;
        packetRouter.SendPacketToUser(target, tanukiServerInfo, out EResult _);
    }

    private void OnStopClient() =>
        Game.Providers.Player.OnPlayerLoaded -= SendTanukiServerInfo;
}
