using BepInEx;
using BepInEx.Logging;

namespace Tanuki.Atlyss.Core;

/*
 * GLOBAL TODO
 *
 * KILL CommandsLegacy.cs
 *
 *
 * NETWORKING MODULE
 *
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
 *
 * explore e621
 *
 *
 * PERMISSION GROUPS
 *   sth like this:
 *     default:
 *       immortality
 *     group1 (assignable by steamid):
 *       immortality
 *       health
 *
 *
 * EAT & SLEEP
 *
 *
 * random foxyjumpscare plugin is ready but i want to sync it with host so need networking :(
 */

[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
public class Main : Plugins.Plugin
{
    internal static Main Instance = null!;
    private bool ConfigurationRefreshRequirement = false;

    internal ManualLogSource ManualLogSource = null!;

    public Main()
    {
        Name = PluginInfo.NAME;
        Configuration.Initialize();
    }

    private void Initialize()
    {
        if (Tanuki.Instance is not null)
            return;

        Tanuki.Instance = new();

        Tanuki.Instance.Plugins.OnBeforePluginsLoad += OnBeforePluginsLoad;

        Game.Managers.Player.Initialize();
    }

    private void OnBeforePluginsLoad()
    {
        if (ConfigurationRefreshRequirement)
        {
            Config.Reload();
            Configuration.Instance.Load(Config);

            ConfigurationRefreshRequirement = false;
        }

        Tanuki.Instance.Settings.Refresh();
    }

    public void Awake()
    {
        Instance = this;
        ManualLogSource = Logger;

        Configuration.Instance.Load(Config);


        Logger.LogInfo("Tanuki.Atlyss by Timofey Tanuki / tanu.su");
    }

    internal void Start()
    {
        Initialize();
        Network.Tanuki.Initialize();
        Network.Tanuki.Instance.Steam.CreateCallbacks();

        Tanuki.Instance.Plugins.Refresh();
        Tanuki.Instance.Plugins.LoadPlugins();
    }

    protected override void Load() =>
        Game.Patches.ChatBehaviour.Send_ChatMessage.OnPrefix += Tanuki.Instance.Chat.OnPlayerChatted;

    protected override void Unload()
    {
        ConfigurationRefreshRequirement = true;
        Game.Patches.ChatBehaviour.Send_ChatMessage.OnPrefix -= Tanuki.Instance.Chat.OnPlayerChatted;
    }
}
