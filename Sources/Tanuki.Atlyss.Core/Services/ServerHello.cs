using Steamworks;

namespace Tanuki.Atlyss.Core.Services;

public sealed class ServerHello
{
    private readonly Network.Tanuki network;
    private readonly Network.Routers.Packets packetRouter;
    private readonly Game.Providers.Player playerProvider;
    private readonly Network.Providers.SteamLobby steamLobbyProvider;
    private readonly Packets.ServerHello packet = new() { Version = PluginInfo.VERSION };

    internal ServerHello(
        Network.Tanuki network,
        Network.Routers.Packets packetRouter,
        Game.Providers.Player playerProvider)
    {
        this.network = network;
        this.packetRouter = packetRouter;
        this.playerProvider = playerProvider;
        steamLobbyProvider = network.Providers.SteamLobby;

        network.Registers.Packets.Register<Packets.ServerHello>();
        network.Managers.Packets.AddHandler<Packets.ServerHello>(OnServerHelloReceived);
        Game.Patches.Player.OnStartAuthority.OnPostfix += OnPlayerStartAuthority;
        Game.Patches.AtlyssNetworkManager.OnStopClient.OnPostfix += OnStopClient;
    }

    private void OnServerHelloReceived(CSteamID sender, Packets.ServerHello receivedPacket)
    {
        if (!steamLobbyProvider.OwnerSteamId.Equals(sender) || receivedPacket.Version != PluginInfo.VERSION)
            return;

        Tanuki.Instance.Routers.Commands.serverSupportsRequests = true;
    }

    private void OnPlayerStartAuthority(Player player)
    {
        Tanuki.Instance.Routers.Commands.serverSupportsRequests = false;
        bool isHost = player._isHostPlayer;

        if (isHost)
            playerProvider.OnPlayerLoaded += SendToPlayer;

        network.Managers.Packets.ChangeMuteState<Packets.ServerHello>(isHost);
    }

    public void SendToLobby() =>
        network.Routers.Packet.SendPacketToLobbyChat(packet);

    private void SendToPlayer(Player player)
    {
        if (player.isLocalPlayer)
            return;

        CSteamID target = playerProvider.PlayerEntries[player.netId].SteamId;
        packetRouter.SendPacketToUser(target, packet, out EResult _);
    }

    private void OnStopClient()
    {
        playerProvider.OnPlayerLoaded -= SendToPlayer;
        Tanuki.Instance.Routers.Commands.serverSupportsRequests = false;
    }
}
