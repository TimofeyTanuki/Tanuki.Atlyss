using BepInEx.Logging;

namespace Tanuki.Atlyss.Core;

public class Tanuki
{
    public static Tanuki Instance;

    internal readonly ManualLogSource ManualLogSource = Logger.CreateLogSource("Tanuki.Atlyss.Core");
    private readonly Patching.Patcher Patcher;
    public readonly Settings Settings;
    public readonly Commands.Manager Commands;
    public readonly Plugins.Manager Plugins;
    private bool Loaded = false;
    private Tanuki()
    {
        Patcher = new();
        Settings = new();
        Commands = new();
        Plugins = new();
        Patcher.Use(typeof(Game.Events.ChatBehaviour.Send_ChatMessage_Prefix));
        Game.Events.ChatBehaviour.Send_ChatMessage_Prefix.OnInvoke += Commands.OnSendMessage;
    }
    public static void Initialize()
    {
        if (Instance is not null)
            return;

        Game.Fields.GameManager.Initialize();
        Patching.Core.Initialize();
        Instance = new();
    }
    public void Load()
    {
        if (Loaded)
            return;

        Loaded = true;
        Plugins.LoadPlugins();
    }
    public void Reload()
    {
        Patching.Core.Instance.UnpatchAll();
        Commands.RemoveAllCommands();
        Plugins.ReloadPlugins();
    }
}