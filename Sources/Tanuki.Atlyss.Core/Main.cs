using BepInEx;
using BepInEx.Logging;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tanuki.Atlyss.Core.Packets.Commands;

namespace Tanuki.Atlyss.Core;

/*
 * GLOBAL TODO
 *
 * CLIENTS MUSTN'T KNOW ABOUT SERVER COMMANDS
 * SERVER MUSTN'T KNOW ABOUT CLIENT' COMMANDS
 * ^^^ safety policy
 *
 * HASH CODES FOR NETWORKING
 *   Command Execution
 *   if command not found
 *     ask server
 *       search by name
 *         if command found
 *           check execution side
 *             client -> ignore
 *             server -> execute
 *         if command not found
 *           send default message from server???? not sure
 *
 *   if command found --> check execution side
 *     client -> execute
 *     server -> send hashcode to server + command name from chat (if hash code not found -> check name)
 *
 *
 * LISTEN CHAT ON HOST FOR NON TANUKI USERS
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


    private bool reloadConfiguration = false;
    private ManualLogSource manualLogSource = null!;
    private TanukiServerHello tanukiServerHelloPacket = new()
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
            settings = new()
        };

        Data.Tanuki.Registers registers = new()
        {
            commands = new(manualLogSource, providers.settings.CommandSection),
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

        Network.Tanuki.Initialize();

        Network.Tanuki network = Network.Tanuki.Instance;
        network.Providers.Steam.CreateCallbacks();

        network.Providers.SteamLobby.OnLobbyChanged += OnNetworkProviderSteamLobbyLobbyChanged;
        Game.Providers.Player.OnPlayerInitialized += Player_OnPlayerInitialized;

        network.Registers.Packets.Register<TanukiServerHello>(null);
        network.Managers.Packets.AddHandler<TanukiServerHello>(TanukiServerHelloPacketHandler);

        registers.plugins.Refresh();
        managers.plugins.LoadPlugins();
    }

    private void Player_OnPlayerInitialized(Player player)
    {
        manualLogSource.LogInfo("Player_OnPlayerInitialized D0");
        if (AtlyssNetworkManager._current.isNetworkActive)
            return;

        manualLogSource.LogInfo("Player_OnPlayerInitialized D1");

        if (player == Player._mainPlayer)
            return;

        manualLogSource.LogInfo("Player_OnPlayerInitialized D3");

        Network.Routers.Packets packetRouter = Network.Tanuki.Instance.Routers.Packet;

        if (!ulong.TryParse(player._steamID, out ulong steamId))
            return;

        bool success = packetRouter.SendPacketToUser(new(steamId), tanukiServerHelloPacket, out EResult result);

        manualLogSource.LogInfo($"Player_OnPlayerInitialized D4 {steamId} {success} {result}");
    }

    float sec = 0;
    long lastGc = 0;
    private void FixedUpdate()
    {
        //if (!Player._mainPlayer)
        //    return;

        Network.Tanuki Network = Atlyss.Network.Tanuki.Instance;

        if (UnityEngine.Time.unscaledTime < sec)
            return;

        sec = UnityEngine.Time.unscaledTime + 3;

        StringBuilder sb = new();

        //Dictionary<ulong, Network.Data.Packets.RateLimitEntry> entries = (Dictionary<ulong, Network.Data.Packets.RateLimitEntry>)typeof(Network.Services.RateLimiter).GetField("entries", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Network.Managers.Network.RateLimiter);

        //GC.Collect();
        GC.GetTotalMemory(false);
        GC.WaitForPendingFinalizers();
        long gc = GC.GetTotalMemory(false);
        
        sb.AppendLine($"{Math.Round(sec, 0)} > {gc} {(lastGc <= gc ? '+' : string.Empty)}{gc - lastGc}");
        lastGc = gc;

        //foreach (var x in entries)
        //{
        //    sb.AppendLine($"{x.Key} {x.Value.Usage}");
        //}

        if (sb.Length == 0)
            return;

        manualLogSource.LogDebug(sb.ToString());
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

    protected override void Load()
    {
        Network.Tanuki network = Network.Tanuki.Instance;
        Providers.Settings settingsProvider = Tanuki.instance.providers.settings;

        Data.Settings.Network settingsProviderNetworkSection = settingsProvider.NetworkSection;

        Network.Managers.Network networkNetworkManager = network.Managers.Network;
        networkNetworkManager.SteamLocalChannel = settingsProviderNetworkSection.mainSteamMessageChannel;
        networkNetworkManager.PreventLobbyOwnerRateLimiting = settingsProviderNetworkSection.preventLobbyOwnerRateLimiting;

        Network.Services.RateLimiter networkNetworkManagerRateLimiter = networkNetworkManager.RateLimiter;
        networkNetworkManagerRateLimiter.Bandwidth = settingsProviderNetworkSection.rateLimiterBandwidth;
        networkNetworkManagerRateLimiter.Window = settingsProviderNetworkSection.rateLimiterWindow;

        Network.Components.SteamNetworkMessagePoller steamNetworkMessagePoller = networkNetworkManager.SteamNetworkMessagesPoller;
        steamNetworkMessagePoller.MessageBufferSize = settingsProviderNetworkSection.steamNetworkMessagePollerBuffer;

        tanukiServerHelloPacket.ServerCommandPrefix = settingsProvider.CommandSection.serverPrefix;

        Game.Patches.ChatBehaviour.Send_ChatMessage.OnPrefix += Tanuki.Instance.managers.chat.OnPlayerChatted;

        if (!Network.Tanuki.Instance.Providers.SteamLobby.SteamId.Equals(CSteamID.Nil))
        {
            if (AtlyssNetworkManager._current.isNetworkActive)
                network.Routers.Packet.SendPacketToLobbyChat(tanukiServerHelloPacket);

            network.Managers.Network.SteamNetworkMessagesPoller.enabled = true;
        }
    }

    private void TanukiServerHelloPacketHandler(CSteamID sender, Packets.Commands.TanukiServerHello packet)
    {
        manualLogSource.LogInfo($"TanukiServerHelloPacketHandler received");
        if (!sender.IsLobby() || !Network.Tanuki.Instance.Providers.SteamLobby.OwnerSteamId.Equals(sender))
            return;

        Routers.Commands commandRouter = Tanuki.instance.routers.commands;
        manualLogSource.LogInfo($"ver {packet.Version}");

        if (packet.Version != PluginInfo.VERSION)
        {
            commandRouter.ServerPrefix = Data.Settings.Commands.SERVER_PREFIX_DEFAULT;
            return;
        }

        commandRouter.ServerPrefix = packet.ServerCommandPrefix;
        manualLogSource.LogInfo($"pref {packet.ServerCommandPrefix}");
    }

    private void OnNetworkProviderSteamLobbyLobbyChanged(CSteamID lobby) =>
        Network.Tanuki.Instance.Managers.Network.SteamNetworkMessagesPoller.enabled = !lobby.Equals(CSteamID.Nil);

    protected override void Unload()
    {
        Network.Tanuki network = Network.Tanuki.Instance;
        network.Managers.Network.SteamNetworkMessagesPoller.enabled = false;

        reloadConfiguration = true;
        Game.Patches.ChatBehaviour.Send_ChatMessage.OnPrefix -= Tanuki.Instance.managers.chat.OnPlayerChatted;
    }
}
