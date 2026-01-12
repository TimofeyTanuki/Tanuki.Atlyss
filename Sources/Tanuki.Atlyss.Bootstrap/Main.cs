using BepInEx;
using Tanuki.Atlyss.Core.Plugins;

namespace Tanuki.Atlyss.Bootstrap;

[BepInPlugin(AssemblyInfo.GUID, AssemblyInfo.Name, AssemblyInfo.Version)]
public class Main : Plugin
{
    internal static Main Instance = null!;
    private bool ShouldReloadConfiguration = false;

    public Main()
    {
        Configuration.Initialize();
        Core.Tanuki.Initialize();
        Core.Tanuki.Instance.Plugins.OnBeforePluginsReload += OnBeforePluginsReload;
    }

    internal void Awake()
    {
        Logger.LogInfo("Tanuki.Atlyss by Timofey Tanuki / tanu.su");

        Instance = this;
        UpdateConfiguration();
    }

    internal void Start()
    {
        Core.Tanuki.Instance.Load();
        enabled = false;
    }

    protected override void Load()
    {
        Game.Patches.ChatBehaviour.Send_ChatMessage.OnPrefix += Core.Tanuki.Instance.Commands.OnSendMessage;
    }

    protected override void Unload()
    {
        ShouldReloadConfiguration = true;
        Game.Patches.ChatBehaviour.Send_ChatMessage.OnPrefix -= Core.Tanuki.Instance.Commands.OnSendMessage;
    }

    private void OnBeforePluginsReload()
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
            OnBeforePluginsReload();

        base.LoadPlugin();
    }
}
