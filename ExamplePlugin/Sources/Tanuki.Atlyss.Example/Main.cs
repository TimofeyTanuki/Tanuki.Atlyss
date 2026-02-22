using BepInEx.Logging;

namespace Tanuki.Atlyss.Example;

[BepInEx.BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
[BepInEx.BepInDependency(Core.PluginInfo.GUID, BepInEx.BepInDependency.DependencyFlags.HardDependency)]
public sealed class Main : Core.Bases.Plugin
{
    internal static Main Instance = null!;

    private ManualLogSource manualLogSource = null!;
    private Game.Tanuki gameTanuki = null!;

    private void Awake()
    {
        Instance = this;
        manualLogSource = Logger;

        Network.Tanuki.OnInitialized += OnTanukiNetworkInitialized;
        Game.Tanuki.OnInitialized += OnTanukiGameInitialized;
        Core.Tanuki.OnInitialized += OnTanukiCoreInitialized;

        Game.Patches.ChatBehaviour.Send_ChatMessage.OnPrefix += OnSendChatMessage;

        manualLogSource.LogDebug(nameof(Awake));
    }

    private void Start()
    {
        manualLogSource.LogDebug(nameof(Start));
    }

    protected override void Load()
    {
        manualLogSource.LogDebug(nameof(Load));

        gameTanuki.Providers.Player.OnPlayerLoaded += OnPlayerLoaded;
        gameTanuki.Providers.Player.OnPlayerAdded += OnPlayerAdded;
        gameTanuki.Providers.Player.OnPlayerRemoved += OnPlayerRemoved;
    }

    private void OnTanukiCoreInitialized()
    {
        manualLogSource.LogDebug($"{typeof(Core.Tanuki).FullName} has been initialized.");
    }

    private void OnTanukiNetworkInitialized()
    {
        manualLogSource.LogDebug($"{typeof(Network.Tanuki).FullName} has been initialized.");
    }

    private void OnTanukiGameInitialized()
    {
        manualLogSource.LogDebug($"{typeof(Game.Tanuki).FullName} has been initialized.");

        gameTanuki = Game.Tanuki.Instance;
    }

    private void OnSendChatMessage(string message, ref bool runOriginal)
    {
        manualLogSource.LogDebug($"{nameof(message)}\n  message: {message}");
    }

    private void OnPlayerAdded(Player player)
    {
        manualLogSource.LogDebug($"{nameof(Game.Providers.Player.OnPlayerAdded)}\n  netId: {player.netId}");
    }

    private void OnPlayerLoaded(Player player)
    {
        manualLogSource.LogDebug($"{nameof(Game.Providers.Player.OnPlayerLoaded)}\n  netId: {player.netId}\n  nickname: {player._nickname}");
    }

    private void OnPlayerRemoved(Player player)
    {
        manualLogSource.LogDebug($"{nameof(Game.Providers.Player.OnPlayerRemoved)}\n  netId: {player.netId}");
    }

    protected override void Unload()
    {
        gameTanuki.Providers.Player.OnPlayerLoaded -= OnPlayerLoaded;
        gameTanuki.Providers.Player.OnPlayerAdded -= OnPlayerAdded;
        gameTanuki.Providers.Player.OnPlayerRemoved -= OnPlayerRemoved;

        manualLogSource.LogDebug(nameof(Unload));
    }
}
