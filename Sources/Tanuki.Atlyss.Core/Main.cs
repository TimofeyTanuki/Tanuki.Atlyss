using BepInEx;
using BepInEx.Logging;
using Steamworks;
using System;
using System.Text;
using Tanuki.Atlyss.Core.Packets;

namespace Tanuki.Atlyss.Core;

/*
 * GLOBAL TODO
 *
 * PERMISSION GROUPS
 *   sth like this:
 *     default:
 *       immortality
 *     group1 (assignable by steamid):
 *       immortality
 *       health
 *
 * random foxyjumpscare plugin is ready but i want to sync it with host so need networking :(
 */

[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
internal sealed class Main : Bases.Plugin
{
    public static Main Instance = null!;
    internal Network.Tanuki Network = null!;


    private bool reloadConfiguration = false;
    private ManualLogSource manualLogSource = null!;
    private readonly TanukiServerHello tanukiServerHelloPacket = new()
    {
        Version = PluginInfo.VERSION
    };

    public Main()
    {
        Name = PluginInfo.NAME;
        Configuration.Initialize();
    }

    public void Awake()
    {
        Instance = this;
        manualLogSource = Logger;

        Configuration.Instance.Load(Config);

        Logger.LogMessage("Tanuki.Atlyss by Timofey Tanuki / tanu.su");
    }

    internal void Start()
    {
        Data.Tanuki.Providers providers = new()
        {
            commands = new(),
            settings = new(),
            commandCallerPolicies = new()
        };

        Data.Tanuki.Registers registers = new()
        {
            commands = new(manualLogSource, providers.commandCallerPolicies, providers.settings.CommandSection),
            plugins = new()
        };

        Data.Tanuki.Routers routers = new()
        {
            commands = new(manualLogSource, new(['"', '\"', '`']), providers.settings.CommandSection, registers.commands, providers.commands)
        };

        Data.Tanuki.Managers managers = new()
        {
            plugins = new(manualLogSource, registers.plugins),
            chat = new(routers.commands)
        };

        Tanuki.instance = new()
        {
            managers = managers,
            providers = providers,
            registers = registers,
            routers = routers
        };

        managers.plugins.OnBeforePluginsLoad += HandleSettingsRefresh;

        Atlyss.Network.Tanuki.Initialize();

        Network = Atlyss.Network.Tanuki.Instance;

        Network.Providers.Steam.CreateCallbacks();

        Network.Providers.SteamLobby.OnLobbyChanged += OnNetworkProviderSteamLobbyLobbyChanged;
        Game.Providers.Player.OnPlayerInitialized += OnPlayerInitialized;
        Game.Patches.Player.OnStartAuthority.OnPostfix += OnPlayerStartAuthority;

        Network.Registers.Packets.Register<TanukiServerHello>(null);
        Network.Registers.Packets.Register<Packets.Commands.Request>(null);
        Network.Managers.Packets.AddHandler<TanukiServerHello>(TanukiServerHelloPacketHandler);
        Network.Managers.Packets.AddHandler<Packets.Commands.Request>(routers.commands.HandleIncomingPacket);
        Network.Managers.Packets.ChangeMuteState<Packets.Commands.Request>(true);

        registers.plugins.Refresh();
        managers.plugins.LoadPlugins();
    }

    private void OnPlayerStartAuthority(Player player)
    {
        Console.WriteLine("OnPlayerStartAuthority1111");

        Tanuki.instance.routers.commands.ServerPrefix = tanukiServerHelloPacket.ServerCommandPrefix;
        Network.Managers.Packets.ChangeMuteState<Packets.Commands.Request>(false);
    }

    private void OnPlayerInitialized(Player player)
    {
        manualLogSource.LogInfo($"Player_OnPlayerInitialized\nisNetworkActive: {AtlyssNetworkManager._current.isNetworkActive}\nplayer == Player._mainPlayer: {player == Player._mainPlayer}");
        if (AtlyssNetworkManager._current.isNetworkActive)
            return;

        if (player == Player._mainPlayer)
            return;

        Network.Routers.Packets packetRouter = Network.Routers.Packet;

        if (!ulong.TryParse(player._steamID, out ulong steamId))
            return;

        bool success = packetRouter.SendPacketToUser(new(steamId), tanukiServerHelloPacket, out EResult result);

        manualLogSource.LogInfo($"Hello packet sent to {steamId}, success: {success}, result: {result}");
    }

    private void HandleSettingsRefresh()
    {
        if (reloadConfiguration)
        {
            Config.Reload();
            Configuration.Instance.Load(Config);

            reloadConfiguration = false;
        }

        Tanuki.Instance.providers.settings.Refresh();
    }

    private void ConfigureNetwork()
    {
        Providers.Settings settingsProvider = Tanuki.instance.providers.settings;
        Data.Settings.Network settingsProviderNetworkSection = settingsProvider.NetworkSection;

        Network.Managers.Network networkManager = Network.Managers.Network;
        networkManager.SteamLocalChannel = settingsProviderNetworkSection.mainSteamMessageChannel;
        networkManager.PreventLobbyOwnerRateLimiting = settingsProviderNetworkSection.preventLobbyOwnerRateLimiting;

        Network.Services.RateLimiter rateLimiter = networkManager.RateLimiter;
        rateLimiter.Bandwidth = settingsProviderNetworkSection.rateLimiterBandwidth;
        rateLimiter.Window = settingsProviderNetworkSection.rateLimiterWindow;

        Network.Components.SteamNetworkMessagePoller steamNetworkMessagePoller = networkManager.SteamNetworkMessagesPoller;
        steamNetworkMessagePoller.MessageBufferSize = settingsProviderNetworkSection.steamNetworkMessagePollerBuffer;

        tanukiServerHelloPacket.ServerCommandPrefix = settingsProvider.CommandSection.serverPrefix;
    }

    protected override void Load()
    {
        ConfigureNetwork();

        Tanuki.Instance.managers.chat.Enable();

        if (!Network.Providers.SteamLobby.SteamId.Equals(CSteamID.Nil))
        {
            if (AtlyssNetworkManager._current.isNetworkActive)
            {
                Network.Routers.Packet.SendPacketToLobbyChat(tanukiServerHelloPacket);
                Network.Managers.Packets.ChangeMuteState<Packets.Commands.Request>(false);
            }

            Network.Managers.Network.SteamNetworkMessagesPoller.enabled = true;
        }
    }

    private void TanukiServerHelloPacketHandler(CSteamID sender, TanukiServerHello packet)
    {
        manualLogSource.LogInfo($"Hello packet received\nsender.IsLobby():{sender.IsLobby()}\nOwnerSteamId.Equals(sender): {Network.Providers.SteamLobby.OwnerSteamId.Equals(sender)}");
        if (!sender.IsLobby() || !Network.Providers.SteamLobby.OwnerSteamId.Equals(sender))
            return;

        Routers.Commands commandRouter = Tanuki.instance.routers.commands;
        manualLogSource.LogInfo($"version: {packet.Version}");

        if (packet.Version != PluginInfo.VERSION)
        {
            commandRouter.ServerPrefix = Data.Settings.Commands.SERVER_PREFIX_DEFAULT;
            return;
        }

        commandRouter.ServerPrefix = packet.ServerCommandPrefix;
        manualLogSource.LogInfo($"server prefix: {packet.ServerCommandPrefix}");
    }

    private void OnNetworkProviderSteamLobbyLobbyChanged(CSteamID lobby) =>
        Network.Managers.Network.SteamNetworkMessagesPoller.enabled = !lobby.Equals(CSteamID.Nil);

    protected override void Unload()
    {
        Network.Managers.Network.SteamNetworkMessagesPoller.enabled = false;

        reloadConfiguration = true;
        Tanuki.Instance.managers.chat.Disable();
        Network.Managers.Packets.ChangeMuteState<Packets.Commands.Request>(true);
    }
}
