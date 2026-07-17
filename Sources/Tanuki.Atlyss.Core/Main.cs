using BepInEx;
using BepInEx.Logging;

namespace Tanuki.Atlyss.Core;

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
        if (Instance is not null)
            return;

        Instance = this;
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

        Network.Tanuki.Initialize(gameObject);
        tanukiNetwork = Network.Tanuki.Instance;

        Tanuki.Initialize(Game.Tanuki.Instance, tanukiNetwork, manualLogSource);
        tanukiCore = Tanuki.Instance;
        tanukiCore.Managers.Plugin.OnBeforePluginsLoad += HandleSettingsRefresh;
        tanukiCore.Registers.Plugins.Refresh();
        tanukiCore.Managers.Plugin.LoadPlugins();
    }

    private void HandleSettingsRefresh()
    {
        if (reloadConfiguration)
        {
            Config.Reload();
            Configuration.Instance.Load(Config);

            reloadConfiguration = false;
        }

        Tanuki.Instance.Providers.Settings.Refresh();
    }

    private void ConfigureNetwork()
    {
        Providers.Settings settingProvider = Tanuki.Instance.Providers.Settings;
        Types.Settings.Network settingProviderNetworkSection = settingProvider.NetworkSection;

        Network.Managers.Network networkManager = tanukiNetwork.Managers.Network;
        networkManager.PreventLobbyOwnerRateLimiting = settingProviderNetworkSection.preventLobbyOwnerRateLimiting;

        Network.Types.Tanuki.Services tanukiNetworkServices = Network.Tanuki.Instance.Services;

        switch (settingProviderNetworkSection.rateLimiter)
        {
            case Types.Settings.ENetworkRateLimiter.Disabled:
                tanukiNetworkServices.RateLimiter = null;
                break;
            case Types.Settings.ENetworkRateLimiter.Window:
                Network.Services.WindowRateLimiter windowRateLimiter = new(settingProviderNetworkSection.windowRateLimiter.window, settingProviderNetworkSection.windowRateLimiter.bandwidth);
                tanukiNetworkServices.RateLimiter = windowRateLimiter;
                break;
        }

        Network.Components.SteamNetworkingMessagePoller steamNetworkMessagePoller = networkManager.SteamNetworkMessagesPoller;
        steamNetworkMessagePoller.MessageBufferSize = settingProviderNetworkSection.steamNetworkMessagePollerBuffer;
    }

    protected override void Load()
    {
        ConfigureNetwork();

        Tanuki.Instance.Routers.Commands.Refresh();

        Player player = Player._mainPlayer;

        if (player && player._isHostPlayer && !AtlyssNetworkManager._current._soloMode)
            Tanuki.Instance.Services.TanukiServer.SendTanukiServerInfo();
    }

    protected override void Unload()
    {
        reloadConfiguration = true;
        tanukiNetwork.Managers.Packets.ChangeMuteState<Packets.Commands.Request>(true);
    }
}
