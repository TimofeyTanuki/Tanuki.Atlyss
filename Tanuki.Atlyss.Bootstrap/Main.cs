using BepInEx;
using Tanuki.Atlyss.Core.Plugins;
using Tanuki.Atlyss.Patching;

namespace Tanuki.Atlyss.Bootstrap;

[BepInPlugin(PluginInfo.GUID, "Tanuki.Atlyss.Bootstrap", PluginInfo.Version)]
public class Main : Plugin
{
    internal static Main Instance;
    private bool ShouldReloadConfiguration = false;
    private readonly Patcher Patcher;

    public Main()
    {
        Patcher = new();
        Configuration.Initialize();
        Core.Tanuki.Initialize();
        Core.Tanuki.Instance.Plugins.OnBeforePluginsReload += BeforePluginsReload;
    }
    internal void Awake()
    {
        Logger.LogInfo("Tanuki.Atlyss by Timofey Tanuki / tanu.su");

        Instance = this;
        UpdateConfiguration();
    }
    internal void Start() =>
        Core.Tanuki.Instance.Load();
    protected override void Load()
    {
        Patcher.Use(typeof(Game.Events.ChatBehaviour.Send_ChatMessage_Prefix));
        Game.Events.ChatBehaviour.Send_ChatMessage_Prefix.OnInvoke += Core.Tanuki.Instance.Commands.OnSendMessage;
    }
    protected override void Unload()
    {
        ShouldReloadConfiguration = true;
        Game.Events.ChatBehaviour.Send_ChatMessage_Prefix.OnInvoke -= Core.Tanuki.Instance.Commands.OnSendMessage;
    }
    private void BeforePluginsReload()
    {
        Config.Reload();
        UpdateConfiguration();

        ShouldReloadConfiguration = false;
    }
    private void UpdateConfiguration()
    {
        Configuration.Instance.Load(Config);
        Core.Tanuki.Instance.Settings.Language = Configuration.Instance.Settings.Language.Value;
    }
    public override void LoadPlugin()
    {
        if (ShouldReloadConfiguration)
            BeforePluginsReload();

        base.LoadPlugin();
    }
}