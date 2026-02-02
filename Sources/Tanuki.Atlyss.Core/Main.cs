using BepInEx;
using BepInEx.Logging;

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
        Atlyss.Network.Tanuki.Initialize();
        Game.Providers.Player.Initialize();
        Network = Atlyss.Network.Tanuki.Instance;
        Network.Providers.Steam.CreateCallbacks();

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
            commands = new(
                Network.Registers.Packets,
                Network.Managers.Packets,
                new(['"', '\"', '`']),
                providers.settings.CommandSection,
                registers.commands,
                providers.commands,
                Network.Providers.SteamLobby,
                Network.Routers.Packet
            )
        };

        Data.Tanuki.Managers managers = new()
        {
            plugins = new(manualLogSource, registers.plugins),
            chat = new(routers.commands)
        };

        Data.Tanuki.Services services = new()
        {
            tanukiServer = new(Network, routers.commands, providers.settings, Network.Routers.Packet)
        };

        Tanuki.instance = new()
        {
            managers = managers,
            providers = providers,
            registers = registers,
            routers = routers,
            services = services
        };

        managers.plugins.OnBeforePluginsLoad += HandleSettingsRefresh;
        registers.plugins.Refresh();
        managers.plugins.LoadPlugins();
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
        Providers.Settings settingProvider = Tanuki.instance.providers.settings;
        Data.Settings.Network settingProviderNetworkSection = settingProvider.NetworkSection;

        Network.Managers.Network networkManager = Network.Managers.Network;
        networkManager.SteamLocalChannel = settingProviderNetworkSection.mainSteamMessageChannel;
        networkManager.PreventLobbyOwnerRateLimiting = settingProviderNetworkSection.preventLobbyOwnerRateLimiting;

        Network.Services.RateLimiter rateLimiter = networkManager.RateLimiter;
        rateLimiter.Bandwidth = settingProviderNetworkSection.rateLimiterBandwidth;
        rateLimiter.Window = settingProviderNetworkSection.rateLimiterWindow;

        Network.Components.SteamNetworkMessagePoller steamNetworkMessagePoller = networkManager.SteamNetworkMessagesPoller;
        steamNetworkMessagePoller.MessageBufferSize = settingProviderNetworkSection.steamNetworkMessagePollerBuffer;
    }

    protected override void Load()
    {
        ConfigureNetwork();

        Tanuki.instance.routers.commands.Refresh();
        Tanuki.instance.services.tanukiServer.Refresh();
    }

    protected override void Unload()
    {
        reloadConfiguration = true;
        Network.Managers.Packets.ChangeMuteState<Packets.Commands.Request>(true);
    }
}
