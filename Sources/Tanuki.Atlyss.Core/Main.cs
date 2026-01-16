using BepInEx;
using BepInEx.Logging;
using Tanuki.Atlyss.Core.Data.Tanuki;

namespace Tanuki.Atlyss.Core;

/*
 * GLOBAL TODO
 *
 * CLIENTS MUSTN'T KNOW ABOUT SERVER COMMANDS
 * SERVER MUSTN'T KNOW ABOUT CLIENT' COMMANDS
 * ^^^ safety policy
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
        Settings settings = new()
        {
            commands = new(),
            translations = new(),
        };

        Data.Tanuki.Registers registers = new()
        {
            commands = new(settings.commands),
            plugins = new()
        };

        Data.Tanuki.Providers providers = new()
        {
            commands = new(registers.commands)
        };

        Data.Tanuki.Routers routers = new()
        {
            commands = new(new(['"', '\"', '`']), settings.commands, registers.commands, providers.commands)
        };

        Data.Tanuki.Managers managers = new()
        {
            settings = new(settings),
            plugins = new(registers.plugins),
            chat = new(routers.commands)
        };

        Tanuki.instance = new()
        {
            managers = managers,
            providers = providers,
            registers = registers,
            settings = settings
        };

        managers.plugins.OnBeforePluginsLoad += HandleSettingsRefresh;

        Game.Providers.Player.Initialize();


        Network.

        //Network.Tanuki.Initialize();
        //Network.Tanuki.Instance.Steam.CreateCallbacks();

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

        Tanuki.Instance.managers.settings.Refresh();
    }

    protected override void Load() =>
        Game.Patches.ChatBehaviour.Send_ChatMessage.OnPrefix += Tanuki.Instance.managers.chat.OnPlayerChatted;

    protected override void Unload()
    {
        reloadConfiguration = true;
        Game.Patches.ChatBehaviour.Send_ChatMessage.OnPrefix -= Tanuki.Instance.managers.chat.OnPlayerChatted;
    }
}
