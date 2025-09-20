using BepInEx;
using Tanuki.Atlyss.Core.Plugins;

namespace Tanuki.Atlyss.Bootstrap;

[BepInPlugin("9c00d52e-10b8-413f-9ee4-bfde81762442", "Tanuki.Atlyss.Bootstrap", "2.0.10")]
public class Main : Plugin
{
    internal static Main Instance;
    private bool ShouldReloadConfiguration = false;
    public Main()
    {
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
    protected override void Unload() =>
        ShouldReloadConfiguration = true;
}