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
 * random foxyjumpscare plugin is ready but i want to sync it with host so need networking :(
 */

[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
internal sealed class Main : Bases.Plugin
{
    internal static Main Instance = null!;
    private bool reloadConfiguration = false;

    public ManualLogSource ManualLogSource { get; internal set; } = null!;

    public Main()
    {
        Name = PluginInfo.NAME;
        Configuration.Initialize();
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
        Tanuki.Instance = new();
        Tanuki.Instance.Managers.Plugins.OnBeforePluginsLoad += HandleSettingsRefresh;

        Game.Providers.Player.Initialize();

        //Network.Tanuki.Initialize();
        //Network.Tanuki.Instance.Steam.CreateCallbacks();

        Tanuki.Instance.Registers.Plugins.Refresh();
        Tanuki.Instance.Managers.Plugins.LoadPlugins();
    }

    private void HandleSettingsRefresh()
    {
        if (reloadConfiguration)
        {
            Config.Reload();
            Configuration.Instance.Load(Config);

            reloadConfiguration = false;
        }

        Tanuki.Instance.Managers.Settings.Refresh();
    }

    protected override void Load() =>
        Game.Patches.ChatBehaviour.Send_ChatMessage.OnPrefix += Tanuki.Instance.Managers.Chat.OnPlayerChatted;

    protected override void Unload()
    {
        reloadConfiguration = true;
        Game.Patches.ChatBehaviour.Send_ChatMessage.OnPrefix -= Tanuki.Instance.Managers.Chat.OnPlayerChatted;
    }
}
