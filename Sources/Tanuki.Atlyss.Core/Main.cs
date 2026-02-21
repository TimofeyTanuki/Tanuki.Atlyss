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

    private bool reloadConfiguration = false;
    private ManualLogSource manualLogSource = null!;

    private Tanuki tanukiCore = null!;
    private Network.Tanuki tanukiNetwork = null!;

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
        Game.Tanuki.Initialize();

        Network.Tanuki.Initialize();
        tanukiNetwork = Network.Tanuki.Instance;

        Tanuki.Initialize(Game.Tanuki.Instance, tanukiNetwork, manualLogSource);

        tanukiCore = Tanuki.instance;
        tanukiCore.managers.plugins.OnBeforePluginsLoad += HandleSettingsRefresh;
        tanukiCore.registers.plugins.Refresh();
        tanukiCore.managers.plugins.LoadPlugins();
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

        Network.Managers.Network networkManager = tanukiNetwork.Managers.Network;
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
        tanukiNetwork.Managers.Packets.ChangeMuteState<Packets.Commands.Request>(true);
    }
}
